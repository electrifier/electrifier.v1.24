using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;
using Vanara.Windows.Forms.Design;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.Shell32;
using Vanara.Collections;
using System.Collections;
using System.Runtime.InteropServices;


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
    private IWinUIExplorerBrowser.ShellItemCollection Items
    {
        get;
    }
    private IWinUIExplorerBrowser.ShellItemCollection SelectedItems
    {
        get;
    }
    public ExplorerBrowser()
    {
        InitializeComponent();

        History = new IWinUIExplorerBrowser.NavigationLog((IWinUIExplorerBrowser)this);
        Items = new IWinUIExplorerBrowser.ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
        SelectedItems = new IWinUIExplorerBrowser.ShellItemCollection(this, SVGIO.SVGIO_SELECTION);
    }
}
