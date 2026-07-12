using Godot;
using RollPunk.Client.Forms;
using RollPunk.Client.Runtime;
using RollPunk.Debug;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client
{
    internal enum MainMenuTab
    {
        Main,
        EnterSession,
        CreateSession,
    }

    internal partial class MainMenu : Form
    {
        [Export] private MainSubMenu _mainMenu;
        [Export] private EnterSessionSubMenu _enterMenu;
        [Export] private MainSubMenu _createMenu;

        public event Action CreateSessionRequested;
        public event Action<string> EnterSessionRequested;

        public override void _Ready()
        {
            base._Ready();
            Initialize();
        }

        public void Initialize()
        {
            _mainMenu.Initialize(this);
            _mainMenu.CreateSessionPressed += () => CreateSessionRequested?.Invoke();

            _enterMenu.Initialize(this);
            _enterMenu.EnterSessionRequested += (adress) => EnterSessionRequested?.Invoke(adress);

            SetMenu(MainMenuTab.Main);
            GD.Print("Главное меню инициализировано");
        }

        public void SetMenu(MainMenuTab tab)
        {
            GD.Print($"Main menu set menu {tab}");

            DisableAllMenus();

            switch (tab)
            {
                case (MainMenuTab.Main):
                    _mainMenu.Show();
                    break;

                case (MainMenuTab.EnterSession):
                    _enterMenu.Show();
                    break;

                case (MainMenuTab.CreateSession):
                    _mainMenu.Show();
                    break;

                default:
                    RPDebug.LogError($"Menu for this button is not exists yet - {tab}");
                    break;
            }
        }

        public void SetInSession(bool isInSession)
        {
            _mainMenu.SetInSession(isInSession);
        }

        public void SetMenuData(string data)
        {

        }

        private void DisableAllMenus()
        {
            _mainMenu.Hide();
            _enterMenu.Hide();
        }
    }
}
