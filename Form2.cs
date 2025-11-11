using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics; // agregado para Stopwatch
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reproductor_Proyecto_P1
{
    public partial class Form2 : Form
    {
        private Random random = new Random();
        private int animationFrame = 0;
        private int currentTheme = 0;
        // spectrum source (target) and working values
        private float[] spectrumBars;
        private float[] targetSpectrum;
        // visual smoothing per-bar for the new AudioMeter effect
        private float[] smoothedBars;
        private float[] peakHold;
        private int[] peakHoldTimer; // ms

        private PointF[] particles;
        private float[] particleVelocities;
        private Color[] themeColors;
        // Added AudioMeter as an extra effect
        private string[] themeNames = { "Espectro", "Ondas", "Partículas", "Psicodélico", "Neón" };
        private bool isFullscreen = false;
        private FormWindowState previousWindowState;
        private FormBorderStyle previousBorderStyle;
        private Size previousSize;
        private Point previousLocation;

        // Variables para cambio automático de temas
        private bool autoChangeThemes = true;
        private const int ThemeDurationSeconds = 10; // duración exacta por tema
        private Stopwatch themeStopwatch; //  conteo por ticks

        // Beat reaction
        private float beatIntensity = 0f; // 0..1
        private const float beatDecayPerTick = 0.08f; // decay per animation tick

        // Track last time live spectrum was updated to avoid overwriting it with random data
        private DateTime lastSpectrumUpdate = DateTime.MinValue;
        private int spectrumHoldMs = 120; // tiempo en ms para considerar datos como recientes (reducido)
        private bool liveReceived = false; // recibimos datos en vivo
        private bool firstLiveHandled = false; // evitar saltos iniciales
        private bool manualSpeedActive = false; // usuario ajustó velocidad manualmente
        private System.Windows.Forms.Timer manualSpeedTimer;

        // Onset reaction state
        private float onsetLowPulse = 0f;
        private float onsetMidPulse = 0f;
        private float onsetHighPulse = 0f;

        // AudioMeter parameters (per-bar independent behavior)
        private const float MeterAttack = 0.6f;   // rapidez de subida (0..1)
        private const float MeterRelease = 0.12f; // rapidez de bajada (0..1)
        private const int PeakHoldMs = 300;       // tiempo que mantiene pico
        // Reduce default max bar height so bars reach about half the panel by default
        private float maxBarHeightFactor = 0.5f;  // escala máxima relativa (0.5 = mitad de la pantalla)

        // reference magnitude for mapping input magnitudes to display range
        private float referenceMagnitude = 60f; // adjust depending on analyzer output

        // region representative indices (updated each spectrum update)
        private int[] regionLowIdx = new int[0];
        private int[] regionMidIdx = new int[0];
        private int[] regionHighIdx = new int[0];

        public Form2()
        {
            InitializeComponent();
            InitializeVisualization();
        }

        private void InitializeVisualization()
        {
            // Inicializar barras de espectro
            spectrumBars = new float[64];
            targetSpectrum = new float[64];
            smoothedBars = new float[64];
            peakHold = new float[64];
            peakHoldTimer = new int[64];
            for (int i = 0; i < spectrumBars.Length; i++)
                spectrumBars[i] = random.Next(10, 100);

            // Inicializar partículas
            particles = new PointF[100];
            particleVelocities = new float[particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new PointF(random.Next(Math.Max(1, panelVisualization.Width)),
                                        random.Next(Math.Max(1, panelVisualization.Height)));
                particleVelocities[i] = (float)(random.NextDouble() * 5 + 1);
            }
            // Inicializar colores del tema actual
            UpdateThemeColors();

            // Timer para detectar actividad manual en control de velocidad
            manualSpeedTimer = new System.Windows.Forms.Timer();
            manualSpeedTimer.Interval = 2000; // 2s de inactividad -> salir modo manual
            manualSpeedTimer.Tick += (s, e) => { manualSpeedActive = false; manualSpeedTimer.Stop(); };
        }

        private void UpdateThemeColors()
        {
            switch (currentTheme)
            {
                case 0: // Espectro
                    themeColors = new Color[] {
                        Color.Red, Color.Orange, Color.Yellow, Color.Green,
                        Color.Blue, Color.Indigo, Color.Violet
                    };
                    break;
                case 1: // Ondas
                    themeColors = new Color[] {
                        Color.DeepSkyBlue, Color.Cyan, Color.Aqua, Color.Teal
                    };
                    break;
                case 2: // Partículas
                    themeColors = new Color[] {
                        Color.Gold, Color.Orange, Color.OrangeRed, Color.Red
                    };
                    break;
                case 3: // Psicodélico
                    themeColors = new Color[] {
                        Color.Magenta, Color.Lime, Color.Cyan, Color.Yellow,
                        Color.HotPink, Color.Purple, Color.SpringGreen
                    };
                    break;
                case 4: // Neón 
                    themeColors = new Color[] {
                        Color.Lime, Color.Cyan, Color.Magenta, Color.Yellow, Color.White
                    };
                    break;
                default:
                    themeColors = new Color[] { Color.White };
                    break;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.DoubleBuffer |
                         ControlStyles.ResizeRedraw, true);
            // Inicializar el checkbox
            chkAutoChange.Checked = autoChangeThemes;
            lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Auto)";
            themeStopwatch = Stopwatch.StartNew();
            timerAnimation.Start();
        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            animationFrame++;

            if (autoChangeThemes)
            {
                if (themeStopwatch == null) themeStopwatch = Stopwatch.StartNew();
                double elapsed = themeStopwatch.Elapsed.TotalSeconds;
                int remaining = Math.Max(0, ThemeDurationSeconds - (int)Math.Floor(elapsed));

                // mostrar cuenta regresiva exacta 10 -> 1
                if (remaining > 0)
                    lblProgress.Text = $"Próximo: {remaining}s";
                else
                    lblProgress.Text = "Cambiando...";

                if (elapsed >= ThemeDurationSeconds)
                {
                    ChangeThemeAuto();
                }
            }
            else
            {
                lblProgress.Text = "Manual";
                if (themeStopwatch != null)
                {
                    themeStopwatch.Stop();
                    themeStopwatch = null;
                }
            }

            // Decay beat intensity
            if (beatIntensity > 0f)
            {
                beatIntensity -= beatDecayPerTick;
                if (beatIntensity < 0f) beatIntensity = 0f;
            }

            // Decay onset pulses
            onsetLowPulse = Math.Max(0f, onsetLowPulse - 0.08f);
            onsetMidPulse = Math.Max(0f, onsetMidPulse - 0.06f);
            onsetHighPulse = Math.Max(0f, onsetHighPulse - 0.05f);

            // Only randomize spectrum if we haven't received live spectrum recently
            bool haveRecentSpectrum = (DateTime.UtcNow - lastSpectrumUpdate).TotalMilliseconds < spectrumHoldMs;
            if (!haveRecentSpectrum)
            {
                if (spectrumBars == null)
                {
                    spectrumBars = new float[64];
                    for (int i = 0; i < spectrumBars.Length; i++) spectrumBars[i] = random.Next(10, 100);
                }

                for (int i = 0; i < spectrumBars.Length; i++)
                {
                    // solo modificar cuando no hay input real
                    float change = (float)(random.NextDouble() - 0.5) * 20;
                    spectrumBars[i] = Math.Max(10, Math.Min(200, spectrumBars[i] + change));
                }
            }

            // Si recibimos espectro en vivo, acercamos spectrumBars a targetSpectrum suavemente
            if (liveReceived && targetSpectrum != null && spectrumBars != null && targetSpectrum.Length == spectrumBars.Length)
            {
                float lerp = manualSpeedActive ? 0.75f : 0.25f; // más rápido si el usuario puso velocidad manual
                for (int i = 0; i < spectrumBars.Length; i++)
                {
                    spectrumBars[i] = Lerp(spectrumBars[i], targetSpectrum[i], lerp);
                }
            }

            for (int i = 0; i < particles.Length; i++)
            {
                float speedMod = 1f + (beatIntensity + onsetLowPulse * 0.6f + onsetMidPulse * 0.4f + onsetHighPulse * 0.2f) * 2f;
                particles[i] = new PointF(
                    particles[i].X + particleVelocities[i] * speedMod * (float)Math.Sin(animationFrame * 0.1 + i),
                    particles[i].Y + particleVelocities[i] * speedMod * (float)Math.Cos(animationFrame * 0.1 + i)
                );
                // Reciclar partículas que salen de los límites
                if (particles[i].X < 0 || particles[i].X > panelVisualization.Width ||
                    particles[i].Y < 0 || particles[i].Y > panelVisualization.Height)
                {
                    particles[i] = new PointF(
                        random.Next(Math.Max(1, panelVisualization.Width)),
                        random.Next(Math.Max(1, panelVisualization.Height))
                    );
                }
            }
            panelVisualization.Invalidate();
        }

        private void ChangeThemeAuto()
        {
            themeStopwatch.Restart();
            currentTheme = (currentTheme + 1) % themeNames.Length;
            lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Auto)";
            UpdateThemeColors();
            // Efecto visual de transición
            FlashTransition();
        }

        private void btnChangeTheme_Click(object sender, EventArgs e)
        {
            // Cambiar tema inmediatamente
            currentTheme = (currentTheme + 1) % themeNames.Length;
            UpdateThemeColors();

            // Si estaba en automático, cambiar temporalmente a manual
            if (autoChangeThemes)
            {
                autoChangeThemes = false;
                chkAutoChange.Checked = false;
                lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Manual)";
                if (themeStopwatch != null) { themeStopwatch.Stop(); themeStopwatch = null; }
                // Programar regreso al modo automático (SIN Task.Delay problemático)
                System.Windows.Forms.Timer tempTimer = new System.Windows.Forms.Timer();
                tempTimer.Interval = 30000; // 30 segundos
                tempTimer.Tick += (s, args) =>
                {
                    if (!this.IsDisposed)
                    {
                        autoChangeThemes = true;
                        chkAutoChange.Checked = true;
                        lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Auto)";
                        themeStopwatch = Stopwatch.StartNew();
                    }
                    tempTimer.Stop();
                    tempTimer.Dispose();
                };
                tempTimer.Start();
            }
            else
            {
                lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Manual)";
            }

            // Efecto visual
            FlashTransition();
        }

        private void FlashTransition()
        {
            // Efecto de transición simple sin Task.Delay problemático
            if (themeColors != null && themeColors.Length > 0)
            {
                panelVisualization.BackColor = themeColors[0];

                // Usar Timer para restaurar el color de fondo
                System.Windows.Forms.Timer flashTimer = new System.Windows.Forms.Timer();
                flashTimer.Interval = 150;
                flashTimer.Tick += (s, args) =>
                {
                    if (!this.IsDisposed)
                    {
                        panelVisualization.BackColor = Color.Black;
                    }
                    flashTimer.Stop();
                    flashTimer.Dispose();
                };
                flashTimer.Start();
            }
        }

        // Public method to be called when a beat is detected
        public void TriggerBeat()
        {
            // Reacciona en todas las formas: aumenta intención del beat
            beatIntensity = Math.Min(1f, beatIntensity + 0.9f);
            // pequeña flash visual
            panelVisualization.BackColor = Color.FromArgb(20, Color.White);
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 80;
            t.Tick += (s, e) =>
            {
                panelVisualization.BackColor = Color.Black;
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }

        // Trigger onset with region type
        public void TriggerOnset(OnsetType type)
        {
            switch (type)
            {
                case OnsetType.Low:
                    onsetLowPulse = 1.0f;
                    // stronger flash in warm color
                    panelVisualization.BackColor = Color.FromArgb(30, Color.OrangeRed);
                    break;
                case OnsetType.Mid:
                    onsetMidPulse = 1.0f;
                    panelVisualization.BackColor = Color.FromArgb(30, Color.DeepSkyBlue);
                    break;
                case OnsetType.High:
                    onsetHighPulse = 1.0f;
                    panelVisualization.BackColor = Color.FromArgb(30, Color.Magenta);
                    break;
            }

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 90;
            t.Tick += (s, e) =>
            {
                if (!this.IsDisposed)
                    panelVisualization.BackColor = Color.Black;
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }

        // Método público para recibir espectro desde Form1
        public void UpdateSpectrum(float[] bands)
        {
            if (bands == null || bands.Length == 0) return;

            // desired output band count (display bars)
            int outputBands = spectrumBars != null && spectrumBars.Length > 0 ? spectrumBars.Length : 64;

            // ensure target/smoothing arrays sized to outputBands
            if (targetSpectrum == null || targetSpectrum.Length != outputBands)
            {
                targetSpectrum = new float[outputBands];
            }
            if (smoothedBars == null || smoothedBars.Length != outputBands)
                smoothedBars = new float[outputBands];
            if (peakHold == null || peakHold.Length != outputBands)
            {
                peakHold = new float[outputBands];
                peakHoldTimer = new int[outputBands];
            }
            if (spectrumBars == null || spectrumBars.Length != outputBands)
                spectrumBars = new float[outputBands];

            int inputBins = bands.Length;

            if (inputBins == outputBands)
            {
                for (int i = 0; i < outputBands; i++)
                {
                    float mapped = 10f + (bands[i] / referenceMagnitude) * 190f;
                    targetSpectrum[i] = Math.Clamp(mapped, 10f, 200f);
                }
            }
            else
            {
                // logarithmic grouping: geometric partition of input bins into output bands
                for (int b = 0; b < outputBands; b++)
                {
                    double startF = Math.Pow((double)inputBins, (double)b / outputBands);
                    double endF = Math.Pow((double)inputBins, (double)(b + 1) / outputBands);
                    int start = (int)Math.Floor(startF);
                    int end = Math.Max(start, (int)Math.Floor(endF) - 1);
                    start = Math.Clamp(start, 0, inputBins - 1);
                    end = Math.Clamp(end, 0, inputBins - 1);

                    float sum = 0f;
                    int count = 0;
                    for (int k = start; k <= end; k++)
                    {
                        sum += bands[k];
                        count++;
                    }
                    float avg = count > 0 ? sum / count : 0f;

                    float mapped = 10f + (avg / referenceMagnitude) * 190f;
                    targetSpectrum[b] = Math.Clamp(mapped, 10f, 200f);
                }
            }

            // compute explicit low/mid/high summaries and assign to representative bars
            if (inputBins >= 3)
            {
                // professional defaults
                int lowBins = Math.Min(6, Math.Max(1, inputBins / 8));
                int highBins = lowBins;
                int midStart = Math.Max(lowBins, inputBins / 3);
                int midEnd = Math.Max(midStart + 1, (2 * inputBins) / 3);

                // averages
                float sumLow = 0f, sumMid = 0f, sumHigh = 0f;
                for (int k = 0; k < lowBins; k++) sumLow += bands[k];
                int midCount = 0;
                for (int k = midStart; k < midEnd; k++) { sumMid += bands[k]; midCount++; }
                for (int k = inputBins - highBins; k < inputBins; k++) sumHigh += bands[k];

                float avgLow = sumLow / Math.Max(1, lowBins);
                float avgMid = midCount > 0 ? sumMid / midCount : avgLow;
                float avgHigh = sumHigh / Math.Max(1, highBins);

                float mappedLow = Math.Clamp(10f + (avgLow / referenceMagnitude) * 190f, 10f, 200f);
                float mappedMid = Math.Clamp(10f + (avgMid / referenceMagnitude) * 190f, 10f, 200f);
                float mappedHigh = Math.Clamp(10f + (avgHigh / referenceMagnitude) * 190f, 10f, 200f);

                // choose representative output indices but remap regions so:
                // center = high frequencies, sides = mid frequencies, extremes = low frequencies
                int outBands = outputBands;

                // determine counts
                int highCount = Math.Min(8, Math.Max(1, outBands / 8)); // center high cluster
                int midSideCount = Math.Min(6, Math.Max(1, outBands / 8)); // each side

                int used = highCount + 2 * midSideCount;
                int remaining = Math.Max(0, outBands - used);
                int leftLow = remaining / 2;
                int rightLow = remaining - leftLow;

                var lowIdx = new List<int>();
                // left extremes
                for (int i = 0; i < leftLow; i++) lowIdx.Add(i);

                // left mid-side
                var midIdx = new List<int>();
                int leftMidStart = leftLow;
                for (int i = 0; i < midSideCount; i++) midIdx.Add(Math.Clamp(leftMidStart + i, 0, outBands - 1));

                // center high
                var highIdx = new List<int>();
                int center = outBands / 2;
                int halfHigh = highCount / 2;
                int highStart = Math.Clamp(center - halfHigh, 0, outBands - 1 - Math.Max(0, highCount - 1));
                for (int i = 0; i < highCount; i++) highIdx.Add(Math.Clamp(highStart + i, 0, outBands - 1));

                // right mid-side
                int rightMidStart = highStart + highCount;
                for (int i = 0; i < midSideCount; i++) midIdx.Add(Math.Clamp(rightMidStart + i, 0, outBands - 1));

                // right extremes
                for (int i = 0; i < rightLow; i++) lowIdx.Add(Math.Clamp(outBands - 1 - i, 0, outBands - 1));

                // ensure uniqueness and sorted for rendering
                lowIdx = lowIdx.Distinct().OrderBy(x => x).ToList();
                midIdx = midIdx.Distinct().OrderBy(x => x).ToList();
                highIdx = highIdx.Distinct().OrderBy(x => x).ToList();

                // store for DrawSpectrum styling
                regionLowIdx = lowIdx.ToArray();
                regionMidIdx = midIdx.ToArray();
                regionHighIdx = highIdx.ToArray();

                // blending to emphasize without overriding grouping
                float blendLow = 0.85f; // emphasize lows across first bars
                float blendMid = 0.9f;
                float blendHigh = 0.9f;

                foreach (var i in lowIdx)
                    targetSpectrum[i] = Lerp(targetSpectrum[i], mappedLow, blendLow);
                foreach (var i in midIdx)
                    targetSpectrum[i] = Lerp(targetSpectrum[i], mappedMid, blendMid);
                foreach (var i in highIdx)
                    targetSpectrum[i] = Lerp(targetSpectrum[i], mappedHigh, blendHigh);
            }

            // on first live update, copy target directly to avoid jump
            if (!firstLiveHandled)
            {
                for (int i = 0; i < targetSpectrum.Length; i++) spectrumBars[i] = targetSpectrum[i];
                firstLiveHandled = true;
            }

            liveReceived = true;
            lastSpectrumUpdate = DateTime.UtcNow;
        }

        private void panelVisualization_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            switch (currentTheme)
            {
                case 0:
                    DrawSpectrum(g);
                    break;
                case 1:
                    DrawWaves(g);
                    break;
                case 2:
                    DrawParticles(g);
                    break;
                case 3:
                    DrawPsychedelic(g);
                    break;
                case 4:
                    DrawNeon(g);
                    break;
            }
        }

        private void DrawSpectrum(Graphics g)
        {
            if (spectrumBars == null || spectrumBars.Length == 0) return;

            int bands = spectrumBars.Length;
            float width = panelVisualization.ClientSize.Width;
            float height = panelVisualization.ClientSize.Height;
            float barW = Math.Max(2f, (width / bands) * 0.85f);
            float gap = Math.Max(1f, (width / bands) * 0.15f);

            // Ensure smoothing arrays exist
            if (smoothedBars == null || smoothedBars.Length != bands) smoothedBars = new float[bands];
            if (peakHold == null || peakHold.Length != bands) { peakHold = new float[bands]; peakHoldTimer = new int[bands]; }

            for (int i = 0; i < bands; i++)
            {
                float target = spectrumBars[i];

                // per-bar attack/release smoothing
                if (target > smoothedBars[i])
                    smoothedBars[i] = Lerp(smoothedBars[i], target, MeterAttack);
                else
                    smoothedBars[i] = Lerp(smoothedBars[i], target, MeterRelease);

                // apply beat emphasis stronger on lower indices (graves)
                float beatBoost = 1f + beatIntensity * (i < Math.Max(1, bands / 8) ? 1.6f : 0.45f);
                float value = smoothedBars[i] * beatBoost;

                // determine region membership
                bool isLow = Array.IndexOf(regionLowIdx, i) >= 0;
                bool isMid = Array.IndexOf(regionMidIdx, i) >= 0;
                bool isHigh = Array.IndexOf(regionHighIdx, i) >= 0;

                // region pulse and extra multiplier so region bars respond more to beats/onsets
                float regionPulse = 0f;
                if (isLow) regionPulse = onsetLowPulse;
                else if (isMid) regionPulse = onsetMidPulse;
                else if (isHigh) regionPulse = onsetHighPulse;

                float extraFromBeat = beatIntensity * (isLow ? 0.9f : isMid ? 0.6f : isHigh ? 0.35f : 0f);
                float extraFromOnset = regionPulse * 1.6f; // makes onsets visually strong
                float extraMultiplier = 1f + extraFromBeat + extraFromOnset;

                float boostedValue = value * extraMultiplier;

                // peak hold handling (track displayed value including beat & region boost)
                if (boostedValue > peakHold[i])
                {
                    peakHold[i] = boostedValue;
                    peakHoldTimer[i] = PeakHoldMs;
                }
                else if (peakHoldTimer[i] > 0)
                {
                    peakHoldTimer[i] -= Math.Max(1, timerAnimation.Interval);
                }
                else
                {
                    peakHold[i] = Lerp(peakHold[i], boostedValue, 0.08f);
                }

                float norm = Math.Clamp(boostedValue / 200f, 0f, 1f);
                // allow region bars to reach full height (override global maxBarHeightFactor)
                float effectiveMaxFactor = (isLow || isMid || isHigh) ? 1.0f : maxBarHeightFactor;
                float bH = Math.Max(2f, norm * height * effectiveMaxFactor);

                float regionHeightFactor = 1.0f;
                Color regionTint = Color.Empty;
                if (isLow) { regionHeightFactor = 1.15f; regionTint = Color.FromArgb(200, Color.LightCoral); }
                else if (isMid) { regionHeightFactor = 1.05f; regionTint = Color.FromArgb(200, Color.LightBlue); }
                else if (isHigh) { regionHeightFactor = 0.95f; regionTint = Color.FromArgb(200, Color.LightGreen); }

                float drawnBH = Math.Max(2f, Math.Min(height, bH * regionHeightFactor));

                float x = i * (barW + gap);
                float y = height - drawnBH;

                var rect = new RectangleF(x, y, barW, drawnBH);

                // base color selection by index
                Color baseColor = themeColors[i % themeColors.Length];
                Color fillStart = Color.FromArgb(220, baseColor);
                Color fillEnd = Color.FromArgb(60, Color.Black);

                // If region tint active, blend tint with base colors
                if (regionTint != Color.Empty)
                {
                    // region pulse based on onset pulses and beatIntensity
                    float regionPulseLocal = regionPulse;
                    regionPulseLocal = Math.Clamp(regionPulseLocal + beatIntensity * 0.5f, 0f, 1f);

                    // glow behind bar
                    int glowAlpha = (int)(Math.Min(120, 60 + regionPulseLocal * 180));
                    using (var glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, regionTint)))
                    {
                        var glowRect = RectangleF.Inflate(rect, 2f, 4f);
                        g.FillRectangle(glowBrush, glowRect);
                    }

                    // blend tint into fill colors, stronger when pulse is high
                    fillStart = InterpolateColor(fillStart, regionTint, 0.3f + 0.7f * regionPulseLocal);
                    fillEnd = InterpolateColor(fillEnd, Color.Black, 0.2f);
                }

                using (var brush = new LinearGradientBrush(rect, fillStart, fillEnd, LinearGradientMode.Vertical))
                using (var path = RoundedRect(rect, Math.Min(barW, 8f)))
                {
                    g.FillPath(brush, path);
                    // Draw a subtle highlight stroke for region bars
                    if (regionTint != Color.Empty)
                    {
                        using (var pen = new Pen(Color.FromArgb(180, Color.White), 1.2f))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(Color.FromArgb(120, Color.Black), 1f)) g.DrawPath(pen, path);
                    }
                }

                // draw peak hold marker aligned to displayed bar (do not exceed bar height)
                float peakHeight = Math.Clamp(peakHold[i] / 200f * height, 0f, height);
                if (peakHeight > drawnBH) peakHeight = drawnBH;
                float peakY = height - peakHeight;
                var peakRect = new RectangleF(x, Math.Max(0, peakY - 3), barW, 3);
                using (var pb = new SolidBrush(Color.FromArgb(220, 255, 255, 255))) g.FillRectangle(pb, peakRect);
            }

            // removed previous overlay drawing; bars themselves are tinted and resized for regions
        }

        // keep RoundedRect helper (already present)
        private static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0f)
            {
                path.AddRectangle(r);
                return path;
            }
            float d = radius * 2f;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawWaves(Graphics g)
        {
            // Ecuaciones harmónicas manuales usando seno y coseno
            for (int wave = 0; wave < themeColors.Length; wave++)
            using (Pen pen = new Pen(themeColors[wave], 2 + wave))
            {
                // Parámetros de la onda
                float amplitude = 40 + wave * 15;  // Amplitud
                // boost amplitude with beat
                amplitude *= (1f + beatIntensity * 0.5f);
                float frequency = 0.01f + wave * 0.005f;  // Frecuencia
                float phase = animationFrame * (0.05f + wave * 0.02f);  // Fase
                float verticalOffset = panelVisualization.Height / 2 + wave * 20;
                
                // Dibujar onda punto por punto conectando líneas
                for (int x = 0; x < panelVisualization.Width - 1; x++)
                {
                    float y1 = verticalOffset + amplitude * (float)Math.Sin(frequency * x + phase);
                    float y2 = verticalOffset + amplitude * (float)Math.Sin(frequency * (x + 1) + phase);
                    
                    // componente adicional
                    y1 += (amplitude * 0.3f) * (float)Math.Cos(frequency * x * 2 + phase * 1.5f);
                    y2 += (amplitude * 0.3f) * (float)Math.Cos(frequency * (x + 1) * 2 + phase * 1.5f);
                    
                    if (spectrumBars != null && spectrumBars.Length > 4)
                    {
                        float low = (spectrumBars[0] + spectrumBars[1] + spectrumBars[2]) / 3f;
                        y1 += (low / 50f) * 20f;
                        y2 += (low / 50f) * 20f;
                    }
                    
                    g.DrawLine(pen, x, y1, x + 1, y2);
                }
            }
        }

        private void DrawParticles(Graphics g)
        {
            // Ecuaciones paramétricas para partículas en movimiento
            for (int i = 0; i < particles.Length; i++)
            {
                Color color = Color.FromArgb(200, themeColors[i % themeColors.Length]);

                float t = animationFrame * 0.1f + i;
                float radius = 3 + 2 * (float)Math.Sin(t);

                float centerX = particles[i].X;
                float centerY = particles[i].Y;

                // If spectrum exists, modulate size
                if (spectrumBars != null && spectrumBars.Length > 0)
                {
                    float idx = (i % spectrumBars.Length);
                    radius += spectrumBars[(int)idx] / 100f;
                }

                // increase radius on beat
                radius *= 1f + beatIntensity * 0.8f;

                using (Pen pen = new Pen(color, 2))
                {
                    int rays = 8;
                    for (int ray = 0; ray < rays; ray++)
                    {
                        float angle = (2 * (float)Math.PI * ray) / rays + t;
                        float x1 = centerX + radius * 0.3f * (float)Math.Cos(angle);
                        float y1 = centerY + radius * 0.3f * (float)Math.Sin(angle);
                        float x2 = centerX + radius * (float)Math.Cos(angle);
                        float y2 = centerY + radius * (float)Math.Sin(angle);

                        g.DrawLine(pen, x1, y1, x2, y2);
                    }

                    int segments = 12;
                    for (int seg = 0; seg < segments; seg++)
                    {
                        float angle1 = (2 * (float)Math.PI * seg) / segments;
                        float angle2 = (2 * (float)Math.PI * (seg + 1)) / segments;

                        float x1 = centerX + radius * (float)Math.Cos(angle1);
                        float y1 = centerY + radius * (float)Math.Sin(angle1);
                        float x2 = centerX + radius * (float)Math.Cos(angle2);
                        float y2 = centerY + radius * (float)Math.Sin(angle2);

                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }
        }

        private void DrawPsychedelic(Graphics g)
        {
            PointF center = new PointF(panelVisualization.Width / 2, panelVisualization.Height / 2);
            
            for (int spiral = 0; spiral < themeColors.Length; spiral++)
            using (Pen pen = new Pen(themeColors[spiral], 2))
            {
                float a = 2 + spiral;  
                float b = 0.5f + spiral * 0.2f;  
                float rotationSpeed = animationFrame * (0.02f + spiral * 0.01f);
                
                for (float theta = 0; theta < 6 * Math.PI; theta += 0.1f)
                {
                    float r1 = a + b * theta;
                    float r2 = a + b * (theta + 0.1f);
                    
                    float x1 = center.X + r1 * (float)Math.Cos(theta + rotationSpeed);
                    float y1 = center.Y + r1 * (float)Math.Sin(theta + rotationSpeed);
                    float x2 = center.X + r2 * (float)Math.Cos(theta + 0.1f + rotationSpeed);
                    float y2 = center.Y + r2 * (float)Math.Sin(theta + 0.1f + rotationSpeed);
                    
                    if (x1 >= 0 && x1 < panelVisualization.Width && y1 >= 0 && y1 < panelVisualization.Height &&
                        x2 >= 0 && x2 < panelVisualization.Width && y2 >= 0 && y2 < panelVisualization.Height)
                    {
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }
            
            for (int i = 0; i < 12; i++)
            using (Pen pen = new Pen(themeColors[i % themeColors.Length], 1))
            {
                float angle = (2 * (float)Math.PI * i) / 12 + animationFrame * 0.03f;
                float x1 = center.X + 50 * (float)Math.Cos(angle);
                float y1 = center.Y + 50 * (float)Math.Sin(angle);
                float x2 = center.X + 150 * (float)Math.Cos(angle);
                float y2 = center.Y + 150 * (float)Math.Sin(angle);
                
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private void DrawNeon(Graphics g)
        {
            for (int i = 0; i < themeColors.Length; i++)
            {
                Color neonColor = themeColors[i];
                float intensity = 0.5f + 0.5f * (float)Math.Sin(animationFrame * 0.1f + i);
                // boost neon glow with beat
                intensity += beatIntensity * 0.8f;
                intensity = Math.Min(1f, intensity);
                Color glowColor = Color.FromArgb((int)(255 * intensity), neonColor);
                
                int y = 80 + i * 70;
                using (Pen pen = new Pen(glowColor, 3))
                {
                    g.DrawLine(pen, 100, y, panelVisualization.Width - 100, y);
                }
                
                for (int glow = 1; glow <= 5; glow++)
                {
                    Color resplandor = Color.FromArgb((int)(50 * intensity / glow), neonColor);
                    using (Pen glowPen = new Pen(resplandor, 1))
                    {
                        g.DrawLine(glowPen, 100, y - glow, panelVisualization.Width - 100, y - glow);
                        g.DrawLine(glowPen, 100, y + glow, panelVisualization.Width - 100, y + glow);
                    }
                }
            }
            
            PointF center = new PointF(panelVisualization.Width / 2, panelVisualization.Height / 2);
            for (int ring = 0; ring < 4; ring++)
            {
                float baseRadius = 60 + ring * 35;
                float pulsation = 10 * (float)Math.Sin(animationFrame * 0.08f + ring);
                float radius = baseRadius + pulsation + beatIntensity * 15f;
                
                Color neonColor = themeColors[ring % themeColors.Length];
                using (Pen pen = new Pen(neonColor, 2))
                {
                    int segments = 36;  
                    for (int seg = 0; seg < segments; seg++)
                    {
                        float angle1 = (2 * (float)Math.PI * seg) / segments;
                        float angle2 = (2 * (float)Math.PI * (seg + 1)) / segments;

                        float x1 = center.X + radius * (float)Math.Cos(angle1);
                        float y1 = center.Y + radius * (float)Math.Sin(angle1);
                        float x2 = center.X + radius * (float)Math.Cos(angle2);
                        float y2 = center.Y + radius * (float)Math.Sin(angle2);

                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            // Cambiar la velocidad de la animación basado en el valor del trackbar
            // Valor más alto = animación más rápida (menor intervalo)
            int newInterval = Math.Max(10, 110 - trackBarSpeed.Value); // Asegurar mínimo de 10ms
            timerAnimation.Interval = newInterval;
            // marcar modo manual para que el espectro siga más rápido
            manualSpeedActive = true;
            manualSpeedTimer.Stop();
            manualSpeedTimer.Start();
            if (autoChangeThemes)
            {
                // reinicia el tiempo para que el segundo siguiente sea exacto
                themeStopwatch?.Restart();
            }
        }

        private void btnFullscreen_Click(object sender, EventArgs e) => ToggleFullscreen();

        private void ToggleFullscreen()
        {
            if (!isFullscreen)
            {
                // Guardar estado actual
                previousWindowState = this.WindowState;
                previousBorderStyle = this.FormBorderStyle;
                previousSize = this.Size;
                previousLocation = this.Location;

                // Cambiar a pantalla completa
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                panelControls.Visible = false;
                btnFullscreen.Text = "Salir Pantalla Completa";
                isFullscreen = true;
            }
            else
            {
                // Restaurar estado anterior
                this.FormBorderStyle = previousBorderStyle;
                this.WindowState = previousWindowState;
                this.Size = previousSize;
                this.Location = previousLocation;
                panelControls.Visible = true;
                btnFullscreen.Text = "Pantalla Completa";
                isFullscreen = false;
            }
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (isFullscreen)
                        ToggleFullscreen();
                    break;
                case Keys.Space:
                    btnChangeTheme_Click(sender, e);
                    break;
                case Keys.F11:
                    ToggleFullscreen();
                    break;
                case Keys.Up:
                    if (trackBarSpeed.Value < trackBarSpeed.Maximum)
                        trackBarSpeed.Value += 5;
                    break;
                case Keys.Down:
                    if (trackBarSpeed.Value > trackBarSpeed.Minimum)
                        trackBarSpeed.Value -= 5;
                    break;
                case Keys.A:
                    // Alternar cambio automático con la tecla 'A'
                    chkAutoChange.Checked = !chkAutoChange.Checked;
                    break;
            }
        }

        private void chkAutoChange_CheckedChanged(object sender, EventArgs e)
        {
            autoChangeThemes = chkAutoChange.Checked;
            if (autoChangeThemes)
            {
                themeStopwatch = Stopwatch.StartNew();
                lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Auto)";
            }
            else
            {
                themeStopwatch?.Stop();
                themeStopwatch = null;
                lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Manual)";
                lblProgress.Text = "Manual";
            }
        }

        // simple lerp helper
        private static float Lerp(float a, float b, float t) => a + (b - a) * t;

        // helper to interpolate between two colors
        private static Color InterpolateColor(Color a, Color b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t)
            );
        }
    }
}