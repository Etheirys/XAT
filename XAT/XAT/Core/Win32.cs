
//
// This code is from the Zero Electric Framework and provided to you under the MIT license 
//

//
// MIT License
// 
// Copyright (c) 2025 Ken M (minmoose), Zero Electric
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace XAT.Core;

internal partial class Win32
{
    /// <summary>
    /// Support dark/light mode switching on Windows 10+, based upon: https://github.com/libsdl-org/SDL/issues/4776#issuecomment-926976455
    /// </summary>
    public static class WindowTheming
    {
        #region Win32 P/Invoke


        [DllImport("uxtheme.dll", SetLastError = true)]
        private static extern bool SetWindowTheme(IntPtr handle, string? subAppName, string? subIDList);

        [DllImport("dwmapi.dll", SetLastError = true)]
        private static extern bool DwmSetWindowAttribute(IntPtr handle, int param, in int value, int size);


        #endregion

        //

        const string REGISTRY_VAL_USE_LIGHT_THEME = "SystemUsesLightTheme";
        const string REGISTRY_KEY_WIN_THEME = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private static IntPtr WindowHandle;
        private static bool IsInitialized = false;

        //

        public static bool IsCurrentThemeDarkMode { get; private set; }

        public static void SetWindowThemeAware(IntPtr handle)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10))
            {
                return;
            }
            IsInitialized = true;

            WindowHandle = handle;

            SetWindowTheme(WindowHandle, "DarkMode_Explorer", null);

            CheckForThemeChange();
        }

        public static void CheckForThemeChange()
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10) || IsInitialized == false)
            {
                return;
            }

            if (Registry.GetValue(REGISTRY_KEY_WIN_THEME, REGISTRY_VAL_USE_LIGHT_THEME, null) is int value)
            {
                bool shouldBeDarkMode = value == 0;

                if (IsCurrentThemeDarkMode != shouldBeDarkMode)
                {
                    SetWindowTheme(shouldBeDarkMode);
                    IsCurrentThemeDarkMode = shouldBeDarkMode;
                }
            }
        }

        public static void SetWindowTheme(bool value)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10) || IsInitialized == false)
            {
                return;
            }

            if (!DwmSetWindowAttribute(WindowHandle, 20, value ? 1 : 0, sizeof(int)))
            {
                DwmSetWindowAttribute(WindowHandle, 19, value ? 1 : 0, sizeof(int));
            }
        }
    }
}