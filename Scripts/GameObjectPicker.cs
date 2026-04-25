using Godot;
using System;

internal partial class GameObjectPicker : Node
{
    //[Export] private SubViewport targetViewport;

    //private bool mouseAtViewPort = false;

    //public Action<GameObject> Hovered;
    //public Action<GameObject> Unhovered;
    //public Action<GameObject, MouseButton> Clicked;

    //private GameObject _lastHover;

    //public void SetViewport(SubViewport viewport)
    //{
    //    targetViewport = viewport;
    //    var container = (SubViewportContainer)targetViewport.GetParent();
    //    container.MouseEntered += () => mouseAtViewPort = true;
    //    container.MouseExited += () => { mouseAtViewPort = false; };
    //}

    //public override void _Input(InputEvent @event)
    //{
    //    if (targetViewport == null || mouseAtViewPort == false)
    //        return;

    //    Vector2 mousePos = targetViewport.GetMousePosition();

    //    var go = GetGameObjectAt(mousePos);

    //    // --- Hover / Unhover ---
    //    if (go != _lastHover)
    //    {
    //        if (_lastHover != null)
    //            Unhovered?.Invoke(_lastHover);

    //        _lastHover = go;

    //        if (go != null)
    //            Hovered?.Invoke(go); 
    //    }

    //    // --- Click ---
    //    if (@event is InputEventMouseButton mb && mb.Pressed)
    //    {
    //        if (go != null)
    //            Clicked?.Invoke(go, mb.ButtonIndex);
    //    }
    //}

    //private GameObject GetGameObjectAt(Vector2 mousePos)
    //{
    //    if (targetViewport == null)
    //        return null;

    //    foreach (Node child in targetViewport.GetChildren(includeInternal: true))
    //    {
    //        var go = FindGameObjectRecursive(child, mousePos);
    //        if (go != null)
    //            return go;
    //    }

    //    return null;
    //}

    //private GameObject FindGameObjectRecursive(Node node, Vector2 mousePos)
    //{
    //    if (node is GameObject go)
    //    {
    //        foreach (Node child in go.GetChildren(includeInternal: true))
    //        {
    //            if (child is CanvasItem ci && ci.Visible)
    //            {
    //                Vector2 localPos = ci.GetLocalMousePosition();

    //                if (ci is Control control)
    //                {
    //                    //if (control.GetRect().HasPoint(localPos))
    //                    //{
    //                    //    GD.Print($"Finded Control ({control.Name}) at {localPos}");
    //                    //    return go;
    //                    //}
    //                }
    //                else if (ci is Sprite2D sprite && sprite.Texture != null)
    //                {
    //                    Vector2 texSize = sprite.Texture.GetSize();
    //                    Rect2 rect;

    //                    if (sprite.Centered)
    //                        rect = new Rect2(-texSize / 2 + sprite.Offset, texSize);
    //                    else
    //                        rect = new Rect2(sprite.Offset, texSize);

    //                    if (rect.HasPoint(sprite.GetLocalMousePosition()))
    //                    {
    //                        return go;
    //                    }
    //                }
    //                else if (ci is ColorRect cr)
    //                {
    //                    Rect2 rect = new Rect2(Vector2.Zero, cr.Size);
    //                    if (rect.HasPoint(localPos))
    //                    {
    //                        GD.Print($"Finded ColorRect ({cr.Name}) at {localPos}");
    //                        return go;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    foreach (Node child in node.GetChildren())
    //    {
    //        var found = FindGameObjectRecursive(child, mousePos);
    //        if (found != null)
    //            return found;
    //    }

    //    return null;
    //}
}