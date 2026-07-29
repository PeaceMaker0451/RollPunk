using Godot;
using RollPunk.UI.Frames;

namespace RollPunk.Client
{
    public class WindowsCreator
    {
        private Node _node;
        
        public WindowsCreator(Node rootNode) { _node = rootNode; }
        
        public Window CreateNewWindowForFrame(Frame frame)
        {
            Window window = new Window
            {
                Borderless = true,
                Transparent = true,
                TransparentBg = true,
                AlwaysOnTop = false
            };

            window.AddChild(frame);

            _node.AddChild(window);

            frame.ShouldChangeWindowResolution = true;
            frame.UpdateSize();

            window.CallDeferred(Window.MethodName.Show);

            return window;
        }
    }
}
