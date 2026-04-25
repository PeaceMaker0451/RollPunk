using Godot;
using PunkCommandSystem;
using RollPunk.Client.Runtime;
using RollPunk.Client.Settings;
using RollPunk.Debug;
using RollPunk.Modding;
using RollPunk.UI.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace RollPunk.Client
{
    public class Client
    {
        private Node _rootNode;

        public static Client Instance { get; private set; }

        internal FileDebugUtils FileDebugUtils { get; private set; }
        internal ClientConsole Console { get; private set; } = new ClientConsole();

        internal SettingsManager SettingsManager { get; private set; }
        internal CommandManager CommandManager { get; private set; } = new CommandManager();

        internal FramesManager FramesManager { get; private set; }
        internal FormsFactory FormsFactory { get; private set; }
        internal UIController UIController { get; private set; }

        public Client(Node rootNode)
        {
            FileDebugUtils = new FileDebugUtils();
            rootNode.AddChild(FileDebugUtils);

            RPDebug.Logged += (log) => Console.ConsoleLog(log);
            LuaErrorsHandler.ErrorLogged += RPDebug.Log;
            
            if (Instance != null)
                throw new InvalidOperationException("Client is not null!!");

            Instance = this;

            _rootNode = rootNode;

            AddCommands(CommandManager);

            SettingsManager = new SettingsManager(new ClientSettingsStorage(), ClientConfig.ClientVersion);

            var settings = SettingsManager.LoadSettings();
            FramesManager = new(_rootNode, settings.OneScreenMode, ClientConfig.TabedFramePath, ClientConfig.DefaultFramePath);
            FormsFactory = new();
            UIController = new(FramesManager, FormsFactory);

            FramesManager.SetMainFrameTitle($"RollPunk {ClientConfig.ClientVersion}");

            RollPunkRuntime rollPunk = new RollPunkRuntime();
        }

        private void AddCommands(CommandManager commandManager)
        {
            var helpCommand = new Command(
            _name: "help",
            _description: "All commands list",
            _action: _CommandHelp,
            _parameters: new List<RequiredParameter>
            {
            }
            );
            commandManager.AddCommand(helpCommand);
        }

        private string _CommandHelp(ParametersData parameters)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var command in CommandManager.CommandsList())
            {
                bool writeParameters = false;
                StringBuilder stringBuilder1 = new StringBuilder();
                foreach (var parameter in command.RequiredParameters())
                {
                    if (parameter.description != "")
                        stringBuilder1.Append($"[{parameter.name} ({parameter.ParameterType.TypeName}) - {parameter.description}] ");
                    else
                        stringBuilder1.Append($"[{parameter.name} ({parameter.ParameterType.TypeName})] ");
                    writeParameters = true;
                }

                if (writeParameters)
                    stringBuilder.Append($"==>\"{command.Name()}\" - {stringBuilder1.ToString()}\n{command.Description()}\n");
                else
                    stringBuilder.Append($"==>\"{command.Name()}\"\n{command.Description()}\n");
            }

            return stringBuilder.ToString();
        }
    }
}
