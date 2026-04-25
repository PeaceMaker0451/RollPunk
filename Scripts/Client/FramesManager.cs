using Godot;
using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System;
using System.Collections.Generic;

namespace RollPunk.Client
{
    internal class FramesManager
    {
        private readonly string _tabedFramePath = "";
        private readonly string _framePath = "";
        
        private Node _rootNode;

        private FramesFactory _framesFactory = new();
        private WindowsManager _windowsManager;

        private bool _oneScreenMode;

        private List<Frame> _subFrames = new();

        public TabedFrame MainFrame { get; private set; }
        public IReadOnlyList<Frame> SubFrames => _subFrames;

        public FramesManager(Node rootNode, bool oneScreenMode, string tabedFramePath, string defFramePath)
        {
            _tabedFramePath = tabedFramePath;
            _framePath = defFramePath;
            
            _rootNode = rootNode;
            _oneScreenMode = oneScreenMode;

            _windowsManager = new(_rootNode);
            
            SetupMainWindow();
            CreateMainFrame();
        }

        public void SetMainFrameTitle(string title)
        {
            MainFrame.SetTitle(title);
        }

        public Frame OpenInNewFrame(Form form, bool alwaysOnTop = false)
        {
            var frame = _framesFactory.LoadFrame(_framePath);
            Window window = null;

            if(_oneScreenMode)
                _rootNode.AddChild(frame);
            else
                window = _windowsManager.CreateNewWindowForFrame(frame);

            frame.SetForm(form);
            frame.SetTitle(form.Title);

            if(_oneScreenMode == false)
            {
                Vector2I screenResolution = DisplayServer.ScreenGetSize();
                window.Position = (screenResolution / 2) - (window.Size / 2);

                if(alwaysOnTop)
                    window.AlwaysOnTop = true;
            }

            _subFrames.Add(frame);
            return frame;
        }

        public void CloseFrame(Frame frame)
        {
            if (frame == MainFrame)
                throw new InvalidOperationException("Unnable to close the Main Frame");

            if(_subFrames.Contains(frame) == false)
                throw new InvalidOperationException("Frame wasn't created by this FrameManager - unnable to close");

            _subFrames.Remove(frame);

            if (_oneScreenMode == false)
                frame.GetWindow().QueueFree();
            else
                frame.QueueFree();
        }

        private void SetupMainWindow()
        {
            Window mainWindow = _rootNode.GetTree().Root.GetWindow();

            if (_oneScreenMode)
            {
                mainWindow.SetMode(Window.ModeEnum.Fullscreen);
                mainWindow.Borderless = false;
                mainWindow.Unresizable = false;
                mainWindow.Transparent = false;
                mainWindow.TransparentBg = false;

                var container = new FlowContainer();
                _rootNode.AddChild(container);
                _rootNode = container;
            }
            else
            {
                mainWindow.SetMode(Window.ModeEnum.Windowed);
                mainWindow.TransparentBg = true;
                mainWindow.Borderless = true;
                mainWindow.Unresizable = false;
                mainWindow.Transparent = true;
                mainWindow.TransparentBg = true;
            }
        }

        private void CreateMainFrame()
        {
            MainFrame = (TabedFrame)_framesFactory.LoadFrame(_tabedFramePath);
            _rootNode.AddChild(MainFrame);

            MainFrame.ShouldChangeWindowResolution = !_oneScreenMode;
            MainFrame.UpdateSize();
        }
    }
}
