using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.Text;

namespace RollPunk.Modding
{
    public class Mod
    {
        public readonly ModMetadata modData;
        public readonly string modPath;
        public readonly Script scriptSpace;

        public Mod(string path, ModMetadata modData)
        {
            this.modData = modData;
            this.modPath = path;

            scriptSpace = new();
            scriptSpace.Options.ScriptLoader = new GodotScriptLoader();
            ((ScriptLoaderBase)scriptSpace.Options.ScriptLoader).IgnoreLuaPathGlobal = true;
            ((ScriptLoaderBase)scriptSpace.Options.ScriptLoader).ModulePaths = new string[]
            {
            $"{modPath}/?.lua",
            $"{modPath}/?/init.lua"
            };
        }

        public string GetModInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Mod {modData.Name}");
            sb.AppendLine($"{modData.Description}");

            return sb.ToString();
        }
    }
}
