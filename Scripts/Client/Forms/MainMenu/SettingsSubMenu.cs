using Godot;
using RollPunk.Client.Settings;

namespace RollPunk.Client
{
    internal partial class SettingsSubMenu : SubMenu
    {
        [Export] private Button _returnButton;
        [Export] private LineEdit _nameField;
        [Export] private SpinBox _fontSizeSpinBox;
        [Export] private SpinBox _formScaleSpinBox;
        [Export] private CheckBox _smoothWindowResizingCheckBox;
        [Export] private CheckBox _waitForResizeCheckBox;
        [Export] private RichTextLabel _clientIdLabel;

        private SettingsData _settingsData;

        public override void _Ready()
        {
            _returnButton.Pressed += () =>
            {
                SaveSettings();
                Menu.SetMenu(MainMenuTab.Main);
            };

            LoadSettings();
        }

        private void LoadSettings()
        {
            _settingsData = ClientRoot.SettingsManager.LoadSettings();

            _nameField.Text = _settingsData.Name;
            _fontSizeSpinBox.Value = _settingsData.FontSize;
            _formScaleSpinBox.Value = _settingsData.FormsScale;
            _smoothWindowResizingCheckBox.ButtonPressed = _settingsData.SmoothWindowResizing;
            _waitForResizeCheckBox.ButtonPressed = _settingsData.WaitForResizeToChangeWindow;
            _clientIdLabel.Text = _settingsData.ClientID.ToString();
        }

        public void SaveSettings()
        {
            _settingsData.Name = _nameField.Text;
            _settingsData.FontSize = (int)_fontSizeSpinBox.Value;
            _settingsData.FormsScale = (float)_formScaleSpinBox.Value;
            _settingsData.SmoothWindowResizing = _smoothWindowResizingCheckBox.ButtonPressed;
            _settingsData.WaitForResizeToChangeWindow = _waitForResizeCheckBox.ButtonPressed;

            ClientRoot.SettingsManager.SaveSettings(_settingsData);
        }

        protected override void OnOpen()
        {
            LoadSettings();
        }
    }
}
