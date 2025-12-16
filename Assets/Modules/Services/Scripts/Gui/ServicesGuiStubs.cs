using System;
using System.Collections.Generic;

namespace Services.Gui
{
    public interface IGuiManager
    {
        void OpenWindow(string windowName, object args = null, Action<WindowExitCode> callback = null);
        void CloseWindow(string windowName); // Hypothesized
    }

    public class WindowArgs
    {
        public WindowArgs(params object[] args) { }
    }

    public enum WindowExitCode
    {
        Ok,
        Cancel,
        None
    }
}

namespace Gui
{
    public interface IWindow { }
    
    public class AnimatedWindow : UnityEngine.MonoBehaviour, IWindow 
    {
        public void Close() { }
    }
    public class ListScrollRect : UnityEngine.MonoBehaviour { }
    
    public interface IContentFiller { }

    namespace Theme
    {
        public class ThemeColor : UnityEngine.MonoBehaviour { }
    }

    namespace Windows
    {
        // Stubs for types/namespaces used as 'using Gui.Windows;'
    }

    namespace Notifications
    {
        public static class WindowNames
        {
            public const string ItemInfoWindow = "ItemInfoWindow";
            public const string LootWindow = "LootWindow";
        }
        
        public class ItemDescriptionWindow : AnimatedWindow { }
        public class LootPanel : AnimatedWindow { }
    }

    namespace Common
    {
        public static class WindowNames
        {
            public const string ConfirmationDialog = "ConfirmationDialog";
            public const string BuyConfirmationDialog = "BuyConfirmationDialog";
            public const string MessageBoxWindow = "MessageBoxWindow";
        }
    }
}

namespace Gui.Components
{
    // Possible location for ListScrollRect if not in root
}

// Global scope stubs if needed, but handled by namespaces above.
