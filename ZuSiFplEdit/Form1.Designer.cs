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
            this.mMap = new System.Windows.Forms.PictureBox();
            this.modListBox = new System.Windows.Forms.ListBox();
            this.ButtonExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).BeginInit();
            this.SuspendLayout();
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
            this.modListBox.Location = new System.Drawing.Point(12, 12);
            this.modListBox.Name = "modListBox";
            this.modListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.modListBox.Size = new System.Drawing.Size(260, 472);
            this.modListBox.TabIndex = 3;
            this.modListBox.SelectedValueChanged += new System.EventHandler(this.modListBox_SelectedValueChanged);
            // 
            // ButtonExport
            // 
            this.ButtonExport.Location = new System.Drawing.Point(12, 490);
            this.ButtonExport.Name = "ButtonExport";
            this.ButtonExport.Size = new System.Drawing.Size(260, 39);
            this.ButtonExport.TabIndex = 0;
            this.ButtonExport.Text = ".fpn ausgeben";
            this.ButtonExport.UseVisualStyleBackColor = true;
            this.ButtonExport.Click += new System.EventHandler(this.ModulButton_Click);
            // 
            // modSelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 543);
            this.Controls.Add(this.modListBox);
            this.Controls.Add(this.mMap);
            this.Controls.Add(this.ButtonExport);
            this.Name = "modSelForm";
            this.Text = "ZuSi 3 Modulauswahl#";
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox mMap;
        private System.Windows.Forms.ListBox modListBox;
        private System.Windows.Forms.Button ButtonExport;
    }
}

