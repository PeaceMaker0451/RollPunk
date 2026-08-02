using RollPunk.Client;
using RollPunk.Modding;

namespace RollPunk.Scripts.Client.Settings
{
    internal static class UserModsLoader
    {
        public static ModsContainer GetUserMods(IReadOnlyModsContainer allMods)
        {
            return ModFilter.GetFilteredMods(allMods, ClientRoot.SettingsManager.LoadSettings().DisabledMods);
        }
    }
}
