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

        public async Task<string> ShowStringInput(string title, string message = "", string placeholder = "")
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            LineEdit textBox = new LineEdit();
            RichTextLabel text = new RichTextLabel();
            container.AddChild(text);
            container.AddChild(textBox);

            textBox.CustomMinimumSize = new Vector2(300, 0);
            textBox.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            textBox.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            textBox.PlaceholderText = placeholder;

            text.Text = message;
            text.CustomMinimumSize = new Vector2(300, 0);
            text.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            text.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            text.FitContent = true;

            var continueButton = CreateButton(buttonContainer, "Ок");
            var cancelButton = CreateButton(buttonContainer, "Отмена");

            string result = null;
            bool completed = false;

            continueButton.Pressed += () => { result = textBox.Text; completed = true; };
            cancelButton.Pressed += () => completed = true;

            var handle = ShowDialog(dialogue);

            while (!completed)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
            return result;
        }

        public async Task<int?> ShowIntInput(string title, string message = "", int? defaultValue = null)
        {
            string defaultText = defaultValue?.ToString() ?? "";
            string result = await ShowStringInput(title, message: message, placeholder: defaultText);
            
            if (string.IsNullOrEmpty(result))
                return null;
                
            if (int.TryParse(result, out int value))
                return value;
                
            return null;
        }

        public async Task ShowInformation(string title, string message)
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);
            dialogue.Size = new Vector2(600, 300);

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

            var continueButton = CreateButton(buttonContainer, "Продолжить");

            bool completed = false;
            continueButton.Pressed += () => completed = true;

            var handle = ShowDialog(dialogue);

            while (!completed)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
        }

        public async Task<bool> ShowConfirmation(string title, string message)
        {
            var (dialogue, container, buttonContainer) = await CreateBaseDialog(title);

            Label messageLabel = new Label();
            container.AddChild(messageLabel);
            messageLabel.Text = message;
            messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
            messageLabel.VerticalAlignment = VerticalAlignment.Center;

            var yesButton = CreateButton(buttonContainer, "Да");
            var noButton = CreateButton(buttonContainer, "Нет");

            bool? result = null;

            yesButton.Pressed += () => result = true;
            noButton.Pressed += () => result = false;

            var handle = ShowDialog(dialogue);

            while (result == null)
                await dialogue.ToSignal(dialogue.GetTree(), SceneTree.SignalName.ProcessFrame);

            _formsManager.CloseForm(handle);
            return result.Value;
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
