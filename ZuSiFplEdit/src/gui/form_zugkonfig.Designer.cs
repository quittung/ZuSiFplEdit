namespace ZuSiFplEdit
{
    partial class ZugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZugForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSetting = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.tbZugGattung = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbZugNummer = new System.Windows.Forms.TextBox();
            this.tbVmax = new System.Windows.Forms.TextBox();
            this.tpFahrzeug = new System.Windows.Forms.TabPage();
            this.tpFaPla = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_waypoint = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.LB_signal = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label_dauer = new System.Windows.Forms.Label();
            this.label_vMin = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_überschrift = new System.Windows.Forms.Label();
            this.label_fahrstraße = new System.Windows.Forms.Label();
            this.label_signal = new System.Windows.Forms.Label();
            this.label_vMax_überschrift = new System.Windows.Forms.Label();
            this.label_vMax = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tpSetting.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tpFaPla.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpSetting);
            this.tabControl1.Controls.Add(this.tpFahrzeug);
            this.tabControl1.Controls.Add(this.tpFaPla);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(651, 432);
            this.tabControl1.TabIndex = 0;
            // 
            // tpSetting
            // 
            this.tpSetting.Controls.Add(this.tableLayoutPanel3);
            this.tpSetting.Location = new System.Drawing.Point(4, 22);
            this.tpSetting.Name = "tpSetting";
            this.tpSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tpSetting.Size = new System.Drawing.Size(643, 406);
            this.tpSetting.TabIndex = 0;
            this.tpSetting.Text = "Grundeinstellungen";
            this.tpSetting.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.tbZugGattung, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tbZugNummer, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.tbVmax, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(637, 400);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Höchstgeschwindigkeit (km/h)";
            // 
            // tbZugGattung
            // 
            this.tbZugGattung.Location = new System.Drawing.Point(321, 3);
            this.tbZugGattung.Name = "tbZugGattung";
            this.tbZugGattung.Size = new System.Drawing.Size(88, 20);
            this.tbZugGattung.TabIndex = 3;
            this.tbZugGattung.Text = "###";
            this.tbZugGattung.TextChanged += new System.EventHandler(this.tbZugGattung_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Gattung";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nummer";
            // 
            // tbZugNummer
            // 
            this.tbZugNummer.Location = new System.Drawing.Point(321, 33);
            this.tbZugNummer.Name = "tbZugNummer";
            this.tbZugNummer.Size = new System.Drawing.Size(88, 20);
            this.tbZugNummer.TabIndex = 0;
            this.tbZugNummer.Text = "###";
            this.tbZugNummer.TextChanged += new System.EventHandler(this.tbZugNummer_TextChanged);
            // 
            // tbVmax
            // 
            this.tbVmax.Location = new System.Drawing.Point(321, 63);
            this.tbVmax.Name = "tbVmax";
            this.tbVmax.Size = new System.Drawing.Size(88, 20);
            this.tbVmax.TabIndex = 4;
            this.tbVmax.Text = "###";
            this.tbVmax.TextChanged += new System.EventHandler(this.tbVmax_TextChanged);
            // 
            // tpFahrzeug
            // 
            this.tpFahrzeug.Location = new System.Drawing.Point(4, 22);
            this.tpFahrzeug.Name = "tpFahrzeug";
            this.tpFahrzeug.Padding = new System.Windows.Forms.Padding(3);
            this.tpFahrzeug.Size = new System.Drawing.Size(643, 406);
            this.tpFahrzeug.TabIndex = 1;
            this.tpFahrzeug.Text = "Fahrzeuge";
            this.tpFahrzeug.UseVisualStyleBackColor = true;
            // 
            // tpFaPla
            // 
            this.tpFaPla.Controls.Add(this.tableLayoutPanel1);
            this.tpFaPla.Location = new System.Drawing.Point(4, 22);
            this.tpFaPla.Name = "tpFaPla";
            this.tpFaPla.Padding = new System.Windows.Forms.Padding(3);
            this.tpFaPla.Size = new System.Drawing.Size(643, 406);
            this.tpFaPla.TabIndex = 3;
            this.tpFaPla.Text = "Fahrplan";
            this.tpFaPla.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.63523F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.36477F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 217F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LB_signal, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(637, 400);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.LB_waypoint, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.button3, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(198, 394);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // LB_waypoint
            // 
            this.LB_waypoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_waypoint.FormattingEnabled = true;
            this.LB_waypoint.Location = new System.Drawing.Point(3, 108);
            this.LB_waypoint.Name = "LB_waypoint";
            this.LB_waypoint.Size = new System.Drawing.Size(192, 283);
            this.LB_waypoint.TabIndex = 0;
            this.LB_waypoint.DragDrop += new System.Windows.Forms.DragEventHandler(this.LB_waypoint_DragDrop);
            this.LB_waypoint.DragOver += new System.Windows.Forms.DragEventHandler(this.LB_waypoint_DragOver);
            this.LB_waypoint.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LB_waypoint_MouseDown);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(192, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Wegpunkt einfügen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.punktEinfügen_Click);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(3, 38);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 29);
            this.button2.TabIndex = 1;
            this.button2.Text = "Wegpunkt ändern";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.punktBearbeiten_Click);
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button3.Location = new System.Drawing.Point(3, 73);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(192, 29);
            this.button3.TabIndex = 2;
            this.button3.Text = "Wegpunkt löschen";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.punktLöschen_Click);
            // 
            // LB_signal
            // 
            this.LB_signal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_signal.FormattingEnabled = true;
            this.LB_signal.Location = new System.Drawing.Point(207, 3);
            this.LB_signal.Name = "LB_signal";
            this.LB_signal.Size = new System.Drawing.Size(209, 394);
            this.LB_signal.TabIndex = 1;
            this.LB_signal.SelectedIndexChanged += new System.EventHandler(this.LB_signal_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_vMax_überschrift);
            this.panel1.Controls.Add(this.label_vMax);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label_dauer);
            this.panel1.Controls.Add(this.label_vMin);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label_überschrift);
            this.panel1.Controls.Add(this.label_fahrstraße);
            this.panel1.Controls.Add(this.label_signal);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(422, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(212, 394);
            this.panel1.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(87, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "vStart:";
            // 
            // label_dauer
            // 
            this.label_dauer.AutoSize = true;
            this.label_dauer.Location = new System.Drawing.Point(4, 86);
            this.label_dauer.Name = "label_dauer";
            this.label_dauer.Size = new System.Drawing.Size(55, 13);
            this.label_dauer.TabIndex = 6;
            this.label_dauer.Text = "Fahrdauer";
            // 
            // label_vMin
            // 
            this.label_vMin.AutoSize = true;
            this.label_vMin.Location = new System.Drawing.Point(87, 86);
            this.label_vMin.Name = "label_vMin";
            this.label_vMin.Size = new System.Drawing.Size(35, 13);
            this.label_vMin.TabIndex = 5;
            this.label_vMin.Text = "vStart";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Fahrdauer:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Fahrstraße:";
            // 
            // label_überschrift
            // 
            this.label_überschrift.AutoSize = true;
            this.label_überschrift.Location = new System.Drawing.Point(4, 3);
            this.label_überschrift.Name = "label_überschrift";
            this.label_überschrift.Size = new System.Drawing.Size(87, 13);
            this.label_überschrift.TabIndex = 2;
            this.label_überschrift.Text = "Signal-Parameter";
            // 
            // label_fahrstraße
            // 
            this.label_fahrstraße.AutoSize = true;
            this.label_fahrstraße.Location = new System.Drawing.Point(14, 51);
            this.label_fahrstraße.Name = "label_fahrstraße";
            this.label_fahrstraße.Size = new System.Drawing.Size(89, 13);
            this.label_fahrstraße.TabIndex = 1;
            this.label_fahrstraße.Text = "Fahrstraßenname";
            // 
            // label_signal
            // 
            this.label_signal.AutoSize = true;
            this.label_signal.Location = new System.Drawing.Point(14, 16);
            this.label_signal.Name = "label_signal";
            this.label_signal.Size = new System.Drawing.Size(62, 13);
            this.label_signal.TabIndex = 0;
            this.label_signal.Text = "Signalname";
            // 
            // label_vMax_überschrift
            // 
            this.label_vMax_überschrift.AutoSize = true;
            this.label_vMax_überschrift.Location = new System.Drawing.Point(156, 73);
            this.label_vMax_überschrift.Name = "label_vMax_überschrift";
            this.label_vMax_überschrift.Size = new System.Drawing.Size(33, 13);
            this.label_vMax_überschrift.TabIndex = 9;
            this.label_vMax_überschrift.Text = "vZiel:";
            // 
            // label_vMax
            // 
            this.label_vMax.AutoSize = true;
            this.label_vMax.Location = new System.Drawing.Point(156, 86);
            this.label_vMax.Name = "label_vMax";
            this.label_vMax.Size = new System.Drawing.Size(30, 13);
            this.label_vMax.TabIndex = 8;
            this.label_vMax.Text = "vZiel";
            // 
            // ZugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 432);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ZugForm";
            this.Text = "Zugkonfiguration";
            this.tabControl1.ResumeLayout(false);
            this.tpSetting.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tpFaPla.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSetting;
        private System.Windows.Forms.TabPage tpFahrzeug;
        private System.Windows.Forms.TabPage tpFaPla;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox LB_waypoint;
        private System.Windows.Forms.ListBox LB_signal;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox tbZugNummer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbVmax;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbZugGattung;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_überschrift;
        private System.Windows.Forms.Label label_fahrstraße;
        private System.Windows.Forms.Label label_signal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_dauer;
        private System.Windows.Forms.Label label_vMin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_vMax_überschrift;
        private System.Windows.Forms.Label label_vMax;
    }
}