using Godot;
using RollPunk.Client;
using RollPunk.Client.Settings;

namespace RollPunk.Scripts.UI
{
    internal class FontSizeSettingsChanger : SettingsApplier
    {
        private readonly string _stylePath = "res://Style/RollPunkMainTheme.tres";

        public override void Apply(SettingsData settings)
        {
            Theme theme = GD.Load<Theme>(_stylePath);
            theme.DefaultFontSize = settings.FontSize;
        }
    }
}
