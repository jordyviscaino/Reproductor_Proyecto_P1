using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;

namespace Reproductor_Proyecto_P1
{
    // Enum público para tipos de onset (grave/medio/agudo)
    public enum OnsetType { Low, Mid, High }

    // Clase vacía de soporte (no usada) - se deja para extensiones
    internal class Funcionalidades
    {
    }

    // AudioAnalyzer: carga WAV PCM 16-bit a memoria y calcula espectro usando FFT (sin librerías externas)
    // Incluye detección de golpes (beat) basada en la energía de las bandas graves y spectral flux.
    internal class AudioAnalyzer : IDisposable
    {
        private int fftSize = 2048; // potencia de 2 (tamaño de la ventana)
        private int hopSize; // solapamiento (no usado en la versión actual)
        private int bands = 64; // número de bandas para la visualización
        private float[] window; // ventana Hann
        private float[] smoothBands; // bandas suavizadas para la UI

        // Muestras cargadas (mono)
        private float[] samplesMono;
        private int sampleRate;
        private int channels;

        // estado para spectral flux
        private float[] prevMags;
        private float fluxAvg = 0f;
        private float fluxAvgLow = 0f, fluxAvgMid = 0f, fluxAvgHigh = 0f;

        // Detección de beat
        public event Action BeatDetected; // backward compatibility
        public event Action<OnsetType> OnsetDetected; // more specific

        private float lowEnergyAvg = 0f; // energía promedio de fondo (bandas bajas)
        private float energyDecay = 0.04f; // velocidad de adaptación de la media (más alto = más rápido)
        private float baseBeatMultiplier = 1.6f; // multiplicador base para umbral (energía)
        private int beatCooldownMs = 200; // tiempo mínimo entre beats en ms
        private DateTime lastBeat = DateTime.MinValue;
        private int lowBandCount = 8; // cuántas bandas bajas se promedian para detectar golpe

        // spectral flux params
        private float fluxDecay = 0.08f; // promedio móvil para flux
        private float fluxMultiplier = 1.8f; // umbral multiplicador para flux

        // smoothing attack/release para bandas
        private float attackAlpha = 0.75f;
        private float releaseAlpha = 0.12f;

        // region-specific cooldowns
        private DateTime lastOnsetLow = DateTime.MinValue, lastOnsetMid = DateTime.MinValue, lastOnsetHigh = DateTime.MinValue;

        public AudioAnalyzer(int fftSize = 2048, int bands = 64)
        {
            this.fftSize = fftSize;
            this.hopSize = fftSize / 2;
            this.bands = bands;
            window = CreateHannWindow(fftSize);
            smoothBands = new float[bands];

            // aplicar preset por defecto equilibrado
            ApplyPreset(GenrePreset.Pop);
        }

        // Presets de género para ajustar la detección automáticamente
        public enum GenrePreset
        {
            Electronica,
            Pop,
            Rock,
            Acustico,
            HipHop,
            Custom
        }

        // Aplica un preset ajustando parámetros de detección para diferentes géneros
        public void ApplyPreset(GenrePreset preset)
        {
            switch (preset)
            {
                case GenrePreset.Electronica:
                    energyDecay = 0.06f;      // se adapta rápido
                    baseBeatMultiplier = 1.45f; // sensitivo
                    lowBandCount = 6;         // centrado en sub/bass
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
                    energyDecay = 0.03f;     // más estable (más dinámica)
                    baseBeatMultiplier = 1.9f; // menos sensible a ruidos
                    lowBandCount = 10;
                    beatCooldownMs = 200;
                    fluxDecay = 0.06f;
                    fluxMultiplier = 2.0f;
                    attackAlpha = 0.65f;
                    releaseAlpha = 0.14f;
                    break;
                case GenrePreset.Acustico:
                    energyDecay = 0.02f;     // muy estable
                    baseBeatMultiplier = 1.35f; // más sensible a golpes suaves
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
                    // Mantiene valores actuales
                    break;
            }
        }

        // Indica si hay muestras cargadas
        public bool HasSamples => samplesMono != null && samplesMono.Length > 0 && sampleRate > 0;

