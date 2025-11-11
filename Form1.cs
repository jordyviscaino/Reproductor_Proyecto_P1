using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace Reproductor_Proyecto_P1
{
    public partial class ReproductorPr : Form
    {
        private Form2 visualizationForm = null;
        private AudioAnalyzer audioAnalyzer;
        private bool currentFileIsAnalyzable = false;
        private Stopwatch vizStopwatch = new Stopwatch();
        private bool beatSubscribed = false;
        private bool onsetSubscribed = false;
        private string tempWavPath = null;

        public ReproductorPr()
        {
            InitializeComponent();

            audioAnalyzer = new AudioAnalyzer(2048, 8);
            this.FormClosing += ReproductorPr_FormClosing;
            vizStopwatch.Start();
        }

        private void ReproductorPr_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (audioAnalyzer != null && beatSubscribed)
            {
                audioAnalyzer.BeatDetected -= OnBeatDetected;
                beatSubscribed = false;
            }
            if (audioAnalyzer != null && onsetSubscribed)
            {
                audioAnalyzer.OnsetDetected -= OnOnsetDetected;
                onsetSubscribed = false;
            }
            audioAnalyzer?.Dispose();
        }
        private void CleanupTempFile()
        {
            if (tempWavPath != null && System.IO.File.Exists(tempWavPath))
            {
                try
                {
                    System.IO.File.Delete(tempWavPath);
                    tempWavPath = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error deleting temp file: " + ex.Message);
                }
            }
        }


        private void OnBeatDetected()
        {
            if (visualizationForm != null && !visualizationForm.IsDisposed)
            {
                try
                {
                    visualizationForm.BeginInvoke(new Action(() => visualizationForm.TriggerBeat()));
                }
                catch { }
            }
        }

        private void OnOnsetDetected(OnsetType type)
        {
            if (visualizationForm != null && !visualizationForm.IsDisposed)
            {
                try
                {
                    visualizationForm.BeginInvoke(new Action(() => visualizationForm.TriggerOnset(type)));
                }
                catch { }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Multiselect = true;
            Dialog.Filter = "Archivos de audio|*.mp3;*.wav;*.wma;*.aac;*.flac;*.ogg|Todos los archivos|*.*";
            Dialog.Title = "Selecciona un archivo de audio";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                btnAnterior.Enabled = false;
                Rutas = Dialog.FileNames;
                Titulos = Dialog.SafeFileNames;
                Reproduciendo = 0;
                if (Rutas.Length == 1)
                {
                    btnAnterior.Visible = false;
                    btnSiguiente.Visible = false;
                }
                else
                {
                    btnAnterior.Visible = true;
                    btnSiguiente.Visible = true;
                    btnSiguiente.Enabled = true;

                }
                txtTitle.Clear();
                txtTitle.Text = Titulos[Reproduciendo];
                uploadAudio(Rutas[Reproduciendo]);
                button2.Enabled = true;
                btnPlay.Enabled = true;
                btnLoop.Enabled = true;
                btnAtrasar.Enabled = true;
                btnAdelantar.Enabled = true;
                mtrackDuracion.Enabled = true;
                btnPlay.Text = ";";

            }
        }

        private void uploadAudio(string ruta)
        {
            try
            {
                if (audioAnalyzer != null && beatSubscribed)
                {
                    audioAnalyzer.BeatDetected -= OnBeatDetected;
                    beatSubscribed = false;
                }
                if (audioAnalyzer != null && onsetSubscribed)
                {
                    audioAnalyzer.OnsetDetected -= OnOnsetDetected;
                    onsetSubscribed = false;
                }

                CleanupTempFile();

                MediaPlayer.URL = @"" + ruta;
                timer1.Start();


                string fileToLoad = ruta;
                bool isSupportedFormat = false;
                string extension = System.IO.Path.GetExtension(ruta).ToLowerInvariant();

                if (extension == ".wav")
                {
                    fileToLoad = ruta;
                    isSupportedFormat = true;
                }
                else if (extension == ".mp3")
                {
                    try
                    {
                        tempWavPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "temp_decoded.wav");

                        using (var reader = new NAudio.Wave.Mp3FileReader(ruta))
                        {
                            NAudio.Wave.WaveFileWriter.CreateWaveFile(tempWavPath, reader);
                        }
                        fileToLoad = tempWavPath; 
                        isSupportedFormat = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Fallo al decodificar MP3: " + ex.Message);
                        isSupportedFormat = false;
                    }
                }

                if (isSupportedFormat)
                {
                    bool loaded = audioAnalyzer.LoadWav(fileToLoad); 
                    currentFileIsAnalyzable = loaded;
                    if (!loaded)
                    {
                        currentFileIsAnalyzable = false;
                    }
                    else
                    {
                        if (visualizationForm == null || visualizationForm.IsDisposed)
                        {
                            visualizationForm = new Form2();
                            visualizationForm.Show();
                        }

                        if (!beatSubscribed)
                        {
                            audioAnalyzer.BeatDetected += OnBeatDetected;
                            beatSubscribed = true;
                        }
                        if (!onsetSubscribed)
                        {
                            audioAnalyzer.OnsetDetected += OnOnsetDetected;
                            onsetSubscribed = true;
                        }
                        vizStopwatch.Restart();
                    }
                }
                else
                {
                    currentFileIsAnalyzable = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el archivo de audio: " + ex.Message);
            }

        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void trackBarVolumen_ValueChanged(object sender, EventArgs e)
        {
            MediaPlayer.settings.volume = trackBarVolumen.Value;

        }

        private void mtrackDuracion_Scroll(object sender, EventArgs e)
        {

        }

        public void ProgesoTrackBar()
        {
            if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                mtrackDuracion.Maximum = (int)MediaPlayer.currentMedia.duration;
                timer1.Start();
            }
            else if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                timer1.Stop();
            }
            else if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                timer1.Stop();
                mtrackDuracion.Value = 0;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProgesoTrackBar();

            if (MediaPlayer.currentMedia != null)
            {
                int dur = (int)MediaPlayer.currentMedia.duration;
                if (dur > mtrackDuracion.Maximum)
                    mtrackDuracion.Maximum = Math.Max(dur, mtrackDuracion.Maximum);

                int pos = (int)MediaPlayer.Ctlcontrols.currentPosition;
                if (pos < mtrackDuracion.Minimum) pos = mtrackDuracion.Minimum;
                if (pos > mtrackDuracion.Maximum) pos = mtrackDuracion.Maximum;

                try
                {
                    mtrackDuracion.Value = pos;
                }
                catch
                {
                    mtrackDuracion.Value = Math.Clamp(pos, mtrackDuracion.Minimum, mtrackDuracion.Maximum);
                }
            }

            trackBarVolumen.Value = MediaPlayer.settings.volume;
            if (MediaPlayer.currentMedia != null)
            {
                txtDuracion.Text = MediaPlayer.currentMedia.durationString;
            }
            txtTranscurrido.Text = MediaPlayer.Ctlcontrols.currentPositionString;

            if (currentFileIsAnalyzable && visualizationForm != null && !visualizationForm.IsDisposed)
            {
                if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying || MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPaused)
                {
                    if (vizStopwatch.ElapsedMilliseconds >= 25) // ~40 fps
                    {
                        double posd = MediaPlayer.Ctlcontrols.currentPosition;
                        var bands = audioAnalyzer.GetBandsAtPosition(posd);
                        if (bands != null)
                        {
                            try
                            {
                                visualizationForm.BeginInvoke(new Action(() => visualizationForm.UpdateSpectrum(bands)));
                            }
                            catch { }
                        }
                        vizStopwatch.Restart();
                    }
                }
            }
        }
        int Reproduciendo;
        string[] Rutas;
        string[] Titulos;
        private void mtrackDuracion_ValueChanged(object sender, EventArgs e)
        {
            bool finished = false;
            if (txtDuracion.Text == txtTranscurrido.Text)
            {
                finished = true;
            }
            if (mtrackDuracion.Value != (int)MediaPlayer.Ctlcontrols.currentPosition)
            {
                MediaPlayer.Ctlcontrols.currentPosition = mtrackDuracion.Value;
            }
            if (finished == true && btnLoop.BackColor == Color.Transparent && Rutas.Length == 1)
            {
                btnPlay.Text = "4";
                timer1.Stop();
            }
            else
            {
                if (finished == true && Rutas.Length > 1 && btnLoop.BackColor == Color.Transparent)
                {
                    Reproduciendo += 1;
                    if (Reproduciendo == Rutas.Length)
                    {
                        Reproduciendo = 0;
                        uploadAudio(Rutas[Reproduciendo]);
                        txtTitle.Text = Titulos[Reproduciendo];
                        btnAnterior.Enabled = false;
                        btnSiguiente.Enabled = true;
                    }
                    else
                    {
                        if (Reproduciendo == Rutas.Length - 1)
                        {
                            btnSiguiente.Enabled = false;

                        }
                        uploadAudio(Rutas[Reproduciendo]);
                        txtTitle.Text = Titulos[Reproduciendo];
                        btnAnterior.Enabled = true;
                    }

                }

            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (btnPlay.Text == ";")
            {
                MediaPlayer.Ctlcontrols.pause();
                btnPlay.Text = "4";
                timer1.Stop();
            }
            else if (btnPlay.Text == "4")
            {
                MediaPlayer.Ctlcontrols.play();
                btnPlay.Text = ";";
                timer1.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MediaPlayer.Ctlcontrols.stop();
            btnPlay.Text = "4";
            txtTranscurrido.Text = "00:00";
            mtrackDuracion.Value = 0;
            timer1.Stop();
            audioAnalyzer.Stop();
        }

        public void OpenVisualization()
        {
            if (visualizationForm == null || visualizationForm.IsDisposed)
            {
                visualizationForm = new Form2();
                visualizationForm.Show();
            }
            else
            {
                visualizationForm.BringToFront();
            }
        }

        int positiony = 0;
        int positionx = 0;
        private void panelCtrlBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                positionx = e.X;
                positiony = e.Y;
            }
            else
            {
                this.Left = this.Left + (e.X - positionx);
                this.Top = this.Top + (e.Y - positiony);
            }
        }

        private void btnMinimizar_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnAdelantar_Click(object sender, EventArgs e)
        {
            mtrackDuracion.Value = mtrackDuracion.Value + 10;
        }

        private void btnAtrasar_Click(object sender, EventArgs e)
        {
            mtrackDuracion.Value = mtrackDuracion.Value - 10;
        }

        private void btnLoop_Click(object sender, EventArgs e)
        {
            if (btnLoop.BackColor == Color.Transparent)
            {
                btnLoop.BackColor = btnLoop.FlatAppearance.MouseOverBackColor;
                MediaPlayer.settings.setMode("loop", true);
            }
            else if (btnLoop.BackColor == btnLoop.FlatAppearance.MouseOverBackColor)
            {
                btnLoop.BackColor = Color.Transparent;
                MediaPlayer.settings.setMode("loop", false);
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            Reproduciendo = Reproduciendo + 1;
            mtrackDuracion.Value = 0;
            uploadAudio(Rutas[Reproduciendo]);
            txtTitle.Text = Titulos[Reproduciendo];
            btnAnterior.Enabled = true;
            if (Reproduciendo == Rutas.Length - 1)
            {
                btnSiguiente.Enabled = false;
            }
            btnPlay.Text = ";";
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            Reproduciendo = Reproduciendo - 1;
            mtrackDuracion.Value = 0;
            uploadAudio(Rutas[Reproduciendo]);
            txtTitle.Text = Titulos[Reproduciendo];
            btnSiguiente.Enabled = true;
            if (Reproduciendo == 0)
            {
                btnAnterior.Enabled = false;
            }
            btnPlay.Text = ";";
        }

        private void panelCtrlBox_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            OpenVisualization();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Multiselect = true;
            Dialog.Filter = "Archivos de audio|*.mp3;*.wav;*.wma;*.aac;*.flac;*.ogg|Todos los archivos|*.*";
            Dialog.Title = "Selecciona un archivo de audio";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                btnAnterior.Enabled = false;
                Rutas = Dialog.FileNames;
                Titulos = Dialog.SafeFileNames;
                Reproduciendo = 0;
                if (Rutas.Length == 1)
                {
                    btnAnterior.Visible = false;
                    btnSiguiente.Visible = false;
                }
                else
                {
                    btnAnterior.Visible = true;
                    btnSiguiente.Visible = true;
                    btnSiguiente.Enabled = true;

                }
                txtTitle.Clear();
                txtTitle.Text = Titulos[Reproduciendo];
                uploadAudio(Rutas[Reproduciendo]);
                button2.Enabled = true;
                btnPlay.Enabled = true;
                btnLoop.Enabled = true;
                btnAtrasar.Enabled = true;
                btnAdelantar.Enabled = true;
                mtrackDuracion.Enabled = true;
                btnPlay.Text = ";";

            }
        }
    }
}
