using Godot;
using RollPunk.Client;
using RollPunk.Modding;
using RollPunk.Scripts.UI.Mods;
using System.Collections.Generic;

namespace RollPunk.Scripts.Client.Forms.MainMenu
{
    internal partial class ModsSubMenu : SubMenu
    {
        [Export] ModsList _modList;

        private IReadOnlyModsContainer _loadedMods;

        public void Initialize(IReadOnlyModsContainer loadedMods)
        {
            _loadedMods = loadedMods;
        }
        
        private void LoadMods()
        {
            List<string> disabledMods = ClientRoot.SettingsManager.LoadSettings().DisabledMods;

            _modList.SetModList(_loadedMods.Mods);
        }

        private void SaveMods()
        {

        }
    }
}
