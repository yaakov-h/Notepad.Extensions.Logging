using System;
using Microsoft.Extensions.ObjectPool;

namespace Notepad.Extensions.Logging
{
    class WindowFinder : IWindowFinder
    {
        public WindowFinder()
        {
            statePool = new DefaultObjectPool<WindowEnumerationState>(WindowEnumerationStatePoolingPolicy.Instance);
        }

        readonly ObjectPool<WindowEnumerationState> statePool;

        public WindowInfo FindNotepadWindow()
        {
            var stateObject = statePool.Get();
            try
            {
                NativeMethods.EnumWindows(enumWindowsDelegate, stateObject);
                return new WindowInfo(stateObject.WindowKind, stateObject.Handle);
            }
            finally
            {
                statePool.Return(stateObject);
            }
        }

        static NativeMethods.EnumWindowsDelegate enumWindowsDelegate = new NativeMethods.EnumWindowsDelegate(EnumWindowsCallback);

        static bool EnumWindowsCallback(IntPtr hWnd, object lParam)
        {
            var state = (WindowEnumerationState)lParam;
            return state.ExamineWindow(hWnd);
        }
    }
}
