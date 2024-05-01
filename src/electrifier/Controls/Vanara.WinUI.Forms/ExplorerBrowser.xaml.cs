using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System;
using Vanara.Collections;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;
using Vanara.Windows.Forms.Design;
using Vanara.Windows.Shell;
using Windows.Foundation.Collections;
using Windows.Foundation;
using static electrifier.Controls.Vanara.WinUI.Forms.IWinUIExplorerBrowser;
using static Vanara.PInvoke.Shell32;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
// and more about our project templates, see: http://aka.ms/winui-project-info.

//public static readonly DependencyProperty SelectedPathProperty =
//    DependencyProperty.Register("SelectedPath", typeof(string), typeof(ExplorerBrowser), new PropertyMetadata(null, OnSelectedPathChanged));
//private static void OnSelectedPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
//    => throw new NotImplementedException();
//public string SelectedPath;

namespace electrifier.Controls.Vanara.WinUI.Forms;

public partial class ExplorerBrowser : UserControl, IWinUIExplorerBrowser
{
    private IWinUIExplorerBrowser.NavigationLog History
    {
        get;
    }
    private ShellItemCollection Items
    {
        get;
    }
    private ShellItemCollection GridViewItems
    {
        get;
    }
    private ShellItemCollection TreeViewItems
    {
        get;
    }

    public ExplorerBrowser()
    {
        InitializeComponent();

        History = new IWinUIExplorerBrowser.NavigationLog(this);
        Items = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
        GridViewItems = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
        TreeViewItems = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);

        NavigateTo(new ShellItem(@"c:\"));
    }

    public void NavigateTo(ShellItem shellItem)
    {
        if (shellItem is null)
            throw new ArgumentNullException(nameof(shellItem));

        // Add the item to the history
        //History.Add(shellItem);

        // Update the items in the TreeView and GridView
        //TreeViewItems.Clear();
        //GridViewItems.Clear();
        //foreach (var item in shellItem.EnumerateItems(SHCONTF.SHCONTF_FOLDERS | SHCONTF.SHCONTF_NONFOLDERS))
        //{
        //    TreeViewItems.Add(item);
        //    GridViewItems.Add(item);
        //}
    }
}
