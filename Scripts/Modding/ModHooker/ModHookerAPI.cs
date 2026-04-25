using MoonSharp.Interpreter;
using RollPunk.Modding.APIs;
using System;

namespace RollPunk.Modding
{
    internal sealed class ModHookerAPI : HeldAPI
    {
        Action<string, DynValue> _addHookFunction;

        public ModHookerAPI(IAPIHandler handler, Action<string, DynValue> addHookFunction) : base(handler)
        {
            _addHookFunction = addHookFunction;
        }

        public void addHook(string eventName, DynValue luaFunction)
        {
            _addHookFunction?.Invoke(eventName, luaFunction);
        }
    }
}
