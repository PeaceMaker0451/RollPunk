using Godot;
using PunkCommandSystem;
using System.Text;


namespace RollPunk.UI.Forms
{
    public class FormsFactory
    {
        public bool TryLoadForm(string scenePath, out Form form)
        {
            form = null;

            var scene = GD.Load<PackedScene>(scenePath);
            if (scene == null)
            {
                GD.PrintErr($"Failed to load scene: {scenePath}");
                return false;
            }

            var sceneInstance = scene.Instantiate();
            if (sceneInstance is Form loadedForm)
            {
                form = loadedForm;
                return true;
            }

            GD.PrintErr($"Scene is not a Form: {scenePath}");
            return false;
        }
    }
}