using Godot;
using RollPunk.Popup;
using System;
using System.Collections.Generic;

public partial class PopupMenu : PopupPanel
{
    [Export] private Control _buttonsContainer;
    
    private List<Button> _buttons = new();

    public event Action ActionExecuted;
    
    public void Setup(IEnumerable<ContextAction> actions)
    {
        foreach(var action in actions)
        {
            Button button = new Button();
            button.Text = action.Name;
            button.Pressed += () => ExecuteAction(action.Action);
            button.Disabled = !action.IsEnabled;

            _buttonsContainer.AddChild(button);
            _buttons.Add(button);
        }
    }

    public void Clear()
    {
        foreach (var button in _buttons)
            button.QueueFree();

        _buttons.Clear();
    }

    private void ExecuteAction(Action action)
    {
        action();
        ActionExecuted?.Invoke();
    }
}
