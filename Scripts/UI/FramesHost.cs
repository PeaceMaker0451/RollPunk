using Godot;
using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System;
using System.Collections.Generic;

namespace RollPunk.Client
{
    internal class FramesHost : IFramesHost
    {
        private readonly string _tabedFramePath;
        private readonly string _framePath;
        
        private Node _rootNode;

        private FramesLoader _framesFactory = new();
        private WindowsCreator _windowsManager;

        private bool _oneScreenMode;
        private bool _smoothResizing;
        private bool _waitForReaizeEnd;
        private float _scale = 1f;

        private List<Frame> _subFrames = new();

        public TabedFrame MainFrame { get; private set; }
        public IReadOnlyList<Frame> SubFrames => _subFrames;

        public IEnumerable<Frame> OpenFrames
        {
            get
            {
                if (MainFrame != null)
                    yield return MainFrame;
                foreach (var f in _subFrames)
                    yield return f;
            }
        }

        public FramesHost(Node rootNode, float scale, bool oneScreenMode, bool smoothResizing, bool waitForReaizeEnd, string tabedFramePath, string defFramePath)
        {
            _scale = scale;
            
            _tabedFramePath = tabedFramePath;
            _framePath = defFramePath;
            
            _rootNode = rootNode;
            _oneScreenMode = oneScreenMode;
            _smoothResizing = smoothResizing;
            _waitForReaizeEnd = waitForReaizeEnd;

            _windowsManager = new(_rootNode);
            
            SetupMainWindow();
            CreateMainFrame();
        }

        public void SetMainFrameTitle(string title)
        {
            MainFrame.SetTitle(title);
        }

        public Frame OpenFrame(Form form, bool alwaysOnTop = false)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            var frame = CreateFrame(alwaysOnTop);
            frame.SetForm(form);
            frame.SetTitle(form.Title);
            return frame;
        }

        public Frame OpenEmptyFrame(bool alwaysOnTop = false)
        {
            return CreateFrame(alwaysOnTop);
        }

        // Устаревшая перегрузка для существующих потребителей. Будет удалена после миграции.
        public Frame OpenInNewFrame(Form form, bool alwaysOnTop = false)
        {
            return OpenFrame(form, alwaysOnTop);
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

        private Frame CreateFrame(bool alwaysOnTop)
        {
            var frame = _framesFactory.LoadFrame(_framePath);
            Window window = null;

            if (_oneScreenMode)
                _rootNode.AddChild(frame);
            else
                window = _windowsManager.CreateNewWindowForFrame(frame);

            frame.SetScaleFactor(_scale);
            frame.WaitForResizeToChangeForm = _waitForReaizeEnd;
            frame.SmoothResizing = _smoothResizing;

            if (window != null)
            {
                if (window.IsNodeReady() == false)
                    window.Ready += () => OnWindowReady(window, alwaysOnTop);
                else
                    OnWindowReady(window, alwaysOnTop);
            }

            _subFrames.Add(frame);
            return frame;
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

            mainWindow.Ready += () => OnWindowReady(mainWindow, false);
        }

        private void CreateMainFrame()
        {
            MainFrame = (TabedFrame)_framesFactory.LoadFrame(_tabedFramePath);
            _rootNode.AddChild(MainFrame);

            MainFrame.ShouldChangeWindowResolution = !_oneScreenMode;
            MainFrame.UpdateSize();

            MainFrame.SetScaleFactor(_scale);
            MainFrame.WaitForResizeToChangeForm = _waitForReaizeEnd;
            MainFrame.SmoothResizing = _smoothResizing;
        }

        private void OnWindowReady(Window window, bool alwaysOnTop)
        {
            window.ContentScaleFactor = _scale;

            Vector2I screenResolution = DisplayServer.ScreenGetSize();
            window.Position = (screenResolution / 2) - (window.Size / 2);

            if (alwaysOnTop)
                window.AlwaysOnTop = true;
        }
    }
}
