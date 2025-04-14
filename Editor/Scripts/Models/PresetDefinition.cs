using System;

namespace OneDevApp.PredefinedPresetLibrary
{
  /// <summary>
  /// Enhanced preset definition with additional metadata
  /// </summary>
  [Serializable]
  public class PresetDefinition
  {
    /// <summary>
    /// Name of the preset displayed in the UI
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Brief description of the preset
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Detailed information about when this preset is most useful
    /// </summary>
    public string BestUsedFor { get; private set; }

    /// <summary>
    /// Whether this preset can be set as a default preset for its type
    /// </summary>
    public bool CanBeDefault { get; private set; }

    /// <summary>
    /// Whether this preset can be applied with a filter
    /// </summary>
    public bool CanUseFilter { get; private set; }

    /// <summary>
    /// Recommended filter pattern if available
    /// </summary>
    public string RecommendedFilter { get; private set; }

    /// <summary>
    /// Description of what the recommended filter does
    /// </summary>
    public string RecommendedFilterDescription { get; private set; }

    public string GetFileName => Name.Replace(" ", "").Replace("-", "_");

    /// <summary>
    /// Action that creates the preset and returns the asset path
    /// </summary>
    public Action<string> CreateAction { get; private set; }

    /// <summary>
    /// Creates a new preset definition with enhanced metadata
    /// </summary>
    /// <param name="name">Name of the preset</param>
    /// <param name="description">Brief description</param>
    /// <param name="createAction">Action that creates the preset</param>
    /// <param name="bestUsedFor">When this preset is most useful</param>
    /// <param name="canBeDefault">Whether it can be set as default</param>
    /// <param name="canUseFilter">Whether it can use a filter</param>
    /// <param name="recommendedFilter">Recommended filter pattern</param>
    /// <param name="recommendedFilterDescription">Description of the filter</param>
    public PresetDefinition(
        string name,
        string description,
        Action<string> createAction,
        string bestUsedFor = "",
        bool canBeDefault = true,
        bool canUseFilter = true,
        string recommendedFilter = "",
        string recommendedFilterDescription = "")
    {
      Name = name;
      Description = description;
      CreateAction = createAction;
      BestUsedFor = bestUsedFor;
      CanBeDefault = canBeDefault;
      CanUseFilter = canUseFilter;
      RecommendedFilter = recommendedFilter;
      RecommendedFilterDescription = recommendedFilterDescription;
    }


  }
}