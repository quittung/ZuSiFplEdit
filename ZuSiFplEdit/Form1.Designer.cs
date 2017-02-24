namespace ZuSiFplEdit
{
    partial class modSelForm
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
            this.mMap = new System.Windows.Forms.PictureBox();
            this.modListBox = new System.Windows.Forms.ListBox();
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
            // modListBox
            // 
            this.modListBox.FormattingEnabled = true;
            this.modListBox.Location = new System.Drawing.Point(12, 57);
            this.modListBox.Name = "modListBox";
            this.modListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.modListBox.Size = new System.Drawing.Size(260, 472);
            this.modListBox.TabIndex = 3;
            this.modListBox.SelectedValueChanged += new System.EventHandler(this.modListBox_SelectedValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 543);
            this.Controls.Add(this.modListBox);
            this.Controls.Add(this.mMap);
            this.Controls.Add(this.ModulButton);
            this.Name = "Form1";
            this.Text = "ZuSi 3 Modulauswahl#";
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ModulButton;
        private System.Windows.Forms.PictureBox mMap;
        private System.Windows.Forms.ListBox modListBox;
    }
}

