using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;

namespace Reproductor_Proyecto_P1
{
   
    public enum OnsetType { Low, Mid, High }

 
    internal class Funcionalidades
    {
    }

  
    internal class AudioAnalyzer : IDisposable
    {
        private int fftSize = 2048; 
        private int hopSize; 
        private int bands = 64; 
        private float[] window; 
        private float[] smoothBands; 

        // Muestras cargadas (mono)
        private float[] samplesMono;
        private int sampleRate;
        private int channels;

        private float[] prevMags;
        private float fluxAvg = 0f;
        private float fluxAvgLow = 0f, fluxAvgMid = 0f, fluxAvgHigh = 0f;

        // Detección de beat
        public event Action BeatDetected;
        public event Action<OnsetType> OnsetDetected;

        private float lowEnergyAvg = 0f; 
        private float energyDecay = 0.04f; 
        private float baseBeatMultiplier = 1.6f; 
        private int beatCooldownMs = 200; 
        private DateTime lastBeat = DateTime.MinValue;
        private int lowBandCount = 8; 

        private float fluxDecay = 0.08f; 
        private float fluxMultiplier = 1.8f; 

        private float attackAlpha = 0.75f;
        private float releaseAlpha = 0.12f;

        private DateTime lastOnsetLow = DateTime.MinValue, lastOnsetMid = DateTime.MinValue, lastOnsetHigh = DateTime.MinValue;

        public AudioAnalyzer(int fftSize = 2048, int bands = 64)
        {
            this.fftSize = fftSize;
            this.hopSize = fftSize / 2;
            this.bands = bands;
            window = CreateHannWindow(fftSize);
            smoothBands = new float[bands];

            ApplyPreset(GenrePreset.Pop);
        }

        public enum GenrePreset
        {
            Electronica,
            Pop,
            Rock,
            Acustico,
            HipHop,
            Custom
        }

