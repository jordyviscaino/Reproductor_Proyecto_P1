namespace Reproductor_Proyecto_P1
{
    partial class ReproductorPr
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReproductorPr));
            mtrackDuracion = new TrackBar();
            txtTitle = new TextBox();
            btnCerrar = new Button();
            btnMinimizar = new Button();
            btnPlay = new Button();
            button2 = new Button();
            btnAdelantar = new Button();
            btnAtrasar = new Button();
            btnSiguiente = new Button();
            btnAnterior = new Button();
            txtTranscurrido = new TextBox();
            txtDuracion = new TextBox();
            trackBarVolumen = new TrackBar();
            button8 = new Button();
            btnLoop = new Button();
            btnUpload = new Button();
            MediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            label1 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            panelCtrlBox = new Panel();
            btnVisualization = new Button();
            ((System.ComponentModel.ISupportInitialize)mtrackDuracion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolumen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MediaPlayer).BeginInit();
            panelCtrlBox.SuspendLayout();
            SuspendLayout();
            // 
            // mtrackDuracion
            // 
            mtrackDuracion.Enabled = false;
            mtrackDuracion.Location = new Point(8, 75);
            mtrackDuracion.Name = "mtrackDuracion";
            mtrackDuracion.Size = new Size(374, 45);
            mtrackDuracion.TabIndex = 0;
            mtrackDuracion.TickStyle = TickStyle.None;
            mtrackDuracion.Scroll += mtrackDuracion_Scroll;
            mtrackDuracion.ValueChanged += mtrackDuracion_ValueChanged;
            // 
            // txtTitle
            // 
            txtTitle.BackColor = Color.FromArgb(227, 204, 174);
            txtTitle.BorderStyle = BorderStyle.None;
            txtTitle.Font = new Font("Bauhaus 93", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTitle.ForeColor = Color.FromArgb(38, 42, 86);
            txtTitle.Location = new Point(8, 32);
            txtTitle.Multiline = true;
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(374, 47);
            txtTitle.TabIndex = 1;
            txtTitle.Text = "--- ----";
            txtTitle.TextAlign = HorizontalAlignment.Center;
            // 
            // btnCerrar
            // 
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Font = new Font("Century", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCerrar.ForeColor = Color.LightCoral;
            btnCerrar.Location = new Point(371, 0);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(27, 25);
            btnCerrar.TabIndex = 2;
            btnCerrar.Text = "X";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // btnMinimizar
            // 
            btnMinimizar.FlatAppearance.BorderSize = 0;
            btnMinimizar.FlatStyle = FlatStyle.Flat;
            btnMinimizar.Font = new Font("Castellar", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMinimizar.ForeColor = Color.Transparent;
            btnMinimizar.Location = new Point(337, 0);
            btnMinimizar.Name = "btnMinimizar";
            btnMinimizar.Size = new Size(27, 25);
            btnMinimizar.TabIndex = 3;
            btnMinimizar.Text = "-";
            btnMinimizar.UseVisualStyleBackColor = true;
            btnMinimizar.MouseClick += btnMinimizar_MouseClick;
            // 
            // btnPlay
            // 
            btnPlay.Enabled = false;
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnPlay.ForeColor = Color.FromArgb(184, 98, 27);
            btnPlay.Location = new Point(197, 101);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(40, 40);
            btnPlay.TabIndex = 4;
            btnPlay.Text = ";";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button2.ForeColor = Color.FromArgb(184, 98, 27);
            button2.Location = new Point(160, 101);
            button2.Name = "button2";
            button2.Size = new Size(40, 40);
            button2.TabIndex = 5;
            button2.Text = "<";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnAdelantar
            // 
            btnAdelantar.Enabled = false;
            btnAdelantar.FlatAppearance.BorderSize = 0;
            btnAdelantar.FlatStyle = FlatStyle.Flat;
            btnAdelantar.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnAdelantar.ForeColor = Color.FromArgb(184, 98, 27);
            btnAdelantar.Location = new Point(243, 101);
            btnAdelantar.Name = "btnAdelantar";
            btnAdelantar.Size = new Size(40, 40);
            btnAdelantar.TabIndex = 5;
            btnAdelantar.Text = "8";
            btnAdelantar.UseVisualStyleBackColor = true;
            btnAdelantar.Click += btnAdelantar_Click;
            // 
            // btnAtrasar
            // 
            btnAtrasar.Enabled = false;
            btnAtrasar.FlatAppearance.BorderSize = 0;
            btnAtrasar.FlatStyle = FlatStyle.Flat;
            btnAtrasar.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnAtrasar.ForeColor = Color.FromArgb(184, 98, 27);
            btnAtrasar.Location = new Point(114, 101);
            btnAtrasar.Name = "btnAtrasar";
            btnAtrasar.Size = new Size(40, 40);
            btnAtrasar.TabIndex = 7;
            btnAtrasar.Text = "7";
            btnAtrasar.UseVisualStyleBackColor = true;
            btnAtrasar.Click += btnAtrasar_Click;
            // 
            // btnSiguiente
            // 
            btnSiguiente.Enabled = false;
            btnSiguiente.FlatAppearance.BorderSize = 0;
            btnSiguiente.FlatStyle = FlatStyle.Flat;
            btnSiguiente.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnSiguiente.ForeColor = Color.FromArgb(184, 98, 27);
            btnSiguiente.Location = new Point(289, 101);
            btnSiguiente.Name = "btnSiguiente";
            btnSiguiente.Size = new Size(40, 40);
            btnSiguiente.TabIndex = 8;
            btnSiguiente.Text = ":";
            btnSiguiente.UseVisualStyleBackColor = true;
            btnSiguiente.Click += btnSiguiente_Click;
            // 
            // btnAnterior
            // 
            btnAnterior.Enabled = false;
            btnAnterior.FlatAppearance.BorderSize = 0;
            btnAnterior.FlatStyle = FlatStyle.Flat;
            btnAnterior.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnAnterior.ForeColor = Color.FromArgb(184, 98, 27);
            btnAnterior.Location = new Point(68, 101);
            btnAnterior.Name = "btnAnterior";
            btnAnterior.Size = new Size(40, 40);
            btnAnterior.TabIndex = 5;
            btnAnterior.Text = "9";
            btnAnterior.UseVisualStyleBackColor = true;
            btnAnterior.Visible = false;
            btnAnterior.Click += btnAnterior_Click;
            // 
            // txtTranscurrido
            // 
            txtTranscurrido.BackColor = Color.FromArgb(227, 204, 174);
            txtTranscurrido.BorderStyle = BorderStyle.None;
            txtTranscurrido.ForeColor = Color.FromArgb(38, 42, 86);
            txtTranscurrido.Location = new Point(12, 91);
            txtTranscurrido.Name = "txtTranscurrido";
            txtTranscurrido.Size = new Size(45, 16);
            txtTranscurrido.TabIndex = 9;
            txtTranscurrido.Text = "-:-";
            txtTranscurrido.TextAlign = HorizontalAlignment.Center;
            // 
            // txtDuracion
            // 
            txtDuracion.BackColor = Color.FromArgb(227, 204, 174);
            txtDuracion.BorderStyle = BorderStyle.None;
            txtDuracion.ForeColor = Color.FromArgb(38, 42, 86);
            txtDuracion.Location = new Point(337, 91);
            txtDuracion.Name = "txtDuracion";
            txtDuracion.Size = new Size(45, 16);
            txtDuracion.TabIndex = 10;
            txtDuracion.Text = "-:-";
            txtDuracion.TextAlign = HorizontalAlignment.Center;
            txtDuracion.TextChanged += textBox2_TextChanged;
            // 
            // trackBarVolumen
            // 
            trackBarVolumen.LargeChange = 1;
            trackBarVolumen.Location = new Point(289, 147);
            trackBarVolumen.Maximum = 100;
            trackBarVolumen.Name = "trackBarVolumen";
            trackBarVolumen.Size = new Size(104, 45);
            trackBarVolumen.TabIndex = 11;
            trackBarVolumen.TickFrequency = 3;
            trackBarVolumen.TickStyle = TickStyle.TopLeft;
            trackBarVolumen.ValueChanged += trackBarVolumen_ValueChanged;
            // 
            // button8
            // 
            button8.FlatAppearance.BorderSize = 0;
            button8.FlatStyle = FlatStyle.Flat;
            button8.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button8.ForeColor = Color.FromArgb(184, 98, 27);
            button8.Location = new Point(262, 147);
            button8.Name = "button8";
            button8.Size = new Size(25, 29);
            button8.TabIndex = 13;
            button8.Text = "X";
            button8.UseVisualStyleBackColor = true;
            // 
            // btnLoop
            // 
            btnLoop.BackColor = Color.Transparent;
            btnLoop.FlatAppearance.BorderSize = 0;
            btnLoop.FlatAppearance.MouseOverBackColor = Color.FromArgb(184, 98, 27);
            btnLoop.FlatStyle = FlatStyle.Flat;
            btnLoop.Font = new Font("Wingdings 3", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnLoop.ForeColor = Color.FromArgb(38, 42, 86);
            btnLoop.Location = new Point(231, 147);
            btnLoop.Name = "btnLoop";
            btnLoop.Size = new Size(25, 29);
            btnLoop.TabIndex = 12;
            btnLoop.Text = "Q";
            btnLoop.UseVisualStyleBackColor = false;
            btnLoop.Click += btnLoop_Click;
            // 
            // btnUpload
            // 
            btnUpload.FlatAppearance.BorderSize = 0;
            btnUpload.FlatStyle = FlatStyle.Flat;
            btnUpload.Font = new Font("Wingdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnUpload.ForeColor = Color.Transparent;
            btnUpload.Location = new Point(0, 0);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(33, 25);
            btnUpload.TabIndex = 14;
            btnUpload.Text = "1";
            btnUpload.UseVisualStyleBackColor = true;
            btnUpload.Click += btnUpload_Click;
            // 
            // MediaPlayer
            // 
            MediaPlayer.Enabled = true;
            MediaPlayer.Location = new Point(405, 115);
            MediaPlayer.Name = "MediaPlayer";
            MediaPlayer.OcxState = (AxHost.State)resources.GetObject("MediaPlayer.OcxState");
            MediaPlayer.Size = new Size(25, 25);
            MediaPlayer.TabIndex = 15;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(32, 6);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 16;
            label1.Text = "Subir archivo";
            // 
            // timer1
            // 
            timer1.Interval = 1;
            timer1.Tick += timer1_Tick;
            // 
            // panelCtrlBox
            // 
            panelCtrlBox.BackColor = Color.FromArgb(38, 42, 86);
            panelCtrlBox.Controls.Add(btnCerrar);
            panelCtrlBox.Controls.Add(label1);
            panelCtrlBox.Controls.Add(btnMinimizar);
            panelCtrlBox.Controls.Add(btnUpload);
            panelCtrlBox.Dock = DockStyle.Top;
            panelCtrlBox.Location = new Point(0, 0);
            panelCtrlBox.Name = "panelCtrlBox";
            panelCtrlBox.Size = new Size(398, 25);
            panelCtrlBox.TabIndex = 17;
            panelCtrlBox.MouseMove += panelCtrlBox_MouseMove;
            // 
            // btnVisualization
            // 
            btnVisualization.FlatAppearance.BorderSize = 0;
            btnVisualization.FlatStyle = FlatStyle.Flat;
            btnVisualization.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVisualization.ForeColor = Color.FromArgb(4, 102, 200);
            btnVisualization.Location = new Point(9, 196);
            btnVisualization.Margin = new Padding(3, 4, 3, 4);
            btnVisualization.Name = "btnVisualization";
            btnVisualization.Size = new Size(120, 39);
            btnVisualization.TabIndex = 18;
            btnVisualization.Text = "Visualización";
            btnVisualization.UseVisualStyleBackColor = true;
            btnVisualization.Click += btnVisualization_Click;
            // 
            // ReproductorPr
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(227, 204, 174);
            ClientSize = new Size(398, 188);
            ControlBox = false;
            Controls.Add(btnVisualization);
            Controls.Add(panelCtrlBox);
            Controls.Add(MediaPlayer);
            Controls.Add(button8);
            Controls.Add(btnLoop);
            Controls.Add(trackBarVolumen);
            Controls.Add(txtDuracion);
            Controls.Add(txtTranscurrido);
            Controls.Add(btnAnterior);
            Controls.Add(btnSiguiente);
            Controls.Add(btnAtrasar);
            Controls.Add(btnAdelantar);
            Controls.Add(button2);
            Controls.Add(btnPlay);
            Controls.Add(txtTitle);
            Controls.Add(mtrackDuracion);
            ForeColor = SystemColors.Control;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "ReproductorPr";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)mtrackDuracion).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolumen).EndInit();
            ((System.ComponentModel.ISupportInitialize)MediaPlayer).EndInit();
            panelCtrlBox.ResumeLayout(false);
            panelCtrlBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar mtrackDuracion;
        private TextBox txtTitle;
        private Button btnCerrar;
        private Button btnMinimizar;
        private Button btnPlay;
        private Button button2;
        private Button btnAdelantar;
        private Button btnAtrasar;
        private Button btnSiguiente;
        private Button btnAnterior;
        private TextBox txtTranscurrido;
        private TextBox txtDuracion;
        private TrackBar trackBarVolumen;
        private Button button8;
        private Button btnLoop;
        private Button btnUpload;
        private AxWMPLib.AxWindowsMediaPlayer MediaPlayer;
        private Label label1;
        private System.Windows.Forms.Timer timer1;
        private Panel panelCtrlBox;
        private Button btnVisualization;
    }
}
