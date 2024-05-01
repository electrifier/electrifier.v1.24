/*
    Copyright 2024 Thorsten Jung, aka tajbender
        https://www.electrifier.org

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

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

        // Get the page type before navigation so you can prevent duplicate entries in the back stack
        var selectedItem = args.InvokedItemContainer as NavigationViewItem;
        //var selectedType = selectedItem?.GetType();
        //var pageKey = args.InvokedItemContainer.GetValue(NavigationHelper.NavigateToProperty) as string;
        // TODO: var selectedType = selectedItem?.GetType();
        // TODO: var pageKey = args.InvokedItemContainer.GetValue(NavigationHelper.NavigateToProperty) as string;

        // WorkbenchViewModel
        if (selectedItem is not null)
        {
            if (selectedItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey1)
            {
                _navigationService.NavigateTo(pageKey1);
            }
        }

        // doc: https://docs.microsoft.com/en-us/windows/apps/design/controls/navigationview#navigationview-and-the-back-button
        //if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey2)
        //{
        //    _navigationService.NavigateTo(pageKey2);
        //}
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
