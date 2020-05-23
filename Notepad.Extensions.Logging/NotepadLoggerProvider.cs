using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Notepad.Extensions.Logging
{
    [ProviderAlias("Notepad")]
    class NotepadLoggerProvider : ILoggerProvider
    {
        NotepadLoggerProvider()
        {
            var poolProvider = new DefaultObjectPoolProvider();
            stringBuilderPool = poolProvider.CreateStringBuilderPool();
            windowFinder = new WindowFinder();
        }

        public NotepadLoggerProvider(IOptionsMonitor<NotepadLoggerOptions> options) : this()
        {
            // Filter would be applied on LoggerFactory level
            optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            ReloadLoggerOptions(options.CurrentValue);
        }

        public NotepadLoggerProvider(NotepadLoggerOptions options) : this()
        {
            this.options = options;
        }
        readonly ObjectPool<StringBuilder> stringBuilderPool;
        readonly IWindowFinder windowFinder;
        readonly IDisposable optionsReloadToken;
        NotepadLoggerOptions options;
        
        public ILogger CreateLogger(string categoryName) => new NotepadLogger(stringBuilderPool, windowFinder, categoryName, options.WindowName);

        public void Dispose()
        {
            optionsReloadToken?.Dispose();
        }

        void ReloadLoggerOptions(NotepadLoggerOptions options)
        {
            this.options = options;
        }
    }
}
