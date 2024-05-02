using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.Shell32;

namespace electrifier.Controls.Vanara.WinUI.Forms;

/// <summary>
/// Interface for the (WinUI3 based) <see cref="ExplorerBrowser"/> control.
/// It is a clone of <a href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-iexplorerbrowser">Win32API - ExplorerBrowser</a>.
/// </summary>
internal interface IWinUIExplorerBrowser
{
    /// <summary>The navigation log is a history of the locations visited by the <see cref="ExplorerBrowser"/>.</summary>
    public class NavigationLog
    {
        /// <summary>A navigation traversal request.</summary>
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

        private readonly ExplorerBrowser parent;

        // <summary>The pending navigation log action. null if the user is not navigating via the navigation log.</summary>
        private PendingNavigation? pendingNavigation;

        /// <summary>Indicates the presence of locations in the log that can be reached by calling Navigate(Backward).</summary>
        public bool CanNavigateBackward => CurrentLocationIndex > 0;

        // <summary>Indicates the presence of locations in the log that can be reached by calling Navigate(Forward).</summary>
        public bool CanNavigateForward => CurrentLocationIndex < Locations.Count - 1;

        // <summary>Gets the shell object in the Locations collection pointed to by CurrentLocationIndex.</summary>
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

        // <summary>Gets or sets the current location index, an index into the Locations collection.
        // The ShellItem pointed to by this index is the current location of the ExplorerBrowser.
        // <value>The current location index.</value>
        // <remarks>Setting this property will cause the <see cref="NavigationLogChanged"/> event to fire.</remarks>
        // </summary>
        public int CurrentLocationIndex { get; set; } = -1;
        


        // <summary>The navigation log</summary>
        public List<ShellItem> Locations { get; } = new List<ShellItem>();


        //
        // Summary:
        //     Fires when the navigation log changes or the current navigation position changes
        public event EventHandler<NavigationLogEventArgs>? NavigationLogChanged;

        internal NavigationLog(IWinUIExplorerBrowser parent)
        {
            this.parent = (ExplorerBrowser)(parent ?? throw new ArgumentNullException("parent"));
            //this.parent.Navigated += OnNavigated;
            //this.parent.NavigationFailed += OnNavigationFailed;
        }

        //
        // Summary:
        //     Clears the contents of the navigation log.
        public void Clear()
        {
            if (Locations.Count != 0)
            {
                var canNavigateBackward = CanNavigateBackward;
                var canNavigateForward = CanNavigateForward;
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
            parent.NavigateTo(shellItem);
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

            var shellItem = Locations[index];
            pendingNavigation = new PendingNavigation(shellItem, index);
            // TODO:            parent?.Navigate(shellItem);
            return true;
        }

        private void OnNavigated(object? sender, NavigatedEventArgs args)
        {
            NavigationLogEventArgs navigationLogEventArgs = new NavigationLogEventArgs();
            var canNavigateBackward = CanNavigateBackward;
            var canNavigateForward = CanNavigateForward;
            if (pendingNavigation != null)
            {
                if (pendingNavigation.Location.IShellItem.Compare(args.NewLocation?.IShellItem, Shell32.SICHINTF.SICHINT_ALLFIELDS) != 0)
                {
                    if (CurrentLocationIndex < Locations.Count - 1)
                    {
                        Locations.RemoveRange(CurrentLocationIndex + 1, Locations.Count - (CurrentLocationIndex + 1));
                    }

                    if (args.NewLocation != null)
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

                if (args.NewLocation != null)
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

        private void OnNavigationFailed(object? sender, /* Vanara.WinUI.Forms.ExplorerBrowser. */ NavigationFailedEventArgs args)
        {
            pendingNavigation = null;
        }
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

    /// <summary>Event argument for the NavigationFailed event</summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>The location the browser would have navigated to.</summary>
        public ShellItem? FailedLocation
        {
            get; set;
        }
    }

    /// <summary>Represents a collection of <see cref="ShellItem"/> attached to an <see cref="ExplorerBrowser"/>.</summary>
    public class ShellItemCollection : IReadOnlyList<ShellItem>
    {
        private readonly IWinUIExplorerBrowser eb;
        private readonly SVGIO option;
        private readonly List<ShellItem> items;

        internal ShellItemCollection(IWinUIExplorerBrowser eb, SVGIO opt)
        {
            this.eb = eb;
            option = opt;

            items = new List<ShellItem>();
            // new List<IShellItem>() => eb.GetItemsArray(option);
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <value>Returns a <see cref="int"/> value.</value>
        public int Count => items.Count();

        private /*ShellItemCollection */ List<ShellItem> Items => items;
        //var array = eb.GetItemsArray(option);
        //try
        //{
        //    return array is null ? Enumerable.Empty<IShellItem>() : array;
        //}

        /// <summary>Gets the <see cref="ShellItem"/> at the specified index.</summary>
        /// <value>The <see cref="ShellItem"/>.</value>
        /// <param name="index">The zero-based index of the element to get.</param>
        public ShellItem this[int index] => items[index];

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ShellItem> GetEnumerator()
            //=> Items.Select(ShellItem.Open).GetEnumerator();
            => items.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
