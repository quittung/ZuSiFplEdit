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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button3 = new System.Windows.Forms.Button();
            this.ZF_bearbeiten = new System.Windows.Forms.Button();
            this.ZugFahrtBox = new System.Windows.Forms.ListBox();
            this.button4 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speichernUnterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.karteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsModule = new System.Windows.Forms.ToolStripMenuItem();
            this.tsModule_Grenzen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsModule_Namen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsModule_Verbindungen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsModule_Punkte = new System.Windows.Forms.ToolStripMenuItem();
            this.tsStrecke = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSignalnamen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFahrstraßen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsRoute = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ladezeitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ladenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.mMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMap
            // 
            this.mMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMap.Location = new System.Drawing.Point(3, 1);
            this.mMap.Name = "mMap";
            this.mMap.Size = new System.Drawing.Size(639, 546);
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
            this.modListBox.Size = new System.Drawing.Size(236, 459);
            this.modListBox.TabIndex = 3;
            this.modListBox.SelectedValueChanged += new System.EventHandler(this.modListBox_SelectedValueChanged);
            // 
            // ButtonExport
            // 
            this.ButtonExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonExport.Location = new System.Drawing.Point(3, 468);
            this.ButtonExport.Name = "ButtonExport";
            this.ButtonExport.Size = new System.Drawing.Size(236, 44);
            this.ButtonExport.TabIndex = 0;
            this.ButtonExport.Text = ".fpn ausgeben";
            this.ButtonExport.UseVisualStyleBackColor = true;
            this.ButtonExport.Click += new System.EventHandler(this.fahrplanAusgeben);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mMap);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3, 1, 8, 8);
            this.splitContainer1.Size = new System.Drawing.Size(926, 555);
            this.splitContainer1.SplitterDistance = 272;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(8, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(256, 547);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(248, 521);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Module";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.modListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonExport, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(242, 515);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(248, 521);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Züge";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.button3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ZF_bearbeiten, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.ZugFahrtBox, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.button4, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 409F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(242, 515);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button3.Location = new System.Drawing.Point(3, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(236, 29);
            this.button3.TabIndex = 3;
            this.button3.Text = "Neuer Zug";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Neuer_Zug_button_Click);
            // 
            // ZF_bearbeiten
            // 
            this.ZF_bearbeiten.Location = new System.Drawing.Point(3, 38);
            this.ZF_bearbeiten.Name = "ZF_bearbeiten";
            this.ZF_bearbeiten.Size = new System.Drawing.Size(236, 29);
            this.ZF_bearbeiten.TabIndex = 6;
            this.ZF_bearbeiten.Text = "Zugfahrt bearbeiten";
            this.ZF_bearbeiten.UseVisualStyleBackColor = true;
            this.ZF_bearbeiten.Click += new System.EventHandler(this.zugBearbeite_click);
            // 
            // ZugFahrtBox
            // 
            this.ZugFahrtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZugFahrtBox.FormattingEnabled = true;
            this.ZugFahrtBox.Location = new System.Drawing.Point(3, 109);
            this.ZugFahrtBox.Name = "ZugFahrtBox";
            this.ZugFahrtBox.Size = new System.Drawing.Size(236, 403);
            this.ZugFahrtBox.TabIndex = 0;
            this.ZugFahrtBox.SelectedIndexChanged += new System.EventHandler(this.ZugFahrtBox_SelectedValueChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(3, 73);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(236, 29);
            this.button4.TabIndex = 5;
            this.button4.Text = "Zug löschen";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.zugLöschen_click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.karteToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(926, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.speichernUnterToolStripMenuItem,
            this.ladenToolStripMenuItem});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.dateiToolStripMenuItem.Text = "Datei";
            // 
            // speichernUnterToolStripMenuItem
            // 
            this.speichernUnterToolStripMenuItem.Name = "speichernUnterToolStripMenuItem";
            this.speichernUnterToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.speichernUnterToolStripMenuItem.Text = "Speichern unter...";
            this.speichernUnterToolStripMenuItem.Click += new System.EventHandler(this.fahrplanAusgeben);
            // 
            // karteToolStripMenuItem
            // 
            this.karteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsModule,
            this.tsStrecke,
            this.tsFahrstraßen,
            this.tsRoute});
            this.karteToolStripMenuItem.Name = "karteToolStripMenuItem";
            this.karteToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.karteToolStripMenuItem.Text = "Layer";
            // 
            // moduToolStripMenuItem
            // 
            this.tsModule.Checked = true;
            this.tsModule.CheckOnClick = true;
            this.tsModule.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsModule.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsModule_Grenzen,
            this.tsModule_Namen,
            this.tsModule_Verbindungen,
            this.tsModule_Punkte});
            this.tsModule.Name = "moduToolStripMenuItem";
            this.tsModule.Size = new System.Drawing.Size(152, 22);
            this.tsModule.Text = "Module";
            this.tsModule.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // modulgrenzenToolStripMenuItem
            // 
            this.tsModule_Grenzen.CheckOnClick = true;
            this.tsModule_Grenzen.Name = "modulgrenzenToolStripMenuItem";
            this.tsModule_Grenzen.Size = new System.Drawing.Size(151, 22);
            this.tsModule_Grenzen.Text = "Modulgrenzen";
            this.tsModule_Grenzen.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // namenToolStripMenuItem
            // 
            this.tsModule_Namen.Checked = true;
            this.tsModule_Namen.CheckOnClick = true;
            this.tsModule_Namen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsModule_Namen.Name = "namenToolStripMenuItem";
            this.tsModule_Namen.Size = new System.Drawing.Size(151, 22);
            this.tsModule_Namen.Text = "Namen";
            this.tsModule_Namen.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // verbindungenToolStripMenuItem
            // 
            this.tsModule_Verbindungen.Checked = true;
            this.tsModule_Verbindungen.CheckOnClick = true;
            this.tsModule_Verbindungen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsModule_Verbindungen.Name = "verbindungenToolStripMenuItem";
            this.tsModule_Verbindungen.Size = new System.Drawing.Size(151, 22);
            this.tsModule_Verbindungen.Text = "Verbindungen";
            this.tsModule_Verbindungen.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // punkteToolStripMenuItem
            // 
            this.tsModule_Punkte.Checked = true;
            this.tsModule_Punkte.CheckOnClick = true;
            this.tsModule_Punkte.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsModule_Punkte.Name = "punkteToolStripMenuItem";
            this.tsModule_Punkte.Size = new System.Drawing.Size(151, 22);
            this.tsModule_Punkte.Text = "Punkte";
            this.tsModule_Punkte.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // mToolStripMenuItem
            // 
            this.tsStrecke.CheckOnClick = true;
            this.tsStrecke.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsSignalnamen});
            this.tsStrecke.Name = "mToolStripMenuItem";
            this.tsStrecke.Size = new System.Drawing.Size(152, 22);
            this.tsStrecke.Text = "Streckenplan";
            this.tsStrecke.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // signalnamenToolStripMenuItem
            // 
            this.tsSignalnamen.CheckOnClick = true;
            this.tsSignalnamen.Name = "signalnamenToolStripMenuItem";
            this.tsSignalnamen.Size = new System.Drawing.Size(143, 22);
            this.tsSignalnamen.Text = "Signalnamen";
            this.tsSignalnamen.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // fahrstraenToolStripMenuItem
            // 
            this.tsFahrstraßen.CheckOnClick = true;
            this.tsFahrstraßen.Name = "fahrstraenToolStripMenuItem";
            this.tsFahrstraßen.Size = new System.Drawing.Size(152, 22);
            this.tsFahrstraßen.Text = "Fahrstraßen";
            this.tsFahrstraßen.Click += new System.EventHandler(this.kartenLayerÄndern);
            // 
            // routeToolStripMenuItem
            // 
            this.tsRoute.Checked = true;
            this.tsRoute.CheckOnClick = true;
            this.tsRoute.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRoute.Name = "routeToolStripMenuItem";
            this.tsRoute.Size = new System.Drawing.Size(152, 22);
            this.tsRoute.Text = "Route";
            this.tsRoute.Click += new System.EventHandler(this.kartenLayerÄndern);
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
            this.ladezeitToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.ladezeitToolStripMenuItem.Text = "Ladezeit...";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Enabled = false;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 22);
            this.toolStripMenuItem1.Text = "Framezeit...";
            // 
            // ladenToolStripMenuItem
            // 
            this.ladenToolStripMenuItem.Name = "ladenToolStripMenuItem";
            this.ladenToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.ladenToolStripMenuItem.Text = "Laden";
            this.ladenToolStripMenuItem.Click += new System.EventHandler(this.ladenToolStripMenuItem_Click);
            // 
            // modSelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 579);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem tsModule;
        private System.Windows.Forms.ToolStripMenuItem tsModule_Grenzen;
        private System.Windows.Forms.ToolStripMenuItem tsModule_Namen;
        private System.Windows.Forms.ToolStripMenuItem tsStrecke;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ladezeitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsModule_Verbindungen;
        private System.Windows.Forms.ToolStripMenuItem tsModule_Punkte;
        private System.Windows.Forms.ToolStripMenuItem tsFahrstraßen;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox ZugFahrtBox;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem tsSignalnamen;
        private System.Windows.Forms.ToolStripMenuItem tsRoute;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speichernUnterToolStripMenuItem;
        private System.Windows.Forms.Button ZF_bearbeiten;
        private System.Windows.Forms.ToolStripMenuItem ladenToolStripMenuItem;
    }
}

