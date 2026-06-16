using Godot;
using System;

namespace RollPunk.Client
{
    internal partial class EnterSessionSubMenu : SubMenu
    {
        [Export] Button _returnButton;
        [Export] Button _connectButton;
        [Export] LineEdit _adressField;

        public event Action<string> EnterSessionRequested;

        public override void _Ready()
        {
            _returnButton.Pressed += () => Menu.SetMenu(MainMenuTab.Main);

            _connectButton.Pressed += () => EnterSessionRequested?.Invoke(_adressField.Text);
        }
    }
}
