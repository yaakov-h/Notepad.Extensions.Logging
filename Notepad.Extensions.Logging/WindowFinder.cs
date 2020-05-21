using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Logging
{
    static class WindowFinder
    {
        public static IntPtr FindNotepadWindow()
        {
            sb ??= new StringBuilder(4096);

            try
            {
                FindMainWindow();
                return handle;
            }
            finally
            {
                handle = IntPtr.Zero;
                sb.Clear();
            }
        }

        static IntPtr FindMainWindow()
        {
            NativeMethods.EnumWindows(enumWindowsDelegate, IntPtr.Zero);
            return handle;
        }

        static NativeMethods.EnumWindowsDelegate enumWindowsDelegate = new NativeMethods.EnumWindowsDelegate(EnumWindowsCallback);

        static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            var result = NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
            if (result < 0)
            {
                throw new Win32Exception(result);
            }

            WindowFinder.handle = hWnd;

            if (IsKnownNotepadWindow(sb.ToString()))
            {
                return false;
            }
            return true;
        }

        [ThreadStatic]
        static IntPtr handle;

        [ThreadStatic]
        static StringBuilder sb;

        static Regex notepadPlusPlusRegex = new Regex(@"^new \d+ - Notepad\+\+$", RegexOptions.Compiled);

        static bool IsKnownNotepadWindow(string titleText)
        {
            switch (titleText)
            {
                case "Untitled - Notepad":
                case "*Untitled - Notepad":
                    handle = NativeMethods.FindWindowEx(handle, IntPtr.Zero, "EDIT", null);
                    return true;
            }

            if (notepadPlusPlusRegex.IsMatch(titleText))
            {
                handle = NativeMethods.FindWindowEx(handle, IntPtr.Zero, "SysTabControl32", null);
                return true;
            }

            return false;
        }
    }
}
