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

using electrifier.Helpers;
using Microsoft.UI;             // Needed for WindowId.
using Microsoft.UI.Dispatching; // Needed for DispatcherQueue.
using Microsoft.UI.Windowing;   // Needed for AppWindow.
using Windows.UI.ViewManagement;
using WinRT.Interop;            // Needed for XAML/HWND interop.

namespace electrifier;

// More information how to customize the https://learn.microsoft.com/en-us/windows/apps/develop/title-bar

public sealed partial class MainWindow : WindowEx
{
    private readonly AppWindow m_AppWindow;

    private readonly DispatcherQueue m_DispatcherQueue;

    private readonly UISettings m_Settings;

    /// <summary>
    /// doc: https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.windowing.appwindow
    /// doc: https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.windowing.appwindow.titlebar
    /// doc: https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.windowing.appwindow.titlebar.buttonbackgroundcolor
    /// doc: https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.windowing.appwindow.titlebar.buttoninactivebackgroundcolor
    /// doc: https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.windowing.appwindow.titlebar.buttonhoverbackgroundcolor
    /// doc: https://docs.microsoft.com/en-us/uwp/api/microsoft.ui.windowing.appwindow.titlebar.buttonpressedbackgroundcolor
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        m_AppWindow = GetAppWindowForCurrentWindow();
        //        m_AppWindow = AppWindow;
        var titleBar = m_AppWindow.TitleBar;
        // Hide system title bar.
        titleBar.ExtendsContentIntoTitleBar = true;

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "electrifier";

        m_AppWindow.Changed += AppWindow_Changed;

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        m_DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        m_Settings = new UISettings();
        // cannot use FrameworkElement.ActualThemeChanged event
        m_Settings.ColorValuesChanged += Settings_ColorValuesChanged;
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        var hWnd = WindowNative.GetWindowHandle(this);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);

        return AppWindow.GetFromWindowId(wndId);
    }

    private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
        {
            //object AppTitleBar = null;

            switch (sender.Presenter.Kind)
            {
                case AppWindowPresenterKind.CompactOverlay:
                    // Compact overlay - hide custom title bar
                    // and use the default system title bar instead.
                    //AppTitleBar.Visibility = Visibility.Collapsed;
                    //sender.TitleBar.ResetToDefault();
                    break;

                case AppWindowPresenterKind.FullScreen:
                    // Full screen - hide the custom title bar
                    // and the default system title bar.
                    //AppTitleBar.Visibility = Visibility.Collapsed;
                    //sender.TitleBar.ExtendsContentIntoTitleBar = true;
                    break;

                case AppWindowPresenterKind.Overlapped:
                    // Normal - hide the system title bar
                    // and use the custom title bar instead.
                    //AppTitleBar.Visibility = Visibility.Visible;
                    //sender.TitleBar.ExtendsContentIntoTitleBar = true;
                    break;

                default:
                    // Use the default system title bar.
                    sender.TitleBar.ResetToDefault();
                    break;
            }
        }
    }
    //    public MainWindow()
    //    {
    //        this.InitializeComponent();
    //
    //        m_AppWindow = this.AppWindow;
    //        m_AppWindow.Changed += AppWindow_Changed;
    //    }
    //
    //    private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    //    {
    //        if (args.DidPresenterChange)
    //        {
    //            switch (sender.Presenter.Kind)
    //            {
    //                case AppWindowPresenterKind.CompactOverlay:
    //                    // Compact overlay - hide custom title bar
    //                    // and use the default system title bar instead.
    //                    AppTitleBar.Visibility = Visibility.Collapsed;
    //                    sender.TitleBar.ResetToDefault();
    //                    break;
    //
    //                case AppWindowPresenterKind.FullScreen:
    //                    // Full screen - hide the custom title bar
    //                    // and the default system title bar.
    //                    AppTitleBar.Visibility = Visibility.Collapsed;
    //                    sender.TitleBar.ExtendsContentIntoTitleBar = true;
    //                    break;
    //
    //                case AppWindowPresenterKind.Overlapped:
    //                    // Normal - hide the system title bar
    //                    // and use the custom title bar instead.
    //                    AppTitleBar.Visibility = Visibility.Visible;
    //                    sender.TitleBar.ExtendsContentIntoTitleBar = true;
    //                    break;
    //
    //                default:
    //                    // Use the default system title bar.
    //                    sender.TitleBar.ResetToDefault();
    //                    break;
    //            }
    //        }
    //    }

    // this handles updating the caption button colors correctly
    // when windows system theme is changed while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        m_DispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}
