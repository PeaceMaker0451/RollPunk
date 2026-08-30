using Godot;
using PunkCommandSystem;
using RollPunk.Client.Forms;
using RollPunk.Client.Game;
using RollPunk.Client.Settings;
using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Modding;
using RollPunk.Scripts.Client.Runtime;
using RollPunk.Scripts.Client.Settings;
using RollPunk.Scripts.UI;
using RollPunk.UI.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace RollPunk.Client
{
    public class Root
    {
        private static Root Instance;

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
        private Application _application;

        private EntityFactory _entityFactory;
        private SessionLifeCycle _sessionStarter;
        private SessionManager _sessionManager;

        private ModReader _modReader = new();
        private ModsContainer _readedMods;

        private GodotThreadManager _threadManager;

        internal static Node Node => Instance._rootNode;
        
        internal static FileDebugUtils FileDebugUtils => Instance._fileDebugUtils;
        internal static ClientConsole Console => Instance._console;

        internal static SettingsManager Settings => Instance._settingsManager;
        internal static CommandManager CommandManager => Instance._commandManager;
        internal static IFormsManager Forms => Instance._formsManager;
        internal static Runtime Runtime => Instance._runtime;
        internal static Application Application => Instance._application;
        internal static SessionManager Sessions => Instance._sessionManager;
        public static IReadOnlyModsContainer ReadedMods => Instance._readedMods;

        internal static GodotThreadManager ThreadManager => Instance._threadManager;

        public Root(Node rootNode)
        {
            if (Instance != null)
                throw new InvalidOperationException("Client is not null!!");

            Instance = this;
            _rootNode = rootNode;

            InitializeErrorLogging();

            _fileDebugUtils = new FileDebugUtils();
            rootNode.AddChild(_fileDebugUtils);

            AddCommands(_commandManager);
            SettingsData settings = SetupSettings();
            CreateUI(settings);
            CreateThreadManager();

            _readedMods = _modReader.ReadMods(ClientConfig.ModsPaths);
            _entityFactory = EntityFactoryCreater.Create();
            _sessionStarter = new(_entityFactory, _readedMods);
            
            _runtime = new Runtime();
            _sessionManager = new(_runtime, _sessionStarter);

            _application = new Application();
        }

        private void InitializeErrorLogging()
        {
            RPDebug.Logged += (log) => _console.ConsoleLog(log);
            RPDebug.ErrorLogged += (log) =>
            {
                _console.ConsoleLog($"[b][color=firebrick]ERROR: {log}[/color][/b]");
            };

            LuaErrorsHandler.ErrorLogged += RPDebug.Log;
        }

        private void CreateUI(SettingsData settings)
        {
            _framesManager = new(_rootNode, settings.FormsScale, settings.OneScreenMode, settings.SmoothWindowResizing, settings.WaitForResizeToChangeWindow, ClientConfig.TabedFramePath, ClientConfig.DefaultFramePath);
            _formsLoader = new();
            _formsManager = new Forms.FormsManager(_framesManager, _formsLoader);

            _framesManager.SetMainFrameTitle($"RollPunk {ClientConfig.ClientVersion}");
        }

        private void CreateThreadManager()
        {
            _threadManager = new();
            _rootNode.AddChild(_threadManager);
            RPDebug.Log($"ThreadManager создан - {_threadManager.Name}");
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

            if(string.IsNullOrEmpty(settings.Name) || string.IsNullOrWhiteSpace(settings.Name))
            {
                FastSettingsAsker asker = new();
                asker.AskUserName();
            }

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
