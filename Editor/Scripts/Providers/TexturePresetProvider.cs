using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Presets;


namespace OneDevApp.PredefinedPresetLibrary
{
    /// <summary>
    /// Implementation for Texture presets
    /// </summary>
    public class TexturePresetProvider : BasePresetProvider
    {
        public override string Category => "Textures";

        public override void Initialize()
        {
            base.Initialize();

            // Add texture presets with enhanced information
            presets.Add(new PresetDefinition(
                "Default",
                "Standard texture with mipmaps",
                (fileName) => CreateTexturePreset(fileName),
                "General purpose textures for 3D models",
                true,
                true
            ));

            presets.Add(new PresetDefinition(
                "Sprite - Single",
                "Single sprite with no mipmaps",
                (fileName) => CreateTexturePreset(fileName),
                "UI elements, buttons, icons, and 2D game characters",
                true,
                true,
                "*_Sprite",
                "Applies to filenames ending with _Sprite"
            ));

            presets.Add(new PresetDefinition(
                "Sprite - Multiple",
                "Multiple sprites from atlas with no mipmaps",
                (fileName) => CreateTexturePreset(fileName),
                "Sprite sheets, character animations, and tile sets",
                true,
                true,
                "*_Atlas",
                "Applies to filenames ending with _Atlas"
            ));

            presets.Add(new PresetDefinition(
                "UI - Single",
                "UI texture with no mipmaps",
                (fileName) => CreateTexturePreset(fileName),
                "Single UI elements, buttons, and icons",
                true,
                true,
                "*_UI",
                "Applies to filenames ending with _UI"
            ));

            presets.Add(new PresetDefinition(
                "UI - Multiple",
                "UI texture with no mipmaps",
                (fileName) => CreateTexturePreset(fileName),
                "UI sprite sheets and complex interface elements",
                true,
                true,
                "*_UIAtlas",
                "Applies to filenames ending with _UIAtlas"
            ));

            presets.Add(new PresetDefinition(
                "Normal Map",
                "Normal map texture",
                (fileName) => CreateTexturePreset(fileName),
                "Surface detail for PBR materials",
                true,
                true,
                "*_Normal*",
                "Applies to filenames containing _Normal"
            ));

            presets.Add(new PresetDefinition(
                "LightMap",
                "Lightmap texture",
                (fileName) => CreateTexturePreset(fileName),
                "Baked lighting data for scenes",
                true,
                true,
                "*_Lightmap",
                "Applies to filenames ending with _Lightmap"
            ));

            presets.Add(new PresetDefinition(
                "SkyBox",
                "Cubemap texture",
                (fileName) => CreateTexturePreset(fileName),
                "Environment maps for sky rendering",
                true,
                true,
                "*_Sky*",
                "Applies to filenames containing _Sky"
            ));

            presets.Add(new PresetDefinition(
                "CubeMap",
                "Cubemap texture",
                (fileName) => CreateTexturePreset(fileName),
                "Reflection maps and environment maps",
                true,
                true,
                "*_Cube*",
                "Applies to filenames containing _Cube"
            ));
        }

        private void CreateTexturePreset(string presetName)
        {
            try
            {
                // Create a temporary texture
                string tempFolder = "Assets/Editor/Temp";
                string tempPath = $"{tempFolder}/tempTexture.png";
                EnsureDirectoryExists(tempPath);

                // Create a 2x2 temporary texture
                string tempTexturePath = AssetDatabase.GenerateUniqueAssetPath(tempPath);
                Texture2D tempTexture = new Texture2D(2, 2);
                File.WriteAllBytes(tempTexturePath, tempTexture.EncodeToPNG());
                AssetDatabase.ImportAsset(tempTexturePath);

                // Get the importer
                TextureImporter importer = AssetImporter.GetAtPath(tempTexturePath) as TextureImporter;
                if (importer != null)
                {
                    ConfigureTextureImporter(importer, presetName);

                    // Apply changes to temporary asset
                    importer.SaveAndReimport();

                    // Create preset from temporary asset
                    Preset preset = new Preset(importer);

                    // Define preset directory
                    string presetPath = $"Assets/Editor/Presets/{Category}/{presetName}.preset";
                    EnsureDirectoryExists(presetPath);
                    // Save the preset
                    AssetDatabase.CreateAsset(preset, presetPath);
                    AssetDatabase.Refresh();

                    // Delete temporary asset
                    AssetDatabase.DeleteAsset(tempFolder);

                }
                else
                {
                    Debug.LogError($"Failed to get TextureImporter for {tempTexturePath}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating texture preset: {ex.Message}");
            }
        }

        private void ConfigureTextureImporter(TextureImporter importer, string presetName)
        {
            TextureImporterSettings textureSettings = new TextureImporterSettings();

            // Apply settings based on preset type
            switch (presetName)
            {
                case "Default":
                    ConfigureDefaultTexture(importer, textureSettings);
                    break;
                case "Sprite_Single":
                    ConfigureSpriteOrUITexture(importer, textureSettings, false, false);
                    break;
                case "Sprite_Multiple":
                    ConfigureSpriteOrUITexture(importer, textureSettings, true, false);
                    break;
                case "UI_Single":
                    ConfigureSpriteOrUITexture(importer, textureSettings, false, true);
                    break;
                case "UI_Multiple":
                    ConfigureSpriteOrUITexture(importer, textureSettings, true, true);
                    break;
                case "Normal_Map":
                    ConfigureNormalMapTexture(importer, textureSettings);
                    break;
                case "Light_Map":
                    ConfigureLightmapTexture(importer, textureSettings);
                    break;
                case "SkyBox":
                case "Cube_Map":
                    ConfigureCubeMapTexture(importer, textureSettings);
                    break;
            }

            // Apply settings to the importer
            importer.SetTextureSettings(textureSettings);
        }

        private void ConfigureDefaultTexture(TextureImporter importer, TextureImporterSettings settings)
        {
            settings.textureType = TextureImporterType.Default;
            settings.textureShape = TextureImporterShape.Texture2D;
            settings.mipmapEnabled = true;
            settings.sRGBTexture = true;
            settings.readable = false;
            settings.alphaIsTransparency = false;
            importer.maxTextureSize = 2048;
            importer.compressionQuality = 50;
            settings.filterMode = FilterMode.Bilinear;

            // Platform-specific settings
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Android", TextureImporterFormat.ASTC_6x6, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("iPhone", TextureImporterFormat.PVRTC_RGB4, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("WebGL", TextureImporterFormat.ETC2_RGBA8, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Standalone", TextureImporterFormat.DXT5, 2048, 90));
        }

