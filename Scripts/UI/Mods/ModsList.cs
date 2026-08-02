using Godot;
using RollPunk.Debug;
using RollPunk.Modding;
using RollPunk.UI;
using System;
using System.Collections.Generic;

namespace RollPunk.Scripts.UI.Mods
{
    public partial class ModsList : VBoxContainer
    {
        private const string s_modControlScenePath = "res://Scenes/Mods/ModData.tscn";

        private Dictionary<Mod, ModDataControl> _modControls;

        public event Action<Mod, bool> ModChecked;

        public IReadOnlyDictionary<Mod, ModDataControl> ModControls => _modControls;

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
                var scene = GD.Load<PackedScene>(s_modControlScenePath);
                ModDataControl dataControl = (ModDataControl)scene.Instantiate();
                AddChild(dataControl);

                RPDebug.Log($"Показываем моды {mod.GetModInfo()}");
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
