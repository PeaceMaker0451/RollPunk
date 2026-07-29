using Godot;
using System;

namespace RollPunk.UI.Frames
{
    public class FramesLoader
    {
        public Frame LoadFrame(string path)
        {
            PackedScene scene = GD.Load<PackedScene>(path);
            Frame frame = GetFrameFromPackedScene(scene);

            return frame;
        }

        protected Frame GetFrameFromPackedScene(PackedScene packedScene)
        {
            var _frame = packedScene.Instantiate() as Frame;
            if (_frame == null)
            {
                throw new InvalidOperationException("Scene is not Frame Type");
            }
            return _frame;
        }
    }
}