using Godot;
using RollPunk.UI.Forms;
using System;
using System.Threading.Tasks;

namespace RollPunk.UI.Frames
{
    public partial class Frame : Control
    {
        [Export] protected Label _title;
        [Export] protected Control _contentPanel;

        [Export] private Button _closeButton;
        [Export] private Button _minimizeButton;

        // --- Resize parameters ---
        [Export] private int resizeBorderThickness = 8;
        [Export] private int resizeCornerSize = 16;

        private bool isResizing = false;
        private const float transitionTime = 0.07f;
        private const float frameRate = 60f;
        private int currentWindowSizeX;
        private int currentWindowSizeY;

        private int minFrameWidth = 200;
        private int minFrameHeight = 120;

        private Vector2I borderOffset;

        private bool formCanBeChanged = true;

        private Control rightZone;
        private Control leftZone;
        private Control bottomZone;
        private Control bottomRightZone;
        private Control bottomLeftZone;

        private enum ResizeMode { None, Right, Left, Bottom, BottomRight, BottomLeft }
        private ResizeMode activeResize = ResizeMode.None;

        private Vector2 initialMouseGlobal;
        private Vector2 initialWindowScreenPos;
        private int initialWidth;
        private int initialHeight;

        public event Action CloseButtonPressed;
        public event Action MinimizeButtonPressed;

        public string Title => _title.Text;
        public Form CurrentForm;
        public Vector2I CurrentFrameSize { get; protected set; }

        public bool ShouldChangeWindowResolution;
        public bool SmoothResizing;
        public bool WaitForResizeToChangeForm;
        public bool CloseOnButtonPress;
        public bool MinimizeOnButtonPress;

        public float ScaleFactor = 1;

        public override void _Ready()
        {
            borderOffset = (Vector2I)(GetRect().Size - _contentPanel.GetRect().Size);

            CreateResizeZones();
            UpdateResizeZones();

            _closeButton.Pressed += OnCloseButtonPressed;
            _minimizeButton.Pressed += OnMinimizeButtonPressed;
        }

        public virtual void SetForm(Form form)
        {
            minFrameHeight = Math.Max((int)form.CustomMinimumSize.Y + borderOffset.Y, (int)CustomMinimumSize.Y);
            minFrameWidth = Math.Max((int)form.CustomMinimumSize.X + borderOffset.X, (int)CustomMinimumSize.X);
            SetFormToContentPanel(form);
        }

        public void UpdateSize()
        {
            SetFrameSize(currentWindowSizeX, currentWindowSizeY);
        }

        public void SetTitle(string title)
        {
            _title.Text = title;
        }

        public void SetCloseButtonVisible(bool visible)
        {
            _closeButton.Visible = visible;
        }

        public void SetMinimizeButtonVisible(bool visible)
        {
            _minimizeButton.Visible = visible;
        }

        public void SetContentPanelInputActive(bool active)
        {
            _contentPanel.ProcessMode = active?ProcessModeEnum.Inherit:ProcessModeEnum.Disabled;
        }

        protected async void SetFormToContentPanel(Form form)
        {
            await AwaitCanChangeForm();

            if (CurrentForm != null)
            {
                CurrentForm.OnHide();
                CurrentForm.Hide();
            }

            CurrentForm = form;

            if (SmoothResizing)
            {
                if (WaitForResizeToChangeForm)
                    await SmoothContentPanelResize((int)form.Size.X, (int)form.Size.Y);
                else
                    _ = SmoothContentPanelResize((int)form.Size.X, (int)form.Size.Y);
            }
            else
            {
                SetContentPanelSize((int)form.Size.X, (int)form.Size.Y);
            }

            if (form.GetParent() != _contentPanel)
                _contentPanel.AddChild(form);

            CurrentForm.OnShow();
            CurrentForm.Show();
        }

