using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Notepad.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NotepadLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<NotepadLoggerOptions, NotepadLoggerProvider>(builder.Services);
            return builder;
        }

        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder, Action<NotepadLoggerOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddNotepad(builder);
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
