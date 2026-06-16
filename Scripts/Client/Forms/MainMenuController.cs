using Godot;
using RollPunk.Client.Runtime;
using RollPunk.Debug;
using System;

namespace RollPunk.Client.Forms
{
    internal class MainMenuController
    {
        private const string _mainMenuPath = "res://Scenes/FormsScenes/MainMenu.tscn";
        
        private UIController _uiController;
        private RollPunkRuntime _runtime;

        public MainMenu MainMenu { get; private set; }
        public bool MenuOpened { get; private set; }

        public MainMenuController(UIController uiController, RollPunkRuntime rollPunkRuntime)
        {
            _uiController = uiController;
            _runtime = rollPunkRuntime;

            _runtime.StateChanged += OnStateChanged;
        }

        public void CreateMainMenu()
        {
            if (MainMenu != null)
                throw new InvalidOperationException("Главное меню уже загружено!");

            if (_uiController.LoadFormAsMainFrameTab(_mainMenuPath, int.MaxValue, out var form) == false)
                throw new InvalidOperationException("Не удалось загрузить главное меню");
            else
                GD.Print("Главное меню загружено");

            MainMenu = (MainMenu)form;

            MainMenu.CreateSessionRequested += () => _runtime.StartSession(_runtime.ReadedMods);
            //MainMenu.ExitSessionPressed += () => _runtime.KillSession();
            MainMenu.EnterSessionRequested += (adress) => _runtime.TryConnectToSession(adress, _runtime.ReadedMods);

            MenuOpened = true;
            MainMenu.Initialize();
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            if(MainMenu != null)
                MainMenu.SetInSession(_runtime.State == RollPunkState.Session);
        }
    }
}
