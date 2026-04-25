using Godot;
using RollPunk.Client.Forms;
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
        
        public Session Session { get; private set; }
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
            Session = new Session(new SessionRuntimeData(Client.Instance.SettingsManager.LoadSettings().ClientID), mods);
            Session.CreatePlayer(Client.Instance.SettingsManager.LoadSettings().Name);

            Session.APIInjector.AddGlobalAPI(Client.Instance.UIController.GetAPI());
            Session.InitializeSession();
            SetState(RollPunkState.Session);
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