        private void ConfigureSpriteOrUITexture(TextureImporter importer, TextureImporterSettings settings, bool isMultiple, bool isUI)
        {
            settings.textureType = TextureImporterType.Sprite;
            settings.textureShape = TextureImporterShape.Texture2D;
            settings.mipmapEnabled = false;
            settings.sRGBTexture = true;
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = !isUI;
            settings.spritePixelsPerUnit = 100;
            settings.spriteExtrude = 0;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
            settings.readable = false;

            // Set sprite mode directly on the importer
            settings.spriteMode = isMultiple ? (int)SpriteImportMode.Multiple : (int)SpriteImportMode.Single;
            // Other settings
            settings.alphaIsTransparency = true;
            importer.maxTextureSize = isUI ? 1024 : 2048;
            importer.compressionQuality = 50;
            importer.textureCompression = TextureImporterCompression.Compressed;
            settings.filterMode = FilterMode.Bilinear;

            // Platform settings
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Android", TextureImporterFormat.RGBA32, 512, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("iPhone", TextureImporterFormat.RGBA32, 512, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("WebGL", TextureImporterFormat.RGBA32, 512, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Standalone", TextureImporterFormat.DXT5, 1024, 90));

        }

        private void ConfigureNormalMapTexture(TextureImporter importer, TextureImporterSettings settings)
        {
            settings.textureType = TextureImporterType.NormalMap;
            settings.textureShape = TextureImporterShape.Texture2D;
            settings.mipmapEnabled = true;
            settings.sRGBTexture = false;
            settings.normalMapFilter = TextureImporterNormalFilter.Standard;

            settings.alphaIsTransparency = false;
            settings.filterMode = FilterMode.Bilinear;

            // Platform settings
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Android", TextureImporterFormat.ASTC_5x5, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("iPhone", TextureImporterFormat.PVRTC_RGB4, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("WebGL", TextureImporterFormat.ETC2_RGBA8, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Standalone", TextureImporterFormat.DXT5, 2048, 90));
        }

        private void ConfigureLightmapTexture(TextureImporter importer, TextureImporterSettings settings)
        {
            settings.textureType = TextureImporterType.Lightmap;
            settings.textureShape = TextureImporterShape.Texture2D;
            settings.mipmapEnabled = true;
            settings.sRGBTexture = true;

            importer.maxTextureSize = 4096;
            importer.compressionQuality = 50;
            settings.alphaIsTransparency = false;
            settings.filterMode = FilterMode.Bilinear;

            // Platform settings
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Android", TextureImporterFormat.ASTC_5x5, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("iPhone", TextureImporterFormat.PVRTC_RGB4, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("WebGL", TextureImporterFormat.ETC2_RGBA8, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Standalone", TextureImporterFormat.DXT5, 2048, 90));
        }

        private void ConfigureCubeMapTexture(TextureImporter importer, TextureImporterSettings settings)
        {
            settings.textureType = TextureImporterType.Default;
            settings.textureShape = TextureImporterShape.TextureCube;
            settings.mipmapEnabled = true;
            settings.sRGBTexture = true;

            importer.maxTextureSize = 4096;
            importer.compressionQuality = 50;
            settings.alphaIsTransparency = false;
            settings.filterMode = FilterMode.Bilinear;

            // Platform settings
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Android", TextureImporterFormat.ASTC_5x5, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("iPhone", TextureImporterFormat.PVRTC_RGB4, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("WebGL", TextureImporterFormat.ETC2_RGBA8, 1024, 75));
            importer.SetPlatformTextureSettings(CreateTextureImporterPlatformSettings("Standalone", TextureImporterFormat.DXT5, 2048, 90));
        }

        // Helper for creating platform-specific texture settings
        private TextureImporterPlatformSettings CreateTextureImporterPlatformSettings(
            string platform, TextureImporterFormat format, int maxTextureSize, int compressionQuality)
        {
            return new TextureImporterPlatformSettings()
            {
                name = platform,
                overridden = true,
                format = format,
                maxTextureSize = maxTextureSize,
                compressionQuality = compressionQuality
            };
        }
    }
}