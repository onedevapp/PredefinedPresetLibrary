
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;


namespace OneDevApp.PredefinedPresetLibrary
{
  /// <summary>
  /// Implementation for Component presets (UI, etc.)
  /// </summary>
  public class ComponentPresetProvider : BasePresetProvider
  {
    public override string Category => "Components";

    public override void Initialize()
    {
      base.Initialize();

      presets.Add(new PresetDefinition(
          "Image - Icon",
          "Optimized UI Image component for icons",
          (fileName) => CreateUIComponentPreset(fileName, typeof(UnityEngine.UI.Image)),
          "UI icons, small graphics, and non-interactive UI elements",
          true,
          true,
          "*_Icon",
          "Applies to components in GameObjects named with _Icon"
      ));

      // Add more UI component presets as needed
    }

    private void CreateUIComponentPreset(string presetName, Type componentType)
    {
      // Create a temporary Canvas to host UI components
      GameObject canvasGO = new GameObject("TempCanvasForPreset");
      Canvas canvas = canvasGO.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
      canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

      // Create a child GameObject for the UI component
      GameObject uiGO = new GameObject("TempUIComponentForPreset");
      uiGO.transform.SetParent(canvasGO.transform, false);
      Component component = null;

      if (componentType == typeof(UnityEngine.UI.Image))
      {
        component = SetupImagePreset(uiGO.AddComponent<UnityEngine.UI.Image>(), presetName);
      }

      if (component != null)
      {
        // Create preset from the component
        Preset preset = new Preset(component);

        // Define preset directory
        string presetPath = $"Assets/Editor/Presets/{Category}/{presetName}.preset";
        EnsureDirectoryExists(presetPath);
        // Save the preset
        AssetDatabase.CreateAsset(preset, presetPath);
        AssetDatabase.Refresh();
        UnityEngine.Object.DestroyImmediate(canvasGO);
      }

      // Clean up
      UnityEngine.Object.DestroyImmediate(canvasGO);
    }

    private UnityEngine.UI.Image SetupImagePreset(UnityEngine.UI.Image image, string presetType)
    {
      // Common properties
      image.raycastTarget = false;

      // Configure based on preset type
      if (presetType == "Image_Icon")
      {
        image.preserveAspect = true;
      }

      return image;
    }
  }

}