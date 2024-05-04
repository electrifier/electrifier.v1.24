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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using electrifier.Contracts.Services;
using electrifier.Views;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Text;

namespace electrifier.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;
    [ObservableProperty]
    private bool isForwardEnabled = true;
    [ObservableProperty]
    private object? selected;

    /// <summary>
    /// Gets the build version description.
    /// Necessary for the xaml page to access the build version description.
    /// </summary>
    [ObservableProperty]
    public string m_buildNumber = "1.24.504";
    [ObservableProperty]
    public string m_versionDescription = "may the 4th be with us";
    [ObservableProperty]
    public readonly string m_buildVersionDescription = "1.24.504 /* TODO concatenate */";

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
    protected void UnselectNavigationItem()
    {
        Selected = null;
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

    private string GetDebuggerDisplay()
    {
        var dbgDisplay = new StringBuilder();
        _ = dbgDisplay.Append(nameof(ShellViewModel));
        _ = dbgDisplay.Append(' ');
        _ = dbgDisplay.Append(Selected?.ToString() ?? "null");
        return dbgDisplay.ToString();
    }
}
