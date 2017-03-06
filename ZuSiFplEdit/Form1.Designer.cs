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
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label_StartSig = new System.Windows.Forms.Label();
            this.label_ZielSig = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Gattung = new System.Windows.Forms.TextBox();
            this.textBox_ZNummer = new System.Windows.Forms.TextBox();
            this.ZugFahrtBox = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label_Fstr = new System.Windows.Forms.Label();
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
            this.routeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signalnamenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button4 = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
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
            this.modListBox.Size = new System.Drawing.Size(239, 477);
            this.modListBox.TabIndex = 3;
            this.modListBox.SelectedValueChanged += new System.EventHandler(this.modListBox_SelectedValueChanged);
            // 
            // ButtonExport
            // 
            this.ButtonExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonExport.Location = new System.Drawing.Point(3, 486);
            this.ButtonExport.Name = "ButtonExport";
            this.ButtonExport.Size = new System.Drawing.Size(239, 44);
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
            this.tabPage1.Size = new System.Drawing.Size(251, 539);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(245, 533);
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
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.ZugFahrtBox, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label_Fstr, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.button3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button4, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(242, 515);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Controls.Add(this.button1, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.button2, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label_StartSig, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label_ZielSig, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 433);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(236, 59);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(160, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Auswahl";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(160, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "Auswahl";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label_StartSig
            // 
            this.label_StartSig.AutoSize = true;
            this.label_StartSig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_StartSig.Location = new System.Drawing.Point(3, 0);
            this.label_StartSig.Name = "label_StartSig";
            this.label_StartSig.Size = new System.Drawing.Size(151, 29);
            this.label_StartSig.TabIndex = 2;
            this.label_StartSig.Text = "Startsignal unbekannt";
            this.label_StartSig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_ZielSig
            // 
            this.label_ZielSig.AutoSize = true;
            this.label_ZielSig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ZielSig.Location = new System.Drawing.Point(3, 29);
            this.label_ZielSig.Name = "label_ZielSig";
            this.label_ZielSig.Size = new System.Drawing.Size(151, 30);
            this.label_ZielSig.TabIndex = 3;
            this.label_ZielSig.Text = "Zielsignal unbekannt";
            this.label_ZielSig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.textBox_Gattung, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBox_ZNummer, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 378);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(236, 49);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gattung";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(168, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Nummer";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_Gattung
            // 
            this.textBox_Gattung.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Gattung.Location = new System.Drawing.Point(3, 27);
            this.textBox_Gattung.Name = "textBox_Gattung";
            this.textBox_Gattung.Size = new System.Drawing.Size(159, 20);
            this.textBox_Gattung.TabIndex = 2;
            this.textBox_Gattung.TextChanged += new System.EventHandler(this.textBox_Gattung_TextChanged);
            // 
            // textBox_ZNummer
            // 
            this.textBox_ZNummer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_ZNummer.Location = new System.Drawing.Point(168, 27);
            this.textBox_ZNummer.Name = "textBox_ZNummer";
            this.textBox_ZNummer.Size = new System.Drawing.Size(65, 20);
            this.textBox_ZNummer.TabIndex = 3;
            this.textBox_ZNummer.TextChanged += new System.EventHandler(this.textBox_ZNummer_TextChanged);
            // 
            // ZugFahrtBox
            // 
            this.ZugFahrtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ZugFahrtBox.FormattingEnabled = true;
            this.ZugFahrtBox.Location = new System.Drawing.Point(3, 73);
            this.ZugFahrtBox.Name = "ZugFahrtBox";
            this.ZugFahrtBox.Size = new System.Drawing.Size(236, 299);
            this.ZugFahrtBox.TabIndex = 0;
            this.ZugFahrtBox.SelectedIndexChanged += new System.EventHandler(this.ZugFahrtBox_SelectedValueChanged);
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
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label_Fstr
            // 
            this.label_Fstr.AutoSize = true;
            this.label_Fstr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Fstr.Location = new System.Drawing.Point(3, 495);
            this.label_Fstr.Name = "label_Fstr";
            this.label_Fstr.Size = new System.Drawing.Size(236, 20);
            this.label_Fstr.TabIndex = 4;
            this.label_Fstr.Text = "Fahrweg konnte nicht gefunden werden";
            this.label_Fstr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.karteToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(926, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // karteToolStripMenuItem
            // 
            this.karteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moduToolStripMenuItem,
            this.mToolStripMenuItem,
            this.fahrstraenToolStripMenuItem,
            this.routeToolStripMenuItem});
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
            this.moduToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.mToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signalnamenToolStripMenuItem});
            this.mToolStripMenuItem.Name = "mToolStripMenuItem";
            this.mToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mToolStripMenuItem.Text = "Streckenplan";
            this.mToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // fahrstraenToolStripMenuItem
            // 
            this.fahrstraenToolStripMenuItem.CheckOnClick = true;
            this.fahrstraenToolStripMenuItem.Name = "fahrstraenToolStripMenuItem";
            this.fahrstraenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            // routeToolStripMenuItem
            // 
            this.routeToolStripMenuItem.Checked = true;
            this.routeToolStripMenuItem.CheckOnClick = true;
            this.routeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.routeToolStripMenuItem.Name = "routeToolStripMenuItem";
            this.routeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.routeToolStripMenuItem.Text = "Route";
            this.routeToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // signalnamenToolStripMenuItem
            // 
            this.signalnamenToolStripMenuItem.Checked = true;
            this.signalnamenToolStripMenuItem.CheckOnClick = true;
            this.signalnamenToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.signalnamenToolStripMenuItem.Name = "signalnamenToolStripMenuItem";
            this.signalnamenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.signalnamenToolStripMenuItem.Text = "Signalnamen";
            this.signalnamenToolStripMenuItem.Click += new System.EventHandler(this.LayerChange_Click);
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button4.Location = new System.Drawing.Point(3, 38);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(236, 29);
            this.button4.TabIndex = 5;
            this.button4.Text = "Zug löschen";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
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
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
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
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox ZugFahrtBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Gattung;
        private System.Windows.Forms.TextBox textBox_ZNummer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label_StartSig;
        private System.Windows.Forms.Label label_ZielSig;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label_Fstr;
        private System.Windows.Forms.ToolStripMenuItem signalnamenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routeToolStripMenuItem;
        private System.Windows.Forms.Button button4;
    }
}

