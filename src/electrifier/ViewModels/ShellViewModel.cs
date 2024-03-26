using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using electrifier.Contracts.Services;
using electrifier.Views;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Text;

namespace electrifier.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class ShellViewModel : ObservableRecipient
{

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private bool isForwardEnabled;

    [ObservableProperty]
    private object? selected;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        // TODO: https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.navigation.framenavigationoptions?view=windows-app-sdk-1.4
        //navigationService.CanForwardChanged += (s, e) => IsForwardEnabled = e;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;

            return;
        }

        if (NavigationViewService.TryGetSelectedItem(e.SourcePageType, out var selectedItem))
        {
            Selected = selectedItem;
            return;
        }

        UnselectNavigationItem();
    }

    protected void UnselectNavigationItem()
    {
        Selected = null;
    }

    public bool NavigateToWorkbench()
    {
        var viewModel = App.GetService<WorkbenchViewModel>();
        var fullName = viewModel.GetType().FullName;

        if (fullName is not null)
        {
            return NavigationService.NavigateTo(fullName);
        }
        return false;
    }

    //    NavigationHelper.SetNavigateTo(navigationViewItem, typeof(MainViewModel).FullName)

    private string GetDebuggerDisplay()
    {
        var dbgDisplay = new StringBuilder();
        _ = dbgDisplay.Append(nameof(ShellViewModel));
        _ = dbgDisplay.Append(' ');
        _ = dbgDisplay.Append(Selected?.ToString() ?? "null");
        return dbgDisplay.ToString();
    }
}
