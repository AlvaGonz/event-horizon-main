using System;

namespace Gui.Windows
{
    public enum WindowExitCode
    {
        Ok,
        Cancel,
        Error
    }

    public static class Window
    {
        public static void Open() { }
        public static void Close(WindowExitCode code) { }
    }
}
