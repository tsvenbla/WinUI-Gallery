//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using WinRT.Interop;

namespace WinUIGallery.Helpers;

/// <summary>
/// Helper class to allow the app to find the Window that contains an arbitrary UIElement.
/// </summary>
public class WindowHelper
{
    /// <summary>
    /// Creates a new Window and tracks it.
    /// </summary>
    /// <returns>The newly created Window.</returns>
    public static Window CreateWindow()
    {
        Window newWindow = new()
        {
            SystemBackdrop = new MicaBackdrop()
        };
        TrackWindow(newWindow);
        return newWindow;
    }

    /// <summary>
    /// Tracks the specified Window.
    /// </summary>
    /// <param name="window">The Window to track.</param>
    public static void TrackWindow(Window window)
    {
        window.Closed += (sender, args) => ActiveWindows.Remove(window);
        ActiveWindows.Add(window);
    }

    /// <summary>
    /// Gets the AppWindow for the specified Window.
    /// </summary>
    /// <param name="window">The Window to get the AppWindow for.</param>
    /// <returns>The AppWindow for the specified Window.</returns>
    public static AppWindow GetAppWindow(Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }

    /// <summary>
    /// Gets the Window that contains the specified UIElement.
    /// </summary>
    /// <param name="element">The UIElement to find the containing Window for.</param>
    /// <returns>The Window that contains the specified UIElement, or null if not found.</returns>
    public static Window? GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Window window in ActiveWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the rasterization scale for the specified UIElement.
    /// </summary>
    /// <param name="element">The UIElement to get the rasterization scale for.</param>
    /// <returns>The rasterization scale for the specified UIElement.</returns>
    public static double GetRasterizationScaleForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Window window in ActiveWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return element.XamlRoot.RasterizationScale;
                }
            }
        }
        return 0.0;
    }

    /// <summary>
    /// Gets the list of active Windows.
    /// </summary>
    public static List<Window> ActiveWindows { get; } = [];

    /// <summary>
    /// Gets the local folder for the app.
    /// </summary>
    /// <returns>The local folder for the app.</returns>
    public static StorageFolder GetAppLocalFolder()
    {
        StorageFolder localFolder = !NativeHelper.IsAppPackaged
            ? Task.Run(async () => await StorageFolder.GetFolderFromPathAsync(AppContext.BaseDirectory)).Result
            : ApplicationData.Current.LocalFolder;
        return localFolder;
    }
}
