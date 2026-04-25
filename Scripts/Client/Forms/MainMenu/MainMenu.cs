using Godot;
using RollPunk.Debug;
using RollPunk.UI.Forms;
using System;

namespace RollPunk.Client
{
    internal enum MainMenuTab
    {
        Main,
    }

    internal partial class MainMenu : Form
    {
        [Export] private MainSubMenu _mainMenu;

        public event Action CreateSessionPressed;
        public event Action ExitSessionPressed;
        public event Action SettingsPressed;
        public event Action ModManagerPressed;
        public event Action ExitPressed;

        public void Initialize()
        {
            _mainMenu.Initialize(this);
            _mainMenu.CreateSessionPressed += () => CreateSessionPressed?.Invoke();
            _mainMenu.ExitSessionPressed += () => ExitSessionPressed?.Invoke();
            _mainMenu.SettingsPressed += () => SettingsPressed?.Invoke();
            _mainMenu.ModManagerPressed += () => ModManagerPressed?.Invoke();
            _mainMenu.ExitPressed += () => ExitPressed?.Invoke();

            SetMenu(MainMenuTab.Main);
            GD.Print("Главное меню инициализировано");
        }

        public void SetMenu(MainMenuTab tab)
        {
            GD.Print($"Main menu set menu {tab}");

            switch (tab)
            {
                case (MainMenuTab.Main):
                    DisableAllMenus();
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
        }
    }
}
