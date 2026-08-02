using Godot;
using RollPunk.Modding;
using RollPunk.UI;
using System;
using System.Collections.Generic;

namespace RollPunk.Scripts.UI.Mods
{
    public partial class ModsList : HBoxContainer
    {
        private const string s_modControlScenePath = "res://Scenes/Mods/ModData.tscn";

        private Dictionary<Mod, ModDataControl> _modControls;

        public event Action<Mod, bool> ModChecked;

        public void SetModList(IEnumerable<Mod> mods)
        {
            if(_modControls != null)
            {
                foreach (var modpair in _modControls)
                {
                    modpair.Value.QueueFree();
                }
            }

            _modControls = new();
            
            foreach (Mod mod in mods)
            {
                ModDataControl dataControl = GD.Load<ModDataControl>(s_modControlScenePath);

                dataControl.WriteModData(mod.modData, true);
                _modControls.Add(mod, dataControl);

                dataControl.Checked += (marked) => ModChecked?.Invoke(mod, marked);
            }
        }

        public List<Mod> GetActiveMods()
        {
            List<Mod> activeMods = new();
            
            foreach(var modPair in _modControls)
            {
                if (modPair.Value.IsEnabled)
                    activeMods.Add(modPair.Key);
            }

            return activeMods;
        }
    }
}
