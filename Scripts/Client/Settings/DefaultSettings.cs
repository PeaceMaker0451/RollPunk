using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client.Settings
{
    internal class DefaultSettings
    {
        public static SettingsData GetDefaultSettings()
        {
            return new SettingsData()
            {
                Version = ClientConfig.ClientVersion,
                FormsScale = 1,
                OneScreenMode = false,
                SmoothWindowResizing = true,
                WaitForResizeToChangeWindow = true,
                ClientID = new Guid()
            };
        }
    }
}
