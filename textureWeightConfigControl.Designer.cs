namespace MinecraftJsonGenerator
{
    partial class textureWeightConfigControl
    {
        /// <summary> 
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod wygenerowany przez Projektanta składników

        /// <summary> 
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            pixelArtPictureBox1 = new PixelArtPictureBox();
            label1 = new Label();
            trackBar1 = new TrackBar();
            numericUpDown1 = new NumericUpDown();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)pixelArtPictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // pixelArtPictureBox1
            // 
            pixelArtPictureBox1.BackColor = Color.White;
            pixelArtPictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pixelArtPictureBox1.Location = new Point(32, 2);
            pixelArtPictureBox1.Name = "pixelArtPictureBox1";
            pixelArtPictureBox1.Size = new Size(120, 120);
            pixelArtPictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pixelArtPictureBox1.TabIndex = 0;
            pixelArtPictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(2, 125);
            label1.Margin = new Padding(0);
            label1.Name = "label1";
            label1.Size = new Size(180, 40);
            label1.TabIndex = 1;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(2, 159);
            trackBar1.Maximum = 100;
            trackBar1.Minimum = 1;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(134, 45);
            trackBar1.TabIndex = 2;
            trackBar1.TickFrequency = 5;
            trackBar1.Value = 1;
            trackBar1.ValueChanged += WeightTrackBar_ValueChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(142, 168);
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(40, 23);
            numericUpDown1.TabIndex = 3;
            numericUpDown1.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDown1.ValueChanged += WeightNumericUpDown_ValueChanged;
            // 
            // label2
            // 
            label2.Location = new Point(2, 194);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Size = new Size(180, 29);
            label2.TabIndex = 4;
            label2.Text = "100%";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textureWeightConfigControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(label2);
            Controls.Add(numericUpDown1);
            Controls.Add(trackBar1);
            Controls.Add(label1);
            Controls.Add(pixelArtPictureBox1);
            Name = "textureWeightConfigControl";
            Padding = new Padding(2);
            Size = new Size(184, 225);
            ((System.ComponentModel.ISupportInitialize)pixelArtPictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PixelArtPictureBox pixelArtPictureBox1;
        private Label label1;
        private TrackBar trackBar1;
        private NumericUpDown numericUpDown1;
        private Label label2;
    }
}
