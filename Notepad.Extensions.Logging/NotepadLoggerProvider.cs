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
        }

        public static ILoggerProvider Instance { get; } = new NotepadLoggerProvider();

        readonly ObjectPool<StringBuilder> stringBuilderPool;

        public ILogger CreateLogger(string categoryName) => new NotepadLogger(stringBuilderPool, categoryName);

        public void Dispose()
        {
        }
    }
}
