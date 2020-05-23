using Microsoft.Extensions.ObjectPool;

namespace Notepad.Extensions.Logging
{
    class WindowEnumerationStatePoolingPolicy : IPooledObjectPolicy<WindowEnumerationState>
    {
        public static IPooledObjectPolicy<WindowEnumerationState> Instance { get; } = new WindowEnumerationStatePoolingPolicy();

        public WindowEnumerationState Create() => new WindowEnumerationState();

        public bool Return(WindowEnumerationState obj)
        {
            obj.Reset();
            return true;
        }
    }
}
