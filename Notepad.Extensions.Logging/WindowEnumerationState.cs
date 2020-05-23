using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Notepad.Extensions.Logging
{
    class WindowEnumerationState
    {
        public WindowEnumerationState()
        {
            sb = new StringBuilder(capacity: 4096);
        }

        readonly StringBuilder sb;

        public IntPtr Handle { get; private set; }

        public WindowKind WindowKind { get; private set; }

        public string WindowName { get; set; }

        public void Reset()
        {
            Handle = default;
            WindowKind = default;
            WindowName = default;
            sb.Clear();
        }

        public bool ExamineWindow(IntPtr hWnd)
        {
            var result = NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
            if (result < 0)
            {
                throw new Win32Exception(result);
            }

            Handle = hWnd;

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

        static Regex notepadPlusPlusRegex = new Regex(@"^new \d+ - Notepad\+\+$", RegexOptions.Compiled);

        bool IsKnownNotepadWindow(string titleText)
        {
            if (!string.IsNullOrWhiteSpace(WindowName))
            {
                if (WindowName.Equals(titleText, StringComparison.Ordinal))
                {
                    WindowKind = titleText.EndsWith(" - Notepad++", StringComparison.Ordinal) ? WindowKind.NotepadPlusPlus : WindowKind.Notepad;
                }
            }
            else if (titleText.Equals("Untitled - Notepad", StringComparison.Ordinal))
            {
                WindowKind = WindowKind.Notepad;
            }
            else if (notepadPlusPlusRegex.IsMatch(titleText))
            {
                WindowKind = WindowKind.NotepadPlusPlus;
            }

            Handle = FindInnerWindow(WindowKind);

            return WindowKind != default;
        }

        IntPtr FindInnerWindow(WindowKind windowKind)
        {
            switch (windowKind)
            {
                case WindowKind.Notepad:
                    return NativeMethods.FindWindowEx(Handle, IntPtr.Zero, "EDIT", null);
                case WindowKind.NotepadPlusPlus:
                    return NativeMethods.FindWindowEx(Handle, IntPtr.Zero, "Scintilla", null);
                default:
                    return Handle;
            }
        }
    }
}
