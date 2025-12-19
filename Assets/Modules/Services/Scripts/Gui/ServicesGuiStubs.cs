using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Gui
{
    public interface IGuiManager
    {
        bool AutoWindowsAllowed { get; set; }
        void OpenWindow(string windowName, object args = null, Action<global::Gui.Windows.WindowExitCode> callback = null);
        void CloseWindow(string windowName);
        void CloseAllWindows();
    }

    public class WindowArgs
    {
        public int Count => 0;
        public WindowArgs(params object[] args) { }
        public T Get<T>(int index) => default(T);
        public bool TryGet<T>(int index, out T value) { value = default(T); return false; }
    }
}

namespace Gui
{
    public interface IWindow 
    {
        bool Enabled { get; set; }
        bool IsVisible { get; }
        void Open(object args = null);
        void Close();
    }
    public interface IContentFiller { }
    
    public class ListScrollRect : UnityEngine.MonoBehaviour 
    {
        public void RefreshContent() { }
        public void ScrollToListItem(int index) { }
    }
    
    public class AnimatedWindow : UnityEngine.MonoBehaviour, IWindow 
    {
        public System.Action OnInitializedEvent;
        public bool Enabled { get; set; }
        public bool IsVisible => true;
        public virtual void Close() { }
        public void Open(object args = null) { }
    }
    
    public class DebugConsole : UnityEngine.MonoBehaviour { } // Stub for DebugConsole

    namespace Theme
    {
    public class ThemeColor : UnityEngine.MonoBehaviour 
        {
            public static Color Window => Color.white;
            public static Color BackgroundDark => Color.black;
            public static Color Text => Color.white;
            public static Color ErrorText => Color.red;
            public static Color HeaderText => Color.yellow;
            public static Color Icon => Color.white;
        }

        public class UiTheme : UnityEngine.MonoBehaviour
        {
            public static UiTheme Current => null;
            public Color GetQualityColor(object quality) => Color.white;
        }

        public class UiThemeLoader { } // Stub for UiThemeLoader missing error
    }

    namespace Windows
    {
        public enum WindowExitCode
        {
            Ok,
            Cancel,
            None,
            Option1, // Added for MainMenu usage
            Option2  // Added for MainMenu usage
        }

        public class AnimatedWindow : Gui.AnimatedWindow { }
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

public static class GuiExtensions
{
    public static void InitializeElements(this UnityEngine.Transform transform) { }
}



// Signals
public class WindowOpenedSignal { }
public class WindowClosedSignal { }
public class WindowDestroyedSignal { }

// Managers
public class GuiManager { }
public class LocalizationManager { }
public class KeyNameLocalizer { }

// ItemId generic stub
public struct ItemId<T> { }