        public void ApplyPreset(GenrePreset preset)
        {
            switch (preset)
            {
                case GenrePreset.Electronica:
                    energyDecay = 0.06f;     
                    baseBeatMultiplier = 1.45f; 
                    lowBandCount = 6;         
                    beatCooldownMs = 140;
                    fluxDecay = 0.1f;
                    fluxMultiplier = 1.6f;
                    attackAlpha = 0.8f;
                    releaseAlpha = 0.10f;
                    break;
                case GenrePreset.Pop:
                    energyDecay = 0.04f;
                    baseBeatMultiplier = 1.6f;
                    lowBandCount = 8;
                    beatCooldownMs = 200;
                    fluxDecay = 0.08f;
                    fluxMultiplier = 1.8f;
                    attackAlpha = 0.75f;
                    releaseAlpha = 0.12f;
                    break;
                case GenrePreset.Rock:
                    energyDecay = 0.03f;     
                    baseBeatMultiplier = 1.9f; 
                    lowBandCount = 10;
                    beatCooldownMs = 200;
                    fluxDecay = 0.06f;
                    fluxMultiplier = 2.0f;
                    attackAlpha = 0.65f;
                    releaseAlpha = 0.14f;
                    break;
                case GenrePreset.Acustico:
                    energyDecay = 0.02f;   
                    baseBeatMultiplier = 1.35f; 
                    lowBandCount = 5;
                    beatCooldownMs = 220;
                    fluxDecay = 0.06f;
                    fluxMultiplier = 1.5f;
                    attackAlpha = 0.7f;
                    releaseAlpha = 0.16f;
                    break;
                case GenrePreset.HipHop:
                    energyDecay = 0.05f;
                    baseBeatMultiplier = 1.5f;
                    lowBandCount = 7;
                    beatCooldownMs = 150;
                    fluxDecay = 0.09f;
                    fluxMultiplier = 1.7f;
                    attackAlpha = 0.78f;
                    releaseAlpha = 0.11f;
                    break;
                case GenrePreset.Custom:
                default:
             
                    break;
            }
        }

        
        public bool HasSamples => samplesMono != null && samplesMono.Length > 0 && sampleRate > 0;

        
        public bool LoadWav(string wavPath)
        {
            try
            {
                using (var fs = File.OpenRead(wavPath))
                using (var br = new BinaryReader(fs))
                {
                    string riff = new string(br.ReadChars(4));
                    if (riff != "RIFF") return false;
                    br.ReadInt32(); 
                    string wave = new string(br.ReadChars(4));
                    if (wave != "WAVE") return false;

                    int audioFormat = 0;
                    channels = 0;
                    sampleRate = 0;
                    int bitsPerSample = 0;
                    long dataChunkPos = -1;
                    int dataChunkSize = 0;

                    while (fs.Position < fs.Length)
                    {
                        string chunkId = new string(br.ReadChars(4));
                        int chunkSize = br.ReadInt32();
                        if (chunkId == "fmt ")
                        {
                            audioFormat = br.ReadInt16();
                            channels = br.ReadInt16();
                            sampleRate = br.ReadInt32();
                            br.ReadInt32(); 
                            br.ReadInt16();
                            bitsPerSample = br.ReadInt16();
                            if (chunkSize > 16)
                                br.ReadBytes(chunkSize - 16);
                        }
                        else if (chunkId == "data")
                        {
                            dataChunkPos = fs.Position;
                            dataChunkSize = chunkSize;
                            break;
                        }
                        else
                        {
                            br.ReadBytes(chunkSize);
                        }
                    }

                    if (dataChunkPos < 0) return false;
                    if (audioFormat != 1) return false; 
                    if (bitsPerSample != 16) return false;

                    fs.Position = dataChunkPos;

                    int totalSamples = dataChunkSize / (bitsPerSample / 8);
                    int totalFrames = totalSamples / Math.Max(1, channels);

                    samplesMono = new float[totalFrames];

                    for (int i = 0; i < totalFrames; i++)
                    {
                        int accum = 0;
                        for (int c = 0; c < channels; c++)
                        {
                            short s = br.ReadInt16();
                            accum += s;
                        }
                        samplesMono[i] = accum / (float)channels / 32768f;
                    }

                    smoothBands = new float[bands];
                    lowEnergyAvg = 0f;
                    lastBeat = DateTime.MinValue;
                    prevMags = null;
                    fluxAvg = 0f;
                    fluxAvgLow = fluxAvgMid = fluxAvgHigh = 0f;
                    lastOnsetLow = lastOnsetMid = lastOnsetHigh = DateTime.MinValue;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

  
        public float[] GetBandsAtPosition(double posSeconds)
        {
            if (!HasSamples) return null;
            int center = (int)(posSeconds * sampleRate);
            int N = fftSize;
            var frame = new float[N];
            int start = center - N / 2;
            for (int i = 0; i < N; i++)
            {
                int idx = start + i;
                float v = 0;
                if (idx >= 0 && idx < samplesMono.Length) v = samplesMono[idx];
                frame[i] = v * window[i];
            }
            var mags = ComputeMagnitudes(frame);
            int len = mags.Length;
            if (prevMags == null || prevMags.Length != mags.Length) prevMags = new float[mags.Length];

            // Determine bin limits for bands
            int binLowLimit = Math.Max(1, (int)(150 * fftSize / (float)sampleRate));  
            int binMidLimit = Math.Max(binLowLimit+1, (int)(2000 * fftSize / (float)sampleRate));
            if (binMidLimit >= len) binMidLimit = len-1;

            float fluxLow = 0f, fluxMid = 0f, fluxHigh = 0f;
            for (int i = 0; i < len; i++)
            {
                float d = mags[i] - prevMags[i];
                if (d <= 0) continue;
                if (i <= binLowLimit) fluxLow += d;
                else if (i <= binMidLimit) fluxMid += d;
                else fluxHigh += d;
            }

            Array.Copy(mags, prevMags, mags.Length);

            fluxAvgLow = (1 - fluxDecay) * fluxAvgLow + fluxDecay * fluxLow;
            fluxAvgMid = (1 - fluxDecay) * fluxAvgMid + fluxDecay * fluxMid;
            fluxAvgHigh = (1 - fluxDecay) * fluxAvgHigh + fluxDecay * fluxHigh;

            bool onsetLow = false, onsetMid = false, onsetHigh = false;
            var nowTime = DateTime.UtcNow;
            if (fluxAvgLow > 0 && fluxLow > fluxAvgLow * fluxMultiplier && (nowTime - lastOnsetLow).TotalMilliseconds >= beatCooldownMs)
            {
                onsetLow = true; lastOnsetLow = nowTime;
            }
            if (fluxAvgMid > 0 && fluxMid > fluxAvgMid * fluxMultiplier && (nowTime - lastOnsetMid).TotalMilliseconds >= beatCooldownMs)
            {
                onsetMid = true; lastOnsetMid = nowTime;
            }
            if (fluxAvgHigh > 0 && fluxHigh > fluxAvgHigh * fluxMultiplier && (nowTime - lastOnsetHigh).TotalMilliseconds >= beatCooldownMs)
            {
                onsetHigh = true; lastOnsetHigh = nowTime;
            }

            var bandVals = GroupBandsLog(mags, bands);

            float lowEnergy = 0f;
            int count = Math.Min(lowBandCount, bandVals.Length);
            for (int i = 0; i < count; i++) lowEnergy += bandVals[i];
            lowEnergy /= count; 

            if (lowEnergyAvg <= 0) lowEnergyAvg = lowEnergy;

            lowEnergyAvg = (1 - energyDecay) * lowEnergyAvg + energyDecay * lowEnergy;

            float levelFactor = 1.0f;
            levelFactor = 1.0f - Math.Min(0.45f, lowEnergyAvg / 120f);
            float effectiveMultiplier = baseBeatMultiplier * levelFactor;

            bool energyBeat = false;
            if (lowEnergy > lowEnergyAvg * effectiveMultiplier && (nowTime - lastBeat).TotalMilliseconds >= beatCooldownMs)
            {
                energyBeat = true; lastBeat = nowTime;
            }

            if (onsetLow)
            {
                try { OnsetDetected?.Invoke(OnsetType.Low); } catch { }
                try { BeatDetected?.Invoke(); } catch { }
            }
            if (onsetMid)
            {
                try { OnsetDetected?.Invoke(OnsetType.Mid); } catch { }
            }
            if (onsetHigh)
            {
                try { OnsetDetected?.Invoke(OnsetType.High); } catch { }
            }
            if (energyBeat)
            {
                try { BeatDetected?.Invoke(); } catch { }
            }

            for (int i = 0; i < bands; i++)
            {
                float newVal = bandVals[i];
                if (newVal > smoothBands[i])
                    smoothBands[i] = attackAlpha * newVal + (1 - attackAlpha) * smoothBands[i];
                else
                    smoothBands[i] = releaseAlpha * newVal + (1 - releaseAlpha) * smoothBands[i];
            }
            return (float[])smoothBands.Clone();
        }

        private float[] CreateHannWindow(int N)
        {
            var w = new float[N];
            for (int n = 0; n < N; n++)
                w[n] = 0.5f * (1f - (float)Math.Cos(2 * Math.PI * n / (N - 1)));
            return w;
        }

        private float[] ComputeMagnitudes(float[] samples)
        {
            int N = fftSize;
            var buffer = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                float v = samples[i];
                buffer[i] = new Complex(v, 0);
            }
            FFT(buffer, false);
            int half = N / 2;
            var mags = new float[half];
            for (int i = 0; i < half; i++)
            {
                mags[i] = (float)buffer[i].Magnitude;
            }
            return mags;
        }

        private float[] GroupBandsLog(float[] mags, int bands)
        {
            int len = mags.Length; 
            var result = new float[bands];
            double fmin = 20.0; 
            double fmax = sampleRate / 2.0;
            if (fmin <= 0) fmin = 20.0;

            for (int b = 0; b < bands; b++)
            {
                double lowFrac = (double)b / bands;
                double highFrac = (double)(b + 1) / bands;
                double fLow = fmin * Math.Pow(fmax / fmin, lowFrac);
                double fHigh = fmin * Math.Pow(fmax / fmin, highFrac);

                int startBin = Math.Max(0, (int)Math.Floor(fLow * fftSize / sampleRate));
                int endBin = Math.Min(len, (int)Math.Ceiling(fHigh * fftSize / sampleRate));
                if (endBin <= startBin) endBin = Math.Min(len, startBin + 1);

                float sum = 0f;
                for (int i = startBin; i < endBin; i++) sum += mags[i];
                float avg = sum / (endBin - startBin);
                result[b] = (float)(20 * Math.Log10(avg + 1e-6) + 60);
                if (result[b] < 0) result[b] = 0;
            }
            return result;
        }

        private void FFT(Complex[] buffer, bool inverse)
        {
            int n = buffer.Length;
            int bits = (int)Math.Log(n, 2);
            for (int j = 1, i = 0; j < n; j++)
            {
                int bit = n >> 1;
                for (; (i & bit) != 0; bit >>= 1) i ^= bit;
                i ^= bit;
                if (j < i)
                {
                    var tmp = buffer[j]; buffer[j] = buffer[i]; buffer[i] = tmp;
                }
            }
            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = 2 * Math.PI / len * (inverse ? 1 : -1);
                Complex wlen = new Complex(Math.Cos(ang), Math.Sin(ang));
                for (int i = 0; i < n; i += len)
                {
                    Complex w = Complex.One;
                    for (int j = 0; j < len / 2; j++)
                    {
                        Complex u = buffer[i + j];
                        Complex v = buffer[i + j + len / 2] * w;
                        buffer[i + j] = u + v;
                        buffer[i + j + len / 2] = u - v;
                        w *= wlen;
                    }
                }
            }
            if (inverse)
            {
                for (int i = 0; i < n; i++) buffer[i] /= n;
            }
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }
        public float EnergyDecay { get => energyDecay; set => energyDecay = ClampF(value, 0.001f, 0.5f); }
        public float BaseBeatMultiplier { get => baseBeatMultiplier; set => baseBeatMultiplier = Math.Max(1.05f, value); }
        public int LowBandCount { get => lowBandCount; set => lowBandCount = Math.Max(1, value); }
        public int BeatCooldownMs { get => beatCooldownMs; set => beatCooldownMs = Math.Max(20, value); }
        public float FluxDecay { get => fluxDecay; set => fluxDecay = ClampF(value, 0.001f, 0.5f); }
        public float FluxMultiplier { get => fluxMultiplier; set => fluxMultiplier = Math.Max(1.05f, value); }
        public float AttackAlpha { get => attackAlpha; set => attackAlpha = ClampF(value, 0.01f, 0.99f); }
        public float ReleaseAlpha { get => releaseAlpha; set => releaseAlpha = ClampF(value, 0.01f, 0.99f); }

        private float ClampF(float v, float lo, float hi) => Math.Min(Math.Max(v, lo), hi);
    }
}
