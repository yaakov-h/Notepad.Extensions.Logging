﻿namespace Notepad.Extensions.Logging
{
    interface IWindowFinder
    {
        WindowInfo FindNotepadWindow(string windowName);
    }
}