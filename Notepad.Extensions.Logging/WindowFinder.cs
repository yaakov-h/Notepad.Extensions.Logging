using System;

namespace Microsoft.Extensions.Logging
{
    static class WindowFinder
    {
        public static IntPtr FindNotepadWindow()
        {
            var hwnd = FindMainWindow();
            IntPtr edit = NativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "EDIT", null);
            return edit;
        }

        static IntPtr FindMainWindow()
        {
            IntPtr hwnd;
            
            hwnd = NativeMethods.FindWindow(null, "Untitled - Notepad");
            if (hwnd != IntPtr.Zero)
            {
                return hwnd;
            }

            hwnd = NativeMethods.FindWindow(null, "*Untitled - Notepad");
            if (hwnd != IntPtr.Zero)
            {
                return hwnd;
            }

            return IntPtr.Zero;
        }
    }
}
