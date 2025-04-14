using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.Presets;

namespace OneDevApp.PredefinedPresetLibrary
{
  /// <summary>
  /// Implementation for Quality Settings presets
  /// </summary>
  public class QualityPresetProvider : BasePresetProvider
  {
    public override string Category => "Built-in Quality";

    public override void Initialize()
    {
      base.Initialize();

      presets.Add(new PresetDefinition(
          "Simple2D",
          "Create Simple 2D Game Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "Basic 2D games with minimal effects",
          true,
          false
      ));

      presets.Add(new PresetDefinition(
          "Effects2D",
          "Create 2D Game with Effects Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "2D games with particles, lights, and special effects",
          true,
          false
      ));

      presets.Add(new PresetDefinition(
          "HyperCasual",
          "Create Hyper Casual Game Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "Simple casual games optimized for mobile devices",
          true,
          false
      ));

      presets.Add(new PresetDefinition(
          "Game2_5D",
          "Create 2.5D Game Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "Games with 2D gameplay but 3D visuals",
          true,
          false
      ));

      presets.Add(new PresetDefinition(
          "Game3D",
          "Create Full 3D Game Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "Fully 3D games with advanced visual features",
          true,
          false
      ));

      presets.Add(new PresetDefinition(
          "MobileGame",
          "Create Mobile Game Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "3D games optimized for mobile performance",
          true,
          false
      ));

      presets.Add(new PresetDefinition(
          "VRGame",
          "Create VR Game Quality Levels",
          (fileName) => CreateFullQualityPreset(fileName),
          "Virtual reality applications with balanced performance",
          true,
          false
      ));
    }

    private void CreateFullQualityPreset(string gameTypePreset)
    {
      // Store original quality settings to restore later
      int originalQualityLevel = QualitySettings.GetQualityLevel();
      string[] qualityNames = QualitySettings.names;

      // Create a backup of all current quality settings
      Dictionary<string, object>[] backupSettings = new Dictionary<string, object>[qualityNames.Length];
      for (int i = 0; i < qualityNames.Length; i++)
      {
        QualitySettings.SetQualityLevel(i, false);
        backupSettings[i] = BackupCurrentQualitySettings();
      }

      try
      {
        // Verify that we have enough quality levels
        if (qualityNames.Length < 6)
        {
          // We can't directly reset to defaults, so we'll inform the user
          Debug.LogWarning("This project doesn't have the standard 6 quality levels. " +
                          "The preset will be created using the available quality levels.");

          // Ask user if they want to continue
          if (!EditorUtility.DisplayDialog("Quality Levels Warning",
              "This project doesn't have the standard 6 quality levels (Very Low to Ultra). " +
              "The preset will be created using only the available quality levels. Continue?",
              "Continue", "Cancel"))
          {
            return; // User chose to cancel
          }
        }

        // Configure each quality level based on the game type
        for (int i = 0; i < qualityNames.Length; i++)
        {
          QualitySettings.SetQualityLevel(i, false);

          // Apply appropriate settings for this quality level and game type
          switch (gameTypePreset)
          {
            case "Simple2D":
              ApplySimple2DSettings(i);
              break;
            case "Effects2D":
              Apply2DWithEffectsSettings(i);
              break;
            case "HyperCasual":
              ApplyHyperCasualSettings(i);
              break;
            case "Game2_5D":
              Apply2_5DSettings(i);
              break;
            case "Game3D":
              Apply3DSettings(i);
              break;
            case "MobileGame":
              ApplyMobileSettings(i);
              break;
            case "VRGame":
              ApplyVRSettings(i);
              break;
          }
        }

        // Get the quality settings asset
        var qualitySettingsObject = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0];
        if (qualitySettingsObject == null)
        {
          Debug.LogError("Could not find QualitySettings asset!");
        }

        // Create a preset from the quality settings
        Preset preset = new Preset(qualitySettingsObject);

        // Save the preset with a name based on game type
        string presetName = gameTypePreset.ToString();

        // Define preset directory
        string presetPath = $"Assets/Editor/Presets/{Category}/{presetName}.preset";
        EnsureDirectoryExists(presetPath);
        // Save the preset
        AssetDatabase.CreateAsset(preset, presetPath);
        AssetDatabase.Refresh();


        Debug.Log($"Full quality preset for {presetName} created at {presetPath}");
      }
      catch (Exception e)
      {
        Debug.LogError($"Error creating full quality preset: {e.Message}");
      }
      finally
      {
        // Restore all original quality settings
        for (int i = 0; i < backupSettings.Length; i++)
        {
          QualitySettings.SetQualityLevel(i, false);
          RestoreQualitySettings(backupSettings[i]);
        }

        // Restore original quality level
        QualitySettings.SetQualityLevel(originalQualityLevel, true);
      }

    }

