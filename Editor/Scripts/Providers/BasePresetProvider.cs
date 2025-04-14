using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Presets;
using System.Linq;
using System;

namespace OneDevApp.PredefinedPresetLibrary
{

  /// <summary>
  /// Base class for preset providers with fixed functionality for default presets and filters
  /// </summary>
  public abstract class BasePresetProvider : IPresetProvider
  {
    protected List<PresetDefinition> presets = new List<PresetDefinition>();

    public abstract string Category { get; }

    public virtual void Initialize()
    {
      // Implement in derived classes
      presets.Clear();
    }

    public List<PresetDefinition> GetPresets()
    {
      return presets;
    }

    public virtual void CreatePreset(PresetDefinition preset)
    {
      try
      {
        // Execute the creation action with the path
        preset.CreateAction(preset.GetFileName);
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error creating preset {preset.Name}: {ex.Message}");
      }
    }

    public virtual void SetPresetWithFilter(PresetDefinition presetDefinition, string filter)
    {
      if (string.IsNullOrWhiteSpace(filter))
      {
        Debug.LogError("Filter cannot be empty when creating a preset with filter");
        return;
      }

      try
      {
        Preset presetAsset = AssetDatabase.LoadAssetAtPath<Preset>($"Assets/Editor/Presets/{Category}/{presetDefinition.GetFileName}.preset");
        if (presetAsset != null)
        {
          SetPresetWithFilter(presetAsset, filter);
        }
        else
        {
          Debug.LogError($"Failed to load preset asset at: Assets/Editor/Presets/{Category}/{presetDefinition.GetFileName}.preset");
        }
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error creating preset {presetDefinition.Name} with filter: {ex.Message}");
      }
    }

    // Helper method to ensure directory exists
    protected void EnsureDirectoryExists(string path)
    {
      string directory = Path.GetDirectoryName(path);
      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }
    }

    public void SetAsDefaultPreset(PresetDefinition presetDefinition)
    {

      Preset preset = AssetDatabase.LoadAssetAtPath<Preset>($"Assets/Editor/Presets/{Category}/{presetDefinition.GetFileName}.preset");
      if (preset == null) return;

      var type = preset.GetPresetType();
      if (type.IsValidDefault())
      {
        try
        {
          // Get current default presets
          var currentDefaults = Preset.GetDefaultPresetsForType(type);
          List<DefaultPreset> defaultPresets = currentDefaults != null
              ? new List<DefaultPreset>(currentDefaults)
              : new List<DefaultPreset>();

          if (defaultPresets.Count > 0)
          {
            var oldDefaultPreset = defaultPresets[0];
            if (oldDefaultPreset.filter == string.Empty && oldDefaultPreset.enabled)
            {
              oldDefaultPreset.enabled = false;
              defaultPresets[0] = oldDefaultPreset;
            }

            int index = defaultPresets.FindIndex(x => x.preset.GetInstanceID() == preset.GetInstanceID());
            if (index > -1)
            {
              var defaultPreset = defaultPresets[index];
              defaultPreset.enabled = true;
              defaultPreset.filter = string.Empty;
              defaultPresets.RemoveAt(index);
              defaultPresets.Insert(0, defaultPreset);
            }
            else
            {
              defaultPresets.Insert(0, new DefaultPreset(string.Empty, preset));
            }

          }
          else
          {
            // Add this preset as the default (at the beginning)
            defaultPresets.Add(new DefaultPreset(string.Empty, preset));
          }
          // Apply updated defaults
          Preset.SetDefaultPresetsForType(type, defaultPresets.ToArray());

          // Force refresh
          AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
          Debug.LogError($"Failed to set preset as default: {ex.Message}");
        }
      }
      else
      {
        Debug.LogWarning($"Preset type {type.GetManagedTypeName()} does not support default presets");
      }
    }
    protected void SetPresetWithFilter(Preset preset, string filter)
    {
      if (preset == null || string.IsNullOrWhiteSpace(filter)) return;

      var type = preset.GetPresetType();
      if (type.IsValidDefault())
      {
        try
        {
          // Get current default presets
          var currentDefaults = Preset.GetDefaultPresetsForType(type);
          List<DefaultPreset> defaultPresets = currentDefaults != null
              ? new List<DefaultPreset>(currentDefaults)
              : new List<DefaultPreset>();

          int index = defaultPresets.FindIndex(x => x.preset.GetInstanceID() == preset.GetInstanceID());
          if (index > -1)
          {
            var defaultPreset = defaultPresets[index];
            defaultPreset.enabled = true;
            defaultPreset.filter = filter;
            defaultPresets[index] = defaultPreset;
          }
          else
          {
            // Add our new filter
            defaultPresets.Add(new DefaultPreset(filter, preset));
          }

          // Apply updated defaults
          Preset.SetDefaultPresetsForType(type, defaultPresets.ToArray());

          // Force refresh
          AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
          Debug.LogError($"Failed to set preset with filter: {ex.Message}");
        }
      }
      else
      {
        Debug.LogWarning($"Preset type {type.GetManagedTypeName()} does not support filtered presets");
      }
    }
  }
}