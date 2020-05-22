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
        readonly ObjectPool<StringBuilder> stringBuilderPool;
        readonly IDisposable optionsReloadToken;
        NotepadLoggerOptions options;

        NotepadLoggerProvider()
        {
            var poolProvider = new DefaultObjectPoolProvider();
            stringBuilderPool = poolProvider.CreateStringBuilderPool();
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

        public ILogger CreateLogger(string categoryName) => new NotepadLogger(stringBuilderPool, categoryName, options.WindowName);

        public void Dispose()
        {
            optionsReloadToken?.Dispose();
        }

        private void ReloadLoggerOptions(NotepadLoggerOptions options)
        {
            this.options = options;
        }
    }
}
