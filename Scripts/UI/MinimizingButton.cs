using Godot;

namespace RollPunk.UI
{
    public partial class MinimizingButton : Button
    {
        public override void _Ready()
        {
            this.Pressed += MinimizeButton_Pressed;
        }

        private void MinimizeButton_Pressed()
        {
            WindowFunctions.MinimizeWindow();
        }
    }
}