        private async Task SmoothContentPanelResize(int targetWidth, int targetHeight)
        {
            if (isResizing) return;
            formCanBeChanged = false;
            isResizing = true;

            float initialWidthF = currentWindowSizeX;
            float initialHeightF = currentWindowSizeY;

            float stepTime = 1f / frameRate;
            float elapsedTime = 0f;

            while (elapsedTime < transitionTime)
            {
                elapsedTime += stepTime;

                float weight = elapsedTime / transitionTime;
                weight = Math.Clamp(weight, 0f, 1f);

                float newWidth = Mathf.Lerp(initialWidthF, targetWidth, weight);
                float newHeight = Mathf.Lerp(initialHeightF, targetHeight, weight);

                SetContentPanelSize((int)newWidth, (int)newHeight);

                await Task.Delay((int)(stepTime * 1000));
            }

            SetContentPanelSize(targetWidth, targetHeight);

            isResizing = false;
            formCanBeChanged = true;
        }

        private async Task AwaitCanChangeForm()
        {
            while (!formCanBeChanged)
                await Task.Delay(10);
        }

        private void SetContentPanelSize(int pixelsX, int pixelsY)
        {
            SetFrameSize(pixelsX + borderOffset.X, pixelsY + borderOffset.Y);
        }

        private void SetFrameSize(int pixelsX, int pixelsY)
        {
            pixelsX = Math.Max(pixelsX, minFrameWidth);
            pixelsY = Math.Max(pixelsY, minFrameHeight);

            currentWindowSizeX = pixelsX;
            currentWindowSizeY = pixelsY;
            CurrentFrameSize = new Vector2I(pixelsX, pixelsY);

            Scale = new Vector2(ScaleFactor, ScaleFactor);

            if (ShouldChangeWindowResolution)
                SetWindowResolution((int)(CurrentFrameSize.X * ScaleFactor), (int)(CurrentFrameSize.Y * ScaleFactor));

            Size = new Vector2(CurrentFrameSize.X, CurrentFrameSize.Y);

            UpdateResizeZones();
        }

        private void SetWindowResolution(int pixelsX, int pixelsY)
        {
            GetWindow().Size = new Vector2I(pixelsX, pixelsY);
        }

        // ------------------- Resize zones -------------------

        private void CreateResizeZones()
        {
            if (rightZone != null) return;

            rightZone = CreateZoneNode("RightZone", CursorShape.Hsize, OnResizeZoneInput);
            leftZone = CreateZoneNode("LeftZone", CursorShape.Hsize, OnResizeZoneInput);
            bottomZone = CreateZoneNode("BottomZone", CursorShape.Vsize, OnResizeZoneInput);
            bottomRightZone = CreateZoneNode("BottomRightZone", CursorShape.Fdiagsize, OnResizeZoneInput);
            bottomLeftZone = CreateZoneNode("BottomLeftZone", CursorShape.Bdiagsize, OnResizeZoneInput);

            AddChild(rightZone);
            AddChild(leftZone);
            AddChild(bottomZone);
            AddChild(bottomRightZone);
            AddChild(bottomLeftZone);
        }

        private Control CreateZoneNode(string name, CursorShape cursor, Action<InputEvent, Control> callback)
        {
            var c = new Control();
            c.Name = name;
            c.MouseDefaultCursorShape = cursor;
            c.MouseFilter = MouseFilterEnum.Stop;
            c.FocusMode = FocusModeEnum.None;
            c.CustomMinimumSize = Vector2.Zero;
            c.GuiInput += (ev) => callback(ev, c);
            return c;
        }

        private void UpdateResizeZones()
        {
            var size = Size;

            rightZone.Position = new Vector2(size.X - resizeBorderThickness, 0);
            rightZone.Size = new Vector2(resizeBorderThickness, size.Y);

            leftZone.Position = new Vector2(0, 0);
            leftZone.Size = new Vector2(resizeBorderThickness, size.Y);

            bottomZone.Position = new Vector2(0, size.Y - resizeBorderThickness);
            bottomZone.Size = new Vector2(size.X, resizeBorderThickness);

            bottomRightZone.Position = new Vector2(size.X - resizeCornerSize, size.Y - resizeCornerSize);
            bottomRightZone.Size = new Vector2(resizeCornerSize, resizeCornerSize);

            bottomLeftZone.Position = new Vector2(0, size.Y - resizeCornerSize);
            bottomLeftZone.Size = new Vector2(resizeCornerSize, resizeCornerSize);
        }

