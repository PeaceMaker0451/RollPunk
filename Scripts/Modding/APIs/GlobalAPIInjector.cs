using MoonSharp.Interpreter;
using PunkCommandSystem;
using RollPunk.Debug;
using System.Collections.Generic;
using System.Text;

namespace RollPunk.Modding.APIs
{
    public class GlobalAPIInjector
    {
        private List<API> _currentGlobalAPIs = new List<API>();
        private ModsContainer _mods;

        public GlobalAPIInjector(ModsContainer mods, CommandManager commandManager = null)
        {
            if(commandManager != null)
                AddCommands(commandManager);

            _mods = mods;
            _mods.ModAdded += InjectAllGlobalAPIs;
        }

        public void AddGlobalAPI(API api)
        {
            RPDebug.Log($"[color=green_yellow]Adding Global API \"{api.GetType().Name}\"...[/color]");

            _currentGlobalAPIs.Add(api);
            InjectAPIToAllObservers(api);
        }

        private void InjectAPIToAllObservers(API globalAPI)
        {
            RPDebug.Log($"[color=green_yellow]Injecting Global API \"{globalAPI.GetType().Name}\"...[/color]");

            foreach (var mod in _mods.Mods)
            {
                InjectAPI(mod.scriptSpace, globalAPI);
            }
        }

        private void InjectAllGlobalAPIs(Mod mod)
        {
            var script = mod.scriptSpace;

            RPDebug.Log($"[color=green_yellow]InjectingAPI's in \"{script}\"[/color]");

            foreach (API api in _currentGlobalAPIs)
            {
                InjectAPI(script, api);
            }
        }

        private void InjectAPI(Script script, API api)
        {
            UserData.RegisterType(api.GetType());
            DynValue apiClass = UserData.Create(api);

            script.Globals.Set(api.type, apiClass);
        }

        private void AddCommands(CommandManager commandManager)
        {
            var apiCommand = new Command(
            _name: "all-apis",
            _description: "All APIs list",
            _action: _CommandAllAPIs,
            _parameters: new List<RequiredParameter>
            {
            }
            );
            commandManager.AddCommand(apiCommand);

            var modsCommand = new Command(
            _name: "all-api-observers",
            _description: "All mods that acces APIs in APIManager",
            _action: _CommandAllMods,
            _parameters: new List<RequiredParameter>
            {
            }
            );
            commandManager.AddCommand(modsCommand);
        }

        private string _CommandAllAPIs(ParametersData parameters)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var api in _currentGlobalAPIs)
            {
                stringBuilder.AppendLine($"({api.GetType()}) - {api.type}");
            }

            return stringBuilder.ToString();
        }

        private string _CommandAllMods(ParametersData parameters)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var mod in _mods.Mods)
            {
                stringBuilder.AppendLine($"({mod.modData.Name}) - {mod.modPath}");
            }

            return stringBuilder.ToString();
        }
    }
}