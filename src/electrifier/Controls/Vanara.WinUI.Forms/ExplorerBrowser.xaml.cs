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
using static Vanara.Windows.Forms.ExplorerBrowser;
using Vanara.Collections;
using System.Collections;
using System.Runtime.InteropServices;

//namespace Vanara.Windows.Forms;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
// and more about our project templates, see: http://aka.ms/winui-project-info.

//public static readonly DependencyProperty SelectedPathProperty =
//    DependencyProperty.Register("SelectedPath", typeof(string), typeof(ExplorerBrowser), new PropertyMetadata(null, OnSelectedPathChanged));
//private static void OnSelectedPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
//    => throw new NotImplementedException();
//public string SelectedPath;

namespace electrifier.Controls.Vanara.WinUI.Forms;

public partial class ExplorerBrowser : UserControl
{
    public NavigationLog History
    {
        get;
    }
    private ShellItemCollection Items
    {
        get;
    }
    private ShellItemCollection SelectedItems
    {
        get;
    }
    public ExplorerBrowser()
    {
        this.InitializeComponent();

        //SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        History = new NavigationLog(this);
        Items = new ShellItemCollection(this, SVGIO.SVGIO_ALLVIEW);
        SelectedItems = new ShellItemCollection(this, SVGIO.SVGIO_SELECTION);
    }

    //
    // Summary:
    //     The navigation log is a history of the locations visited by the explorer browser.
    public class NavigationLog
    {
        //
        // Summary:
        //     A navigation traversal request
        private class PendingNavigation
        {
            internal int Index
            {
                get; set;
            }

            internal ShellItem Location
            {
                get; set;
            }

            internal PendingNavigation(ShellItem location, int index)
            {
                Location = location;
                Index = index;
            }
        }

        private readonly ExplorerBrowser? parent;

        //
        // Summary:
        //     The pending navigation log action. null if the user is not navigating via the
        //     navigation log.
        private PendingNavigation? pendingNavigation;

        //
        // Summary:
        //     Indicates the presence of locations in the log that can be reached by calling
        //     Navigate(Backward)
        public bool CanNavigateBackward => CurrentLocationIndex > 0;

        //
        // Summary:
        //     Indicates the presence of locations in the log that can be reached by calling
        //     Navigate(Forward)
        public bool CanNavigateForward => CurrentLocationIndex < Locations.Count - 1;

        //
        // Summary:
        //     Gets the shell object in the Locations collection pointed to by CurrentLocationIndex.
        public ShellItem? CurrentLocation
        {
            get
            {
                if (CurrentLocationIndex >= 0)
                {
                    return Locations[CurrentLocationIndex];
                }

                return null;
            }
        }

        //
        // Summary:
        //     An index into the Locations collection. The ShellItem pointed to by this index
        //     is the current location of the ExplorerBrowser.
        public int CurrentLocationIndex { get; set; } = -1;


        //
        // Summary:
        //     The navigation log
        public List<ShellItem> Locations { get; } = new List<ShellItem>();


        //
        // Summary:
        //     Fires when the navigation log changes or the current navigation position changes
        public event EventHandler<NavigationLogEventArgs>? NavigationLogChanged;

        internal NavigationLog(ExplorerBrowser parent)
        {
            this.parent = parent ?? throw new ArgumentNullException("parent");
            // TODO:            this.parent.Navigated += OnNavigated;
            // TODO:            this.parent.NavigationFailed += OnNavigationFailed;
        }

        //
        // Summary:
        //     Clears the contents of the navigation log.
        public void Clear()
        {
            if (Locations.Count != 0)
            {
                bool canNavigateBackward = CanNavigateBackward;
                bool canNavigateForward = CanNavigateForward;
                Locations.Clear();
                CurrentLocationIndex = -1;
                NavigationLogEventArgs e = new NavigationLogEventArgs
                {
                    LocationsChanged = true,
                    CanNavigateBackwardChanged = (canNavigateBackward != CanNavigateBackward),
                    CanNavigateForwardChanged = (canNavigateForward != CanNavigateForward)
                };
                this.NavigationLogChanged?.Invoke(this, e);
            }
        }

        internal bool NavigateLog(NavigationLogDirection direction)
        {
            int index;
            if (direction != 0)
            {
                if (direction != NavigationLogDirection.Backward || !CanNavigateBackward)
                {
                    goto IL_002f;
                }

                index = CurrentLocationIndex - 1;
            }
            else
            {
                if (!CanNavigateForward)
                {
                    goto IL_002f;
                }

                index = CurrentLocationIndex + 1;
            }

            ShellItem shellItem = Locations[index];
            pendingNavigation = new PendingNavigation(shellItem, index);
            //            parent?.Navigate(shellItem);
            return true;
        IL_002f:
            return false;
        }

        internal bool NavigateLog(int index)
        {
            if (index >= Locations.Count || index < 0)
            {
                return false;
            }

            if (index == CurrentLocationIndex)
            {
                return false;
            }

            ShellItem shellItem = Locations[index];
            pendingNavigation = new PendingNavigation(shellItem, index);
            // TODO:            parent?.Navigate(shellItem);
            return true;
        }

