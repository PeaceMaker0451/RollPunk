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

            MainMenu.CreateSessionPressed += () => _runtime.StartSession(_runtime.ReadedMods);
            MainMenu.ExitSessionPressed += () => _runtime.KillSession();

            MainMenu.SettingsPressed += MainMenu_SettingsPressed;

            MenuOpened = true;
            MainMenu.Initialize();
            OnStateChanged();
        }

        private async void MainMenu_SettingsPressed()
        {
            string result = await _uiController.OpenStringDialogue("В чем ваша проблема?");
            await _uiController.OpenInformationDialogue("Помощь", "Мы вам ооОООоочень сочуствуем!!" +
                "\n\"[i]{Вставьте сюда проблему пользователя}[/i]\" - Это ООООчень серьезно!!" +
                "\nМы сейчас же позовем кого-то с этим разобраться!!!");
        }

        public void CloseMainMenu()
        {
            if(MainMenu == null)
                throw new InvalidOperationException("Главное меню не было загружено");

            _uiController.CloseForm(MainMenu);

            MenuOpened = false;
            GD.Print("Главное меню закрыто");
        }

        private void OpenSettings()
        {

        }

        private void OpenModManager()
        {

        }

        private void OnStateChanged()
        {
            if(MainMenu != null)
                MainMenu.SetInSession(_runtime.State == RollPunkState.Session);
        }
    }
}
