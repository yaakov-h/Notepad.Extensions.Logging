using System;
using System.ComponentModel;
using System.Text;

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
                if (handle == IntPtr.Zero)
                {
                    return handle;
                }

                IntPtr edit = NativeMethods.FindWindowEx(handle, IntPtr.Zero, "EDIT", null);
                return edit;
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

            if (IsKnownNotepadWindow(sb.ToString()))
            {
                WindowFinder.handle = hWnd;
                return false;
            }
            return true;
        }

        [ThreadStatic]
        static IntPtr handle;

        [ThreadStatic]
        static StringBuilder sb;

        static bool IsKnownNotepadWindow(string titleText)
        {
            switch (titleText)
            {
                case "Untitled - Notepad":
                case "*Untitled - Notepad":
                    return true;
            }

            return false;
        }
    }
}
