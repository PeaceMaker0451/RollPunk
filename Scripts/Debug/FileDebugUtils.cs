using Godot;

namespace RollPunk.Debug
{
    public partial class FileDebugUtils : Node
    {
        public void SaveStringWithDialog(string data)
        {
            var dialog = new FileDialog
            {
                FileMode = FileDialog.FileModeEnum.SaveFile,
                Access = FileDialog.AccessEnum.Filesystem,
                UseNativeDialog = true,
                Filters = new[] { "*.json ; Text Files" }
            };

            dialog.FileSelected += path =>
            {
                File.WriteAllText(path, data);
                dialog.QueueFree();
            };

            AddChild(dialog);
            dialog.PopupCentered();
        }

        public void LoadStringWithDialog(Action<string> onLoaded)
        {
            var dialog = new FileDialog
            {
                FileMode = FileDialog.FileModeEnum.OpenFile,
                Access = FileDialog.AccessEnum.Filesystem,
                UseNativeDialog = true,
                Filters = new[] { "*.json ; Text Files" }
            };

            dialog.FileSelected += path =>
            {
                string data = File.ReadAllText(path);
                onLoaded?.Invoke(data);
                dialog.QueueFree();
            };

            AddChild(dialog);
            dialog.PopupCentered();
        }
    }
}