    // Creates a backup of current quality settings
    private Dictionary<string, object> BackupCurrentQualitySettings()
    {
      Dictionary<string, object> settings = new Dictionary<string, object>();

      // Store current values
      settings["pixelLightCount"] = QualitySettings.pixelLightCount;
      settings["shadows"] = QualitySettings.shadows;
      settings["shadowResolution"] = QualitySettings.shadowResolution;
      settings["shadowProjection"] = QualitySettings.shadowProjection;
      settings["shadowCascades"] = QualitySettings.shadowCascades;
      settings["shadowDistance"] = QualitySettings.shadowDistance;
      settings["shadowNearPlaneOffset"] = QualitySettings.shadowNearPlaneOffset;
      settings["shadowCascade2Split"] = QualitySettings.shadowCascade2Split;
      settings["shadowCascade4Split"] = QualitySettings.shadowCascade4Split;
      settings["skinWeights"] = QualitySettings.skinWeights;
      settings["vSyncCount"] = QualitySettings.vSyncCount;
      settings["antiAliasing"] = QualitySettings.antiAliasing;
      settings["softParticles"] = QualitySettings.softParticles;
      settings["softVegetation"] = QualitySettings.softVegetation;
      settings["realtimeReflectionProbes"] = QualitySettings.realtimeReflectionProbes;
      settings["billboardsFaceCameraPosition"] = QualitySettings.billboardsFaceCameraPosition;
      settings["lodBias"] = QualitySettings.lodBias;
      settings["maximumLODLevel"] = QualitySettings.maximumLODLevel;
      settings["streamingMipmapsActive"] = QualitySettings.streamingMipmapsActive;
      settings["streamingMipmapsAddAllCameras"] = QualitySettings.streamingMipmapsAddAllCameras;
      settings["streamingMipmapsMemoryBudget"] = QualitySettings.streamingMipmapsMemoryBudget;
      settings["streamingMipmapsRenderersPerFrame"] = QualitySettings.streamingMipmapsRenderersPerFrame;
      settings["streamingMipmapsMaxLevelReduction"] = QualitySettings.streamingMipmapsMaxLevelReduction;
      settings["streamingMipmapsMaxFileIORequests"] = QualitySettings.streamingMipmapsMaxFileIORequests;
      settings["particleRaycastBudget"] = QualitySettings.particleRaycastBudget;
      settings["asyncUploadTimeSlice"] = QualitySettings.asyncUploadTimeSlice;
      settings["asyncUploadBufferSize"] = QualitySettings.asyncUploadBufferSize;
      settings["asyncUploadPersistentBuffer"] = QualitySettings.asyncUploadPersistentBuffer;
      settings["resolutionScalingFixedDPIFactor"] = QualitySettings.resolutionScalingFixedDPIFactor;
      settings["masterTextureLimit"] = QualitySettings.globalTextureMipmapLimit;
      settings["anisotropicFiltering"] = QualitySettings.anisotropicFiltering;

      return settings;
    }

