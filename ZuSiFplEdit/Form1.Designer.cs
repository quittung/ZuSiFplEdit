namespace ZuSiFplEdit
{
    partial class Form1
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
            this.ModulButton = new System.Windows.Forms.Button();
            this.ModText = new System.Windows.Forms.TextBox();
            this.mMap = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).BeginInit();
            this.SuspendLayout();
            // 
            // ModulButton
            // 
            this.ModulButton.Location = new System.Drawing.Point(12, 12);
            this.ModulButton.Name = "ModulButton";
            this.ModulButton.Size = new System.Drawing.Size(260, 39);
            this.ModulButton.TabIndex = 0;
            this.ModulButton.Text = "Module Lesen";
            this.ModulButton.UseVisualStyleBackColor = true;
            this.ModulButton.Click += new System.EventHandler(this.ModulButton_Click);
            // 
            // ModText
            // 
            this.ModText.AcceptsReturn = true;
            this.ModText.Location = new System.Drawing.Point(12, 57);
            this.ModText.Multiline = true;
            this.ModText.Name = "ModText";
            this.ModText.ReadOnly = true;
            this.ModText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ModText.Size = new System.Drawing.Size(260, 474);
            this.ModText.TabIndex = 1;
            // 
            // mMap
            // 
            this.mMap.Location = new System.Drawing.Point(278, 12);
            this.mMap.Name = "mMap";
            this.mMap.Size = new System.Drawing.Size(549, 519);
            this.mMap.TabIndex = 2;
            this.mMap.TabStop = false;
            this.mMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mMap_MouseDown);
            this.mMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mMap_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 543);
            this.Controls.Add(this.mMap);
            this.Controls.Add(this.ModText);
            this.Controls.Add(this.ModulButton);
            this.Name = "Form1";
            this.Text = "ZuSi 3 Fahrplaneditor++";
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ModulButton;
        private System.Windows.Forms.TextBox ModText;
        private System.Windows.Forms.PictureBox mMap;
    }
}

