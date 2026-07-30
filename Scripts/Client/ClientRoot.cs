using Godot;
using PunkCommandSystem;
using RollPunk.Client.Forms;
using RollPunk.Client.Game;
using RollPunk.Client.Settings;
using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Modding;
using RollPunk.Scripts.Client.Settings;
using RollPunk.Scripts.UI;
using RollPunk.UI.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace RollPunk.Client
{
    public class ClientRoot
    {
        private static ClientRoot Instance;

        private Node _rootNode;
        private FileDebugUtils _fileDebugUtils;
        private ClientConsole _console = new ClientConsole();

        private SettingsManager _settingsManager;
        private RuntimeSettingsApplier _settingsApplier;
        private CommandManager _commandManager = new CommandManager();

        private FramesHost _framesManager;
        private FormsLoader _formsLoader;
        private IFormsManager _formsManager;
        private Runtime _runtime;

        private GodotThreadManager _threadManager;

        internal static FileDebugUtils FileDebugUtils => Instance._fileDebugUtils;
        internal static ClientConsole Console => Instance._console;

        internal static SettingsManager SettingsManager => Instance._settingsManager;
        internal static CommandManager CommandManager => Instance._commandManager;

        //internal static FramesHost FramesManager => Instance._framesManager;
        //internal static FormsLoader FormsFactory { get; private set; }
        internal static IFormsManager FormsManager => Instance._formsManager;
        internal static Runtime Runtime => Instance._runtime;

        internal static GodotThreadManager ThreadManager => Instance._threadManager;

        public ClientRoot(Node rootNode)
        {
            _fileDebugUtils = new FileDebugUtils();
            rootNode.AddChild(_fileDebugUtils);

            RPDebug.Logged += (log) => _console.ConsoleLog(log);
            LuaErrorsHandler.ErrorLogged += RPDebug.Log;

            if (Instance != null)
                throw new InvalidOperationException("Client is not null!!");

            Instance = this;

            _rootNode = rootNode;

            AddCommands(_commandManager);

            SettingsData settings = SetupSettings();

            _framesManager = new(_rootNode, settings.FormsScale, settings.OneScreenMode, settings.SmoothWindowResizing, settings.WaitForResizeToChangeWindow, ClientConfig.TabedFramePath, ClientConfig.DefaultFramePath);
            _formsLoader = new();
            _formsManager = new Forms.FormsManager(_framesManager, _formsLoader);

            _framesManager.SetMainFrameTitle($"RollPunk {ClientConfig.ClientVersion}");

            _threadManager = new();
            _rootNode.AddChild(_threadManager);
            RPDebug.Log($"ThreadManager создан - {_threadManager.Name}");

            _runtime = new Runtime();



            FastSettingsAsker asker = new();
            asker.AskUserName();
        }

        private SettingsData SetupSettings()
        {
            _settingsManager = new SettingsManager(new ClientSettingsStorage(), ClientConfig.ClientVersion);
            _settingsApplier = new(new SettingsApplier[]
            {
                new FontSizeSettingsChanger(),
            });

            _settingsManager.SettingsSaved += (settings) => _settingsApplier.UpdateRuntime();

            var settings = _settingsManager.LoadSettings();
            _settingsManager.SaveSettings(settings);

            return settings;
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

            foreach (var command in _commandManager.CommandsList())
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
