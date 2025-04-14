
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;


namespace OneDevApp.PredefinedPresetLibrary
{
  /// <summary>
  /// Implementation for Built-in Material presets
  /// </summary>
  public class BuiltInMaterialPresetProvider : BasePresetProvider
  {
    public override string Category => "Built-in Materials";

    public override void Initialize()
    {
      base.Initialize();

      presets.Add(new PresetDefinition(
          "Standard",
          "Standard built-in material",
          (fileName) => CreateBuiltinMaterialPreset(fileName, "Standard"),
          "General purpose materials for most objects",
          false,
          false,
          "*_Standard",
          "Applies to materials ending with _Standard"
      ));

      presets.Add(new PresetDefinition(
          "Standard Specular",
          "Standard specular material",
          (fileName) => CreateBuiltinMaterialPreset(fileName, "Standard (Specular setup)"),
          "Shiny objects with specular highlights",
          false,
          false,
          "*_Specular",
          "Applies to materials ending with _Specular"
      ));

      presets.Add(new PresetDefinition(
          "Unlit",
          "Unlit built-in material",
          (fileName) => CreateBuiltinMaterialPreset(fileName, "Unlit/Color"),
          "UI elements, skyboxes, and non-shaded objects",
          false,
          false,
          "*_Unlit",
          "Applies to materials ending with _Unlit"
      ));

      presets.Add(new PresetDefinition(
          "Transparent",
          "Transparent material",
          (fileName) => CreateBuiltinMaterialPreset(fileName, "Standard", true),
          "Glass, water, and semi-transparent surfaces",
          false,
          false,
          "*_Transparent",
          "Applies to materials ending with _Transparent"
      ));

      presets.Add(new PresetDefinition(
          "Cutout",
          "Cutout material",
          (fileName) => CreateBuiltinMaterialPreset(fileName, "Standard", false, true),
          "Foliage, fences, and objects with transparent parts",
          false,
          false,
          "*_Cutout",
          "Applies to materials ending with _Cutout"
      ));

      presets.Add(new PresetDefinition(
          "Mobile Optimized",
          "Mobile-friendly material",
          (fileName) => CreateBuiltinMaterialPreset(fileName, "Mobile/Diffuse"),
          "Low-end mobile devices and performance-critical scenarios",
          false,
          false,
          "*_Mobile",
          "Applies to materials ending with _Mobile"
      ));
    }


    private void CreateBuiltinMaterialPreset(string presetName, string shaderName, bool isTransparent = false, bool isCutout = false)
    {
      // Create a new material
      Material material = new Material(Shader.Find(shaderName));

      if (material != null)
      {
        // Configure material properties
        if (isTransparent)
        {
          material.SetFloat("_Mode", 3); // Transparent
          material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
          material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
          material.SetInt("_ZWrite", 0);
          material.DisableKeyword("_ALPHATEST_ON");
          material.EnableKeyword("_ALPHABLEND_ON");
          material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
          material.renderQueue = 3000;
        }
        else if (isCutout)
        {
          material.SetFloat("_Mode", 1); // Cutout
          material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
          material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
          material.SetInt("_ZWrite", 1);
          material.EnableKeyword("_ALPHATEST_ON");
          material.DisableKeyword("_ALPHABLEND_ON");
          material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
          material.renderQueue = 2450;
        }

        // Create a temporary texture
        string tempFolder = "Assets/Editor/Temp";
        string tempPath = $"{tempFolder}/tempMaterial.mat";
        EnsureDirectoryExists(tempPath);

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
        AssetDatabase.DeleteAsset(tempFolder);
      }
      else
      {
        Debug.LogError("Failed to create material preset. Material is null.");
      }
    }
  }

}