using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Notepad.Extensions.Logging;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NotepadLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<NotepadLoggerOptions, NotepadLoggerProvider>(builder.Services);
            return builder;
        }

        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder, Action<NotepadLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            AddNotepad(builder);
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
