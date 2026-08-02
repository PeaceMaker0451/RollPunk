using Godot;
using RollPunk.Modding;
using System;

namespace RollPunk.UI
{
    public partial class ModDataControl : Control
    {
        [Export] Label _title;
        [Export] RichTextLabel _description;
        [Export] RichTextLabel _author;
        [Export] RichTextLabel _version;
        [Export] TextureRect _icon;

        [Export] CheckBox _checkBox;

        public event Action<bool> Checked;

        public bool IsEnabled => _checkBox.ButtonPressed;

        public override void _Ready()
        {
            _checkBox.Pressed += OnCheckBoxPressed;
        }

        private void OnCheckBoxPressed()
        {
            Checked?.Invoke(_checkBox.ButtonPressed);
        }

        public void WriteModData(ModMetadata mod, bool active)
        {
            _title.Text = $"{mod.Name} - {mod.Author}";

            _description.Text = mod.Description;
            _author.Text = mod.Author;
            _version.Text = mod.Version;
        }

        public void SetChecked(bool marked)
        {
            _checkBox.SetPressedNoSignal(marked);
        }
    }
}

