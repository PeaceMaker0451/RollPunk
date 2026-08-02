using Godot;
using RollPunk.Client;
using RollPunk.Client.Game;
using RollPunk.Modding;
using RollPunk.Scripts.Client.Settings;
using RollPunk.Scripts.UI.Mods;
using System.Collections.Generic;

namespace RollPunk.Scripts.Client.Forms.MainMenu
{
    internal partial class ModsSubMenu : SubMenu
    {
        [Export] private Button _returnButton;
        [Export] ModsList _modList;

        private IReadOnlyModsContainer _loadedMods;

        public override void _Ready()
        {
            _returnButton.Pressed += () =>
            {
                SaveMods();
                Menu.SetMenu(MainMenuTab.Main);
            };
        }

        protected override void OnInitialize()
        {
            _loadedMods = Menu.Mods;
        }
        
        private void LoadMods()
        {
            List<string> disabledMods = ClientRoot.SettingsManager.LoadSettings().DisabledMods;
            _modList.SetModList(_loadedMods.Mods);

            foreach(var modPair in _modList.ModControls)
            {
                if (disabledMods.Contains(modPair.Key.modData.Id))
                    modPair.Value.SetChecked(false);
            }
        }

        private void SaveMods()
        {
            List<string> disabledMods = new();

            foreach (var modPair in _modList.ModControls)
            {
                if (modPair.Value.IsEnabled == false)
                    disabledMods.Add(modPair.Key.modData.Id);
            }

            var settings = ClientRoot.SettingsManager.LoadSettings();
            settings.DisabledMods = disabledMods;
            ClientRoot.SettingsManager.SaveSettings(settings);
        }

        protected override void OnOpen()
        {
            LoadMods();
        }
    }
}
