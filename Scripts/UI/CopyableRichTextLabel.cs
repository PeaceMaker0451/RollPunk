using Godot;
using System;

public partial class CopyableRichTextLabel : RichTextLabel
{
    public override void _Ready()
    {
        MetaClicked += OnMetaClicked;
    }

    private void OnMetaClicked(Variant meta)
    {
        string textToCopy = meta.AsString();

        DisplayServer.ClipboardSet(textToCopy);

        GD.Print("Текст скопирован в буфер обмена: " + textToCopy);
    }
}
