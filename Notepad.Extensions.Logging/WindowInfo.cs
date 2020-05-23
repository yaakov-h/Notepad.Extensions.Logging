using System;

namespace Notepad.Extensions.Logging
{
    struct WindowInfo
    {
        public WindowInfo(WindowKind kind, IntPtr handle)
        {
            Kind = kind;
            Handle = handle;
        }

        public WindowKind Kind { get; }

        public IntPtr Handle { get; }
    }
}