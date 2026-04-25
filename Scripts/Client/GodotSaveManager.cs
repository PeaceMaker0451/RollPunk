using Godot;
using Newtonsoft.Json;

namespace RollPunk
{
    internal enum SaveFolder
    {
        players,
        autosave,
        application,
        settings
    }

    internal class GodotSaveManager
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public const string PLAYERS_FOLDER = "Players/";
        public const string AUTOSAVE_FOLDER = "temp/";
        public const string MODS_FOLDER = "Mods/";
        public const string APP_FOLDER = "";
        public const string SETTINGS_FOLDER = "Settings/";


        public static void Save(SaveFolder folder, string fileName, string data)
        {

            using (var dirAccess = DirAccess.Open($"user://"))
            {
                var error = dirAccess.MakeDir(GetFolder(folder));
            }

            using (var file = FileAccess.Open($"user://{GetFolder(folder)}{fileName}", FileAccess.ModeFlags.Write))
            {
                if (file != null)
                {
                    file.StoreString(data);
                    file.Close();
                }
                else
                {
                    GD.PrintErr($"Can't write file user://{GetFolder(folder)}{fileName}");
                }
            }
        }

        public static bool TryLoad(SaveFolder folder, string fileName, out string result)
        {
            result = null;

            using FileAccess file = FileAccess.Open($"user://{GetFolder(folder)}{fileName}", FileAccess.ModeFlags.Read);
            {
                if (file != null)
                {
                    result = file.GetAsText();
                    file.Close();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetFolder(SaveFolder folder)
        {
            switch (folder)
            {
                case SaveFolder.players:
                    return PLAYERS_FOLDER;
                case SaveFolder.autosave:
                    return AUTOSAVE_FOLDER;
                case SaveFolder.application:
                    return APP_FOLDER;
                case SaveFolder.settings:
                    return SETTINGS_FOLDER;
            }

            return null;
        }
    }
}
