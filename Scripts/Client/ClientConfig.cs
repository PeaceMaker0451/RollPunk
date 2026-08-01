using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    internal static class ClientConfig
    {
        public const string TabedFramePath = "res://Scenes/FramesScenes/MainFrame.tscn";
        public const string DefaultFramePath = "res://Scenes/FramesScenes/Frame.tscn";
        public const string ClientVersion = "0.6.0";

        public static readonly string[] ModsPaths = new string[]
        {
            "res://Mods/",
            "user://Mods/"
        };

        // Настройки для работы с изображениями
        public static class ImageSettings
        {
            public const int MaxDimension = 1024;
            public const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB
            public const int MaxValidationDimension = 8192;
        }
    }
}
