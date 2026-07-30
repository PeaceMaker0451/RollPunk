using RollPunk.Client;
using RollPunk.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Scripts.Client.Settings
{
    internal class RuntimeSettingsApplier
    {
        private ICollection<SettingsApplier> _settingsAppliers;


        public RuntimeSettingsApplier(ICollection<SettingsApplier> settingsAppliers)
        {
            _settingsAppliers = settingsAppliers;
        }

        public void UpdateRuntime()
        {
            var settings = ClientRoot.SettingsManager.LoadSettings();

            foreach (var applier in _settingsAppliers)
                applier.Apply(settings);
        }
    }
}
