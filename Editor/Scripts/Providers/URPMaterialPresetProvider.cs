using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Presets;


namespace OneDevApp.PredefinedPresetLibrary
{
    /// <summary>
    /// Implementation for URP Material presets
    /// </summary>
    public class URPMaterialPresetProvider : BasePresetProvider
    {
        public override string Category => "URP Materials";

        public override void Initialize()
        {
            base.Initialize();

            presets.Add(new PresetDefinition(
                "URP - Lit Standard",
                "Standard URP lit material",
                (fileName) => CreateURPMaterialPreset(fileName, "Universal Render Pipeline/Lit"),
                "General purpose materials for most objects",
                false,
                false,
                "*_Lit",
                "Applies to materials ending with _Lit"
            ));

            presets.Add(new PresetDefinition(
                "URP - Unlit",
                "Unlit URP material",
                (fileName) => CreateURPMaterialPreset(fileName, "Universal Render Pipeline/Unlit"),
                "UI elements, skyboxes, and non-shaded objects",
                false,
                false,
                "*_Unlit",
                "Applies to materials ending with _Unlit"
            ));

            presets.Add(new PresetDefinition(
                "URP - Simple Lit",
                "Simplified lit URP material",
                (fileName) => CreateURPMaterialPreset(fileName, "Universal Render Pipeline/Simple Lit"),
                "Mobile-friendly objects and performance-critical scenarios",
                false,
                false,
                "*_SimpleLit",
                "Applies to materials ending with _SimpleLit"
            ));

            presets.Add(new PresetDefinition(
                "URP - Transparent",
                "Transparent URP material",
                (fileName) => CreateURPMaterialPreset(fileName, "Universal Render Pipeline/Lit", true),
                "Glass, water, and semi-transparent surfaces",
                false,
                false,
                "*_Transparent",
                "Applies to materials ending with _Transparent"
            ));

            presets.Add(new PresetDefinition(
                "URP - Cutout",
                "Cutout URP material",
                (fileName) => CreateURPMaterialPreset(fileName, "Universal Render Pipeline/Lit", false, true),
                "Foliage, fences, and objects with transparent parts",
                false,
                false,
                "*_Cutout",
                "Applies to materials ending with _Cutout"
            ));
        }

        private void CreateURPMaterialPreset(string presetName, string shaderName, bool isTransparent = false, bool isCutout = false)
        {
            try
            {
                // Check if shader exists
                Shader shader = Shader.Find(shaderName);
                if (shader == null)
                {
                    Debug.LogError($"Shader {shaderName} not found. URP may not be installed.");
                }

                // Create a new material
                Material material = new Material(shader);

                if (material != null)
                {
                    // Configure material properties
                    if (isTransparent)
                    {
                        material.SetFloat("_Surface", 1); // Transparent
                        material.SetFloat("_Blend", 0);   // SrcAlpha
                        material.SetFloat("_DstBlend", 10); // OneMinusSrcAlpha
                        material.SetFloat("_ZWrite", 0);    // Off
                        material.renderQueue = 3000;        // Transparent
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    }
                    else if (isCutout)
                    {
                        material.SetFloat("_Surface", 0);      // Opaque
                        material.SetFloat("_AlphaClip", 1);    // Alpha Clipping
                        material.SetFloat("_Cutoff", 0.5f);    // Cutoff value
                        material.renderQueue = 2450;           // AlphaTest
                        material.EnableKeyword("_ALPHATEST_ON");
                    }

                    // Create a temporary folder for intermediate assets
                    string tempFolder = "Assets/Editor/Temp";
                    string tempPath = $"{tempFolder}/tempMaterial.mat";
                    EnsureDirectoryExists(tempPath);

                    // Save the material
                    AssetDatabase.CreateAsset(material, tempPath);
                    AssetDatabase.Refresh();

                    // Create preset
                    Preset preset = new Preset(material);

                    // Define preset directory
                    string presetPath = $"Assets/Editor/Presets/{Category}/{presetName}.preset";
                    EnsureDirectoryExists(presetPath);
                    // Save the preset
                    AssetDatabase.CreateAsset(preset, presetPath);
                    AssetDatabase.Refresh();


                    // Delete temporary asset
                    AssetDatabase.DeleteAsset(tempPath);
                    if (Directory.Exists(tempFolder))
                    {
                        Directory.Delete(tempFolder, true);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to create material with shader {shaderName}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating URP material preset: {ex.Message}");
            }
        }
    }
}