using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace OneDevApp.PredefinedPresetLibrary
{

  /// <summary>
  /// Implementation for Audio presets
  /// </summary>
  public class AudioPresetProvider : BasePresetProvider
  {
    public override string Category => "Audio";

    public override void Initialize()
    {
      base.Initialize();

      presets.Add(new PresetDefinition(
          "Music - High Quality",
          "High quality stereo music with minimal compression",
          (fileName) => CreateAudioPreset(fileName),
          "Background music, orchestral tracks, and high-fidelity audio",
          true,
          true,
          "*_Music",
          "Applies to filenames ending with _Music"
      ));

      presets.Add(new PresetDefinition(
          "Music - Compressed",
          "Compressed stereo music for lower file size",
          (fileName) => CreateAudioPreset(fileName),
          "Background music for mobile games and low-memory platforms",
          true,
          true,
          "*_MusicCompressed",
          "Applies to filenames ending with _MusicCompressed"
      ));

      presets.Add(new PresetDefinition(
          "SFX - Short",
          "Short sound effects with minimal compression",
          (fileName) => CreateAudioPreset(fileName),
          "UI sounds, clicks, impacts, and short game effects",
          true,
          true,
          "*_SFX",
          "Applies to filenames ending with _SFX"
      ));

      presets.Add(new PresetDefinition(
          "SFX - Long",
          "Longer sound effects with more compression",
          (fileName) => CreateAudioPreset(fileName),
          "Vehicle sounds, ambient effects, and longer game sounds",
          true,
          true,
          "*_SFXLong",
          "Applies to filenames ending with _SFXLong"
      ));

      presets.Add(new PresetDefinition(
          "Voice - Dialog",
          "Voice dialog with medium quality",
          (fileName) => CreateAudioPreset(fileName),
          "Character dialog, narration, and voice-overs",
          true,
          true,
          "*_Voice",
          "Applies to filenames ending with _Voice"
      ));

      presets.Add(new PresetDefinition(
          "Ambient",
          "Ambient sound with loop",
          (fileName) => CreateAudioPreset(fileName),
          "Background ambient sounds, atmosphere, and environments",
          true,
          true,
          "*_Ambient",
          "Applies to filenames ending with _Ambient"
      ));
    }

    private void CreateAudioPreset(string presetName)
    {
        // Create a temporary texture
        string tempFolder = "Assets/Editor/Temp";
        string tempPath = $"{tempFolder}/TempAudioForPreset.wav";
        EnsureDirectoryExists(tempPath);

        string tempAudioPath = AssetDatabase.GenerateUniqueAssetPath(tempPath);
        AudioClip silence = AudioClip.Create("TempSilence", 44100, 1, 44100, false);
        WavAudioGenerator.Save(tempAudioPath, silence);
        UnityEngine.Object.DestroyImmediate(silence);
        AssetDatabase.ImportAsset(tempAudioPath);

        // Get the importer
        AudioImporter importer = AssetImporter.GetAtPath(tempAudioPath) as AudioImporter;
        if (importer != null)
        {
            // Apply settings based on preset type
            ConfigureAudioImporter(importer, presetName);

            // Apply changes to temporary asset
            importer.SaveAndReimport();

            // Create preset
            Preset preset = new Preset(importer);


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
            Debug.LogError("Failed to create audio preset. AudioImporter is null.");
        }
    }

    private void ConfigureAudioImporter(AudioImporter importer, string presetName)
    {
      float defaultCompressionQuality = 70f;
      float bestCompressionQuality = 60f;
      float optimizedCompressedQuality = 50f;
      int normalSampleRate = 44100;
      int optimizedSampleRate = 22050;

      switch (presetName)
      {
        case "SFX_Short":
          // UI sounds need to be responsive
          importer.forceToMono = true;
          importer.loadInBackground = false;
          importer.defaultSampleSettings = CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              defaultCompressionQuality,
              normalSampleRate
          );
          importer.SetOverrideSampleSettings("Android", CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("iPhone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("WebGL", CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("Standalone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              defaultCompressionQuality,
              normalSampleRate
          ));
          break;

        case "SFX_Long":
          // Longer sound effects
          importer.forceToMono = true;
          importer.loadInBackground = false;
          importer.defaultSampleSettings = CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              defaultCompressionQuality,
              normalSampleRate
          );
          importer.SetOverrideSampleSettings("Android", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.ADPCM,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("iPhone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.ADPCM,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("WebGL", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.ADPCM,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("Standalone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.DecompressOnLoad,
              AudioCompressionFormat.ADPCM,
              defaultCompressionQuality,
              normalSampleRate
          ));
          break;

        case "Music_HQ":
          // High quality music
          importer.forceToMono = false;
          importer.loadInBackground = true;
          importer.defaultSampleSettings = CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              defaultCompressionQuality,
              normalSampleRate
          );
          importer.SetOverrideSampleSettings("Android", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              bestCompressionQuality,
              normalSampleRate
          ));
          importer.SetOverrideSampleSettings("iPhone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.MP3,
              bestCompressionQuality,
              normalSampleRate
          ));
          importer.SetOverrideSampleSettings("WebGL", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.MP3,
              optimizedCompressedQuality,
              normalSampleRate
          ));
          importer.SetOverrideSampleSettings("Standalone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              defaultCompressionQuality,
              normalSampleRate
          ));
          break;

        case "Music_Compressed":
          // Compressed music for mobile
          importer.forceToMono = false;
          importer.loadInBackground = true;
          importer.defaultSampleSettings = CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              optimizedCompressedQuality,
              optimizedSampleRate
          );
          importer.SetOverrideSampleSettings("Android", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("iPhone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.MP3,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("WebGL", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.MP3,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("Standalone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              optimizedCompressedQuality,
              normalSampleRate
          ));
          break;

        case "Voice_Dialog":
          // Voice dialog
          importer.forceToMono = true;
          importer.loadInBackground = false;
          importer.defaultSampleSettings = CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.Vorbis,
              optimizedCompressedQuality,
              optimizedSampleRate
          );
          importer.SetOverrideSampleSettings("Android", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.Vorbis,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("iPhone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.MP3,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("WebGL", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.MP3,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("Standalone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.CompressedInMemory,
              AudioCompressionFormat.ADPCM,
              defaultCompressionQuality,
              normalSampleRate
          ));
          break;

        case "Ambient":
          // Ambient/looping sounds
          importer.forceToMono = false;
          importer.loadInBackground = true;
          importer.defaultSampleSettings = CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              bestCompressionQuality,
              normalSampleRate
          );
          importer.SetOverrideSampleSettings("Android", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              bestCompressionQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("iPhone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.MP3,
              bestCompressionQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("WebGL", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.MP3,
              optimizedCompressedQuality,
              optimizedSampleRate
          ));
          importer.SetOverrideSampleSettings("Standalone", CreateAudioImporterSampleSettings(
              AudioClipLoadType.Streaming,
              AudioCompressionFormat.Vorbis,
              defaultCompressionQuality,
              normalSampleRate
          ));
          break;
      }
    }

    private AudioImporterSampleSettings CreateAudioImporterSampleSettings(
        AudioClipLoadType loadType,
        AudioCompressionFormat compressionFormat,
        float quality,
        int sampleRate)
    {
      return new AudioImporterSampleSettings()
      {
        loadType = loadType,
        compressionFormat = compressionFormat,
        quality = quality / 100,
        sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate,
        sampleRateOverride = (uint)sampleRate
      };
    }
  }



}

