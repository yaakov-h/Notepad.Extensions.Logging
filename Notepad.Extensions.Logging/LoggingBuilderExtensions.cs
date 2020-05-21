using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder)
             => AddNotepad(builder, null);

        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder, Action<NotepadProviderOptions> optionsConfiguration)
        {
            var options = NotepadProviderOptions.Default;
            optionsConfiguration?.Invoke(options);
            builder.AddProvider(new NotepadLoggerProvider(options));
            return builder;
        }
    }
}
