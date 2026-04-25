using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Client
{
    internal static class ClientConfig
    {
        public const string TabedFramePath = "res://Scenes/FramesScenes/TabledFrame.tscn";
        public const string DefaultFramePath = "res://Scenes/FramesScenes/Frame.tscn";
        public const string ClientVersion = "0.5.0";

        public static readonly string[] ModsPaths = new string[]
        {
            "res://Mods/",
            "user://Mods/"
        };
    }
}