        // Carga entero WAV PCM 16-bit a memoria y lo pasa a mono (promedio de canales)
        public bool LoadWav(string wavPath)
        {
            try
            {
                using (var fs = File.OpenRead(wavPath))
                using (var br = new BinaryReader(fs))
                {
                    string riff = new string(br.ReadChars(4));
                    if (riff != "RIFF") return false;
                    br.ReadInt32(); // tamaño
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
                            br.ReadInt32(); // byteRate
                            br.ReadInt16(); // blockAlign
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
                    if (audioFormat != 1) return false; // sólo PCM
                    if (bitsPerSample != 16) return false; // sólo 16-bit soportado

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

                    // reiniciar estado de visualización y detección
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

        // Calcula las bandas para una posición de reproducción dada (en segundos)
        // Devuelve un arreglo de 'bands' valores listos para la visualización.
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
            var mags = ComputeMagnitudes(frame); // length = N/2

            // calcular spectral flux en bandas bajas/medias/altas
            int len = mags.Length;
            if (prevMags == null || prevMags.Length != mags.Length) prevMags = new float[mags.Length];

            // Determine bin limits for bands
            int binLowLimit = Math.Max(1, (int)(150 * fftSize / (float)sampleRate));   // <150Hz
            int binMidLimit = Math.Max(binLowLimit+1, (int)(2000 * fftSize / (float)sampleRate)); // <2000Hz
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

            // actualizar medias móviles de flux por región
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

            // Agrupar magnitudes en bandas logarítmicas
            var bandVals = GroupBandsLog(mags, bands);

            // Detección de beat basada en la energía de las bandas bajas (complementaria)
            float lowEnergy = 0f;
            int count = Math.Min(lowBandCount, bandVals.Length);
            for (int i = 0; i < count; i++) lowEnergy += bandVals[i];
            lowEnergy /= count; // energía media en bandas graves

            // inicializar la media si es la primera vez
            if (lowEnergyAvg <= 0) lowEnergyAvg = lowEnergy;

            // media móvil exponencial para energía de fondo
            lowEnergyAvg = (1 - energyDecay) * lowEnergyAvg + energyDecay * lowEnergy;

            // multiplicador adaptativo: hacer más sensible si la energía es baja, menos si muy alta
            float levelFactor = 1.0f;
            levelFactor = 1.0f - Math.Min(0.45f, lowEnergyAvg / 120f);
            float effectiveMultiplier = baseBeatMultiplier * levelFactor;

            bool energyBeat = false;
            if (lowEnergy > lowEnergyAvg * effectiveMultiplier && (nowTime - lastBeat).TotalMilliseconds >= beatCooldownMs)
            {
                energyBeat = true; lastBeat = nowTime;
            }

            // disparar eventos por región
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

            // suavizado asimétrico (attack/release) para visualización
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

        // Crea ventana Hann de tamaño N
        private float[] CreateHannWindow(int N)
        {
            var w = new float[N];
            for (int n = 0; n < N; n++)
                w[n] = 0.5f * (1f - (float)Math.Cos(2 * Math.PI * n / (N - 1)));
            return w;
        }

        // Calcula magnitudes a partir de muestras aplicando FFT
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

        // Agrupa bins en bandas logarítmicas
        private float[] GroupBandsLog(float[] mags, int bands)
        {
            int len = mags.Length; // bins from 0..N/2-1
            var result = new float[bands];
            double fmin = 20.0; // Hz
            double fmax = sampleRate / 2.0; // Nyquist
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

        // Implementación iterativa in-place de Cooley-Tukey (radix-2)
        private void FFT(Complex[] buffer, bool inverse)
        {
            int n = buffer.Length;
            int bits = (int)Math.Log(n, 2);
            // reordenamiento por bits
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

        // Métodos vacíos por compatibilidad
        public void Stop()
        {
            // nada que detener en este modo
        }

        public void Dispose()
        {
            // nada especial
        }

        // Exponer algunos parámetros para afinación desde UI si se desea
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
