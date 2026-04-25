using Godot;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using RollPunk.Debug;

namespace RollPunk.Modding
{
    internal class GodotScriptLoader : ScriptLoaderBase
    {
        public override object LoadFile(string file, Table globalContext)
        {
            RPDebug.Log($"[i][color=plum]Loading file: {file}...[/color][/i]");
            var scriptFile = FileAccess.Open(file, FileAccess.ModeFlags.Read);
            if (scriptFile != null)
            {
                string data = scriptFile.GetAsText();
                scriptFile.Close();
                scriptFile.Dispose();

                return data;
            }
            else
            {
                return null;
            }
        }

        public override bool ScriptFileExists(string name)
        {
            GD.Print($"Checking file: {name}");
            return FileAccess.FileExists(name);
        }
    }
}
