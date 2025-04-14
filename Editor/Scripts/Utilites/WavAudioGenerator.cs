
using System;
using System.IO;
using UnityEngine;

namespace OneDevApp.PredefinedPresetLibrary
{

  /// <summary>
  /// Utility class for saving AudioClip to WAV
  /// </summary>
  public static class WavAudioGenerator
  {
      const int HEADER_SIZE = 44;

      public static bool Save(string filepath, AudioClip clip)
      {
          if (!filepath.ToLower().EndsWith(".wav"))
          {
              filepath = filepath + ".wav";
          }

          // Make sure directory exists
          Directory.CreateDirectory(Path.GetDirectoryName(filepath));

          using (var fileStream = CreateEmpty(filepath))
          {
              ConvertAndWrite(fileStream, clip);
              WriteHeader(fileStream, clip);
          }

          return true;
      }

      static FileStream CreateEmpty(string filepath)
      {
          var fileStream = new FileStream(filepath, FileMode.Create);
          byte emptyByte = new byte();

          for (int i = 0; i < HEADER_SIZE; i++)
          {
              fileStream.WriteByte(emptyByte);
          }

          return fileStream;
      }

      static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
      {
          var samples = new float[clip.samples * clip.channels];
          clip.GetData(samples, 0);

          // Convert to Int16 and write to file
          Int16[] intData = new Int16[samples.Length];
          for (int i = 0; i < samples.Length; i++)
          {
              intData[i] = (short)(samples[i] * 32767);
          }

          byte[] byteArray = new byte[intData.Length * 2];
          for (int i = 0; i < intData.Length; i++)
          {
              short sample = intData[i];
              byteArray[i * 2] = (byte)(sample & 0xff);
              byteArray[i * 2 + 1] = (byte)((sample >> 8) & 0xff);
          }

          fileStream.Write(byteArray, 0, byteArray.Length);
      }

      static void WriteHeader(FileStream fileStream, AudioClip clip)
      {
          var hz = clip.frequency;
          var channels = clip.channels;
          var samples = clip.samples;

          fileStream.Seek(0, SeekOrigin.Begin);

          // RIFF header
          byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
          fileStream.Write(riff, 0, 4);

          // Chunk size
          int chunkSize = (36 + (samples * channels * 2));
          fileStream.Write(BitConverter.GetBytes(chunkSize), 0, 4);

          // Format
          byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
          fileStream.Write(wave, 0, 4);

          // Sub-chunk 1 ID
          byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
          fileStream.Write(fmt, 0, 4);

          // Sub-chunk 1 size
          fileStream.Write(BitConverter.GetBytes(16), 0, 4);

          // Audio format (PCM)
          fileStream.Write(BitConverter.GetBytes((ushort)1), 0, 2);

          // Number of channels
          fileStream.Write(BitConverter.GetBytes((ushort)channels), 0, 2);

          // Sample rate
          fileStream.Write(BitConverter.GetBytes(hz), 0, 4);

          // Byte rate
          fileStream.Write(BitConverter.GetBytes(hz * channels * 2), 0, 4);

          // Block align
          fileStream.Write(BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);

          // Bits per sample
          fileStream.Write(BitConverter.GetBytes((ushort)16), 0, 2);

          // Sub-chunk 2 ID
          byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
          fileStream.Write(datastring, 0, 4);

          // Sub-chunk 2 size
          fileStream.Write(BitConverter.GetBytes(samples * channels * 2), 0, 4);
      }
  }

}