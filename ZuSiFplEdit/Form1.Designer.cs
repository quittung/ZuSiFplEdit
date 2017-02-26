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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(modSelForm));
            this.mMap = new System.Windows.Forms.PictureBox();
            this.modListBox = new System.Windows.Forms.ListBox();
            this.ButtonExport = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMap
            // 
            this.mMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMap.Location = new System.Drawing.Point(0, 0);
            this.mMap.Name = "mMap";
            this.mMap.Size = new System.Drawing.Size(575, 527);
            this.mMap.TabIndex = 2;
            this.mMap.TabStop = false;
            this.mMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mMap_MouseDown);
            this.mMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mMap_MouseUp);
            this.mMap.Resize += new System.EventHandler(this.mMap_Resize);
            // 
            // modListBox
            // 
            this.modListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modListBox.FormattingEnabled = true;
            this.modListBox.Location = new System.Drawing.Point(3, 3);
            this.modListBox.Name = "modListBox";
            this.modListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.modListBox.Size = new System.Drawing.Size(238, 467);
            this.modListBox.TabIndex = 3;
            this.modListBox.SelectedValueChanged += new System.EventHandler(this.modListBox_SelectedValueChanged);
            // 
            // ButtonExport
            // 
            this.ButtonExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonExport.Location = new System.Drawing.Point(3, 476);
            this.ButtonExport.Name = "ButtonExport";
            this.ButtonExport.Size = new System.Drawing.Size(238, 48);
            this.ButtonExport.TabIndex = 0;
            this.ButtonExport.Text = ".fpn ausgeben";
            this.ButtonExport.UseVisualStyleBackColor = true;
            this.ButtonExport.Click += new System.EventHandler(this.ModulButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(8, 8);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mMap);
            this.splitContainer1.Size = new System.Drawing.Size(823, 527);
            this.splitContainer1.SplitterDistance = 244;
            this.splitContainer1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.modListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonExport, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.87109F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.12891F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(244, 527);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // modSelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 543);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "modSelForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Text = "ZuSi 3 Modulauswahl";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.modSelForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox mMap;
        private System.Windows.Forms.ListBox modListBox;
        private System.Windows.Forms.Button ButtonExport;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

