using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace electrifier.Services;


/**
 * 
 * https://learn.microsoft.com/en-us/uwp/api/windows.storage.search.commonfolderquery?view=winrt-22621
 * 
 */

#region DosShellItemFactory
// TODO: Implement DosShellItemFactory
//public class DosShellItemFactory


//public class NewShellItemImplementation : INotifyPropertyChanged
//{
//    public ObservableCollection<NewShellItemImplementation> Data
//    {
//        get; set;
//    }
//    // Static async method acting as a constructor
//    public static async Task<NewShellItemImplementation> BuildViewModelAsync()
//    {
//        ObservableCollection<NewShellItemImplementation> tmpData = await GetDataTask();
//        return new NewShellItemImplementation(tmpData);
//    }
//    // Private constructor called by the async method
//    private NewShellItemImplementation(ObservableCollection<NewShellItemImplementation> data)
//    {
//        Data = data;
//    }
//    private static async Task<ObservableCollection<NewShellItemImplementation>> GetDataTask()
//    {
//        return new ObservableCollection<NewShellItemImplementation>
//        {
//            new(),
//            //new("Item 1"),
//            //new("Item 2"),
//            //new("Item 3"),
//            //new("Item 4"),
//            //new("Item 36"),
//            //new("Item 37"),
//            //new("Item 38"),
//            //new("Item 39"),
//            //new("Item 40"),
//            //new("Item 41"),
//            //new("Item 42"),
//            //new("Item 43"),
//            //new("Item 44"),
//        };
//    }
//}
#endregion

public class DosShellItem : INotifyPropertyChanged
{
    public ObservableCollection<DosShellItem> Children
    {
        get => Children;
        set =>
            //Children = value;
            OnPropertyChanged();
    }

    //doc: https://docs.microsoft.com/en-us/uwp/api/windows.storage.search.queryoptions
    public QueryOptions ChildEnumerationQueryOptions
    {
        get;
    }
    public bool HasChildren => Children.Count > 0;
    public bool IsFile => !IsFolder;
    //public bool IsLibrary => StorageItem is StorageFolder folder && folder.IsOfType(StorageItemTypes.Library);
    public bool IsFolder
    {
        get;
    }
    public string Name => StorageItem?.Name ?? "[Unknown Item]";
    public string Path => StorageItem?.Path ?? "[Invalid Path]";
    public ImageIcon ShellIcon
    {
        get;
    }
    public IStorageItem? StorageItem
    {
        get; private set;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // TODO: Move to DosShellItemFactory
    //        public static DosShellItem CreateRootItem() =>
    //        new DosShellItem(new StorageFolder("C:\\"));
    // TODO: public readonly QueryOptions defaultFolderQueryOptions = new(CommonFileQuery.OrderByName, null);  
    // => Helpers.DefaultQueryOptionsCommonFile

    /// <summary>
    /// Create a new instance of DosShellItem
    /// </summary>
    /// <param name="storageItem"><a cref="IStorageItem"/></param>
    /// <param name="forcedFolderQueryOptions"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DosShellItem(IStorageItem storageItem, QueryOptions? forcedFolderQueryOptions = null)
    {
        StorageItem = storageItem ?? throw new ArgumentNullException(nameof(storageItem));
        Children = new ObservableCollection<DosShellItem>();

        // Determine if the item is a folder
        IsFolder = StorageItem.IsOfType(StorageItemTypes.Folder);
        // Set temporary icon
        ShellIcon = IsFolder ?
            DosShellItemHelpers.DefaultFolderIcon :
            DosShellItemHelpers.DefaultUnknownFileIcon;

        if (forcedFolderQueryOptions != null)
        {
            ChildEnumerationQueryOptions = forcedFolderQueryOptions;
        }
        else
        {
            ChildEnumerationQueryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery);
        }

        _ = GetChildsAsync();

        #region old stuff
        //        ChildEnumerationQueryOptions = forcedFolderQueryOptions != null ? forcedFolderQueryOptions // : DosShellItemHelpers.DefaultFolderIcon;


        //if (forcedFolderQueryOptions != null)
        //{
        //    //  Enumerate Children
        //    _ = GetChildsAsync();
        //}



        //Children.Add(new DosShellItem(storageItem)); // TODO: Get reference to root object and add their children

        //if ((IsFolder) && (enumerateChilds))
        //{
        //    // => Children = new ObservableCollection<DosShellItem>();
        //    _ = GetChildsAsync();
        //}
        #endregion
    }

    // Copilot: KnownLibraryIdGetLibraryId() => StorageItem is StorageFolder folder ? folder.LibraryRelativePath : KnownLibraryId.Unknown;
    public DosShellItem(KnownLibraryId libraryId, QueryOptions? forcedFolderQueryOptions = null)
    {
        // TODO: Check if libraryId is valid
        // TODO: try / catch, reset values if exception has been raised
        var library = StorageLibrary.GetLibraryAsync(libraryId);
        var libraryIdName = Enum.GetName(typeof(KnownLibraryId), libraryId);

        var newChildren = new ObservableCollection<DosShellItem>();

        if (forcedFolderQueryOptions != null)
        {
            ChildEnumerationQueryOptions = forcedFolderQueryOptions;
        }
        else
        {
            ChildEnumerationQueryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery);
        }

        if (library != null)
        {
            //addRootItem(library);

            //using (StorageItem is StorageFolder storeItem ? library.LibraryRelativePath : KnownLibraryId.Unknown)
            //{
            //StorageItem = storeItem;
            //}

            //StorageItem = library;
            IsFolder = true;
        }
        else
        {

            throw new ArgumentException($"can't create StorageItem from LibraryId: {libraryIdName}");
        }

