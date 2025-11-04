namespace Reproductor_Proyecto_P1
{
    public partial class ReproductorPr : Form
    {
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
                string Titulo = Dialog.SafeFileName;
                txtTitle.Text = Titulo;
                uploadAudio(Dialog.FileName);
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
    }
}
