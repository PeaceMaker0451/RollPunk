using Godot;
using RollPunk.Popup;

public sealed partial class Pointer : Node
{
    [Export] private ContextMenuManager _contextMenuManager;

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton button)
            return;

        if (!button.Pressed)
            return;

        switch (button.ButtonIndex)
        {
            case MouseButton.Right:
                OnRightClick(button.Position);
                break;

            case MouseButton.Left:
                _contextMenuManager.Hide();
                break;
        }
    }

    private void OnRightClick(Vector2 mousePosition)
    {
        var provider = FindContextProvider(mousePosition);

        if (provider == null)
        {
            _contextMenuManager.Hide();
            return;
        }

        _contextMenuManager.Show(provider, (Vector2I)mousePosition);
    }

    private IContextActionsProvider? FindContextProvider(Vector2 mousePosition)
    {
        Control? control = GetViewport().GuiGetHoveredControl();

        while (control != null)
        {
            if (control is IContextActionsProvider provider)
                return provider;

            control = control.GetParent() as Control;
        }

        return null;
    }
}