        private void OnNavigated(object? sender, NavigatedEventArgs args)
        {
            NavigationLogEventArgs navigationLogEventArgs = new NavigationLogEventArgs();
            bool canNavigateBackward = CanNavigateBackward;
            bool canNavigateForward = CanNavigateForward;
            if (pendingNavigation != null)
            {
                if (pendingNavigation.Location.IShellItem.Compare(args.NewLocation?.IShellItem, Shell32.SICHINTF.SICHINT_ALLFIELDS) != 0)
                {
                    if (CurrentLocationIndex < Locations.Count - 1)
                    {
                        Locations.RemoveRange(CurrentLocationIndex + 1, Locations.Count - (CurrentLocationIndex + 1));
                    }

                    if ((object)args.NewLocation != null)
                    {
                        Locations.Add(args.NewLocation);
                    }

                    CurrentLocationIndex = Locations.Count - 1;
                    navigationLogEventArgs.LocationsChanged = true;
                }
                else
                {
                    CurrentLocationIndex = pendingNavigation.Index;
                    navigationLogEventArgs.LocationsChanged = false;
                }

                pendingNavigation = null;
            }
            else
            {
                if (CurrentLocationIndex < Locations.Count - 1)
                {
                    Locations.RemoveRange(CurrentLocationIndex + 1, Locations.Count - (CurrentLocationIndex + 1));
                }

                if ((object)args.NewLocation != null)
                {
                    Locations.Add(args.NewLocation);
                }

                CurrentLocationIndex = Locations.Count - 1;
                navigationLogEventArgs.LocationsChanged = true;
            }

            navigationLogEventArgs.CanNavigateBackwardChanged = canNavigateBackward != CanNavigateBackward;
            navigationLogEventArgs.CanNavigateForwardChanged = canNavigateForward != CanNavigateForward;
            this.NavigationLogChanged?.Invoke(this, navigationLogEventArgs);
        }

        private void OnNavigationFailed(object? sender, Vanara.WinUI.Forms.ExplorerBrowser.NavigationFailedEventArgs args)
        {
            pendingNavigation = null;
        }
    }


    /// <summary>Represents a collection of <see cref="ShellItem"/> attached to an <see cref="ExplorerBrowser"/>.</summary>
    private class ShellItemCollection : IReadOnlyList<ShellItem>
    {
        private readonly ExplorerBrowser eb;
        private readonly SVGIO option;

        internal ShellItemCollection(ExplorerBrowser eb, SVGIO opt)
        {
            this.eb = eb;
            option = opt;
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <value>Returns a <see cref="int"/> value.</value>
        public int Count
        {
            get
            {
                return Items.Count();
            }
        }

        private IEnumerable<IShellItem> Items
        {
            get
            {
                return Enumerable.Empty<IShellItem>();
                //var array = eb.GetItemsArray(option);
                //try
                //{
                //    return array is null ? Enumerable.Empty<IShellItem>() : array;
                //}
                //finally
                //{
                //    if (array != null)
                //        Marshal.ReleaseComObject(array);
                //}
            }
        }

        /// <summary>Gets the <see cref="ShellItem"/> at the specified index.</summary>
        /// <value>The <see cref="ShellItem"/>.</value>
        /// <param name="index">The zero-based index of the element to get.</param>
        public ShellItem this[int index]
        {
            get
            {
                return ShellItem.Open(Items.ElementAt(index));
                //var array = Items;
                //try
                //{
                //    return array is null ? null : ShellItem.Open(array.GetItemAt((uint)index));
                //}
                //catch
                //{
                //    return null;
                //}
                //finally
                //{
                //    if (array != null)
                //        Marshal.ReleaseComObject(array);
                //}
            }
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ShellItem> GetEnumerator() => Items.Select(ShellItem.Open).GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private ShellItemCollection GetItemsArray(SVGIO option)
    {
        return Items;
    }

    /// <summary>The event argument for NavigationLogChangedEvent</summary>
    public class NavigationLogEventArgs : EventArgs
    {
        /// <summary>Indicates CanNavigateBackward has changed</summary>
        public bool CanNavigateBackwardChanged
        {
            get; set;
        }

        /// <summary>Indicates CanNavigateForward has changed</summary>
        public bool CanNavigateForwardChanged
        {
            get; set;
        }

        /// <summary>Indicates the Locations collection has changed</summary>
        public bool LocationsChanged
        {
            get; set;
        }
    }

    /// <summary>Event argument for The Navigated event</summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>The new location of the explorer browser</summary>
        public ShellItem? NewLocation
        {
            get; set;
        }
    }

    /// <summary>Event argument for The Navigating event</summary>
    public class NavigatingEventArgs : EventArgs
    {
        /// <summary>Set to 'True' to cancel the navigation.</summary>
        public bool Cancel
        {
            get; set;
        }

        /// <summary>The location being navigated to</summary>
        public ShellItem? PendingLocation
        {
            get; set;
        }
    }

    /// <summary>Event argument for the NavigatinoFailed event</summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>The location the browser would have navigated to.</summary>
        public ShellItem? FailedLocation
        {
            get; set;
        }
    }

}
