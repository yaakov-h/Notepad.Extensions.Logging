using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using static Notepad.Extensions.Logging.NativeMethods;

namespace Notepad.Extensions.Logging
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
            var (kind, hwnd) = WindowFinder.FindNotepadWindow();
            switch (kind)
            {
                case WindowKind.Notepad:
                        SendMessage(hwnd, EM_REPLACESEL, (IntPtr)1, message);
                    break;

                case WindowKind.NotepadPlusPlus:
                    {
                        WriteToNotepadPlusPlus(hwnd, message);
                        break;
                    }
            }
        }

        unsafe static void WriteToNotepadPlusPlus(IntPtr hwnd, string message)
        {
            var dataLength = Encoding.UTF8.GetByteCount(message);

            //
            // HERE BE DRAGONS
            // We need to copy the message into Notepad++'s memory so that it can read it.
            // Look away now, before its too late.
            // 
 
            var threadID = GetWindowThreadProcessId(hwnd, out var remoteProcessId);
            using var remoteProcess = Process.GetProcessById(remoteProcessId);
            var mem = VirtualAllocEx(remoteProcess.Handle, IntPtr.Zero, (IntPtr)dataLength, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (mem == IntPtr.Zero) throw new Win32Exception();

            try
            {
                var data = ArrayPool<byte>.Shared.Rent(dataLength);
                try
                {
                    var idx = Encoding.UTF8.GetBytes(message, 0, message.Length, data, 0);

                    WriteProcessMemory(remoteProcess.Handle, mem, data, (IntPtr)dataLength, out var bytesWritten);
                    SendMessage(hwnd, SCI_ADDTEXT, (IntPtr)dataLength, mem);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
            }
            finally
            {
                VirtualFreeEx(remoteProcess.Handle, IntPtr.Zero, IntPtr.Zero, MEM_RELEASE);
            }
        }
    }
}
