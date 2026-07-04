using Godot;
using RollPunk.Popup;
using System;

internal partial class ContextMenuManager : Node
{
    [Export] private PopupMenu _popupMenu;

    public override void _Ready()
    {
        _popupMenu.ActionExecuted += Hide;
    }
    
    public void Show(IContextActionsProvider provider, Vector2I mousePosition)
    {
        _popupMenu.Clear();
        
        _popupMenu.Setup(provider.GetContextActions());
        _popupMenu.Show();
        _popupMenu.Position = mousePosition + GetWindow().Position;
    }

    public void Hide()
    {
        _popupMenu.Hide();
        _popupMenu.Clear();
    }
}
