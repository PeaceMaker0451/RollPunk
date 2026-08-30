using Godot;
using RollPunk.Modding.APIs;
using RollPunk.UI.Forms;
using System.Threading.Tasks;

namespace RollPunk.Client.Forms
{
    internal class DialogFactory : IDialogFactory
    {
        private readonly IFormsManager _formsManager;
        private readonly DialogFactoryAPI _api;

        public DialogFactory(IFormsManager formsManager)
        {
            _formsManager = formsManager;
            _api = new(this);
        }

        public async Task<DialogResult<string>> ShowStringInput(string title, string message = "", string placeholder = "", bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            dialogue.CustomMinimumSize = minSize ?? new Vector2(350, 150);
            dialogue.CustomMaximumSize = minSize ?? new Vector2(350, 150);

            PanelContainer panel = new PanelContainer();
            container.AddChild(panel);
            panel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            panel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            MarginContainer margin = new MarginContainer();
            panel.AddChild(margin);
            margin.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            margin.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            margin.AddThemeConstantOverride("margin_left", 10);
            margin.AddThemeConstantOverride("margin_right", 10);
            margin.AddThemeConstantOverride("margin_top", 10);
            margin.AddThemeConstantOverride("margin_bottom", 10);

            VBoxContainer vbox = new VBoxContainer();
            margin.AddChild(vbox);
            vbox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            vbox.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            vbox.Alignment = BoxContainer.AlignmentMode.Center;
            //vbox.Separation = 6;

            RichTextLabel text = new RichTextLabel();
            vbox.AddChild(text);
            text.Text = message;
            text.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            text.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            text.FitContent = true;

            LineEdit textBox = new LineEdit();
            vbox.AddChild(textBox);
            textBox.CustomMinimumSize = new Vector2(300, 0);
            textBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            textBox.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            textBox.PlaceholderText = placeholder;

            var continueButton = CreateButton(buttonContainer, okButtonText);
            Button cancelButton = null;
            if (allowCancel)
                cancelButton = CreateButton(buttonContainer, cancelButtonText);

            string result = null;
            bool? completed = null;

            continueButton.Pressed += () => { result = textBox.Text; completed = true; };
            if (allowCancel)
                cancelButton.Pressed += () => completed = false;

            ShowDialog(dialogue);

            while (completed == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            dialogue.Close();
            return new DialogResult<string>(completed == true, result);
        }

        public async Task<DialogResult<int?>> ShowIntInput(string title, string message = "", int? defaultValue = null, int? minValue = null, int? maxValue = null, int step = 1, bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            dialogue.CustomMinimumSize = minSize ?? new Vector2(400, 220);
            dialogue.CustomMaximumSize = minSize ?? new Vector2(400, 220);

            PanelContainer panel = new PanelContainer();
            container.AddChild(panel);
            panel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            panel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            MarginContainer margin = new MarginContainer();
            panel.AddChild(margin);
            margin.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            margin.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            margin.AddThemeConstantOverride("margin_left", 10);
            margin.AddThemeConstantOverride("margin_right", 10);
            margin.AddThemeConstantOverride("margin_top", 10);
            margin.AddThemeConstantOverride("margin_bottom", 10);

            VBoxContainer vbox = new VBoxContainer();
            margin.AddChild(vbox);
            vbox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            vbox.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            vbox.Alignment = BoxContainer.AlignmentMode.Center;
            //vbox.Separation = 6;

            RichTextLabel text = new RichTextLabel();
            vbox.AddChild(text);
            text.Text = message;
            text.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            text.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            text.FitContent = true;

            SpinBox spinBox = new SpinBox();
            vbox.AddChild(spinBox);
            spinBox.CustomMinimumSize = new Vector2(100, 0);
            spinBox.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            spinBox.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            spinBox.MinValue = minValue ?? int.MinValue;
            spinBox.MaxValue = maxValue ?? int.MaxValue;
            spinBox.Step = step;
            spinBox.Value = defaultValue ?? 0;

            var continueButton = CreateButton(buttonContainer, okButtonText);
            Button cancelButton = null;
            if (allowCancel)
                cancelButton = CreateButton(buttonContainer, cancelButtonText);

            int? result = null;
            bool? completed = null;

            continueButton.Pressed += () => { result = (int)spinBox.Value; completed = true; };
            if (allowCancel)
                cancelButton.Pressed += () => completed = false;

            ShowDialog(dialogue);

            while (completed == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            dialogue.Close();
            return new DialogResult<int?>(completed == true, result);
        }

        public async Task ShowInformation(string title, string message, Vector2? minSize = null, bool allowCancel = true, string okButtonText = "Продолжить")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);
            dialogue.CustomMinimumSize = minSize ?? new Vector2(400, 300);
            dialogue.CustomMaximumSize = minSize ?? new Vector2(400, 300);

            Vector2 textBoxMinSizeOffset = new(50, 80);

            ScrollContainer scrollContainer = new ScrollContainer();
            container.AddChild(scrollContainer);
            scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            scrollContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            scrollContainer.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            scrollContainer.CustomMinimumSize = dialogue.CustomMinimumSize - textBoxMinSizeOffset;

            RichTextLabel text = new RichTextLabel();
            scrollContainer.AddChild(text);
            text.FitContent = true;
            text.BbcodeEnabled = true;
            text.Text = message;
            text.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            text.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.ScrollActive = false;
            text.SelectionEnabled = true;

            var continueButton = CreateButton(buttonContainer, okButtonText);

            bool completed = false;
            continueButton.Pressed += () => completed = true;

            ShowDialog(dialogue);

            while (!completed)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            dialogue.Close();
        }

