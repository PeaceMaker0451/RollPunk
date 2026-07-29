using Godot;
using RollPunk.UI.Forms;
using System.Threading.Tasks;

namespace RollPunk.Client.Forms
{
    internal class DialogFactory : IDialogFactory
    {
        private readonly IFormsManager _formsManager;

        public DialogFactory(IFormsManager formsManager)
        {
            _formsManager = formsManager;
        }

        public async Task<DialogResult<string>> ShowStringInput(string title, string placeholder = "", bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            dialogue.CustomMinimumSize = minSize ?? new Vector2(350, 150);

            LineEdit textBox = new LineEdit();
            RichTextLabel text = new RichTextLabel();
            container.AddChild(text);
            container.AddChild(textBox);

            textBox.CustomMinimumSize = new Vector2(300, 0);
            textBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            textBox.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            textBox.PlaceholderText = placeholder;

            text.Text = "";
            text.CustomMinimumSize = new Vector2(300, 0);
            text.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            text.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            text.FitContent = true;

            var continueButton = CreateButton(buttonContainer, okButtonText);
            Button cancelButton = null;
            if (allowCancel)
                cancelButton = CreateButton(buttonContainer, cancelButtonText);

            string result = null;
            bool? completed = null;

            continueButton.Pressed += () => { result = textBox.Text; completed = true; };
            if (allowCancel)
                cancelButton.Pressed += () => completed = false;

            var handle = ShowDialog(dialogue);

            while (completed == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
            return new DialogResult<string>(completed == true, result);
        }

        public async Task<DialogResult<int?>> ShowIntInput(string title, int? defaultValue = null, int? minValue = null, int? maxValue = null, int step = 1, bool allowCancel = true, Vector2? minSize = null, string okButtonText = "Ок", string cancelButtonText = "Отмена")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            dialogue.CustomMinimumSize = minSize ?? new Vector2(300, 130);

            Label messageLabel = new Label();
            container.AddChild(messageLabel);
            messageLabel.Text = "";
            messageLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            messageLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

            SpinBox spinBox = new SpinBox();
            container.AddChild(spinBox);
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

            var handle = ShowDialog(dialogue);

            while (completed == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
            return new DialogResult<int?>(completed == true, result);
        }

        public async Task ShowInformation(string title, string message, Vector2? minSize = null, bool allowCancel = true, string okButtonText = "Продолжить")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);
            dialogue.CustomMinimumSize = minSize ?? new Vector2(400, 250);

            ScrollContainer scrollContainer = new ScrollContainer();
            container.AddChild(scrollContainer);
            scrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
            scrollContainer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            scrollContainer.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

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

            var handle = ShowDialog(dialogue);

            while (!completed)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
        }

        public async Task<DialogResult<bool>> ShowConfirmation(string title, string message, bool allowCancel = true, Vector2? minSize = null, string yesButtonText = "Да", string noButtonText = "Нет")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            Label messageLabel = new Label();
            container.AddChild(messageLabel);
            messageLabel.Text = message;
            messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
            messageLabel.VerticalAlignment = VerticalAlignment.Center;

            var yesButton = CreateButton(buttonContainer, yesButtonText);
            Button noButton = null;
            if (allowCancel)
                noButton = CreateButton(buttonContainer, noButtonText);

            bool? result = null;

            yesButton.Pressed += () => result = true;
            if (allowCancel)
                noButton.Pressed += () => result = false;

            var handle = ShowDialog(dialogue);

            while (result == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
            return new DialogResult<bool>(result == true, result == true);
        }

        private async Task<(Form, Container, Container)> CreateBaseDialog(string title)
        {
            Form dialogue = new Form(title);
            dialogue.CustomMinimumSize = new Vector2(350, 150);

            MarginContainer marginContainer = new();
            dialogue.AddChild(marginContainer);
            marginContainer.Ready += () => marginContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

            VBoxContainer upperLayoutContainer = new();
            marginContainer.AddChild(upperLayoutContainer);

            BoxContainer container = new();
            upperLayoutContainer.AddChild(container);
            container.Vertical = true;
            container.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            container.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            container.Alignment = BoxContainer.AlignmentMode.Center;
            container.AddThemeConstantOverride("separation", 10);

            HBoxContainer buttonsContainer = new();
            upperLayoutContainer.AddChild(buttonsContainer);
            buttonsContainer.CustomMinimumSize = new(0, 30);
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

        private IFormHandle ShowDialog(Form dialogue)
        {
            return _formsManager.ShowForm(dialogue);
        }
    }
}
