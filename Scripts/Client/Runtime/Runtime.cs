using Godot;
using RollPunk.Client.Forms;
using RollPunk.ClientNetcode;
using RollPunk.ClientSide.Runtime.UI;
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

namespace RollPunk.Client.Game
{
    internal enum RollPunkState
    {
        None,
        Menu,
        Session
    }
    
    internal class Runtime
    {
        private MainMenuController _mainMenuController;
        private SessionViewController _sessionViewController;
        private ConsoleController _consoleController;
        
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

        public Runtime()
        {
            _mods = _modReader.ReadMods(ClientConfig.ModsPaths);

            var entityFactory = new EntityFactory();
            entityFactory.RegisterFields();
            entityFactory.RegisterHierarchyFields();
            entityFactory.RegisterLineFields();
            entityFactory.RegisterRules();

            LuaErrorsHandler.ErrorLogged += (error) => _ = ClientRoot.FormsManager.Dialogs.ShowInformation("LuaError", error);

            Guid clientId = ClientRoot.SettingsManager.LoadSettings().ClientID;
            Guid? overridedGuid = TryOverrideGuid();

            if (overridedGuid != null)
            {
                clientId = (Guid)overridedGuid;
                RPDebug.Log($"Client ID will be changed to {clientId}");
            }
                
            _runtimeData = new SessionRuntimeData(clientId);
            
            CreateControllers();
            SetState(RollPunkState.Menu);

            ClientRoot.FormsManager.Dialogs.ShowIntInput("Тест", );
            ClientRoot.FormsManager.Dialogs.ShowInformation("Тест", "какая-то информация");
            ClientRoot.FormsManager.Dialogs.ShowConfirmation("Тест", "какая-то информация");
        }

        public void StartSession(IReadOnlyList<Mod> mods)
        {
            Session = new ClientSession(_runtimeData, mods);
            Session.CreatePlayer(ClientRoot.SettingsManager.LoadSettings().Name);

            Session.APIInjector.AddGlobalAPI(ClientRoot.FormsManager.GetAPI());
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
                TcpClient client = new(adressParts[0], Convert.ToInt32(adressParts[1]), ClientRoot.ThreadManager.ThreadManager);

                client.ReceivedWelcome += (message) =>
                {
                    RPDebug.Log($"Сервер передал нам: {message}");
                    client.SendClientData(ClientRoot.SettingsManager.LoadSettings().Name, _runtimeData.ClientID);

                    Session = new ClientSession(_runtimeData, mods, client);

                    Session.APIInjector.AddGlobalAPI(ClientRoot.FormsManager.GetAPI());
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
            CleanupSession();
            Session?.Dispose();
            Session = null;
            SetState(RollPunkState.Menu);
        }

        private void CreateControllers()
        {
            _mainMenuController = new MainMenuController(this);
            _mainMenuHandle = ClientRoot.FormsManager.ShowController(_mainMenuController, FormDisplayMode.MainTab, int.MaxValue);
            
            CreateConsole();
        }

        private void SetState(RollPunkState state)
        {
            GD.Print($"Runtime set state {state}");

            switch (state)
            {
                case RollPunkState.Session:
                    if (_sessionViewController == null)
                    {
                        _sessionViewController = new SessionViewController(_controlsConstructor);
                        _gameViewHandle = ClientRoot.FormsManager.ShowController(_sessionViewController, FormDisplayMode.MainTab, 1);
                        _sessionViewController.SetSession(Session);
                    }
                    break;
            }
            
            State = state;
            StateChanged?.Invoke();
        }

        private void CleanupSession()
        {
            if (_gameViewHandle != null)
            {
                ClientRoot.FormsManager.CloseForm(_gameViewHandle);
                _gameViewHandle = null;
            }
            _sessionViewController = null;
        }

        private void CreateConsole()
        {
            if (_consoleController == null)
            {
                _consoleController = new Scripts.Client.Forms.ConsoleController(ClientRoot.Console);
                _consoleHandle = ClientRoot.FormsManager.ShowController(_consoleController, FormDisplayMode.NewWindow);
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
