namespace Reproductor_Proyecto_P1
{
    public partial class ReproductorPr : Form
    {
        private Form2 visualizationForm = null;

        public ReproductorPr()
        {
            InitializeComponent();
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
                MediaPlayer.URL = @"" + ruta;
                timer1.Start();
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
            mtrackDuracion.Value = (int)MediaPlayer.Ctlcontrols.currentPosition;
            trackBarVolumen.Value = MediaPlayer.settings.volume;
            txtDuracion.Text = MediaPlayer.currentMedia.durationString;
            txtTranscurrido.Text = MediaPlayer.Ctlcontrols.currentPositionString;
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
        }

        // Método para abrir la ventana de visualización
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

        private void panelCtrlBox_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnVisualization_Click(object sender, EventArgs e)
        {
            OpenVisualization();
        }
    }
}
