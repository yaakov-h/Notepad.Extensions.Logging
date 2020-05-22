using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Notepad.Extensions.Logging
{
    static class WindowFinder
    {
        public static (WindowKind kind, IntPtr hwnd) FindNotepadWindow()
        {
            sb ??= new StringBuilder(4096);

            try
            {
                FindMainWindow();
                return (windowKind, handle);
            }
            finally
            {
                handle = IntPtr.Zero;
                sb.Clear();
                windowKind = WindowKind.Invalid;
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

            if (sb.Length > 0 && sb[0] == '*')
            {
                // Notepad and Notepad++ both mark dirty documents by adding a leading asterisk to the window name.
                sb.Remove(0, 1);
            }

            if (IsKnownNotepadWindow(sb.ToString()))
            {
                return false;
            }
            return true;
        }

        [ThreadStatic]
        static IntPtr handle;

        [ThreadStatic]
        static WindowKind windowKind;

        [ThreadStatic]
        static StringBuilder sb;

        static Regex notepadPlusPlusRegex = new Regex(@"^new \d+ - Notepad\+\+$", RegexOptions.Compiled);

        static bool IsKnownNotepadWindow(string titleText)
        {
            switch (titleText)
            {
                case "Untitled - Notepad":
                    windowKind = WindowKind.Notepad;
                    handle = NativeMethods.FindWindowEx(handle, IntPtr.Zero, "EDIT", null);
                    return true;
            }

            if (notepadPlusPlusRegex.IsMatch(titleText))
            {
                windowKind = WindowKind.NotepadPlusPlus;
                handle = NativeMethods.FindWindowEx(handle, IntPtr.Zero, "Scintilla", null);
                return true;
            }

            return false;
        }
    }
}
