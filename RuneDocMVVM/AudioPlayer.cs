namespace RuneDocMVVM;

using System;
using NAudio.Wave;

public class AudioPlayer : IDisposable
{
    private IWavePlayer waveOutDevice;
    private AudioFileReader audioFileReader;

    public void Play(string filePath)
    {
        waveOutDevice = new WaveOutEvent();
        audioFileReader = new AudioFileReader(filePath);
        waveOutDevice.Init(audioFileReader);
        waveOutDevice.Play();
    }

    public void Stop()
    {
        waveOutDevice?.Stop();
    }

    public void Dispose()
    {
        waveOutDevice?.Dispose();
        audioFileReader?.Dispose();
    }
}