namespace ZuSiFplEdit
{
    partial class form_bildfahrplancs
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
            this.BFP = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.BFP)).BeginInit();
            this.SuspendLayout();
            // 
            // BFP
            // 
            this.BFP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BFP.Location = new System.Drawing.Point(0, 0);
            this.BFP.Name = "BFP";
            this.BFP.Size = new System.Drawing.Size(921, 515);
            this.BFP.TabIndex = 0;
            this.BFP.TabStop = false;
            this.BFP.Click += new System.EventHandler(this.BFP_Click);
            // 
            // form_bildfahrplancs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 515);
            this.Controls.Add(this.BFP);
            this.Name = "form_bildfahrplancs";
            this.Text = "form_bildfahrplancs";
            ((System.ComponentModel.ISupportInitialize)(this.BFP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox BFP;
    }
}