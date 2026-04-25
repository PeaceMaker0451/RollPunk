using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client.Settings
{
    internal class SettingsData
    {
        [JsonProperty] public string Version;

        [JsonProperty] public string Name;

        [JsonProperty] public float FormsScale;
        [JsonProperty] public bool OneScreenMode;
        [JsonProperty] public bool SmoothWindowResizing;
        [JsonProperty] public bool WaitForResizeToChangeWindow;
        [JsonProperty] public Guid ClientID;
    }
}
