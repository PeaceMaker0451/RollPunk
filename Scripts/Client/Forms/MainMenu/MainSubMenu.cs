using Godot;
using System;

namespace RollPunk.Client
{
    internal partial class MainSubMenu : SubMenu
    {
        [Export] Button _createSessionButton;
        [Export] Button _exitSessionButton;
        [Export] Button _settingsButton;
        [Export] Button _modManagerButton;
        [Export] Button _exitButton;

        public event Action CreateSessionPressed;
        public event Action ExitSessionPressed;
        public event Action SettingsPressed;
        public event Action ModManagerPressed;
        public event Action ExitPressed;

        public override void _Ready()
        {
            _createSessionButton.Pressed += () => CreateSessionPressed?.Invoke();
            _exitSessionButton.Pressed += () => ExitSessionPressed?.Invoke();
            _settingsButton.Pressed += () => SettingsPressed?.Invoke();
            _modManagerButton.Pressed += () => ModManagerPressed?.Invoke();
            _exitButton.Pressed += () => ExitPressed?.Invoke();
        }

        public void SetInSession(bool isInSession)
        {
            if(isInSession)
            {
                _modManagerButton.Disabled = true;
                _createSessionButton.Disabled = true;
                _settingsButton.Disabled = true;
                _exitSessionButton.Visible = false;
            }
            else
            {
                _modManagerButton.Disabled = false;
                _createSessionButton.Disabled = false;
                _settingsButton.Disabled = false;
                _exitSessionButton.Visible = false;
            }
        }
    }
}
