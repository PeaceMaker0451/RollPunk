using System;
using System.Collections.Generic;

namespace RollPunk.Modding
{
    public class ModsContainer : IReadOnlyModsContainer
    {
        private readonly Dictionary<string, Mod> _mods = new();

        public event Action<Mod> ModAdded;
        public event Action<Mod> ModRemoved;

        public IReadOnlyCollection<Mod> Mods => _mods.Values;

        public ModsContainer() { }

        public void AddMod(Mod mod)
        {
            _mods.Add(mod.modData.Id, mod);
            ModAdded?.Invoke(mod);
        }

        public void RemoveMod(Mod mod)
        {
            _mods.Remove(mod.modData.Id);
            ModRemoved?.Invoke(mod);
        }

        public void RemoveMod(string modId)
        {
            var mod = _mods[modId];
            _mods.Remove(modId);
            ModRemoved?.Invoke(mod);
        }
    }

    public interface IReadOnlyModsContainer
    {
        public event Action<Mod> ModAdded;
        public event Action<Mod> ModRemoved;

        public IReadOnlyCollection<Mod> Mods { get; }
    }
}