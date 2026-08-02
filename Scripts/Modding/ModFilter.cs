using System.Collections.Generic;

namespace RollPunk.Modding
{
    public static class ModFilter
    {
        public static ModsContainer GetFilteredMods(IReadOnlyModsContainer mods, List<string> excludedMods)
        {
            ModsContainer container = new();

            foreach (var mod in mods.Mods)
                container.AddMod(mod);

            foreach (string id in excludedMods)
                container.RemoveMod(id);

            return container;
        }
    }
}
