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
        private MainMenuController _mainMenuController;
        private SessionViewController _sessionViewController;
        private ConsoleController _consoleController;

        private FieldControlsConstructor _controlsConstructor = new();
        
        private ModReader _modReader = new ();
        private List<Mod> _mods;

        public event Action StateChanged;
        
        public ClientSession Session { get; private set; }
        public RollPunkState State { get; private set; }
        public IReadOnlyList<Mod> ReadedMods => _mods;

        public RollPunkRuntime()
        {
            _mainMenuController = new(Client.Instance.UIController, this);
            _sessionViewController = new(Client.Instance.UIController, _controlsConstructor);
            _mods = _modReader.ReadMods(ClientConfig.ModsPaths);

            SetState(RollPunkState.Menu);
            CreateConsole();

            var entityFactory = new EntityFactory();
            entityFactory.RegisterFields();
            entityFactory.RegisterHierarchyFields();
            entityFactory.RegisterLineFields();
            entityFactory.RegisterRules();

            LuaErrorsHandler.ErrorLogged += (error) => _ = Client.Instance.UIController.OpenInformationDialogue("LuaError", error);
        }

        public void StartSession(IReadOnlyList<Mod> mods)
        {
            Session = new ClientSession(new SessionRuntimeData(Client.Instance.SettingsManager.LoadSettings().ClientID), mods);
            Session.CreatePlayer(Client.Instance.SettingsManager.LoadSettings().Name);

            Session.APIInjector.AddGlobalAPI(Client.Instance.UIController.GetAPI());
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
                    client.SendClientData(Client.Instance.SettingsManager.LoadSettings().Name, Client.Instance.SettingsManager.LoadSettings().ClientID);

                    Session = new ClientSession(new SessionRuntimeData(Client.Instance.SettingsManager.LoadSettings().ClientID), mods, client);
                    Session.CreatePlayer(Client.Instance.SettingsManager.LoadSettings().Name);

                    Session.APIInjector.AddGlobalAPI(Client.Instance.UIController.GetAPI());
                    Session.InitializeSession();
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
                    if(_mainMenuController.MenuOpened == false)
                        _mainMenuController.CreateMainMenu();
                    break;
                case RollPunkState.Session:
                    _sessionViewController.OpenSessionView(Session);
                    break;
            }
            
            State = state;
            StateChanged?.Invoke();
        }

        private void CreateConsole()
        {
            if (_consoleController == null)
                _consoleController = new(Client.Instance.UIController, Client.Instance.Console);

            _consoleController.CreateConsole();
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
