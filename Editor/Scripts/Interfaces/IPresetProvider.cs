

using System.Collections.Generic;

namespace OneDevApp.PredefinedPresetLibrary
{
  /// <summary>
  /// Interface for preset providers to implement
  /// </summary>
  public interface IPresetProvider
  {
    string Category { get; }
    void Initialize();
    List<PresetDefinition> GetPresets();
    void CreatePreset(PresetDefinition preset);
    void SetAsDefaultPreset(PresetDefinition preset);
    void SetPresetWithFilter(PresetDefinition preset, string filter);

  }
}