using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Game
{
    public static class GameLogger
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            Debug.LogError(message);
        }

        public static void LogException(Exception exception)
        {
            Debug.LogError(exception);
        }

        public static void LogException(string message, Exception exception)
        {
            Debug.LogError(message + "\n" + exception);
        }
    }
}