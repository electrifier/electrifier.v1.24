using electrifier.Contracts.Services;
using electrifier.Helpers;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;

namespace electrifier.Services;

public class NavigationViewService : INavigationViewService
{
    private readonly INavigationService _navigationService;

    private readonly IPageService _pageService;

    private NavigationView? _navigationView;
    public object? FooterMenuItems => _navigationView?.FooterMenuItems;

    public IList<object>? MenuItems => _navigationView?.MenuItems;

    public object? SettingsItem => _navigationView?.SettingsItem;
    
    //public object? WorkbenchItem => _navigationView?.WorkbenchItem;

    public NavigationViewService(INavigationService navigationService, IPageService pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;
    }

    [MemberNotNull(nameof(_navigationView))]
    public void Initialize(NavigationView navigationView)
    {
        _navigationView = navigationView;
        _navigationView.BackRequested += OnBackRequested;
        _navigationView.ItemInvoked += OnItemInvoked;
    }

    public void UnregisterEvents()
    {
        if (_navigationView != null)
        {
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }
    }

    public NavigationViewItem? GetSelectedItem(Type pageType)
    {
        if (_navigationView != null)
        {
            return GetSelectedItem(_navigationView.MenuItems, pageType) ?? GetSelectedItem(_navigationView.FooterMenuItems, pageType);
        }

        return null;
    }

    bool INavigationViewService.TryGetSelectedItem(Type sourcePageType, [NotNullWhen(true)] out object? selectedItem)
    {
        selectedItem = GetSelectedItem(sourcePageType);
        return selectedItem != null;
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => _navigationService.GoBack();

    /// <summary>
    /// OnItemInvoked is called when a NavigationViewItem is clicked or tapped.
    /// <list type="bullet">
    /// If SettingsItem is invoked, navigate to <see cref="SettingsViewModel"/>
    /// </list>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

            return;
        }

        // Get the page type before navigation so you can prevent duplicate entries in the backstack
        var selectedItem = args.InvokedItemContainer as NavigationViewItem;
        // TODO: var selectedType = selectedItem?.GetType();

        // WorkbenchViewModel
        if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string selectedItemPageKey)
        {
            _navigationService.NavigateTo(selectedItemPageKey);
        }

        // doc: https://docs.microsoft.com/en-us/windows/apps/design/controls/navigationview#navigationview-and-the-back-button
        if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
        {
            _navigationService.NavigateTo(pageKey);
        }
    }

    private NavigationViewItem? GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<NavigationViewItem>())
        {
            if (IsMenuItemForPageType(item, pageType))
            {
                return item;
            }

            var selectedChild = GetSelectedItem(item.MenuItems, pageType);
            if (selectedChild != null)
            {
                return selectedChild;
            }
        }

        return null;
    }
    
    private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
    {
        if (menuItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
        {
            return _pageService.GetPageType(pageKey) == sourcePageType;
        }

        return false;
    }
}