        public async Task<DialogResult<bool>> ShowConfirmation(string title, string message, bool allowCancel = true, Vector2? minSize = null, string yesButtonText = "Да", string noButtonText = "Нет")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            RichTextLabel messageLabel = new();
            container.AddChild(messageLabel);
            messageLabel.Text = message;
            messageLabel.FitContent = true;
            messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
            messageLabel.VerticalAlignment = VerticalAlignment.Center;
            messageLabel.ScrollActive = true;

            var yesButton = CreateButton(buttonContainer, yesButtonText);
            Button noButton = null;
            if (allowCancel)
                noButton = CreateButton(buttonContainer, noButtonText);

            bool? result = null;

            yesButton.Pressed += () => result = true;
            if (allowCancel)
                noButton.Pressed += () => result = false;

            ShowDialog(dialogue);

            while (result == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            dialogue.Close();
            return new DialogResult<bool>(result == true, result == true);
        }

        public API GetAPI()
        {
            return _api;
        }

        private async Task<(Form, Container, Container)> CreateBaseDialog(string title)
        {
            Form dialogue = new Form(title);
            dialogue.CustomMinimumSize = new Vector2(450, 200);
            dialogue.CustomMaximumSize = new Vector2(450, 200);
            Vector2 panelMinSize = new Vector2(350, 150);

            CenterContainer centerContainer = new CenterContainer();
            dialogue.AddChild(centerContainer);
            centerContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            centerContainer.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            centerContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);

            PanelContainer panelContainer = new PanelContainer();
            centerContainer.AddChild(panelContainer);
            panelContainer.CustomMinimumSize = panelMinSize;

            MarginContainer marginContainer = new MarginContainer();
            panelContainer.AddChild(marginContainer);
            marginContainer.Ready += () => marginContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            marginContainer.AddThemeConstantOverride("margin_left", 10);
            marginContainer.AddThemeConstantOverride("margin_right", 10);
            marginContainer.AddThemeConstantOverride("margin_top", 10);
            marginContainer.AddThemeConstantOverride("margin_bottom", 10);

            VBoxContainer upperLayoutContainer = new VBoxContainer();
            marginContainer.AddChild(upperLayoutContainer);
            upperLayoutContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            upperLayoutContainer.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            BoxContainer container = new BoxContainer();
            upperLayoutContainer.AddChild(container);
            container.Vertical = true;
            container.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            container.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            container.Alignment = BoxContainer.AlignmentMode.Center;
            container.AddThemeConstantOverride("separation", 10);

            HBoxContainer buttonsContainer = new HBoxContainer();
            upperLayoutContainer.AddChild(buttonsContainer);
            buttonsContainer.CustomMinimumSize = new Vector2(0, 30);
            buttonsContainer.Alignment = BoxContainer.AlignmentMode.Center;

            return (dialogue, container, buttonsContainer);
        }

        private Button CreateButton(Container container, string text)
        {
            Button button = new Button();
            container.AddChild(button);
            button.Text = text;
            button.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            button.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            return button;
        }

        private void ShowDialog(Form dialogue)
        {
            _formsManager.Open(dialogue);
        }
    }
}
