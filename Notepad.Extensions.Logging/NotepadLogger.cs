using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Microsoft.Extensions.Logging
{
    class NotepadLogger : ILogger
    {
        public NotepadLogger(ObjectPool<StringBuilder> stringBuilderPool, string categoryName, string windowName = null)
        {
            this.stringBuilderPool = stringBuilderPool;
            this.categoryName = categoryName;
            this.windowName = windowName;
        }

        readonly ObjectPool<StringBuilder> stringBuilderPool;
        readonly string categoryName;
        readonly string windowName;

        public IDisposable BeginScope<TState>(TState state) => NullDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception is null)
            {
                return;
            }

            WriteMessage(logLevel, eventId.Id, message, exception);
        }

        void WriteMessage(LogLevel logLevel, int eventId, string message, Exception ex)
        {
            var builder = stringBuilderPool.Get();
            try
            {
                builder
                    .Append('[')
                    .Append(GetLogLevelString(logLevel))
                    .Append("] ")
                    .Append(categoryName)
                    .Append(" (")
                    .Append(eventId)
                    .Append(") ")
                    .AppendLine(message);

                if (ex is { })
                {
                    builder.Append("    ");
                    builder.AppendLine(ex.ToString());
                }

                WriteToNotepad(builder.ToString());
            }
            finally
            {
                stringBuilderPool.Return(builder);
            }
        }

        static string GetLogLevelString(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };

        void WriteToNotepad(string message)
        {
            IntPtr hwnd = FindNotepadWindow();

            if (hwnd.Equals(IntPtr.Zero))
            {
                return;
            }

            IntPtr edit = NativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "EDIT", null);
            NativeMethods.SendMessage(edit, NativeMethods.EM_REPLACESEL, (IntPtr)1, message);
        }

        IntPtr FindNotepadWindow()
        {
            IntPtr hwnd;

            hwnd = NativeMethods.FindWindow(null, windowName);
            if (hwnd.Equals(IntPtr.Zero))
            {
                // when the file changes, notepad changes the name to "* Window Name", so later created loggers cannot find window
                var builder = stringBuilderPool.Get();
                builder.Append("*").Append(windowName);
                hwnd = NativeMethods.FindWindow(null, builder.ToString());
                stringBuilderPool.Return(builder);
            }
            return hwnd;
        }
    }

    static class NativeMethods
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        public const int EM_REPLACESEL = 0x00C2;

        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
    }
}
