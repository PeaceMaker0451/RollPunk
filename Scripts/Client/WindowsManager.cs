using Godot;
using RollPunk.UI.Frames;

namespace RollPunk.Client
{
    public class WindowsManager
    {
        private Node _node;
        
        public WindowsManager(Node rootNode) { _node = rootNode; }
        
        public Window CreateNewWindowForFrame(Frame frame)
        {
            Window window = new Window
            {
                Borderless = true,
                Transparent = false,
                TransparentBg = true,
                AlwaysOnTop = false
            };

            window.AddChild(frame);
            window.Show();

            _node.AddChild(window);

            frame.ShouldChangeWindowResolution = true;
            frame.UpdateSize();

            return window;
        }
    }
}
