using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace electrifier.Services;

internal static class DosShellItemHelpers
{
    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS8601 // Possible null reference assignment.
    public static readonly ImageIcon DefaultFolderIcon = defaultFolderImageIcon;
#pragma warning restore CS8601 // Possible null reference assignment.

#pragma warning disable CS8601 // Possible null reference assignment.
    public static readonly ImageIcon DefaultUnknownFileIcon = defaultUnknownFileImageIcon;
#pragma warning restore CS8601 // Possible null reference assignment.

    public static readonly ImageIcon DefaultFolderContainingFileIcon = new()
    {
        Source = new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Folder containing File.ico"))
    };

    // TODO: Convert to array and use constants as index
    public const string shell32DefaultFolderIcon = "ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico";
    //public const string shell32FolderContainingFileIcon = "ms-appx:///Assets/Views/Workbench/Shell32 Folder containing File.ico";
    public const string shell32NetworkIcon = "ms-appx:///Assets/Views/Workbench/Shell32 Network.ico";
    //public const string shell32NetworkFolderIcon = "ms-appx:///Assets/Views/Workbench/Shell32 Network Folder.ico";
    //public const string shell32NetworkFolderOpenIcon = "ms-appx:///Assets/Views/Workbench/Shell32 Network Folder Open.ico";
    //public const string shell32NetworkOfflineIcon = "ms-appx:///Assets/Views/Workbench/Shell32 Network Offline.ico";

    public static QueryOptions DefaultQueryOptionsCommonFile = new(CommonFileQuery.OrderByName, null);

    private static readonly ImageIcon defaultUnknownFileImageIcon = new()
    {
        Source = new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"))
    };

    private static readonly ImageIcon defaultFolderImageIcon = new()
    {
        Source = new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"))
    };

    public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path)
    {

        if (thumbnail is not null)
        {
            //            return thumbnail.Path;
        }

        if (path is not null)
        {

            return path;
        }

        return "ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico";
    }

    // Document this// Document this method
    public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder)
    {
        if (thumbnail is not null)
        {
            //            return thumbnail.Path;
        }
        if (path is not null)
        {
            return path;
        }
        return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico";
    }
    public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder, bool isNetwork)
    {
        if (thumbnail is not null)
        {
            //            return thumbnail.Path;
        }
        if (path is not null)
        {
            return path;
        }
        if (isNetwork)
        {
            return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Network Folder.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Network.ico";
        }
        return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico";
    }
    public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder, bool isNetwork, bool isOffline)
    {
        if (thumbnail is not null)
        {
            //            return thumbnail.Path;
        }
        if (path is not null)
        {
            return path;
        }
        if (isNetwork)
        {
            return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Network Folder.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Network.ico";
        }
        if (isOffline)
        {
            return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Network Offline.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico";
        }
        return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico";
    }
    //public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder)
    //{
    //    if (thumbnail is not null)
    //    {
    //        //            return thumbnail.Path;
    //    }

    //    if (path is not null)
    //    {
    //        return path;
    //    }

    //    return isFolder ? "ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico" : "ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico";
    //}

    //public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder, bool isNetwork)
    //{
    //}

    //public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder, bool isNetwork, bool isOffline)
    //{

    //}

    //public static string GetIconPath(StorageItemThumbnail? thumbnail, string? path, bool isFolder, bool isNetwork, bool isOffline, bool isShared)
    //{

    //}
}
