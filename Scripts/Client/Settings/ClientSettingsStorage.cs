using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client.Settings
{
    internal class ClientSettingsStorage : ISettingsStorage
    {
        private SaveFolder _saveFolder = SaveFolder.settings;
        private string _fileName = @"Settings.json";

        public bool LoadSettings(out SettingsData data)
        {
            data = null;

            if (GodotSaveManager.TryLoad(_saveFolder, _fileName, out string json) == false)
                return false;

            if (json == null || json == "")
                return false;

            data = JsonConvert.DeserializeObject<SettingsData>(json);
            return data != null;
        }

        public void SaveSettings(SettingsData settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            GodotSaveManager.Save(_saveFolder, _fileName, json);
        }
    }
}
