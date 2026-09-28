using MoonSharp.Interpreter;
using RollPunk.Modding;
using System;
public class LuaExecuter
{
    public static DynValue Call(Script script, DynValue function, params DynValue[] args)
    {
        try
        {
            return function.Function.Call(args);
        }
        catch (Exception ex)
        {
            LuaErrorsHandler.Handle(script, ex);
            return DynValue.Nil;
        }
    }
}
