using Godot;
using RollPunk.Client.Forms;
using RollPunk.ClientNetcode;
using RollPunk.Debug;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Modding;
using RollPunk.Rules;
using RollPunk.Scripts.Client.Forms;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Runtime
{
    internal enum RollPunkState
    {
        None,
        Menu,
        Session
    }
    
    internal class RollPunkRuntime
    {
        private IFormHandle _mainMenuHandle;
        private IFormHandle _gameViewHandle;
        private IFormHandle _consoleHandle;

        private FieldControlsConstructor _controlsConstructor = new();
        
        private ModReader _modReader = new ();
        private List<Mod> _mods;

        private SessionRuntimeData _runtimeData;

        public event Action StateChanged;
        
        public ClientSession Session { get; private set; }
        public RollPunkState State { get; private set; }
        public IReadOnlyList<Mod> ReadedMods => _mods;

        public RollPunkRuntime()
        {
            _mods = _modReader.ReadMods(ClientConfig.ModsPaths);

            SetState(RollPunkState.Menu);
            CreateConsole();

            var entityFactory = new EntityFactory();
            entityFactory.RegisterFields();
            entityFactory.RegisterHierarchyFields();
            entityFactory.RegisterLineFields();
            entityFactory.RegisterRules();

            LuaErrorsHandler.ErrorLogged += (error) => _ = Client.Instance.UIController.OpenInformationDialogue("LuaError", error);

            Guid clientId = Client.Instance.SettingsManager.LoadSettings().ClientID;
            Guid? overridedGuid = TryOverrideGuid();

            if (overridedGuid != null)
            {
                clientId = (Guid)overridedGuid;
                RPDebug.Log($"Client ID will be changed to {clientId}");
            }
                

            _runtimeData = new SessionRuntimeData(clientId);
        }

        public void StartSession(IReadOnlyList<Mod> mods)
        {
            Session = new ClientSession(_runtimeData, mods);
            Session.CreatePlayer(Client.Instance.SettingsManager.LoadSettings().Name);

            Session.APIInjector.AddGlobalAPI(Client.Instance.FormsManager.GetAPI());
            Session.InitializeSession();
            SetState(RollPunkState.Session);
        }

        public bool TryConnectToSession(string adress, IReadOnlyList<Mod> mods)
        {
            var adressParts = adress.Split(new char[] { ':' });
            
            if (adressParts.Length != 2)
            {
                RPDebug.LogError("Невозможно подключиться к хосту: неправильный формат адресной строки.");
                return false;
            }

            try
            {
                TcpClient client = new(adressParts[0], Convert.ToInt32(adressParts[1]), Client.Instance.ThreadManager.ThreadManager);

                client.ReceivedWelcome += (message) =>
                {
                    RPDebug.Log($"Сервер передал нам: {message}");
                    client.SendClientData(Client.Instance.SettingsManager.LoadSettings().Name, _runtimeData.ClientID);

                    Session = new ClientSession(_runtimeData, mods, client);

                    Session.APIInjector.AddGlobalAPI(Client.Instance.FormsManager.GetAPI());
                    SetState(RollPunkState.Session);
                };

                client.ConnectToServer();
            }
            catch (Exception ex)
            {
                RPDebug.LogError($"Невозможно подключиться к хосту: {ex.Message}." +
                    $"\n{ex.StackTrace}");
                return false;
            }

            return true;
        }

        public void KillSession()
        {
            Session.Dispose();
            Session = null;
            SetState(RollPunkState.Menu);
        }

        private void SetState(RollPunkState state)
        {
            GD.Print($"Runtime set state {state}");

            switch (state)
            {
                case RollPunkState.Menu:
                    if (_mainMenuHandle == null || !_mainMenuHandle.IsValid)
                        _mainMenuHandle = Client.Instance.FormsManager.ShowInMainTab("res://Scenes/FormsScenes/MainMenu.tscn", int.MaxValue);
                    break;
                case RollPunkState.Session:
                    if (_gameViewHandle == null || !_gameViewHandle.IsValid)
                    {
                        _gameViewHandle = Client.Instance.FormsManager.ShowInMainTab("res://Scenes/FormsScenes/GameView.tscn", 1);
                        var gameView = Client.Instance.FormsManager.GetForm<GameView>(_gameViewHandle);
                        var controller = new SessionViewController(gameView, _controlsConstructor);
                        gameView.SetController(controller);
                        controller.SetSession(Session);
                    }
                    break;
            }
            
            State = state;
            StateChanged?.Invoke();
        }

        private void CreateConsole()
        {
            if (_consoleHandle == null || !_consoleHandle.IsValid)
            {
                _consoleHandle = Client.Instance.FormsManager.ShowInNewWindow("res://Scenes/FormsScenes/Console.tscn");
                var console = Client.Instance.FormsManager.GetForm<Console>(_consoleHandle);
                // Консоль сама создает свой контроллер в _Ready()
            }
        }

        private Guid? TryOverrideGuid()
        {
            const string ClientIdPrefix = "--clientId=";

            bool TryExtractClientId(string[] args, out Guid result)
            {
                result = Guid.Empty;

                string clientArg = Array.Find(args, arg => arg.StartsWith(ClientIdPrefix, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(clientArg))
                {
                    return false;
                }

                string guidString = clientArg.Substring(ClientIdPrefix.Length);

                return Guid.TryParse(guidString, out result);
            }

            string[] args = OS.GetCmdlineArgs();

            if (TryExtractClientId(args, out Guid parsedGuid))
                return parsedGuid;
            else
                return null;
        }

        private class SessionRuntimeData : IRuntimeData
        {
            public Guid ClientID { get; private set; }

            public SessionRuntimeData(Guid clientID)
            {
                ClientID = clientID;
            }
        }
    }
}
