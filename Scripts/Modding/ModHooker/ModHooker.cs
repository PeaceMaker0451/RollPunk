using MoonSharp.Interpreter;
using RollPunk.Debug;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RollPunk.Modding
{
    public sealed class ModHooker : IAPIHandler
    {
        private ModHookerAPI _api;
        private Dictionary<string, List<DynValue>> hooks = new Dictionary<string, List<DynValue>>();

        public ModHooker()
        {
            _api = new ModHookerAPI(this, AddHook);
        }

        public object[] CallHook(string eventName, params object[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"[color=olive] Calling hook {eventName} with args (");

            if(args != null )
            {
                foreach (var arg in args)
                    stringBuilder.Append($"{arg} ");
            }

            stringBuilder.Append(")[/color]");

            RPDebug.Log(stringBuilder.ToString());

            if (hooks.TryGetValue(eventName, out var hookList) == false)
            {
                RPDebug.Log($"[color=olive] No hooks finded [/color]");
                return Array.Empty<object>();
            }

            object[] finalResult = Array.Empty<object>();

            foreach (DynValue hook in hookList)
            {
                try
                {
                    RPDebug.Log($"[color=olive] Calling function from {hook.Function.OwnerScript}... [/color]");

                    DynValue result = hook.Function.Call(args);
                    object[] parsed = ParseDynValue(result);

                    finalResult = MergeResults(finalResult, parsed);
                }
                catch (Exception ex)
                {
                    LuaErrorsHandler.Handle(ex);
                }
            }

            stringBuilder = new StringBuilder();

            stringBuilder.Append("[color=olive] Final return parameters: (");

            foreach (var parameter in finalResult)
                stringBuilder.Append($"{parameter.GetType()} ");

            stringBuilder.Append(")[/color]");

            RPDebug.Log(stringBuilder.ToString());

            return finalResult;
        }

        public API GetAPI()
        {
            return _api;
        }

        private void AddHook(string eventName, DynValue luaFunction)
        {
            RPDebug.Log($"[color=olive] Added hook '{eventName}'[/color]");
            if (!hooks.ContainsKey(eventName))
            {
                hooks[eventName] = new List<DynValue>();
            }

            hooks[eventName].Add(luaFunction);
        }

        private object[] ParseDynValue(DynValue result)
        {
            if (result.Type == DataType.Void || result.Type == DataType.Nil)
                return Array.Empty<object>();

            if (result.Type == DataType.Tuple)
            {
                return result.Tuple
                    .Where(v => v.Type != DataType.Void)
                    .Select(v => v.Type == DataType.Nil ? null : v.ToObject())
                    .ToArray();
            }

            return new[]
            {
                result.Type == DataType.Nil ? null : result.ToObject()
            };
        }

        private object[] MergeResults(object[] current, object[] next)
        {
            if (current.Length == 0)
                return next;

            int maxLength = Math.Max(current.Length, next.Length);
            object[] merged = new object[maxLength];

            for (int i = 0; i < maxLength; i++)
            {
                object nextValue = i < next.Length ? next[i] : null;
                object currentValue = i < current.Length ? current[i] : null;

                merged[i] = nextValue ?? currentValue;
            }

            return merged;
        }
    }
}
