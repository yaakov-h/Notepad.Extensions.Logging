using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Notepad.Extensions.Logging
{
    class NotepadLoggerProvider : ILoggerProvider
    {
        NotepadLoggerProvider()
        {
            var poolProvider = new DefaultObjectPoolProvider();
            stringBuilderPool = poolProvider.CreateStringBuilderPool();
            windowFinder = new WindowFinder();
        }

        public static ILoggerProvider Instance { get; } = new NotepadLoggerProvider();

        readonly ObjectPool<StringBuilder> stringBuilderPool;
        readonly IWindowFinder windowFinder;

        public ILogger CreateLogger(string categoryName) => new NotepadLogger(stringBuilderPool, windowFinder, categoryName);

        public void Dispose()
        {
        }
    }
}
