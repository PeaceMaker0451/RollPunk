using Godot;
using System;

namespace RollPunk.Popup
{
    public sealed class ContextAction
    {
        public string Name { get; init; }
        public Action Action { get; init; }

        public bool IsEnabled { get; init; } = true;
        //public Texture2D? Icon { get; init; }
    }
}
