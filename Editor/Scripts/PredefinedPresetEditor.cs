using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor.Presets;
using System.Linq;

namespace OneDevApp.PredefinedPresetLibrary
{
    /// <summary>
    /// The main editor window for the Asset Preset Library with improved UI
    /// </summary>
    public class PredefinedPresetEditor : EditorWindow
    {
        private Vector2 scrollPosition;
        private string successMessage = "";
        private bool showSuccessMessage = false;
        private float messageTimer = 0;

        // Dictionary to track expanded/collapsed state of categories
        private Dictionary<string, bool> categoryFoldouts = new Dictionary<string, bool>();

        // Registry for all preset providers
        private List<IPresetProvider> presetProviders = new List<IPresetProvider>();

        // Singleton instance for easy access
        private static PredefinedPresetEditor _instance;
        public static PredefinedPresetEditor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<PredefinedPresetEditor>();
                }
                return _instance;
            }
        }

        [MenuItem("Tools/Predefined Preset Library")]
        public static void ShowWindow()
        {
            _instance = GetWindow<PredefinedPresetEditor>("Predefined Preset Library");
            _instance.minSize = new Vector2(600, 400);
        }

        private void OnEnable()
        {
            // Register all preset providers
            RegisterPresetProviders();

            // Initialize category foldouts
            foreach (var provider in presetProviders)
            {
                if (!categoryFoldouts.ContainsKey(provider.Category))
                {
                    categoryFoldouts[provider.Category] = true; // Default to expanded
                }
            }
        }

        private void RegisterPresetProviders()
        {
            presetProviders.Clear();

            // Register built-in providers
            presetProviders.Add(new TexturePresetProvider());
            presetProviders.Add(new AudioPresetProvider());
            presetProviders.Add(new ModelPresetProvider());
            presetProviders.Add(new ComponentPresetProvider());

            // Check which render pipeline is active and register appropriate providers
            bool isURP = DetectRenderPipeline();
            if (isURP)
            {
                presetProviders.Add(new URPMaterialPresetProvider());
            }
            else
            {
                presetProviders.Add(new BuiltInMaterialPresetProvider());
                presetProviders.Add(new QualityPresetProvider());
            }

            // Initialize all providers
            foreach (var provider in presetProviders)
            {
                provider.Initialize();
            }
        }

        private bool DetectRenderPipeline()
        {
            // Check if URP is installed and active
            try
            {
                var pipelineAsset = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
                return pipelineAsset != null && pipelineAsset.GetType().ToString().Contains("Universal");
            }
            catch
            {
                return false;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Header
            GUILayout.Label("Asset Preset Library", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Preset Categories and Items
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var provider in presetProviders)
            {
                DrawCategoryWithPresets(provider);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();

            // Display success message
            if (showSuccessMessage)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(successMessage, MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCategoryWithPresets(IPresetProvider provider)
        {
            // Ensure the category exists in dictionary
            if (!categoryFoldouts.ContainsKey(provider.Category))
            {
                categoryFoldouts[provider.Category] = true;
            }

            // Draw foldout header
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Use bold foldout style for category
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            bool expanded = categoryFoldouts[provider.Category];
            bool newExpanded = EditorGUILayout.Foldout(expanded, provider.Category, true, foldoutStyle);

            // If the expansion state changed
            if (expanded != newExpanded)
            {
                categoryFoldouts[provider.Category] = newExpanded;
            }

            // If expanded, draw the preset items
            if (categoryFoldouts[provider.Category])
            {
                EditorGUILayout.Space();
                foreach (var preset in provider.GetPresets())
                {
                    DrawPresetItem(preset, provider);
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndVertical();
        }

private void DrawPresetItem(PresetDefinition preset, IPresetProvider provider)
{
    // Main preset row
    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

    // Top row with preset name, description, and create button
    EditorGUILayout.BeginHorizontal();

    // Left side content
    EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth - 150));
    GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel);
    nameStyle.fontSize = 12;
    EditorGUILayout.LabelField(preset.Name, nameStyle);
    EditorGUILayout.LabelField(preset.Description, EditorStyles.miniLabel);

    if (!string.IsNullOrEmpty(preset.BestUsedFor))
    {
        GUIStyle italicStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontStyle = FontStyle.Italic,
            normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
        };
        EditorGUILayout.LabelField("Best for: " + preset.BestUsedFor, italicStyle);
    }
    EditorGUILayout.EndVertical();

    // Right side button
    bool presetExists = DoesPresetExist(preset, provider.Category);

    if (!presetExists)
    {
        // Show only Create button if preset doesn't exist
        if (GUILayout.Button("Create Preset", GUILayout.Width(120), GUILayout.Height(30)))
        {
            provider.CreatePreset(preset);
            ShowSuccessMessage($"Created preset: {preset.Name}");
        }
    }
    else
    {
        // Show applicable buttons for existing preset
        EditorGUILayout.BeginVertical(GUILayout.Width(120));

        bool isDefault = IsPresetDefault(preset, provider.Category);
        if (preset.CanBeDefault && !isDefault)
        {
            if (GUILayout.Button("Set as Default", GUILayout.Width(120), GUILayout.Height(30)))
            {
                provider.SetAsDefaultPreset(preset);
                ShowSuccessMessage($"Set {preset.Name} as default preset");
            }
        }

        if (preset.CanUseFilter)
        {
            if (GUILayout.Button("Add with Filter", GUILayout.Width(120), GUILayout.Height(30)))
            {
                ShowFilterDialog(preset, provider);
            }
        }

        EditorGUILayout.EndVertical();
    }

    EditorGUILayout.EndHorizontal();
    EditorGUILayout.EndVertical();
}    // Check if the preset file exists
private bool DoesPresetExist(PresetDefinition preset, string Category)
{
    string expectedPath = $"Assets/Editor/Presets/{Category}/{preset.GetFileName}.preset";
    return File.Exists(expectedPath);
}

        // Status method to determine if this preset is already set as default
        private bool IsPresetDefault(PresetDefinition preset, string Category)
        {
            if (!DoesPresetExist(preset, Category)) return false;

            string path = $"Assets/Editor/Presets/{Category}/{preset.GetFileName}.preset";
            Preset presetAsset = AssetDatabase.LoadAssetAtPath<Preset>(path);
            if (presetAsset == null) return false;

            var type = presetAsset.GetPresetType();
            var defaultPresets = Preset.GetDefaultPresetsForType(type);

            return defaultPresets.Any(x => x.preset.GetInstanceID() == presetAsset.GetInstanceID() && x.enabled && x.filter == string.Empty);
        }

   private void ShowFilterDialog(PresetDefinition preset, IPresetProvider provider)
        {
            // Show filter dialog as a popup window
            FilterPopupWindow.ShowWindow(preset, provider, (success, message) =>
            {
                if (success)
                {
                    ShowSuccessMessage(message);
                }
            });
        }

        public void ShowSuccessMessage(string message)
        {
            successMessage = message;
            showSuccessMessage = true;
            messageTimer = 3.0f;
            Repaint();
        }

        private void Update()
        {
            // Handle timed success message
            if (showSuccessMessage)
            {
                messageTimer -= 0.01f;
                if (messageTimer <= 0)
                {
                    showSuccessMessage = false;
                    Repaint();
                }
            }
        }
    }

    /// <summary>
    /// Popup window for filter input
    /// </summary>
    public class FilterPopupWindow : EditorWindow
    {
        private PresetDefinition preset;
        private IPresetProvider provider;
        private string filterInput = "";
        private Action<bool, string> callback;
        private bool useRecommendedFilter = true;

        public static void ShowWindow(PresetDefinition preset, IPresetProvider provider, Action<bool, string> callback)
        {
            var window = GetWindow<FilterPopupWindow>(true, $"Filter for: {preset.Name}", true);
            window.minSize = new Vector2(400, 180);
            window.maxSize = new Vector2(500, 250);
            window.preset = preset;
            window.provider = provider;
            window.callback = callback;

            // Set recommended filter if available
            if (!string.IsNullOrEmpty(preset.RecommendedFilter))
            {
                window.filterInput = preset.RecommendedFilter;
            }

            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

            EditorGUILayout.LabelField($"Add Preset Filter for: {preset.Name}", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Filters allow the preset to be automatically applied to assets matching the filter criteria.", MessageType.Info);

            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(preset.RecommendedFilter))
            {
                useRecommendedFilter = EditorGUILayout.Toggle("Use Recommended Filter", useRecommendedFilter);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Filter Value");

                if (useRecommendedFilter)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField(preset.RecommendedFilter);
                    EditorGUI.EndDisabledGroup();
                    filterInput = preset.RecommendedFilter;
                }
                else
                {
                    filterInput = EditorGUILayout.TextField(filterInput);
                }
                EditorGUILayout.EndHorizontal();

                if (useRecommendedFilter && !string.IsNullOrEmpty(preset.RecommendedFilterDescription))
                {
                    EditorGUILayout.HelpBox(preset.RecommendedFilterDescription, MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Filter Value");
                filterInput = EditorGUILayout.TextField(filterInput);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Examples: '*_normal*' for normal maps, '*@2x*' for high-res textures", MessageType.Info);
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                Close();
            }

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(filterInput));
            if (GUILayout.Button("Apply With Filter", GUILayout.Width(150)))
            {
                provider.SetPresetWithFilter(preset, filterInput);
                callback?.Invoke(true, $"Created preset {preset.Name} with filter: {filterInput}");
                Close();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}