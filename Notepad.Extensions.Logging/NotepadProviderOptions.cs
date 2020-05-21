using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public class NotepadProviderOptions
    {
        /// <summary>
        /// Default options for <see cref="NotepadLoggerProvider"/>.
        /// </summary>
        public static NotepadProviderOptions Default { get; } = new NotepadProviderOptions
        {
            WindowName = "Untitled - Notepad",
        };
        /// <summary>
        /// Name of window to search.
        /// </summary>
        public string WindowName { get; set; }
    }
}
