using Godot;
using Newtonsoft.Json;
using RollPunk.Debug;
using System;
using System.Collections.Generic;

namespace RollPunk.Modding
{
    public partial class ModReader
    {
        public List<Mod> ReadMods(string[] directories)
        {
            List<Mod> mods = new List<Mod>();

            foreach (string directory in directories)
            {
                RPDebug.Log($"[color=medium_purple]{this} - Scanning {directory} directory...[/color]");
                List<Mod> readedMods = ScanDirectory(directory);

                foreach(Mod mod in readedMods)
                {
                    if (mod != null)
                        mods.Add(mod);
                }
                
            }

            return mods;
        }

        private List<Mod> ScanDirectory(string directoryPath)
        {
            List<Mod> mods = new List<Mod>();

            var dirAccess = DirAccess.Open(directoryPath);

            if (dirAccess != null)
            {
                dirAccess.ListDirBegin();

                string folderName;
                while ((folderName = dirAccess.GetNext()) != "")
                {
                    if (dirAccess.CurrentIsDir() && folderName != "." && folderName != "..")
                    {
                        string modPath = directoryPath + folderName;

                        if (FileAccess.FileExists(modPath + "/data.json"))
                        {
                            ModMetadata metadata = ReadMetadata(modPath + "/data.json");

                            if (metadata != null)
                            {
                                mods.Add(CreateModInstance(modPath, metadata));
                            }
                        }
                    }
                }

                dirAccess.ListDirEnd();
            }
            else
            {
                RPDebug.LogError($"{this} - Directory '{directoryPath}' could not be opened.");
            }

            return mods;
        }

        private ModMetadata ReadMetadata(string metadataFilePath)
        {
            var file = FileAccess.Open(metadataFilePath, FileAccess.ModeFlags.Read);
            if (file != null)
            {
                try
                {
                    string metadataJson = file.GetAsText();
                    ModMetadata metadata = JsonConvert.DeserializeObject<ModMetadata>(metadataJson);

                    if (IsValidMetadata(metadata))
                    {
                        RPDebug.Log($"[color=medium_purple]{this} - Metadata of '{metadata.Name}' successfully loaded.[/color]");
                        return metadata;
                    }
                    else
                    {
                        RPDebug.LogError($"{this} - Mod metadata in {metadataFilePath} is invalid.");
                    }
                }
                catch (Exception ex)
                {
                    RPDebug.LogError($"{this} - Failed to load metadata from {metadataFilePath}: {ex.Message}");
                }
                finally
                {
                    file.Close();
                    file.Dispose();
                }
            }
            else
            {
                RPDebug.LogError($"{this} - Could not open metadata file: {metadataFilePath}");
            }
            return null;
        }

        private Mod CreateModInstance(string modPath, ModMetadata metadata)
        {
            switch (metadata.Type.ToLower())
            {
                default:
                    var mod = new Mod(modPath, metadata);
                    return mod;
            }
        }

        private bool IsValidMetadata(ModMetadata metadata)
        {
            return !string.IsNullOrEmpty(metadata.Name) && !string.IsNullOrEmpty(metadata.Type);
        }
    }
}
