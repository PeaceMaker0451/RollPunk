using System;
using System.Collections.Generic;

namespace RollPunk.Modding
{
    public class ModsContainer
    {
        private readonly List<Mod> _mods = new List<Mod>();

        public event Action<Mod> ModAdded;
        public event Action<Mod> ModRemoved;

        public IReadOnlyList<Mod> Mods => _mods;

        public ModsContainer()
        {
        }

        public void AddMod(Mod mod)
        {
            _mods.Add(mod);
            ModAdded?.Invoke(mod);
        }

        public void RemoveMod(Mod mod)
        {
            _mods.Remove(mod);
            ModRemoved?.Invoke(mod);
        }
    }
}