using System;

namespace Notepad.Extensions.Logging
{
    class NullDisposable : IDisposable
    {
        public static IDisposable Instance { get; } = new NullDisposable();

        public void Dispose()
        {
        }
    }
}