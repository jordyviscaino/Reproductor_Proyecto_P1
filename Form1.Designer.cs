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
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            txtTranscurrido = new TextBox();
            txtDuracion = new TextBox();
            trackBarVolumen = new TrackBar();
            button8 = new Button();
            button7 = new Button();
            btnUpload = new Button();
            MediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            label1 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            panelCtrlBox = new Panel();
            ((System.ComponentModel.ISupportInitialize)mtrackDuracion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolumen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MediaPlayer).BeginInit();
            panelCtrlBox.SuspendLayout();
            SuspendLayout();
            // 
            // mtrackDuracion
            // 
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
            txtTitle.BackColor = Color.FromArgb(0, 18, 51);
            txtTitle.BorderStyle = BorderStyle.None;
            txtTitle.Font = new Font("Bauhaus 93", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTitle.ForeColor = Color.FromArgb(151, 157, 172);
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
            btnMinimizar.ForeColor = Color.Black;
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
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnPlay.ForeColor = Color.FromArgb(4, 102, 200);
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
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button2.ForeColor = Color.FromArgb(4, 102, 200);
            button2.Location = new Point(160, 101);
            button2.Name = "button2";
            button2.Size = new Size(40, 40);
            button2.TabIndex = 5;
            button2.Text = "<";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button3.ForeColor = Color.FromArgb(3, 83, 164);
            button3.Location = new Point(243, 101);
            button3.Name = "button3";
            button3.Size = new Size(40, 40);
            button3.TabIndex = 5;
            button3.Text = "8";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button4.ForeColor = Color.FromArgb(3, 83, 164);
            button4.Location = new Point(114, 101);
            button4.Name = "button4";
            button4.Size = new Size(40, 40);
            button4.TabIndex = 7;
            button4.Text = "7";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button5.ForeColor = Color.FromArgb(3, 83, 164);
            button5.Location = new Point(289, 101);
            button5.Name = "button5";
            button5.Size = new Size(40, 40);
            button5.TabIndex = 8;
            button5.Text = ":";
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Webdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button6.ForeColor = Color.FromArgb(3, 83, 164);
            button6.Location = new Point(68, 101);
            button6.Name = "button6";
            button6.Size = new Size(40, 40);
            button6.TabIndex = 5;
            button6.Text = "9";
            button6.UseVisualStyleBackColor = true;
            // 
            // txtTranscurrido
            // 
            txtTranscurrido.BackColor = Color.FromArgb(0, 18, 51);
            txtTranscurrido.BorderStyle = BorderStyle.None;
            txtTranscurrido.ForeColor = SystemColors.MenuBar;
            txtTranscurrido.Location = new Point(12, 91);
            txtTranscurrido.Name = "txtTranscurrido";
            txtTranscurrido.Size = new Size(45, 16);
            txtTranscurrido.TabIndex = 9;
            txtTranscurrido.Text = "-:-";
            txtTranscurrido.TextAlign = HorizontalAlignment.Center;
            // 
            // txtDuracion
            // 
            txtDuracion.BackColor = Color.FromArgb(0, 18, 51);
            txtDuracion.BorderStyle = BorderStyle.None;
            txtDuracion.ForeColor = SystemColors.Window;
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
            button8.ForeColor = Color.FromArgb(4, 102, 200);
            button8.Location = new Point(262, 147);
            button8.Name = "button8";
            button8.Size = new Size(25, 29);
            button8.TabIndex = 13;
            button8.Text = "X";
            button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.FlatAppearance.BorderSize = 0;
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("Wingdings 3", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            button7.ForeColor = Color.FromArgb(4, 102, 200);
            button7.Location = new Point(231, 147);
            button7.Name = "button7";
            button7.Size = new Size(25, 29);
            button7.TabIndex = 12;
            button7.Text = "Q";
            button7.UseVisualStyleBackColor = true;
            // 
            // btnUpload
            // 
            btnUpload.FlatAppearance.BorderSize = 0;
            btnUpload.FlatStyle = FlatStyle.Flat;
            btnUpload.Font = new Font("Wingdings", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 2);
            btnUpload.ForeColor = Color.Black;
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
            panelCtrlBox.BackColor = Color.FromArgb(125, 133, 151);
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
            // ReproductorPr
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 18, 51);
            ClientSize = new Size(398, 188);
            ControlBox = false;
            Controls.Add(panelCtrlBox);
            Controls.Add(MediaPlayer);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(trackBarVolumen);
            Controls.Add(txtDuracion);
            Controls.Add(txtTranscurrido);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
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
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private TextBox txtTranscurrido;
        private TextBox txtDuracion;
        private TrackBar trackBarVolumen;
        private Button button8;
        private Button button7;
        private Button btnUpload;
        private AxWMPLib.AxWindowsMediaPlayer MediaPlayer;
        private Label label1;
        private System.Windows.Forms.Timer timer1;
        private Panel panelCtrlBox;
    }
}