        ShellIcon = DosShellItemHelpers.DefaultFolderIcon;
        Children = newChildren;



        //        if (IsFolder)
        //        {
        //            //var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, null);
        //
        //            // Create query and retrieve files
        //            //        var result = Windows.Storage.Search.StorageFileQueryResult(this.StorageItem as StorageFolder, forcedFolderQueryOptions);
        //
        //
        //            //= KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(forcedFolderQueryOptions);
        //            //IReadOnlyList<StorageFile> childReadOnlyList = await query.GetChildsAsync();
        //
        //            //IReadOnlyList<StorageFile> childReadOnlyList = new IReadOnlyList<StorageFile>();
        //
        //            // Process results
        //            //foreach (StorageFile file in childReadOnlyList)
        //            //{
        //            //    Children.Add(new DosShellItem(file));
        //            //}
        //
        //            /*      if (enumerateChilds)
        //                    {
        //                        _ = GetChildsAsync();
        //                    } */
        //            /*      var folder = (StorageFolder)StorageItem;
        //                    var items = await folder.ShellGridViewItems_GetItemsAsync();
        //                    foreach (var item in items)
        //                    {
        //
        //                        Children.Add(new DosShellItem(item));
        //                    } */
        //        }
        //        else
        //        {
        //            return;
        //        }

        //// StorageItem = library as IStorageItem ?? throw new ArgumentException(nameof(libraryId), "can't create StorageItem from LibraryId");
        //if (library is IStorageItem storageItem)
        //{
        //    StorageItem = storageItem;

        //    //public bool IsLibrary => StorageItem is StorageFolder folder && folder.IsOfType(StorageItemTypes.Library);


        //    // Determine if the item is a folder
        //    IsFolder = StorageItem.IsOfType(StorageItemTypes.Folder);
        //    // Set temporary icon
        //    ShellIcon = IsFolder ? DosShellItemHelpers.DefaultFolderIcon : DosShellItemHelpers.DefaultUnknownFileIcon;

        //    _ = GetChildsAsync();
        //}
        //else
        //{
        //    IsFolder = true;
        //    ShellIcon = DosShellItemHelpers.DefaultFolderContainingFileIcon;
        //}
    }

    //Task GetChildsAsync()
    //{
    //    //if (IsFolder)
    //    //{
    //    //    var folder = (StorageFolder)StorageItem;
    //    //    var items = await folder.GetItemsAsync();

    //    //    foreach (var item in items)
    //    //    {
    //    //        Children.Add(new DosShellItem(item));
    //    //    }
    //    //}
    //}
    //    {
    //        //if(KnownLibraryId.Unknown == library)
    //        //{
    //        //    throw new ArgumentException("Unknown library");
    //        //}
    //        
    //        //StorageItem = library.GetLibraryAsync();
    //        //Children = new ObservableCollection<DosShellItem>();
    //
    //
    //
    //        //StorageItem = parentShellItem.StorageItem;
    //
    //        //this(StorageItem, forcedFolderQueryOptions);
    //    }

    /*
    var forcedFolderQueryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);

    // Create query and retrieve files
    var query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(forcedFolderQueryOptions);
    IReadOnlyList<StorageFile> childReadOnlyList = await query.GetFilesAsync();
    // Process results
    foreach (StorageFile file in childReadOnlyList)
    {
        // Process file
    } */

    //#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    //    private async Task QueryChildItemsAsync()
    //#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    //    {
    //        if (IsFolder)
    //        {
    //            //var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, null);
    //
    //            // Create query and retrieve files
    //            //        var result = Windows.Storage.Search.StorageFileQueryResult(this.StorageItem as StorageFolder, forcedFolderQueryOptions);
    //
    //
    //            //= KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(forcedFolderQueryOptions);
    //            //IReadOnlyList<StorageFile> childReadOnlyList = await query.GetChildsAsync();
    //
    //            //IReadOnlyList<StorageFile> childReadOnlyList = new IReadOnlyList<StorageFile>();
    //
    //            // Process results
    //            //foreach (StorageFile file in childReadOnlyList)
    //            //{
    //            //    Children.Add(new DosShellItem(file));
    //            //}
    //
    //            /*      if (enumerateChilds)
    //                    {
    //                        _ = GetChildsAsync();
    //                    } */
    //            /*      var folder = (StorageFolder)StorageItem;
    //                    var items = await folder.ShellGridViewItems_GetItemsAsync();
    //                    foreach (var item in items)
    //                    {
    //
    //                        Children.Add(new DosShellItem(item));
    //                    } */
    //        }
    //        else
    //        {
    //            return;
    //        }
    //    }

    public async Task GetChildsAsync()
    {
        if (IsFolder)
        {
            var storeItem = StorageItem as StorageFolder;
            var subItems = await storeItem?.GetItemsAsync();

            foreach (var item in subItems)
            {
                Children.Add(new DosShellItem(item));
            }
        }
    }

    public async Task<BitmapImage> GetImageThumbnailAsync()
    {
        if (IsFolder)
        {
            try
            {
                var thumbnail = await (StorageItem as StorageFolder)?.GetThumbnailAsync(ThumbnailMode.SingleItem);
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(thumbnail);
                thumbnail.Dispose();
                return bitmapImage;
            }
            catch (Exception)
            {
                throw;
            }
            //return DosShellItemHelpers.DefaultFolderIcon;
            // return new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Folder containing File.ico"));
        }
        else
        {
            try
            {
                var bitmapImage = new BitmapImage();

                using (var thumbnail = await (StorageItem as StorageFile)?.GetThumbnailAsync(ThumbnailMode.SingleItem))
                {
                    bitmapImage.SetSource(thumbnail);
                    thumbnail.Dispose();
                }

                return bitmapImage;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
