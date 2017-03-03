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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.karteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moduToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modulgrenzenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.namenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verbindungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.punkteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fahrstraenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ladezeitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMap
            // 
            this.mMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMap.Location = new System.Drawing.Point(0, 0);
            this.mMap.Name = "mMap";
            this.mMap.Size = new System.Drawing.Size(580, 511);
            this.mMap.TabIndex = 2;
            this.mMap.TabStop = false;
            this.mMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mMap_MouseDown);
            this.mMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mMap_MouseMove);
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
            this.modListBox.Size = new System.Drawing.Size(225, 455);
            this.modListBox.TabIndex = 3;
            this.modListBox.SelectedValueChanged += new System.EventHandler(this.modListBox_SelectedValueChanged);
            // 
            // ButtonExport
            // 
            this.ButtonExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonExport.Location = new System.Drawing.Point(3, 464);
            this.ButtonExport.Name = "ButtonExport";
            this.ButtonExport.Size = new System.Drawing.Size(225, 44);
            this.ButtonExport.TabIndex = 0;
            this.ButtonExport.Text = ".fpn ausgeben";
            this.ButtonExport.UseVisualStyleBackColor = true;
            this.ButtonExport.Click += new System.EventHandler(this.ModulButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mMap);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 8, 8);
            this.splitContainer1.Size = new System.Drawing.Size(839, 519);
            this.splitContainer1.SplitterDistance = 247;
            this.splitContainer1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.modListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonExport, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 511);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.karteToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(839, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // karteToolStripMenuItem
            // 
            this.karteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moduToolStripMenuItem,
            this.mToolStripMenuItem,
            this.fahrstraenToolStripMenuItem});
            this.karteToolStripMenuItem.Name = "karteToolStripMenuItem";
            this.karteToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.karteToolStripMenuItem.Text = "Layer";
            // 
            // moduToolStripMenuItem
            // 
            this.moduToolStripMenuItem.Checked = true;
            this.moduToolStripMenuItem.CheckOnClick = true;
            this.moduToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.moduToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modulgrenzenToolStripMenuItem,
            this.namenToolStripMenuItem,
            this.verbindungenToolStripMenuItem,
            this.punkteToolStripMenuItem});
            this.moduToolStripMenuItem.Name = "moduToolStripMenuItem";
            this.moduToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.moduToolStripMenuItem.Text = "Module";
            this.moduToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // modulgrenzenToolStripMenuItem
            // 
            this.modulgrenzenToolStripMenuItem.CheckOnClick = true;
            this.modulgrenzenToolStripMenuItem.Name = "modulgrenzenToolStripMenuItem";
            this.modulgrenzenToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.modulgrenzenToolStripMenuItem.Text = "Modulgrenzen";
            this.modulgrenzenToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // namenToolStripMenuItem
            // 
            this.namenToolStripMenuItem.Checked = true;
            this.namenToolStripMenuItem.CheckOnClick = true;
            this.namenToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.namenToolStripMenuItem.Name = "namenToolStripMenuItem";
            this.namenToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.namenToolStripMenuItem.Text = "Namen";
            this.namenToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // verbindungenToolStripMenuItem
            // 
            this.verbindungenToolStripMenuItem.Checked = true;
            this.verbindungenToolStripMenuItem.CheckOnClick = true;
            this.verbindungenToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verbindungenToolStripMenuItem.Name = "verbindungenToolStripMenuItem";
            this.verbindungenToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.verbindungenToolStripMenuItem.Text = "Verbindungen";
            this.verbindungenToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // punkteToolStripMenuItem
            // 
            this.punkteToolStripMenuItem.Checked = true;
            this.punkteToolStripMenuItem.CheckOnClick = true;
            this.punkteToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.punkteToolStripMenuItem.Name = "punkteToolStripMenuItem";
            this.punkteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.punkteToolStripMenuItem.Text = "Punkte";
            this.punkteToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // mToolStripMenuItem
            // 
            this.mToolStripMenuItem.CheckOnClick = true;
            this.mToolStripMenuItem.Name = "mToolStripMenuItem";
            this.mToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.mToolStripMenuItem.Text = "Streckenplan";
            this.mToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // fahrstraenToolStripMenuItem
            // 
            this.fahrstraenToolStripMenuItem.CheckOnClick = true;
            this.fahrstraenToolStripMenuItem.Name = "fahrstraenToolStripMenuItem";
            this.fahrstraenToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.fahrstraenToolStripMenuItem.Text = "Fahrstraßen";
            this.fahrstraenToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ladezeitToolStripMenuItem,
            this.toolStripMenuItem1});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // ladezeitToolStripMenuItem
            // 
            this.ladezeitToolStripMenuItem.Enabled = false;
            this.ladezeitToolStripMenuItem.Name = "ladezeitToolStripMenuItem";
            this.ladezeitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ladezeitToolStripMenuItem.Text = "Ladezeit...";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Enabled = false;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem1.Text = "Framezeit...";
            // 
            // modSelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 543);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "modSelForm";
            this.Text = "ZuSi 3 Modulauswahl";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.modSelForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox mMap;
        private System.Windows.Forms.ListBox modListBox;
        private System.Windows.Forms.Button ButtonExport;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem karteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moduToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modulgrenzenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem namenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ladezeitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verbindungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem punkteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fahrstraenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    }
}

