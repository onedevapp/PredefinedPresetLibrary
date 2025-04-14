# Predefined Presets

This system provides a flexible, extensible framework for creating and managing Unity asset presets. It organizes presets by category, offers detailed preset information, and supports both default presets and filtered presets.

## Installation
via Package Manager (**Add package from git url**):

```
https://github.com/onedevapp/PredefinedPresetLibrary.git
```

## ScreenShot

![PredefinedPresetLibrary](https://github.com/onedevapp/PredefinedPresetLibrary/blob/main/.github/predefined_preset_library.png)


## Note:
This system generate Unity's [Preset](https://docs.unity3d.com/Manual/Presets.html) programmatically and set to Unity's [PresetManager](https://docs.unity3d.com/Manual/class-PresetManager.html) as default or with filter.



## Architecture Overview

The Predefined Presets is built using the following components:

### Core Classes

- **PredefinedPresetEditor**: The main EditorWindow that displays available presets and handles user interactions
- **PresetDefinition**: Defines a preset with enhanced metadata (usage recommendations, filter options)
- **IPresetProvider**: Interface for preset providers to implement
- **BasePresetProvider**: Abstract base class with common functionality for preset providers

### Preset Providers

Each asset type has its own provider class:

- **TexturePresetProvider**: Handles texture import settings presets
- **AudioPresetProvider**: Handles audio import settings presets
- **ModelPresetProvider**: Handles 3D model import settings presets
- **ComponentPresetProvider**: Handles Unity component presets (e.g., UI elements)
- **URPMaterialPresetProvider**: Handles URP material presets
- **BuiltInMaterialPresetProvider**: Handles built-in render pipeline material presets
- **QualityPresetProvider**: Handles quality settings presets

### Utility Classes

- **WavAudioGenerator**: Audio utility for creating temporary WAV files

## Key Features

1. **Single Responsibility Principle**: Each preset type has its own provider class
2. **Extended Preset Information**:
   - Basic description
   - "Best used for" recommendations
   - Default preset capability
   - Filter capability with recommended filter values
3. **Preset Application Options**:
   - Create preset only
   - Set as default preset for type
   - Add preset with custom filter
4. **System Extensibility**:
   - Easy to add new preset providers
   - Plug-and-play architecture with automatic registration

## Adding New Preset Types

To add support for a new preset type:

1. Create a new class that inherits from `BasePresetProvider`
2. Implement the required methods and properties:
   ```csharp
   public override string Category => "Your Category";

   public override void Initialize()
   {
       base.Initialize();

       // Add your presets
       presets.Add(new PresetDefinition(
           "Preset Name",
           "Short description",
           () => CreateYourPreset("PresetId"),
           "Best used for description",
           true,  // Can be default
           true,  // Can use filter
           "*_RecommendedFilter*",  // Recommended filter pattern
           "Filter description"     // Description of what filter does
       ));
   }
   ```
3. Implement preset creation methods specific to your asset type
4. Register your provider in the `PredefinedPresetEditor.RegisterPresetProviders()` method


## Note:
Most of the settings values has been used from [claude.ai](https://claude.ai/)


## :open_hands: Contributions
Any contributions are welcome!

1. Fork it
2. Create your feature branch (git checkout -b my-new-feature)
3. Commit your changes (git commit -am 'Add some feature')
4. Push to the branch (git push origin my-new-feature)
5. Create New Pull Request

<br><br>
