using Godot;
using System;

public partial class VideoStreamPlayerScaler : Control
{
    [Export] private VideoStreamPlayer _videoPlayer;
    
    public override void _Ready()
    {
        Resized += UpdateVideoSize;
        UpdateVideoSize();
    }

    private void UpdateVideoSize()
    {
        Vector2 container = Size;
        Vector2 videoSize = _videoPlayer.GetVideoTexture().GetSize();

        if (videoSize == Vector2.Zero)
            return;

        float scale = Mathf.Max(
            container.X / videoSize.X,
            container.Y / videoSize.Y);

        Vector2 scaled = videoSize * scale;

        _videoPlayer.Size = scaled;
        _videoPlayer.Position = (container - scaled) * 0.5f;
    }
}
