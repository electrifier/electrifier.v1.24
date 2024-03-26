using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class WorkbenchPage : Page
{
    public WorkbenchViewModel ViewModel
    {
        get;
    }

    public WorkbenchPage()
    {
        ViewModel = App.GetService<WorkbenchViewModel>();
        InitializeComponent();
    }
}
