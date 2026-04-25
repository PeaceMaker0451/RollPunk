using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client.Settings
{
    internal interface ISettingsStorage
    {
        void SaveSettings(SettingsData settings);
        bool LoadSettings(out SettingsData data);
    }
}
