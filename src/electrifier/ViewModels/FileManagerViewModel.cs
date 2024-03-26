using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using electrifier.Services;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Search;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    // AdvancedCollectionView can be bound to anything that uses collections.
    public AdvancedCollectionView ShellGridCollectionViewItems
    {
        get;
    }
    public ObservableCollection<DosShellItem> ShellGridViewItems { get; } = new ObservableCollection<DosShellItem>();
    public ObservableCollection<DosShellItem> ShellTreeViewItems { get; } = new ObservableCollection<DosShellItem>();
    public uint FolderCount
    {
        get;
    }
    public uint FileCount
    {
        get;
    }
    private DosShellItem DocumentsShellItem
    {
        get;
    }
    public DosShellItem MusicShellItem
    {
        get;
    }
    public DosShellItem PicturesShellItem
    {
        get;
        private set;
    }
    public DosShellItem VideosShellItem
    {
        get;
        private set;
    }

    public FileManagerViewModel()
    {
        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        ShellGridCollectionViewItems = new AdvancedCollectionView(ShellGridViewItems, true);
        // And sort ascending by the property "Name"
        ShellGridCollectionViewItems.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
        // TODO: TreeView -> OnSelection: _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);

        // Add root items to ShellTreeViewItems collection

        // Set up Library ShellTreeViewItems
        DocumentsShellItem = new DosShellItem(KnownLibraryId.Documents);
        ShellTreeViewItems.Add(DocumentsShellItem);
        MusicShellItem = new DosShellItem(KnownLibraryId.Music);
        ShellTreeViewItems.Add(MusicShellItem);
        PicturesShellItem = new DosShellItem(KnownLibraryId.Pictures);
        ShellTreeViewItems.Add(PicturesShellItem);
        VideosShellItem = new DosShellItem(KnownLibraryId.Videos);
        ShellTreeViewItems.Add(VideosShellItem);
        //_ = ShellTreeViewItems_GetItemsAsync(parentShellItem, enumChildren: true);
        //_ = ShellTreeViewItems_GetItemsAsync(parentShellItem, enumChildren: true);
        //_ = ShellTreeViewItems_GetItemsAsync(parentShellItem, enumChildren: true);
        //_ = ShellTreeViewItems_GetItemsAsync(parentShellItem, enumChildren: true);

        // add children to the root items in ShellTreeViewItems
        //foreach (var item in ShellTreeViewItems)
        //{
        //    _ = ShellTreeViewItems_GetItemsAsync(item, enumChildren: true);
        //}

        ShellGridViewItems.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var addedItems = e.NewItems;

                if (addedItems is null)
                {
                    throw new ArgumentNullException(nameof(addedItems));
                }

                if (addedItems.Count == 0)
                {
                    return;
                }

                foreach (DosShellItem item in addedItems)
                {
                    ShellTreeViewItems.Add(item);
                }
            }
        };



    }

    //    private readonly Task enumerateFoldersTask = Task.Run(() => EnumerateFolders(@"C:\", true));
    //
    //    /// <summary>
    //    /// functionality to enumerate folders
    //    /// <throws cref="DirectoryNotFoundException"></throws>
    //    /// <throws cref="UnauthorizedAccessException"></throws>
    //    /// <throws cref="PathTooLongException"></throws>
    //    /// </summary>
    //    /// 
    //    public static string EnumerateFolders(string rootPath, bool enumerateChildFoldersIfAllowed = false)
    //    {
    //        try
    //        {
    //            foreach (var dirName in Directory.EnumerateDirectories(rootPath))
    //            {
    //                return dirName;
    //                //Folders.Add(dirName);
    //
    //                //if (enumerateChildFoldersIfAllowed)
    //                //{
    //                //    EnumerateFolders(dirName, false);
    //                //}
    //            }
    //        }
    //        catch (DirectoryNotFoundException ex)
    //        {
    //            throw new DirectoryNotFoundException(rootPath, ex);
    //        }
    //        catch (UnauthorizedAccessException ex)
    //        {
    //            throw new UnauthorizedAccessException(rootPath, ex);
    //        }
    //        catch (PathTooLongException ex)
    //        {
    //            throw new PathTooLongException(rootPath, ex);
    //        }
    //
    //        return "{ unknown error in $EnumerateFolders }";
    //    }

    //private async Task ShellTreeViewItems_GetItemsAsync(KnownLibraryId libraryId)
    //{
    //    var library = await StorageLibrary.GetLibraryAsync(libraryId);
    //    var saveFolder = library.SaveFolder;

    //    if (saveFolder != null)
    //    {
    //        _ = ShellTreeViewItems_GetItemsAsync(saveFolder);
    //    }
    //}


#pragma warning disable IDE0051 // Remove unused private members
    private static async Task ShellTreeViewItems_GetItemsAsync(DosShellItem parentShellItem, bool enumChildren = false)
