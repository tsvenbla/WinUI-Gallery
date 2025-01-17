using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.SamplePages;
using WinUIGallery.Helper;
using Windows.ApplicationModel.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Dispatching;
using WinUIGallery.TabViewPages;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Media;
using System.Collections;

namespace WinUIGallery.ControlPages
{
    public class MyData
    {
        public string DataHeader { get; set; }
        public Microsoft.UI.Xaml.Controls.IconSource DataIconSource { get; set; }
        public object DataContent { get; set; }
    }

    public sealed partial class TabViewPage : Page
    {
        ObservableCollection<MyData> myDatas;

        public TabViewPage()
        {
            this.InitializeComponent();

            // Launching isn't supported yet on Desktop
            // Blocked on Task 27517663: DCPP Preview 2 Bug: Dragging in TabView windowing sample causes app to crash
            //this.LaunchExample.Visibility = Visibility.Collapsed;

            InitializeDataBindingSampleData();
        }

#region SharedTabViewLogic
        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                (sender as TabView).TabItems.Add(CreateNewTab(i));
            }
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab(sender.TabItems.Count));
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private TabViewItem CreateNewTab(int index)
        {
            TabViewItem newItem = new TabViewItem
            {
                Header = $"Document {index}",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document },
                ContextFlyout = TabViewContextMenu
            };

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            Frame frame = new Frame();

            switch (index % 3)
            {
                case 0:
                    frame.Navigate(typeof(SamplePage1));
                    break;
                case 1:
                    frame.Navigate(typeof(SamplePage2));
                    break;
                case 2:
                    frame.Navigate(typeof(SamplePage3));
                    break;
            }

            newItem.Content = frame;

            return newItem;
        }
#endregion

#region ItemsSourceSample
        private void InitializeDataBindingSampleData()
        {
            myDatas = new ObservableCollection<MyData>();

            for (int index = 0; index < 3; index++)
            {
                myDatas.Add(CreateNewMyData(index));
            }
        }

        private MyData CreateNewMyData(int index)
        {
            var newData = new MyData
            {
                DataHeader = $"MyData Doc {index}",
                DataIconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder }
            };

            Frame frame = new Frame();

            switch (index % 3)
            {
                case 0:
                    frame.Navigate(typeof(SamplePage1));
                    break;
                case 1:
                    frame.Navigate(typeof(SamplePage2));
                    break;
                case 2:
                    frame.Navigate(typeof(SamplePage3));
                    break;
            }

            newData.DataContent = frame;

            return newData;
        }

        private void TabViewItemsSourceSample_AddTabButtonClick(TabView sender, object args)
        {
            // Add a new MyData item to the collection. TabView automatically generates a TabViewItem.
            myDatas.Add(CreateNewMyData(myDatas.Count));
        }

        private void TabViewItemsSourceSample_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            // Remove the requested MyData object from the collection.
            myDatas.Remove(args.Item as MyData);
        }
#endregion

#region KeyboardAcceleratorSample
        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var senderTabView = args.Element as TabView;
            senderTabView.TabItems.Add(CreateNewTab(senderTabView.TabItems.Count));

            args.Handled = true;
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            // Only close the selected tab if it is closeable
            if (((TabViewItem)InvokedTabView.SelectedItem).IsClosable)
            {
                InvokedTabView.TabItems.Remove(InvokedTabView.SelectedItem);
            }

            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = InvokedTabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < InvokedTabView.TabItems.Count)
            {
                InvokedTabView.SelectedIndex = tabToSelect;
            }

            args.Handled = true;
        }
#endregion

        private void TabWidthBehaviorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string widthModeString = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
            TabViewWidthMode widthMode = TabViewWidthMode.Equal;
            switch (widthModeString)
            {
                case "Equal":
                    widthMode = TabViewWidthMode.Equal;
                    break;
                case "SizeToContent":
                    widthMode = TabViewWidthMode.SizeToContent;
                    break;
                case "Compact":
                    widthMode = TabViewWidthMode.Compact;
                    break;
            }
            TabView3.TabWidthMode = widthMode;
        }

        private void TabCloseButtonOverlayModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string overlayModeString = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
            TabViewCloseButtonOverlayMode overlayMode = TabViewCloseButtonOverlayMode.Auto;
            switch (overlayModeString)
            {
                case "Auto":
                    overlayMode = TabViewCloseButtonOverlayMode.Auto;
                    break;
                case "OnHover":
                    overlayMode = TabViewCloseButtonOverlayMode.OnPointerOver;
                    break;
                case "Always":
                    overlayMode = TabViewCloseButtonOverlayMode.Always;
                    break;
            }
            TabView4.CloseButtonOverlayMode = overlayMode;
        }

        private void TabViewWindowingButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var tabViewSample = new TabViewWindowingSamplePage();

            var newWindow = WindowHelper.CreateWindow();
            newWindow.ExtendsContentIntoTitleBar = true;
            newWindow.Content = tabViewSample;
            newWindow.AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
            tabViewSample.LoadDemoData();
            tabViewSample.SetupWindowMinSize(newWindow);

            newWindow.Activate();
        }

        private void TabViewContextMenu_Opening(object sender, object e)
        {
            MenuFlyout contextMenu = (MenuFlyout)sender;
            contextMenu.Items.Clear();

            var item = (TabViewItem)contextMenu.Target;
            ListView tabViewListView = null;
            TabView tabView = null;

            DependencyObject current = item;

            while (current != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(current);

                if (parent is ListView parentTabViewListView)
                {
                    tabViewListView = parentTabViewListView;
                }
                else if (parent is TabView parentTabView)
                {
                    tabView = parentTabView;
                }

                if (tabViewListView != null && tabView != null)
                {
                    break;
                }

                current = parent;
            }

            if (tabViewListView == null || tabView == null)
            {
                return;
            }

            int index = tabViewListView.IndexFromContainer(item);

            if (index > 0)
            {
                MenuFlyoutItem moveLeftItem = new() { Text = "Move tab left" };
                moveLeftItem.Click += (s, args) =>
                {
                    if (tabView.TabItemsSource is IList itemsSourceList)
                    {
                        var item = itemsSourceList[index];
                        itemsSourceList.RemoveAt(index);
                        itemsSourceList.Insert(index - 1, item);
                    }
                    else
                    {
                        var item = tabView.TabItems[index];
                        tabView.TabItems.RemoveAt(index);
                        tabView.TabItems.Insert(index - 1, item);
                    }
                };
                contextMenu.Items.Add(moveLeftItem);
            }

            if (index < tabViewListView.Items.Count - 1)
            {
                MenuFlyoutItem moveRightItem = new() { Text = "Move tab right" };
                moveRightItem.Click += (s, args) =>
                {
                    if (tabView.TabItemsSource is IList itemsSourceList)
                    {
                        var item = itemsSourceList[index];
                        itemsSourceList.RemoveAt(index);
                        itemsSourceList.Insert(index + 1, item);
                    }
                    else
                    {
                        var item = tabView.TabItems[index];
                        tabView.TabItems.RemoveAt(index);
                        tabView.TabItems.Insert(index + 1, item);
                    }
                };
                contextMenu.Items.Add(moveRightItem);
            }
        }
    }
}
