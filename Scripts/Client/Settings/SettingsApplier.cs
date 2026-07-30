using RollPunk.Client.Settings;

namespace RollPunk.Scripts.UI
{
    internal abstract class SettingsApplier
    {
        public abstract void Apply(SettingsData settings);
    }
}