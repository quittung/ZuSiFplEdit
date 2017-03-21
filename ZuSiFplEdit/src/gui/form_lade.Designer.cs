namespace ZuSiFplEdit
{
    partial class form_lade
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_lade));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Beschreibung = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Maximum = 4;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(405, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // Beschreibung
            // 
            this.Beschreibung.AutoSize = true;
            this.Beschreibung.Location = new System.Drawing.Point(12, 67);
            this.Beschreibung.Name = "Beschreibung";
            this.Beschreibung.Size = new System.Drawing.Size(64, 13);
            this.Beschreibung.TabIndex = 1;
            this.Beschreibung.Text = "Initialisiere...";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(12, 41);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(405, 23);
            this.progressBar2.TabIndex = 2;
            // 
            // form_lade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 92);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.Beschreibung);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "form_lade";
            this.Text = "Laden...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label Beschreibung;
        public System.Windows.Forms.ProgressBar progressBar2;
    }
}