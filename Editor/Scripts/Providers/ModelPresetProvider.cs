
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace OneDevApp.PredefinedPresetLibrary
{

  /// <summary>
  /// Implementation for Model presets
  /// </summary>
  public class ModelPresetProvider : BasePresetProvider
  {
    public override string Category => "Models";

    public override void Initialize()
    {
      base.Initialize();

      presets.Add(new PresetDefinition(
          "Character - High Detail",
          "High poly character with animation",
          (fileName) => CreateModelPreset(fileName, true, true, ModelImporterMeshCompression.Off),
          "Main player characters and important NPCs",
          true,
          true,
          "*_Character_HD",
          "Applies to filenames ending with _Character_HD"
      ));

      presets.Add(new PresetDefinition(
          "Character - Low Poly",
          "Low poly character with animation",
          (fileName) => CreateModelPreset(fileName, true, true, ModelImporterMeshCompression.Medium),
          "Background NPCs and mobile game characters",
          true,
          true,
          "*_Character_LP",
          "Applies to filenames ending with _Character_LP"
      ));

      presets.Add(new PresetDefinition(
          "Environment - Static",
          "Static environment with lightmap UVs",
          (fileName) => CreateModelPreset(fileName, false, true, ModelImporterMeshCompression.Low),
          "Buildings, terrain, and non-moving scenery",
          true,
          true,
          "*_Env_Static",
          "Applies to filenames ending with _Env_Static"
      ));

      presets.Add(new PresetDefinition(
          "Environment - Dynamic",
          "Dynamic environment with no lightmap UVs",
          (fileName) => CreateModelPreset(fileName, false, false, ModelImporterMeshCompression.Medium),
          "Moving or interactive environmental objects",
          true,
          true,
          "*_Env_Dynamic",
          "Applies to filenames ending with _Env_Dynamic"
      ));

      presets.Add(new PresetDefinition(
          "Props - High Detail",
          "High detail props with no animation",
          (fileName) => CreateModelPreset(fileName, false, true, ModelImporterMeshCompression.Off),
          "Foreground props and important game objects",
          true,
          true,
          "*_Prop_HD",
          "Applies to filenames ending with _Prop_HD"
      ));

      presets.Add(new PresetDefinition(
          "Props - Low Poly",
          "Low poly props for mobile",
          (fileName) => CreateModelPreset(fileName, false, false, ModelImporterMeshCompression.High),
          "Background props and mobile-optimized objects",
          true,
          true,
          "*_Prop_LP",
          "Applies to filenames ending with _Prop_LP"
      ));
    }


    private void CreateModelPreset(string presetName, bool importAnimation, bool generateLightmapUVs, ModelImporterMeshCompression meshCompression)
    {
      // Create temporary FBX model or find a default one
      string tempPath = "Packages/com.onedevapp.predefinedpresetlibrary/Editor/Models/preset_cube_model.fbx";
      ModelImporter importer = AssetImporter.GetAtPath(tempPath) as ModelImporter;
      if (importer != null)
      {
        // Model settings
        importer.generateAnimations = importAnimation ? ModelImporterGenerateAnimations.InRoot : ModelImporterGenerateAnimations.None;
        importer.animationType = importAnimation ? ModelImporterAnimationType.Generic : ModelImporterAnimationType.None;
        importer.importAnimation = importAnimation;

        // Mesh settings
        importer.generateSecondaryUV = generateLightmapUVs;
        importer.meshCompression = meshCompression;
        importer.weldVertices = true;

        // Material settings
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;

        // Platform override settings
        SetupModelPlatformSettings(importer, meshCompression, presetName.Contains("LowPoly"));

        // Apply changes
        AssetDatabase.ImportAsset(tempPath);

        // Create preset
        Preset preset = new Preset(importer);

        // Define preset directory
        string presetPath = $"Assets/Editor/Presets/{Category}/{presetName}.preset";
        EnsureDirectoryExists(presetPath);
        // Save the preset
        AssetDatabase.CreateAsset(preset, presetPath);
        AssetDatabase.Refresh();

      }
      else
      {
        Debug.LogError("Failed to create model preset. ModelImporter is null.");
      }

    }

    private void SetupModelPlatformSettings(ModelImporter importer, ModelImporterMeshCompression defaultCompression, bool isMobileFriendlyPreset = false)
    {
      // Set global compression settings
      importer.meshCompression = defaultCompression;

      if (isMobileFriendlyPreset)
      {
        // Optimize mesh for mobile
        importer.optimizeMeshVertices = true;
        importer.optimizeMeshPolygons = true;
        importer.importBlendShapes = false; // Disable blend shapes for mobile
        importer.importVisibility = false;
        importer.importCameras = false;
        importer.importLights = false;

        // More aggressive mesh compression for mobile
        importer.meshCompression = ModelImporterMeshCompression.High;
      }
      else
      {
        // High quality for desktop/console
        importer.optimizeMeshVertices = false;
        importer.optimizeMeshPolygons = false;
        importer.importBlendShapes = true;
        importer.importVisibility = true;
        importer.importCameras = true;
        importer.importLights = true;
      }

      // Animation optimization
      if (importer.importAnimation)
      {
        importer.animationCompression = isMobileFriendlyPreset ?
            ModelImporterAnimationCompression.Optimal :
            ModelImporterAnimationCompression.KeyframeReduction;

        // Reduce animation file size for mobile
        if (isMobileFriendlyPreset)
        {
          importer.animationRotationError = 0.5f;  // Higher error tolerance = smaller file
          importer.animationPositionError = 0.5f;
          importer.animationScaleError = 0.5f;
        }
        else
        {
          importer.animationRotationError = 0.1f;  // Lower error tolerance = higher quality
          importer.animationPositionError = 0.1f;
          importer.animationScaleError = 0.1f;
        }
      }
    }
  }

}