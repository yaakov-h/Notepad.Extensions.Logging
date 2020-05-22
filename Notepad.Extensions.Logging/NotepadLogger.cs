using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Microsoft.Extensions.Logging
{
    class NotepadLogger : ILogger
    {
        public NotepadLogger(ObjectPool<StringBuilder> stringBuilderPool, string categoryName)
        {
            this.stringBuilderPool = stringBuilderPool;
            this.categoryName = categoryName;
        }

        readonly ObjectPool<StringBuilder> stringBuilderPool;
        readonly string categoryName;

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

        static void WriteToNotepad(string message)
        {
            IntPtr hwnd = FindNotepadWindow();
            IntPtr edit = NativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "EDIT", null);
            NativeMethods.SendMessage(edit, NativeMethods.EM_REPLACESEL, (IntPtr)1, message);
        }

        static IntPtr FindNotepadWindow()
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
