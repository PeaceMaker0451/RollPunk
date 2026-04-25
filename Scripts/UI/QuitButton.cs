using Godot;

namespace RollPunk.UI
{
    public partial class QuitButton : Button
    {
        public override void _Ready()
        {
            this.Pressed += QuitButton_Pressed;
        }

        private void QuitButton_Pressed()
        {
            if (this.GetViewport() == GetTree().Root)
                GetTree().Quit();
            else
                GetWindow().QueueFree();
        }
    }
}
