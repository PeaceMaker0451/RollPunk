using MoonSharp.Interpreter;
using RollPunk.Debug;
using System;
using System.Text;


namespace RollPunk.Modding
{
    public static class LuaErrorsHandler
    {
        public static event Action<string> ErrorLogged;
        public static void Handle(Exception exception)
        {

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("[b][color=firebrick]-----LUA ERROR----");

            if (exception is InterpreterException interpreterException)
            {
                stringBuilder.AppendLine(
                $"[Lua Interpreter Exception]\n" +
                $"{interpreterException.DecoratedMessage}"
                );

                try
                {
                    foreach (var call in interpreterException.CallStack)
                        stringBuilder.AppendLine($"=> {call})");
                }
                catch(Exception ex)
                { 
                    stringBuilder.AppendLine($"[i][АХАХАХА БЛЯТЬ ЧТО!??? {ex.Message}");
                    stringBuilder.AppendLine($"{ex.StackTrace}][/i]");
                    stringBuilder.AppendLine($"{interpreterException.StackTrace}");
                }
                
            }
            else
            {
                stringBuilder.AppendLine(
                $"[Lua Exception]\n" +
                $"{exception.Message}\n" +
                $"{exception.StackTrace}\n"
                );
            }

            stringBuilder.AppendLine("----------------------------------[/color][/b]");

            ErrorLogged?.Invoke( stringBuilder.ToString() );
        }
    }
}
