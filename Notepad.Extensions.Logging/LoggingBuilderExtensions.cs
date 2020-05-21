namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddNotepad(this ILoggingBuilder builder)
        {
            builder.AddProvider(NotepadLoggerProvider.Instance);
            return builder;
        }
    }
}
