using Godot;
using RollPunk.Debug;
using RollPunk.Modding;
using RollPunk.Scripts.Client.Forms.MainMenu;
using RollPunk.UI.Forms;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    internal enum MainMenuTab
    {
        Main,
        EnterSession,
        Settings,
        ModMenu
    }

    [FormScene("res://Scenes/FormsScenes/MainMenu.tscn")]
    internal partial class MainMenu : Form
    {
        [Export] private MainSubMenu _mainMenu;
        [Export] private EnterSessionSubMenu _enterMenu;
        [Export] private SettingsSubMenu _settingsMenu;
        [Export] private ModsSubMenu _modsMenu;
        
        [Export] private RichTextLabel _motdLabel;
        [Export] private RichTextLabel _updateLogsLabel;
        [Export] private RichTextLabel _authorsLabel;
        [Export] private RichTextLabel _versionLabel;

        public event Action CreateSessionRequested;
        public event Action<string> EnterSessionRequested;
        public event Action ExitSessionRequested;

        public IReadOnlyModsContainer Mods {  get; private set; }

        public async void Initialize(IReadOnlyModsContainer mods)
        {
            Mods = mods;
            
            _mainMenu.Initialize(this);
            _mainMenu.CreateSessionPressed += () => CreateSessionRequested?.Invoke();
            _mainMenu.ExitSessionPressed += () => ExitSessionRequested?.Invoke();

            _enterMenu.Initialize(this);
            _enterMenu.EnterSessionRequested += (adress) => EnterSessionRequested?.Invoke(adress);

            _settingsMenu.Initialize(this);
            _modsMenu.Initialize(this);

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
                    _mainMenu.Open();
                    break;

                case (MainMenuTab.EnterSession):
                    _enterMenu.Open();
                    break;

                case (MainMenuTab.Settings):
                    _settingsMenu.Open();
                    break;

                case (MainMenuTab.ModMenu):
                    _modsMenu.Open();
                    break;

                default:
                    RPDebug.LogError($"Menu for this button is not exists yet - {tab}");
                    break;
            }
        }

        public void SetInSession(bool isInSession)
        {
            _mainMenu.SetInSession(isInSession);

            if (isInSession)
                SetMenu(MainMenuTab.Main);
        }

        public void SetMotdText(string text)
        {
            if (_motdLabel != null)
            {
                _motdLabel.Text = text;
            }
        }

        public void SetUpdateLogsText(string text)
        {
            if (_updateLogsLabel != null)
            {
                _updateLogsLabel.Text = text;
            }
        }

        public void SetAuthorsText(string text)
        {
            if (_authorsLabel != null)
            {
                _authorsLabel.Text = text;
            }
        }

        public void SetVersionText(string text)
        {
            if (_versionLabel != null)
            {
                _versionLabel.Text = text;
            }
        }

        private void DisableAllMenus()
        {
            _mainMenu.Hide();
            _enterMenu.Hide();
            _settingsMenu.Hide();
            _modsMenu.Hide();
        }
    }
}