    // Restores quality settings from backup
    private void RestoreQualitySettings(Dictionary<string, object> settings)
    {
      QualitySettings.pixelLightCount = (int)settings["pixelLightCount"];
      QualitySettings.shadows = (ShadowQuality)settings["shadows"];
      QualitySettings.shadowResolution = (ShadowResolution)settings["shadowResolution"];
      QualitySettings.shadowProjection = (ShadowProjection)settings["shadowProjection"];
      QualitySettings.shadowCascades = (int)settings["shadowCascades"];
      QualitySettings.shadowDistance = (float)settings["shadowDistance"];
      QualitySettings.shadowNearPlaneOffset = (float)settings["shadowNearPlaneOffset"];
      QualitySettings.shadowCascade2Split = (float)settings["shadowCascade2Split"];
      QualitySettings.shadowCascade4Split = (Vector3)settings["shadowCascade4Split"];
      QualitySettings.skinWeights = (SkinWeights)settings["skinWeights"];
      QualitySettings.vSyncCount = (int)settings["vSyncCount"];
      QualitySettings.antiAliasing = (int)settings["antiAliasing"];
      QualitySettings.softParticles = (bool)settings["softParticles"];
      QualitySettings.softVegetation = (bool)settings["softVegetation"];
      QualitySettings.realtimeReflectionProbes = (bool)settings["realtimeReflectionProbes"];
      QualitySettings.billboardsFaceCameraPosition = (bool)settings["billboardsFaceCameraPosition"];
      QualitySettings.lodBias = (float)settings["lodBias"];
      QualitySettings.maximumLODLevel = (int)settings["maximumLODLevel"];
      QualitySettings.streamingMipmapsActive = (bool)settings["streamingMipmapsActive"];
      QualitySettings.streamingMipmapsAddAllCameras = (bool)settings["streamingMipmapsAddAllCameras"];
      QualitySettings.streamingMipmapsMemoryBudget = (float)settings["streamingMipmapsMemoryBudget"];
      QualitySettings.streamingMipmapsRenderersPerFrame = (int)settings["streamingMipmapsRenderersPerFrame"];
      QualitySettings.streamingMipmapsMaxLevelReduction = (int)settings["streamingMipmapsMaxLevelReduction"];
      QualitySettings.streamingMipmapsMaxFileIORequests = (int)settings["streamingMipmapsMaxFileIORequests"];
      QualitySettings.particleRaycastBudget = (int)settings["particleRaycastBudget"];
      QualitySettings.asyncUploadTimeSlice = (int)settings["asyncUploadTimeSlice"];
      QualitySettings.asyncUploadBufferSize = (int)settings["asyncUploadBufferSize"];
      QualitySettings.asyncUploadPersistentBuffer = (bool)settings["asyncUploadPersistentBuffer"];
      QualitySettings.resolutionScalingFixedDPIFactor = (float)settings["resolutionScalingFixedDPIFactor"];
      QualitySettings.globalTextureMipmapLimit = (int)settings["masterTextureLimit"];
      QualitySettings.anisotropicFiltering = (AnisotropicFiltering)settings["anisotropicFiltering"];
    }

