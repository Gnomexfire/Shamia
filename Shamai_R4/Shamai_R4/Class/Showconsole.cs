﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shamai_R4.Class
{
    public static class Showconsole
    { 
        #region declare
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SwHide = 0;
        const int SwShow = 5;
        #endregion

        public static void ShowConsole(bool show=true)
        {
            var handle = GetConsoleWindow();
            if (show)
            {
                ShowWindow(handle, SwShow);
                return;
            }
            ShowWindow(handle, SwHide);
        }

        public static bool ConsoleIsShow()
        {
            var handle = GetConsoleWindow();
            return handle != IntPtr.Zero;
        }
    }
}
