using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace TexturePackLoader
{
    [BepInPlugin("com.Melon.TexturePackLoader", "TexturePackLoader", "1.0.0")]
    public class TextureReplacer : BaseUnityPlugin
    {
        private static ManualLogSource logger;

        private ConfigEntry<string> texturePackPathConfig;
        private string texturePackPath;
        private Dictionary<string, Texture2D> textureReplacements = new Dictionary<string, Texture2D>();

        void Awake()
        {
            logger = Logger;
            logger.LogInfo("TexturePackLoader loaded!");

            texturePackPathConfig = Config.Bind("General", "TexturePackPath", "TexturePacks", "The directory where texture packs are stored.");
            texturePackPath = Path.Combine(Paths.PluginPath, texturePackPathConfig.Value);

            LoadTexturePacks();
            ApplyTextureReplacements();
        }

        void LoadTexturePacks()
        {
            if (!Directory.Exists(texturePackPath))
            {
                logger.LogWarning($"TexturePacks directory '{texturePackPath}' does not exist! Creating it now.");
                Directory.CreateDirectory(texturePackPath);
            }

            string[] zipFiles = Directory.GetFiles(texturePackPath, "*.zip");
            foreach (var zipFile in zipFiles)
            {
                logger.LogInfo($"Loading texture pack: {zipFile}");
                LoadTexturePack(zipFile);
            }
        }


        void LoadTexturePack(string zipFile)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFile))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".json"))
                        {
                            logger.LogInfo($"Found mapping file: {entry.FullName}");
                            using (StreamReader reader = new StreamReader(entry.Open()))
                            {
                                string json = reader.ReadToEnd();
                                var mapping = JsonUtility.FromJson<Dictionary<string, string>>(json);
                                foreach (var kvp in mapping)
                                {
                                    string oldTextureName = kvp.Key;
                                    string newTexturePath = kvp.Value;

                                    var newTextureEntry = archive.GetEntry(newTexturePath);
                                    if (newTextureEntry != null)
                                    {
                                        logger.LogInfo($"Replacing texture: {oldTextureName} with {newTexturePath}");
                                        using (var textureStream = newTextureEntry.Open())
                                        {
                                            byte[] textureData = new byte[newTextureEntry.Length];
                                            textureStream.Read(textureData, 0, textureData.Length);

                                            Texture2D texture = new Texture2D(2, 2);
                                            texture.LoadImage(textureData);

                                            textureReplacements[oldTextureName] = texture;
                                        }
                                    }
                                    else
                                    {
                                        logger.LogWarning($"Texture '{newTexturePath}' not found in '{zipFile}'");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Failed to load texture pack '{zipFile}': {ex.Message}");
            }
        }

        void ApplyTextureReplacements()
        {
            foreach (var kvp in textureReplacements)
            {
                string oldTextureName = kvp.Key;
                Texture2D newTexture = kvp.Value;

                logger.LogInfo($"Applying texture replacement: {oldTextureName}");
                ReplaceTexture(oldTextureName, newTexture);
            }
        }

        void ReplaceTexture(string oldTextureName, Texture2D newTexture)
        {
            var allMaterials = Resources.FindObjectsOfTypeAll<Material>();
            bool replaced = false;

            foreach (var mat in allMaterials)
            {
                if (mat.HasProperty(oldTextureName))
                {
                    mat.SetTexture(oldTextureName, newTexture);
                    logger.LogInfo($"Replaced texture '{oldTextureName}' in material '{mat.name}'");
                    replaced = true;
                }
            }

            if (!replaced)
            {
                logger.LogWarning($"No materials found with texture property '{oldTextureName}'");
            }

            var allRenderers = Resources.FindObjectsOfTypeAll<Renderer>();
            foreach (var renderer in allRenderers)
            {
                var materials = renderer.sharedMaterials;
                foreach (var mat in materials)
                {
                    if (mat.HasProperty(oldTextureName))
                    {
                        mat.SetTexture(oldTextureName, newTexture);
                        logger.LogInfo($"Replaced texture '{oldTextureName}' in renderer '{renderer.name}'");
                    }
                }
            }
        }
    }
}