#pragma warning restore IDE0051 // Remove unused private members
    {
        if (parentShellItem is null)
        {
            throw new ArgumentNullException(nameof(parentShellItem));
        }

        var storageFolder = parentShellItem.StorageItem as StorageFolder;

        if (storageFolder != null)
        {
            var item = new DosShellItem(storageFolder);

            if (item.IsFolder)
            {

                parentShellItem.Children.Add(item);
                if (enumChildren)
                {
                    await item.GetChildsAsync();
                    //await ShellTreeViewItems_GetItemsAsync(saveFolder);
                }
            }
        }
    }

#pragma warning disable IDE0051 // Remove unused private members
    private async Task ShellTreeViewItems_GetItemsAsync(StorageFolder storageFolder)
    {
        var childrenQueryResult = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        var storageFolders = await childrenQueryResult.GetFilesAsync();

        foreach (var storageItem in storageFolders)
        {
            ShellTreeViewItems.Add(await LoadShellItemInfo(storageItem));
        }
    }

    private async Task ShellGridViewItems_GetItemsAsync(KnownLibraryId storageLibrary)
    {
        var library = await StorageLibrary.GetLibraryAsync(storageLibrary);
        var storageFolder = library.SaveFolder;

        if (storageFolder != null)
        {
            _ = ShellGridViewItems_GetItemsAsync(storageFolder);
        }
    }

    private async Task ShellGridViewItems_GetItemsAsync(StorageFolder storageFolder)
    {
        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        var storageFolders = await fileQuery.GetFilesAsync();

        foreach (var storageItem in storageFolders)
        {
            ShellGridViewItems.Add(await LoadShellItemInfo(storageItem));
        }

        /*
                StorageFolder picturesFolder;
                picturesFolder = KnownFolders.DocumentsLibrary;
                var result = picturesFolder.CreateFileQueryWithOptions(new QueryOptions());
                var storageFiles = await result.GetFilesAsync();
                foreach (var storageItem in storageFiles)
                {
                    ShellGridViewItems.Add(await LoadShellItemInfo(storageItem));
                }
                ImageGridView.ItemsSource = ShellGridViewItems;
        */
    }


    private void ShellGridViewItems_ContainerContentChanging(
#pragma warning disable IDE0060 // Remove unused parameter
            ListViewBase sender,
#pragma warning restore IDE0060 // Remove unused parameter
            ContainerContentChangingEventArgs args)
    {
        if (args.InRecycleQueue)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var image = templateRoot?.FindName("ImageIcon") as Image;

            if (image != null)
            {
                image.Source = null;
            }

            args.Handled = true;
        }

        if (args.Phase == 0)
        {
            args.RegisterUpdateCallback(ShellGridViewItems_GetImageAsync);
            args.Handled = true;
        }
    }

    private async void ShellGridViewItems_GetImageAsync(
        ListViewBase sender,
        ContainerContentChangingEventArgs args)
    {
        if (args.Phase == 1)
        {
            // It's phase 1, so show this item's image.
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;

            if (templateRoot != null)
            {
                var imageElement = templateRoot.FindName("ItemImageIcon") as ImageIcon;

                if (imageElement != null)
                {
                    if (args.Item is DosShellItem item)
                    {
                        var bitmapImage = await item.GetImageThumbnailAsync();
                        imageElement.Source = bitmapImage;

                        /*
                        Image img = sender as Image; 
                        //BitmapImage bitmapImage = new BitmapImage();
                        //img.Width = bitmapImage.DecodePixelWidth = 80; 
                        //// Natural px width of image source.
                        //// You don't need to set Height; the system maintains aspect ratio, and calculates the other
                        //// dimension, as long as one dimension measurement is provided.
                        //bitmapImage.UriSource = new Uri(img.BaseUri,"Assets/StoreLogo.png");
                        //img.Source = bitmapImage;*/
                        //imageElement.Source = new ImageSource(item.ShellIcon);
                        //var task = item?.GetImageThumbnailAsync();
                        //if (task != null)
                        //{
                        //    imageElement.Source = await task;
                        //}
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(args), ".Item is null");
                    }
                }
            }
        }
    }


    //    private async Task ShellTreeViewItems_GetItemsAsync(KnownLibraryId libraryId)
    //    {
    //        var library = await StorageLibrary.GetLibraryAsync(libraryId);
    //        var saveFolder = library.SaveFolder;
    //        if (saveFolder != null)
    //        {
    //            var shellItem = new DosShellItem(saveFolder);
    //            ShellTreeViewItems.Add(shellItem);
    ////            _ = ShellTreeViewItems_GetItemsAsync(saveFolder);
    //        }
    //    }


    public static async Task<DosShellItem> LoadShellItemInfo(IStorageItem item)
    {
        DosShellItem shellItem = new(item ?? throw new ArgumentNullException(nameof(item)));
        Debug.Assert(shellItem != null);

        await item.GetBasicPropertiesAsync();

        return shellItem;
    }

    //    private void ImageGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        Debug.Assert(e != null);
    //
    //        var addedItems = e.AddedItems;
    //        var removedItems = e.RemovedItems;
    //
    //        if (addedItems.Count > 0)
    //        {
    //            var item = e.AddedItems[0] as DosShellItem;
    //            //ImageGridView.SelectedItem = item;
    //
    //        }
    //    }
}
