using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Microsoft.Extensions.Logging
{
    class NotepadLoggerProvider : ILoggerProvider
    {
        NotepadLoggerProvider()
        {
            var poolProvider = new DefaultObjectPoolProvider();
            stringBuilderPool = poolProvider.CreateStringBuilderPool();
        }

        internal NotepadLoggerProvider(NotepadProviderOptions options) : this()
        {
            this.options = options;
        }

        readonly ObjectPool<StringBuilder> stringBuilderPool;
        readonly NotepadProviderOptions options;

        public ILogger CreateLogger(string categoryName) => new NotepadLogger(stringBuilderPool, categoryName, options.WindowName);

        public void Dispose()
        {
        }
    }
}
