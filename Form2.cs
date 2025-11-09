using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics; // agregado para Stopwatch

namespace Reproductor_Proyecto_P1
{
    public partial class Form2 : Form
    {
        private Random random = new Random();
        private int animationFrame = 0;
        private int currentTheme = 0;
        private float[] spectrumBars;
        private PointF[] particles;
        private float[] particleVelocities;
        private Color[] themeColors;
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

        public Form2()
        {
            InitializeComponent();
            InitializeVisualization();
        }

        private void InitializeVisualization()
        {
            // Inicializar barras de espectro
            spectrumBars = new float[64];
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

            // Actualizar datos visuales
            for (int i = 0; i < spectrumBars.Length; i++)
            {
                float change = (float)(random.NextDouble() - 0.5) * 20;
                spectrumBars[i] = Math.Max(10, Math.Min(200, spectrumBars[i] + change));
            }

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new PointF(
                    particles[i].X + particleVelocities[i] * (float)Math.Sin(animationFrame * 0.1 + i),
                    particles[i].Y + particleVelocities[i] * (float)Math.Cos(animationFrame * 0.1 + i)
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
            int barWidth = panelVisualization.Width / spectrumBars.Length;
            
            for (int i = 0; i < spectrumBars.Length; i++)
            {
                int barHeight = (int)spectrumBars[i];
                Color color = themeColors[i % themeColors.Length];
                
                using (Pen pen = new Pen(color, 2))
                {
                    // Dibujar barras del espectro línea por línea desde abajo hacia arriba
                    int x = i * barWidth;
                    int startY = panelVisualization.Height;
                    int endY = panelVisualization.Height - barHeight;
                    
                    // Dibujar múltiples líneas verticales para crear grosor
                    for (int thickness = 0; thickness < barWidth - 2; thickness++)
                    {
                        g.DrawLine(pen, x + thickness, startY, x + thickness, endY);
                    }
                    
                    // Agregar efecto de degradado dibujando líneas con transparencia variable
                    for (int y = endY; y < startY; y += 2)
                    {
                        float alpha = (float)(startY - y) / barHeight;
                        Color gradientColor = Color.FromArgb((int)(255 * alpha), color);
                        using (Pen gradientPen = new Pen(gradientColor, 1))
                        {
                            g.DrawLine(gradientPen, x, y, x + barWidth - 2, y);
                        }
                    }
                }
            }
        }

        private void DrawWaves(Graphics g)
        {
            // Ecuaciones harmónicas manuales usando seno y coseno
            for (int wave = 0; wave < themeColors.Length; wave++)
            {
                using (Pen pen = new Pen(themeColors[wave], 2 + wave))
                {
                    // Parámetros de la onda
                    float amplitude = 40 + wave * 15;  // Amplitud
                    float frequency = 0.01f + wave * 0.005f;  // Frecuencia
                    float phase = animationFrame * (0.05f + wave * 0.02f);  // Fase
                    float verticalOffset = panelVisualization.Height / 2 + wave * 20;
                    
                    // Dibujar onda punto por punto conectando líneas
                    for (int x = 0; x < panelVisualization.Width - 1; x++)
                    {
                        // Ecuación harmónica: y = A * sin(fx + φ) + offset
                        float y1 = verticalOffset + amplitude * (float)Math.Sin(frequency * x + phase);
                        float y2 = verticalOffset + amplitude * (float)Math.Sin(frequency * (x + 1) + phase);
						
						// Agregar componente adicional para mayor complejidad
						y1 += (amplitude * 0.3f) * (float)Math.Cos(frequency * x * 2 + phase * 1.5f);
                        y2 += (amplitude * 0.3f) * (float)Math.Cos(frequency * (x + 1) * 2 + phase * 1.5f);
						
                        g.DrawLine(pen, x, y1, x + 1, y2);
                    }
                }
            }
        }

        private void DrawParticles(Graphics g)
        {
            // Ecuaciones paramétricas para partículas en movimiento
            for (int i = 0; i < particles.Length; i++)
            {
                Color color = Color.FromArgb(200, themeColors[i % themeColors.Length]);
                
                // Ecuaciones paramétricas para movimiento circular/espiral
                float t = animationFrame * 0.1f + i;
                float radius = 3 + 2 * (float)Math.Sin(t);
                
                // Calcular posición con ecuaciones paramétricas
                // x = r*cos(t), y = r*sin(t) para movimiento circular
                float centerX = particles[i].X;
                float centerY = particles[i].Y;
                
                using (Pen pen = new Pen(color, 2))
                {
                    // Dibujar partícula como estrella usando líneas radiales
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
                    
                    // Dibujar círculo usando líneas (aproximación poligonal)
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
            // Espirales matemáticas usando ecuaciones polares
            PointF center = new PointF(panelVisualization.Width / 2, panelVisualization.Height / 2);
            
            // Dibujar múltiples espirales con diferentes parámetros
            for (int spiral = 0; spiral < themeColors.Length; spiral++)
            {
                using (Pen pen = new Pen(themeColors[spiral], 2))
                {
                    // Parámetros de la espiral
                    float a = 2 + spiral;  // Factor de expansión
                    float b = 0.5f + spiral * 0.2f;  // Factor de separación
                    float rotationSpeed = animationFrame * (0.02f + spiral * 0.01f);
                    
                    // Dibujar espiral usando ecuaciones polares: r = a + b*θ
                    for (float theta = 0; theta < 6 * Math.PI; theta += 0.1f)
                    {
                        // Ecuación de espiral de Arquímedes: r = a + b*θ
                        float r1 = a + b * theta;
                        float r2 = a + b * (theta + 0.1f);
                        
                        // Convertir coordenadas polares a cartesianas
                        float x1 = center.X + r1 * (float)Math.Cos(theta + rotationSpeed);
                        float y1 = center.Y + r1 * (float)Math.Sin(theta + rotationSpeed);
                        float x2 = center.X + r2 * (float)Math.Cos(theta + 0.1f + rotationSpeed);
                        float y2 = center.Y + r2 * (float)Math.Sin(theta + 0.1f + rotationSpeed);
                        
                        // Verificar que los puntos estén dentro de la pantalla
                        if (x1 >= 0 && x1 < panelVisualization.Width && y1 >= 0 && y1 < panelVisualization.Height &&
                            x2 >= 0 && x2 < panelVisualization.Width && y2 >= 0 && y2 < panelVisualization.Height)
                        {
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }
                    }
                }
            }
            
            // Agregar líneas radiales rotativas
            for (int i = 0; i < 12; i++)
            {
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
        }

        private void DrawNeon(Graphics g)
        {
            // Efectos de neón usando líneas con resplandor matemático
            for (int i = 0; i < themeColors.Length; i++)
            {
                Color neonColor = themeColors[i];
                
                // Calcular intensidad parpadeante usando función seno
                float intensity = 0.5f + 0.5f * (float)Math.Sin(animationFrame * 0.1f + i);
                Color glowColor = Color.FromArgb((int)(255 * intensity), neonColor);
                
                // Dibujar líneas horizontales de neón
                int y = 80 + i * 70;
                using (Pen pen = new Pen(glowColor, 3))
                {
                    g.DrawLine(pen, 100, y, panelVisualization.Width - 100, y);
                }
                
                // Efecto de resplandor dibujando líneas paralelas con transparencia
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
            
            // Círculos de neón pulsantes dibujados con líneas
            PointF center = new PointF(panelVisualization.Width / 2, panelVisualization.Height / 2);
            for (int ring = 0; ring < 4; ring++)
            {
                float baseRadius = 60 + ring * 35;
                float pulsation = 10 * (float)Math.Sin(animationFrame * 0.08f + ring);
                float radius = baseRadius + pulsation;
                
                Color neonColor = themeColors[ring % themeColors.Length];
                using (Pen pen = new Pen(neonColor, 2))
                {
                    // Dibujar círculo usando líneas (aproximación poligonal de alta resolución)
                    int segments = 36;  // Más segmentos para círculo más suave
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
    }
}

