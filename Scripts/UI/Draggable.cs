using Godot;
using System;

namespace RollPunk.UI
{
    public partial class Draggable : Control
    {
        bool _dragging = false;
        [Export] Control scaleHandler;
        Vector2I draggingStartPosition;
        public override void _Ready()
        {
            this.GuiInput += Dragable_GuiInput;
        }
        public override void _Process(double delta)
        {
            if (_dragging)
            {
                GetWindow().Position = (Vector2I)(GetWindow().Position + (GetLocalMousePosition()) - (draggingStartPosition));
            }
        }

        private void Dragable_GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
            {
                if ((@event as InputEventMouseButton).Pressed)
                {
                    _dragging = true;
                    draggingStartPosition = (Vector2I)GetLocalMousePosition();
                }
                else
                {
                    _dragging = false;
                }

            }
        }
    }
}
