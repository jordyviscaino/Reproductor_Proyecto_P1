namespace Reproductor_Proyecto_P1
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelVisualization = new System.Windows.Forms.Panel();
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.btnChangeTheme = new System.Windows.Forms.Button();
            this.lblTheme = new System.Windows.Forms.Label();
            this.panelControls = new System.Windows.Forms.Panel();
            this.trackBarSpeed = new System.Windows.Forms.TrackBar();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.btnFullscreen = new System.Windows.Forms.Button();
            this.chkAutoChange = new System.Windows.Forms.CheckBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // panelVisualization
            // 
            this.panelVisualization.BackColor = System.Drawing.Color.Black;
            this.panelVisualization.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVisualization.Location = new System.Drawing.Point(0, 50);
            this.panelVisualization.Name = "panelVisualization";
            this.panelVisualization.Size = new System.Drawing.Size(1000, 550);
            this.panelVisualization.TabIndex = 0;
            this.panelVisualization.Paint += new System.Windows.Forms.PaintEventHandler(this.panelVisualization_Paint);
            // 
            // timerAnimation
            // 
            this.timerAnimation.Interval = 50;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // btnChangeTheme
            // 
            this.btnChangeTheme.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnChangeTheme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeTheme.ForeColor = System.Drawing.Color.White;
            this.btnChangeTheme.Location = new System.Drawing.Point(12, 12);
            this.btnChangeTheme.Name = "btnChangeTheme";
            this.btnChangeTheme.Size = new System.Drawing.Size(120, 26);
            this.btnChangeTheme.TabIndex = 1;
            this.btnChangeTheme.Text = "Cambiar Tema";
            this.btnChangeTheme.UseVisualStyleBackColor = false;
            this.btnChangeTheme.Click += new System.EventHandler(this.btnChangeTheme_Click);
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.ForeColor = System.Drawing.Color.White;
            this.lblTheme.Location = new System.Drawing.Point(150, 19);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(100, 15);
            this.lblTheme.TabIndex = 2;
            this.lblTheme.Text = "Tema: Espectro";
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.panelControls.Controls.Add(this.lblProgress);
            this.panelControls.Controls.Add(this.chkAutoChange);
            this.panelControls.Controls.Add(this.btnFullscreen);
            this.panelControls.Controls.Add(this.lblSpeed);
            this.panelControls.Controls.Add(this.trackBarSpeed);
            this.panelControls.Controls.Add(this.btnChangeTheme);
            this.panelControls.Controls.Add(this.lblTheme);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControls.Location = new System.Drawing.Point(0, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(1000, 50);
            this.panelControls.TabIndex = 3;
            // 
            // trackBarSpeed
            // 
            this.trackBarSpeed.Location = new System.Drawing.Point(350, 12);
            this.trackBarSpeed.Maximum = 100;
            this.trackBarSpeed.Minimum = 10;
            this.trackBarSpeed.Name = "trackBarSpeed";
            this.trackBarSpeed.Size = new System.Drawing.Size(150, 45);
            this.trackBarSpeed.TabIndex = 3;
            this.trackBarSpeed.TickFrequency = 10;
            this.trackBarSpeed.Value = 50;
            this.trackBarSpeed.ValueChanged += new System.EventHandler(this.trackBarSpeed_ValueChanged);
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.ForeColor = System.Drawing.Color.White;
            this.lblSpeed.Location = new System.Drawing.Point(280, 19);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(64, 15);
            this.lblSpeed.TabIndex = 4;
            this.lblSpeed.Text = "Velocidad:";
            // 
            // btnFullscreen
            // 
            this.btnFullscreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnFullscreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFullscreen.ForeColor = System.Drawing.Color.White;
            this.btnFullscreen.Location = new System.Drawing.Point(520, 12);
            this.btnFullscreen.Name = "btnFullscreen";
            this.btnFullscreen.Size = new System.Drawing.Size(100, 26);
            this.btnFullscreen.TabIndex = 5;
            this.btnFullscreen.Text = "Pantalla Completa";
            this.btnFullscreen.UseVisualStyleBackColor = false;
            this.btnFullscreen.Click += new System.EventHandler(this.btnFullscreen_Click);
            // 
            // chkAutoChange
            // 
            this.chkAutoChange.AutoSize = true;
            this.chkAutoChange.ForeColor = System.Drawing.Color.White;
            this.chkAutoChange.Location = new System.Drawing.Point(650, 16);
            this.chkAutoChange.Name = "chkAutoChange";
            this.chkAutoChange.Size = new System.Drawing.Size(127, 19);
            this.chkAutoChange.TabIndex = 6;
            this.chkAutoChange.Text = "Cambio Automático";
            this.chkAutoChange.UseVisualStyleBackColor = true;
            this.chkAutoChange.CheckedChanged += new System.EventHandler(this.chkAutoChange_CheckedChanged);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.ForeColor = System.Drawing.Color.LightGray;
            this.lblProgress.Location = new System.Drawing.Point(800, 19);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(50, 15);
            this.lblProgress.TabIndex = 7;
            this.lblProgress.Text = "10s";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.panelVisualization);
            this.Controls.Add(this.panelControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visualización de Audio";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form2_KeyDown);
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSpeed)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelVisualization;
        private System.Windows.Forms.Timer timerAnimation;
        private System.Windows.Forms.Button btnChangeTheme;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.TrackBar trackBarSpeed;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Button btnFullscreen;
        private System.Windows.Forms.CheckBox chkAutoChange;
        private System.Windows.Forms.Label lblProgress;
    }
}