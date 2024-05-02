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

/// <summary>
/// ExplorerBrowser is a WinUI UserControl that displays a Windows Explorer-like view of the file system.
/// </summary>
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

    private readonly ShellItem m_CurrentFolder;

    public ShellItem CurrentFolder
    {
        get => m_CurrentFolder;
        private set => PrepareNavigation(value);
    }

    public ExplorerBrowser()
    {
        InitializeComponent();

        m_CurrentFolder = new ShellItem(@"C:\");

        History = new IWinUIExplorerBrowser.NavigationLog(this);
        Items = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
        GridViewItems = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
        TreeViewItems = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
    }

    /// <summary>Prepare Navigation for the specified folder.</summary>
    /// <param name="shellItem">The folder to prepare Navigation for</param>
    /// <exception cref="ArgumentNullException">shellItem is null</exception>
    private void PrepareNavigation(ShellItem shellItem)
    {
        var navigationTarget = shellItem ?? throw new ArgumentNullException(nameof(shellItem));
        var targetPath = navigationTarget.ParsingName;

        if (targetPath != null)
        {
            //targetPath = Path.Combine(targetPath, "*");
        }


        // TODO: implement navigation history
        // TODO: Lock member variables to prevent reentrancy
        //finally { m_CurrentFolder = new_CurrentFolder; }

        try
        {
            // TODO: build item arrays temporarily, finally assign them to the Items property
            //var targetItems = shellItem.GetChildren(SHCONTF.SHCONTF_FOLDERS | SHCONTF.SHCONTF_NONFOLDERS);

            //Items.Clear();
            //Items.AddRange(shellItem.GetChildren(SHCONTF.SHCONTF_FOLDERS | SHCONTF.SHCONTF_NONFOLDERS));
        }
        catch
        {
            throw;
        }
    }

    /// <summary>Let ExplorerBrowser navigate to the specified folder.</summary>
    /// <param name="shellItem">Folder to navigate to</param>
    internal void NavigateTo(ShellItem shellItem)     /* Info, was: public void NavigateTo(ShellItem shellItem) */
    {
        // Todo: if shellItem is a file, trigger event to open the file with the caller's default application
        PrepareNavigation(shellItem);
    }
}
