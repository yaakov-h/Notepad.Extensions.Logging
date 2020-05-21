using System;

namespace Microsoft.Extensions.Logging
{
    static class WindowFinder
    {
        public static IntPtr FindNotepadWindow()
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
