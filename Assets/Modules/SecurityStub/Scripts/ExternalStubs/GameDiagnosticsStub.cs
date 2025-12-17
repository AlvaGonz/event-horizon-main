// Stub for missing GameDiagnostics class
// Placed in Global Namespace to allow usage without imports

public static class GameDiagnostics
{
    public static class Trace
    {
        public static void LogError(string message) => UnityEngine.Debug.LogError(message);
        public static void LogWarning(string message) => UnityEngine.Debug.LogWarning(message);
        public static void Log(string message) => UnityEngine.Debug.Log(message);
        public static void LogException(System.Exception e) => UnityEngine.Debug.LogException(e);
    }
    
    public static class Debug
    {
        public static void LogError(string message) => UnityEngine.Debug.LogError(message);
        public static void LogWarning(string message) => UnityEngine.Debug.LogWarning(message);
        public static void Log(string message) => UnityEngine.Debug.Log(message);
        public static void LogException(System.Exception e) => UnityEngine.Debug.LogException(e);
    }
}
