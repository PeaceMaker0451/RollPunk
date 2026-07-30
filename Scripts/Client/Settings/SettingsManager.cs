using Godot;
using System;

namespace RollPunk.Client.Settings
{
    internal class SettingsManager
    {
        private readonly ISettingsStorage _storage;
        private readonly string _currentSettingsVersion;

        public event Action<SettingsData> SettingsSaved;

        public SettingsManager(ISettingsStorage storage, string currentSettingsVersion)
        {
            _storage = storage;
            _currentSettingsVersion = currentSettingsVersion;
        }

        public SettingsData LoadSettings()
        {
            if (_storage.LoadSettings(out var settings) == false)
                return DefaultSettings.GetDefaultSettings();

            if (settings.Version != _currentSettingsVersion)
                MigrateSettings(settings);

            settings.Validate();

            return settings;
        }

        public void SaveSettings(SettingsData settings)
        {
            settings.Version = _currentSettingsVersion;
            _storage.SaveSettings(settings);
            SettingsSaved?.Invoke(settings);
        }

        private void MigrateSettings(SettingsData settings) { }
    }
}
