using Godot;
using Godot.Collections;
using RollPunk.Debug;
using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using RollPunk.UI.Frames;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Godot.Control;

namespace RollPunk.Client
{
    internal class UIController : IAPIHandler
    {
        private UIAPI _api;
        
        private FramesManager _framesManager;
        private FormsFactory _formsFactory;

        private Dictionary<Form, Frame> _forms = new();

        public UIController(FramesManager framesManager, FormsFactory formsFactory)
        {
            _api = new(this);
            
            _framesManager = framesManager;
            _formsFactory = formsFactory;
        }

        public bool TryGetFrame(Form form, out Frame frame)
        {
            frame = null;

            if (_forms.TryGetValue(form, out frame))
                return true;

            if(_framesManager.MainFrame.Tabs.Values.Contains(form))
            {
                frame = _framesManager.MainFrame;
                return true;
            }

            return false;
        }

        public bool LoadFormAsMainFrameTab(string formPath, int priority, out Form form)
        {
            form = null;
            
            if (_formsFactory.TryLoadForm(formPath, out form) == false)
                return false;

            _framesManager.MainFrame.AddTab(form, form.Title, priority);
            _forms.Add(form, _framesManager.MainFrame);
            return true;
        }

        public bool LoadFormAsNewFrame(string formPath, out Form form)
        {
            form = null;

            if (_formsFactory.TryLoadForm(formPath, out form) == false)
                return false;

            Frame frame = _framesManager.OpenInNewFrame(form);
            _forms.Add(form, frame);
            return true;
        }

        public void CloseForm(Form form)
        {
            if (_forms.ContainsKey(form) == false)
                throw new InvalidOperationException("Форма не может быть закрыта.");

            if (_forms[form] == _framesManager.MainFrame)
                _framesManager.MainFrame.RemoveTab(form);
            else
                _framesManager.CloseFrame(_forms[form]);

            form.QueueFree();
        }

        public void SetMainFrameInputActive(bool active)
        {
            SetFrameInputActive(_framesManager.MainFrame, active);
        }

        public void SetFrameInputActive(Frame frame, bool active)
        {
            if(active)
                frame.Modulate = new Color(1, 1, 1);
            else
                frame.Modulate = new Color(0.5f, 0.5f, 0.5f);

            frame.SetContentPanelInputActive(active);
        }

        public async Task<string> OpenStringDialogue(string title)
        {
            var (dialogue, container, buttonContainer) = await GetDialogue(title);

            LineEdit textBox = new LineEdit();
            container.AddChild(textBox);
            textBox.CustomMinimumSize = new Vector2(300, 0);
            textBox.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
            textBox.SizeFlagsVertical = SizeFlags.ShrinkCenter;

            var continueButton = CreateAndAddButton(buttonContainer, "Ок");

            bool buttonPressed = false;
            continueButton.Pressed += () => buttonPressed = true;

            Frame frame = _framesManager.OpenInNewFrame(dialogue, false);
            frame.SetCloseButtonVisible(false);

            while (buttonPressed == false)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            string result = textBox.Text;
            _framesManager.CloseFrame(frame);
            return result;
        }

        public async Task OpenInformationDialogue(string title, string information)
        {
            var (dialogue, container, buttonContainer) = await GetDialogue(title);

            ScrollContainer scrollContainer = new ScrollContainer();
            container.AddChild(scrollContainer);
            scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            scrollContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            scrollContainer.SizeFlagsVertical = SizeFlags.ExpandFill;

            RichTextLabel text = new RichTextLabel();
            scrollContainer.AddChild(text);
            text.FitContent = true;
            text.BbcodeEnabled = true;
            text.Text = information;
            text.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            text.SizeFlagsVertical = SizeFlags.ExpandFill;
            //text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.ScrollActive = false;
            text.SelectionEnabled = true;

            var continueButton = CreateAndAddButton(buttonContainer, "Продолжить");

            bool buttonPressed = false;
            continueButton.Pressed += () => buttonPressed = true;

            Frame frame = _framesManager.OpenInNewFrame(dialogue, false);
            frame.SetCloseButtonVisible(false);

            while (buttonPressed == false)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _framesManager.CloseFrame(frame);
        }

        private async Task<(Form, Godot.Container, Godot.Container)> GetDialogue(string formTitle)
        {
            Form dialogue = new Form(formTitle);
            dialogue.CustomMinimumSize = new Godot.Vector2(350, 150);

            MarginContainer marginContainer = new();
            dialogue.AddChild(marginContainer);
            marginContainer.Ready += () => marginContainer.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

            VBoxContainer upperLayoutContainer = new();
            marginContainer.AddChild(upperLayoutContainer);

            BoxContainer container = new();
            upperLayoutContainer.AddChild(container);
            container.Vertical = true;
            container.SizeFlagsVertical = SizeFlags.ExpandFill;
            container.Alignment = BoxContainer.AlignmentMode.Center;
            container.AddThemeConstantOverride("separation", 10);
            

            HBoxContainer buttonsContainer = new();
            upperLayoutContainer.AddChild(buttonsContainer);
            buttonsContainer.CustomMinimumSize = new(0, 30);
            buttonsContainer.Alignment = BoxContainer.AlignmentMode.Center;

            return (dialogue, container, buttonsContainer);
        }

        private Button CreateAndAddButton(Container container, string text)
        {
            Button button = new Button();
            container.AddChild(button);
            button.Text = text;
            button.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
            button.SizeFlagsVertical = SizeFlags.ShrinkCenter;

            return button;
        }

        public API GetAPI()
        {
            return _api;
        }
    }
}