    #region Simple 2D Game Settings
    private void ApplySimple2DSettings(int qualityLevel)
    {
      // Base settings for 2D games - minimal 3D features
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 15f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 1;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 64;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 256;
          break;

        case 5: // Ultra
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 1024;
          break;
      }
    }
    #endregion

    #region 3D Game Settings
    private void Apply3DSettings(int qualityLevel)
    {
      // Full 3D game settings - maximum visual quality
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 64;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.5f;
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 30f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 1;
          QualitySettings.particleRaycastBudget = 256;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 8;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.75f;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 50f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 512;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 16;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 3;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 70f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 1024;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 32;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          QualitySettings.streamingMipmapsActive = true;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 4;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 4;
          QualitySettings.shadowDistance = 100f;
          QualitySettings.skinWeights = SkinWeights.FourBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.5f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 2048;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 64;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          QualitySettings.streamingMipmapsActive = true;
          QualitySettings.streamingMipmapsMemoryBudget = 512;
          break;

        case 5: // Ultra
          QualitySettings.pixelLightCount = 4;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
          QualitySettings.shadowCascades = 4;
          QualitySettings.shadowDistance = 150f;
          QualitySettings.shadowNearPlaneOffset = 3f;
          QualitySettings.skinWeights = SkinWeights.Unlimited;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 8;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 2.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 4096;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 128;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          QualitySettings.streamingMipmapsActive = true;
          QualitySettings.streamingMipmapsMemoryBudget = 1024;
          QualitySettings.streamingMipmapsMaxLevelReduction = 0;
          QualitySettings.streamingMipmapsAddAllCameras = true;
          QualitySettings.streamingMipmapsMaxFileIORequests = 1024;
          break;
      }
    }
    #endregion

    #region Mobile Game Settings
    private void ApplyMobileSettings(int qualityLevel)
    {
      // Mobile optimized settings - focus on performance
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 10f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 4;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.5f;
          Application.targetFrameRate = 30;
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 15f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 16;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.6f;
          Application.targetFrameRate = 30;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.5f;
          QualitySettings.globalTextureMipmapLimit = 1;
          QualitySettings.particleRaycastBudget = 32;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 8;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.75f;
          Application.targetFrameRate = 30;
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 30f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 64;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 16;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          Application.targetFrameRate = 30;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.8f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 256;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 32;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          Application.targetFrameRate = 60;
          break;

        case 5: // Ultra (high-end mobile devices)
          QualitySettings.pixelLightCount = 3;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 50f;
          QualitySettings.skinWeights = SkinWeights.FourBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 512;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 64;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          Application.targetFrameRate = 60;
          break;
      }
    }
    #endregion

    #region 2D Game with Effects Settings
    private void Apply2DWithEffectsSettings(int qualityLevel)
    {
      // 2D games with more visual effects
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 15f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 16;
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 1;
          QualitySettings.particleRaycastBudget = 64;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 256;
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 512;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 3;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 70f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 1.5f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 1024;
          break;

        case 5: // Ultra
          QualitySettings.pixelLightCount = 4;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 70f;
          QualitySettings.skinWeights = SkinWeights.FourBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 8;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 2.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 4096;
          break;
      }
    }
    #endregion

    #region Hyper Casual Game Settings
    private void ApplyHyperCasualSettings(int qualityLevel)
    {
      // Hyper casual - optimize for low-end and mobile devices across all quality levels
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 15f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 4;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.5f;
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 15f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 16;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.65f;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.5f;
          QualitySettings.globalTextureMipmapLimit = 1;
          QualitySettings.particleRaycastBudget = 32;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.8f;
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 64;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 8;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.8f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 256;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 16;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          break;

        case 5: // Ultra
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 1024;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 32;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          break;
      }
    }
    #endregion

    #region 2.5D Game Settings
    private void Apply2_5DSettings(int qualityLevel)
    {
      // 2.5D settings - balance between 2D and 3D
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 0;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 15f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = false;
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 16;
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 1;
          QualitySettings.particleRaycastBudget = 64;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 256;
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 2;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 512;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 3;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 60f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.5f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 1024;
          break;

        case 5: // Ultra
          QualitySettings.pixelLightCount = 4;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
          QualitySettings.shadowCascades = 4;
          QualitySettings.shadowDistance = 80f;
          QualitySettings.skinWeights = SkinWeights.FourBones;
          QualitySettings.vSyncCount = 1;
          QualitySettings.antiAliasing = 8;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 2.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 4096;
          break;
      }
    }
    #endregion

    #region VR Game Settings
    private void ApplyVRSettings(int qualityLevel)
    {
      // VR optimized settings - focus on high frame rate and reduced latency
      switch (qualityLevel)
      {
        case 0: // Very Low
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.Disable;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 20f;
          QualitySettings.skinWeights = SkinWeights.OneBone;
          QualitySettings.vSyncCount = 0; // VR SDKs typically manage VSync themselves
          QualitySettings.antiAliasing = 0;
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true; // Important for VR
          QualitySettings.lodBias = 0.3f;
          QualitySettings.globalTextureMipmapLimit = 2;
          QualitySettings.particleRaycastBudget = 32;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 4;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.7f; // Lower resolution for performance
          QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

          // VR-specific settings
          Application.targetFrameRate = 72; // Minimum comfortable VR frame rate
          break;

        case 1: // Low
          QualitySettings.pixelLightCount = 1;
          QualitySettings.shadows = ShadowQuality.HardOnly;
          QualitySettings.shadowResolution = ShadowResolution.Low;
          QualitySettings.shadowCascades = 1;
          QualitySettings.shadowDistance = 30f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 2; // Some AA is important for VR comfort
          QualitySettings.softParticles = false;
          QualitySettings.softVegetation = false;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.4f;
          QualitySettings.globalTextureMipmapLimit = 1;
          QualitySettings.particleRaycastBudget = 64;
          QualitySettings.asyncUploadTimeSlice = 1;
          QualitySettings.asyncUploadBufferSize = 8;
          QualitySettings.resolutionScalingFixedDPIFactor = 0.8f;
          QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable; // Important for text clarity in VR

          Application.targetFrameRate = 80;
          break;

        case 2: // Medium
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 40f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 4; // Higher AA for VR
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = false;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 0.7f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 256;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 16;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

          Application.targetFrameRate = 90; // Standard refresh rate for most VR headsets
          break;

        case 3: // High
          QualitySettings.pixelLightCount = 2;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.Medium;
          QualitySettings.shadowCascades = 2;
          QualitySettings.shadowDistance = 50f;
          QualitySettings.skinWeights = SkinWeights.TwoBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 512;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 32;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.0f;
          QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

          // VR-specific settings
          Application.targetFrameRate = 90;
          QualitySettings.streamingMipmapsActive = true;
          break;

        case 4: // Very High
          QualitySettings.pixelLightCount = 3;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.High;
          QualitySettings.shadowCascades = 4;
          QualitySettings.shadowDistance = 70f;
          QualitySettings.skinWeights = SkinWeights.FourBones;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 4;
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 1.5f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 1024;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 64;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.1f; // Slight supersampling
          QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

          Application.targetFrameRate = 90;
          QualitySettings.streamingMipmapsActive = true;
          QualitySettings.streamingMipmapsMemoryBudget = 512;
          break;

        case 5: // Ultra
          QualitySettings.pixelLightCount = 4;
          QualitySettings.shadows = ShadowQuality.All;
          QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
          QualitySettings.shadowCascades = 4;
          QualitySettings.shadowDistance = 100f;
          QualitySettings.shadowNearPlaneOffset = 3f;
          QualitySettings.skinWeights = SkinWeights.Unlimited;
          QualitySettings.vSyncCount = 0;
          QualitySettings.antiAliasing = 8; // Maximum AA for VR clarity
          QualitySettings.softParticles = true;
          QualitySettings.softVegetation = true;
          QualitySettings.realtimeReflectionProbes = true;
          QualitySettings.billboardsFaceCameraPosition = true;
          QualitySettings.lodBias = 2.0f;
          QualitySettings.globalTextureMipmapLimit = 0;
          QualitySettings.particleRaycastBudget = 2048;
          QualitySettings.asyncUploadTimeSlice = 2;
          QualitySettings.asyncUploadBufferSize = 128;
          QualitySettings.resolutionScalingFixedDPIFactor = 1.2f; // Higher supersampling for high-end systems
          QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

          // VR specific settings
          Application.targetFrameRate = 120; // Target high refresh rate for premium headsets
          QualitySettings.streamingMipmapsActive = true;
          QualitySettings.streamingMipmapsMemoryBudget = 1024;
          QualitySettings.streamingMipmapsMaxLevelReduction = 1;
          QualitySettings.streamingMipmapsAddAllCameras = true;
          QualitySettings.streamingMipmapsMaxFileIORequests = 1024;
          break;
      }
    }
    #endregion
  }
}