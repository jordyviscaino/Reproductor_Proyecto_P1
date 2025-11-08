using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private float[] spectrumBars;
        private PointF[] particles;
        private float[] particleVelocities;
        private Color[] themeColors;
        private string[] themeNames = { "Espectro", "Ondas", "Partículas", "Psicodélico", "Fuego", "Neón" };
        private bool isFullscreen = false;
        private FormWindowState previousWindowState;
        private FormBorderStyle previousBorderStyle;
        private Size previousSize;
        private Point previousLocation;
        
        // Variables para cambio automático de temas
        private bool autoChangeThemes = true;
        private int themeChangeCounter = 0;
        private int themeChangeInterval = 200; // 200 ticks * 50ms = 10 segundos

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
            {
                spectrumBars[i] = random.Next(10, 100);
            }

            // Inicializar partículas
            particles = new PointF[100];
            particleVelocities = new float[particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new PointF(random.Next(panelVisualization.Width), 
                                        random.Next(panelVisualization.Height));
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
                case 4: // Fuego
                    themeColors = new Color[] { 
                        Color.DarkRed, Color.Red, Color.Orange, Color.Yellow, Color.White 
                    };
                    break;
                case 5: // Neón
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
            
            timerAnimation.Start();
        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            animationFrame++;
            
            // Sistema de cambio automático de temas
            if (autoChangeThemes)
            {
                themeChangeCounter++;
                
                // Actualizar indicador de progreso
                int secondsRemaining = (themeChangeInterval - themeChangeCounter) / 20; // Aproximado
                if (secondsRemaining >= 0)
                {
                    lblProgress.Text = $"Próximo tema: {secondsRemaining}s";
                }
                
                if (themeChangeCounter >= themeChangeInterval)
                {
                    themeChangeCounter = 0;
                    currentTheme = (currentTheme + 1) % themeNames.Length;
                    lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Auto)";
                    UpdateThemeColors();
                    
                    // Efecto visual de transición
                    panelVisualization.BackColor = themeColors[0];
                    System.Threading.Tasks.Task.Delay(100).ContinueWith(t => 
                    {
                        if (!this.IsDisposed)
                        {
                            this.Invoke(new Action(() => 
                            {
                                panelVisualization.BackColor = Color.Black;
                            }));
                        }
                    });
                }
            }
            else
            {
                lblProgress.Text = "Manual";
            }
            
            // Actualizar barras de espectro con movimiento aleatorio
            for (int i = 0; i < spectrumBars.Length; i++)
            {
                float change = (float)(random.NextDouble() - 0.5) * 20;
                spectrumBars[i] = Math.Max(10, Math.Min(200, spectrumBars[i] + change));
            }

            // Actualizar partículas
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
                        random.Next(panelVisualization.Width),
                        random.Next(panelVisualization.Height)
                    );
                }
            }

            panelVisualization.Invalidate();
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
                    DrawFire(g);
                    break;
                case 5:
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
                
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, 
                        i * barWidth, 
                        panelVisualization.Height - barHeight, 
                        barWidth - 2, 
                        barHeight);
                }
            }
        }

        private void DrawWaves(Graphics g)
        {
            using (Pen pen = new Pen(themeColors[0], 3))
            {
                List<PointF> wavePoints = new List<PointF>();
                for (int x = 0; x < panelVisualization.Width; x += 5)
                {
                    float y = panelVisualization.Height / 2 + 
                             (float)(Math.Sin((x + animationFrame * 5) * 0.02) * 50 +
                             Math.Sin((x + animationFrame * 3) * 0.01) * 30);
                    wavePoints.Add(new PointF(x, y));
                }
                
                if (wavePoints.Count > 1)
                {
                    g.DrawCurve(pen, wavePoints.ToArray());
                }
            }

            // Múltiples ondas con diferentes colores
            for (int wave = 1; wave < themeColors.Length; wave++)
            {
                using (Pen pen = new Pen(themeColors[wave], 2))
                {
                    List<PointF> wavePoints = new List<PointF>();
                    for (int x = 0; x < panelVisualization.Width; x += 5)
                    {
                        float y = panelVisualization.Height / 2 + 
                                 (float)(Math.Sin((x + animationFrame * (3 + wave)) * 0.015) * (30 + wave * 10));
                        wavePoints.Add(new PointF(x, y));
                    }
                    
                    if (wavePoints.Count > 1)
                    {
                        g.DrawCurve(pen, wavePoints.ToArray());
                    }
                }
            }
        }

        private void DrawParticles(Graphics g)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Color color = Color.FromArgb(
                    200,
                    themeColors[i % themeColors.Length].R,
                    themeColors[i % themeColors.Length].G,
                    themeColors[i % themeColors.Length].B
                );
                
                using (Brush brush = new SolidBrush(color))
                {
                    float size = 3 + (float)Math.Sin(animationFrame * 0.1 + i) * 2;
                    g.FillEllipse(brush, 
                        particles[i].X - size/2, 
                        particles[i].Y - size/2, 
                        size, size);
                }
            }
        }

        private void DrawPsychedelic(Graphics g)
        {
            // Círculos concéntricos rotativos
            PointF center = new PointF(panelVisualization.Width / 2, panelVisualization.Height / 2);
            
            for (int ring = 0; ring < 8; ring++)
            {
                float radius = 50 + ring * 30;
                Color color = Color.FromArgb(100, themeColors[ring % themeColors.Length]);
                
                float angle = (animationFrame * (2 + ring)) * 0.05f;
                float x = center.X + (float)(Math.Cos(angle) * radius);
                float y = center.Y + (float)(Math.Sin(angle) * radius);
                
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, x - 20, y - 20, 40, 40);
                }
            }

            // Líneas rotativas
            using (Pen pen = new Pen(themeColors[animationFrame % themeColors.Length], 2))
            {
                for (int i = 0; i < 12; i++)
                {
                    float angle = (animationFrame + i * 30) * 0.03f;
                    float x1 = center.X + (float)(Math.Cos(angle) * 100);
                    float y1 = center.Y + (float)(Math.Sin(angle) * 100);
                    float x2 = center.X + (float)(Math.Cos(angle) * 200);
                    float y2 = center.Y + (float)(Math.Sin(angle) * 200);
                    
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        private void DrawFire(Graphics g)
        {
            // Simulación de llamas con partículas que suben
            for (int i = 0; i < 50; i++)
            {
                float x = (float)(panelVisualization.Width * 0.2 + random.NextDouble() * panelVisualization.Width * 0.6);
                float y = panelVisualization.Height - (float)(random.NextDouble() * 200 + Math.Sin(animationFrame * 0.1 + i) * 50);
                
                int colorIndex = Math.Min(themeColors.Length - 1, (int)(random.NextDouble() * themeColors.Length));
                Color color = Color.FromArgb(
                    (int)(150 + random.NextDouble() * 105),
                    themeColors[colorIndex]);
                
                using (Brush brush = new SolidBrush(color))
                {
                    float size = (float)(5 + random.NextDouble() * 15);
                    g.FillEllipse(brush, x - size/2, y - size/2, size, size);
                }
            }
        }

        private void DrawNeon(Graphics g)
        {
            // Efectos de neón con líneas brillantes
            for (int i = 0; i < themeColors.Length; i++)
            {
                using (Pen pen = new Pen(themeColors[i], 4))
                {
                    // Líneas horizontales parpadeantes
                    float alpha = (float)(0.5 + 0.5 * Math.Sin(animationFrame * 0.1 + i));
                    Color glowColor = Color.FromArgb((int)(255 * alpha), themeColors[i]);
                    pen.Color = glowColor;
                    
                    int y = 100 + i * 80;
                    g.DrawLine(pen, 50, y, panelVisualization.Width - 50, y);
                    
                    // Efecto de resplandor
                    using (Pen glowPen = new Pen(Color.FromArgb(50, themeColors[i]), 8))
                    {
                        g.DrawLine(glowPen, 50, y, panelVisualization.Width - 50, y);
                    }
                }
            }

            // Círculos de neón pulsantes
            PointF center = new PointF(panelVisualization.Width / 2, panelVisualization.Height / 2);
            for (int ring = 0; ring < 5; ring++)
            {
                float radius = 50 + ring * 40 + (float)(Math.Sin(animationFrame * 0.08) * 20);
                Color neonColor = themeColors[ring % themeColors.Length];
                
                using (Pen pen = new Pen(neonColor, 3))
                {
                    g.DrawEllipse(pen, 
                        center.X - radius, center.Y - radius, 
                        radius * 2, radius * 2);
                }
            }
        }

        private void btnChangeTheme_Click(object sender, EventArgs e)
        {
            // Si está en modo automático, cambiar a manual temporalmente
            if (autoChangeThemes)
            {
                autoChangeThemes = false;
                themeChangeCounter = 0;
            }
            
            currentTheme = (currentTheme + 1) % themeNames.Length;
            lblTheme.Text = "Tema: " + themeNames[currentTheme] + " (Manual)";
            UpdateThemeColors();
            
            // Reactivar modo automático después de 30 segundos de inactividad
            System.Threading.Tasks.Task.Delay(30000).ContinueWith(t => 
            {
                if (!this.IsDisposed)
                {
                    this.Invoke(new Action(() => 
                    {
                        autoChangeThemes = true;
                        themeChangeCounter = 0;
                    }));
                }
            });
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            // Cambiar la velocidad de la animación basado en el valor del trackbar
            // Valor más alto = animación más rápida (menor intervalo)
            int newInterval = 110 - trackBarSpeed.Value; // 10-100ms
            timerAnimation.Interval = newInterval;
        }

        private void btnFullscreen_Click(object sender, EventArgs e)
        {
            ToggleFullscreen();
        }

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
            themeChangeCounter = 0; // Reiniciar contador
            
            // Actualizar la etiqueta del tema
            string mode = autoChangeThemes ? " (Auto)" : " (Manual)";
            lblTheme.Text = "Tema: " + themeNames[currentTheme] + mode;
        }
    }
}
