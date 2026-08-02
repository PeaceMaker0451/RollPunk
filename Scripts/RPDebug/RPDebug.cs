using System;

namespace RollPunk.Debug
{
    public static class RPDebug
    {
        public static event Action<string>? Logged;
        public static event Action<string>? ErrorLogged;
        
        public static void Log(string text)
        {
            Logged?.Invoke(text);
        }

        public static void DebugLog(string text)
        {
            Logged?.Invoke(text);
        }
        
        public static void LogError(string text)
        {
            ErrorLogged?.Invoke(text);
            //ErrorLogged?.Invoke($"[b][color=firebrick]ERROR: {text}[/color][/b]");
        }
    }
}
