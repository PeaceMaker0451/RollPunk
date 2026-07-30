using Godot;
using Newtonsoft.Json;
using System;

namespace RollPunk.Client.Settings
{
    internal class SettingsData
    {
        [JsonProperty] public string Version;

        [JsonProperty] public string Name;

        [JsonProperty] public float FormsScale;
        [JsonProperty] public int FontSize;
        [JsonProperty] public bool OneScreenMode;
        [JsonProperty] public bool SmoothWindowResizing;
        [JsonProperty] public bool WaitForResizeToChangeWindow;
        [JsonProperty] public Guid ClientID;

        public void Validate()
        {
            FormsScale = Mathf.Clamp(FormsScale, 0.75f, 10);
            FontSize = Mathf.Clamp(FontSize, 2, 40);
        }
    }
}