        private void OnResizeZoneInput(InputEvent ev, Control zone)
        {
            if (ev is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
            {
                if (mb.Pressed)
                    StartResize(zone);
                else
                    EndResize();
            }
            else if (ev is InputEventMouseMotion mm && activeResize != ResizeMode.None)
            {
                DoResize();
            }
        }

        private void StartResize(Control zone)
        {
            initialMouseGlobal = DisplayServer.MouseGetPosition();
            initialWindowScreenPos = GetWindow().Position;

            initialWidth = currentWindowSizeX;
            initialHeight = currentWindowSizeY;

            if (zone == rightZone) activeResize = ResizeMode.Right;
            else if (zone == leftZone) activeResize = ResizeMode.Left;
            else if (zone == bottomZone) activeResize = ResizeMode.Bottom;
            else if (zone == bottomRightZone) activeResize = ResizeMode.BottomRight;
            else if (zone == bottomLeftZone) activeResize = ResizeMode.BottomLeft;
            else activeResize = ResizeMode.None;

            formCanBeChanged = false;
        }

        private void EndResize()
        {
            activeResize = ResizeMode.None;
            formCanBeChanged = true;
        }

        private void DoResize()
        {
            if (activeResize == ResizeMode.None)
                return;

            Vector2 currentMouse = DisplayServer.MouseGetPosition();
            Vector2 deltaScreen = currentMouse - initialMouseGlobal;

            float desiredWidthLogical = initialWidth;
            float desiredHeightLogical = initialHeight;

            if (activeResize == ResizeMode.Right)
                desiredWidthLogical = initialWidth + (deltaScreen.X / ScaleFactor);
            else if (activeResize == ResizeMode.Left)
                desiredWidthLogical = initialWidth - (deltaScreen.X / ScaleFactor);
            else if (activeResize == ResizeMode.Bottom)
                desiredHeightLogical = initialHeight + (deltaScreen.Y / ScaleFactor);
            else if (activeResize == ResizeMode.BottomRight)
            {
                desiredWidthLogical = initialWidth + (deltaScreen.X / ScaleFactor);
                desiredHeightLogical = initialHeight + (deltaScreen.Y / ScaleFactor);
            }
            else if (activeResize == ResizeMode.BottomLeft)
            {
                desiredWidthLogical = initialWidth - (deltaScreen.X / ScaleFactor);
                desiredHeightLogical = initialHeight + (deltaScreen.Y / ScaleFactor);
            }

            int finalWidthLogical = (int)Math.Max(Math.Round(desiredWidthLogical), minFrameWidth);
            int finalHeightLogical = (int)Math.Max(Math.Round(desiredHeightLogical), minFrameHeight);

            SetFrameSize(finalWidthLogical, finalHeightLogical);

            if (activeResize == ResizeMode.Left || activeResize == ResizeMode.BottomLeft)
            {
                float appliedDeltaLogical = initialWidth - finalWidthLogical;
                float appliedDeltaScreen = appliedDeltaLogical * ScaleFactor;

                Vector2 newWindowScreenPos = initialWindowScreenPos + new Vector2(appliedDeltaScreen, 0f);
                Vector2I newWinPosI = new Vector2I(
                    (int)Math.Round(newWindowScreenPos.X),
                    (int)Math.Round(newWindowScreenPos.Y)
                );

                if (GetWindow().Position != newWinPosI)
                    GetWindow().Position = newWinPosI;
            }
        }

        private void OnMinimizeButtonPressed()
        {
            if(MinimizeOnButtonPress)
                WindowFunctions.MinimizeWindow();

            MinimizeButtonPressed?.Invoke();
        }

        private void OnCloseButtonPressed()
        {
            CloseButtonPressed?.Invoke();

            if (CloseOnButtonPress)
                WindowFunctions.MinimizeWindow();
        }
    }
}