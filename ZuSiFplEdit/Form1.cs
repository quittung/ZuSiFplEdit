﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace ZuSiFplEdit
{
    public partial class modSelForm : Form
    {
        modContainer Module;
        mapDraw kartenZeichner;

        int mouseDownX_rel = 0;
        int mouseDownY_rel = 0;
        int mouseDownX_abs = 0;
        int mouseDownY_abs = 0;
        bool mouseDown = false;
        bool mouseMoved = false;
        bool updatingMap = false;

        streckenModul horribleHackVariableThatHoldsRightClickModule;

        public modSelForm()
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(mMap_MouseWheel);
            modListBox.SelectedIndexChanged += new EventHandler(modListBox_SelectedValueChanged);

            appInit();
        }

        private void mMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                kartenZeichner.updateScale(1.25);
            if (e.Delta < 0)
                kartenZeichner.updateScale(0.8);

            mMap.Image = kartenZeichner.draw();
        }

        private void ModulButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = Module.DirBase + "Timetables\\";
            saveFileDialog1.Filter = "Fahrplandateien (*.fpn)|*.fpn|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Module.writeToFile(saveFileDialog1.FileName);
            }
        }    

        private void appInit()
        {
            //Module einlesen
            Module = new modContainer();
            ladezeitToolStripMenuItem.Text = "Ladezeit: " + Module.loadTime + " ms";
            //Module ausgeben
            foreach (streckenModul modul in Module.mSammlung)
            {
                modListBox.Items.Add(modul.modName);
            }

            //Modulekarte vorbereiten
            kartenZeichner = new mapDraw(mMap.CreateGraphics(), mMap.Width, mMap.Height, Module.mSammlung);
        }

        private void mMap_MouseDown(object sender, MouseEventArgs e)
        {
            mMap.Focus();

            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                mouseDownX_rel = e.X;
                mouseDownY_rel = e.Y;
                mouseDownX_abs = e.X;
                mouseDownY_abs = e.Y;
            }

            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void mMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown && (e.Button == MouseButtons.Left))
            {
                mouseDown = false;
                int deltaX = e.X - mouseDownX_rel;
                int deltaY = e.Y - mouseDownY_rel;


                int movementThreshold = 3;

                if ((Math.Abs(deltaX) > movementThreshold) || (Math.Abs(deltaY) > movementThreshold))
                {
                    kartenZeichner.move(deltaY, deltaX);
                    mMap.Image = kartenZeichner.draw();
                } else if (!mouseMoved) 
                {
                    var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                    if (kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y) < 10)
                    {
                        nächsteStation.selected = !nächsteStation.selected;
                        mMap.Image = kartenZeichner.draw();
                        modListBox.SetSelected(Module.mSammlung.IndexOf(nächsteStation), nächsteStation.selected);
                    }
                }

                mouseMoved = false;
            }

            if (e.Button == MouseButtons.Right)
            {
                var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                horribleHackVariableThatHoldsRightClickModule = nächsteStation;

                MenuItem[] menuItems = new MenuItem[]{new MenuItem("Pixel: X" + e.X + " - Y" + e.Y),
                new MenuItem("Koordinaten: X" + kartenZeichner.pixToCoord(e.X, false).ToString("F1") + " - Y" + kartenZeichner.pixToCoord(e.Y, true).ToString("F1")),
                new MenuItem("Nächste Station: " + nächsteStation.modName + "; Distanz: " + kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y).ToString()),
                new MenuItem(nächsteStation.modName + " im Explorer anzeigen", new EventHandler(showMod))};

                ContextMenu buttonMenu = new ContextMenu(menuItems);
                buttonMenu.Show(mMap, new Point(e.X, e.Y));
            }
        }

        private void showMod(object sender, EventArgs e)
        {
            Process.Start(Module.DirBase + horribleHackVariableThatHoldsRightClickModule.modPath.Substring(0, horribleHackVariableThatHoldsRightClickModule.modPath.LastIndexOf('\\'))); //HACK: HACKHACKHACKHACKHACK Shame!
        }

        private void modListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Module.mSammlung.Count; i++)
            {
                if (modListBox.SelectedIndices.Contains(i))
                {
                    Module.mSammlung[i].selected = true;
                } else
                {
                    Module.mSammlung[i].selected = false;
                }
            }
            mMap.Image = kartenZeichner.draw();
        }

        private void modSelForm_Paint(object sender, PaintEventArgs e)
        {
            Application.DoEvents();
            mMap.Image = kartenZeichner.draw();
        }

        private void mMap_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
            kartenZeichner.updateMapSize(mMap.CreateGraphics(), mMap.Width, mMap.Height);
            mMap.Image = kartenZeichner.draw();
        }

        private void LayerChange_Click(object sender, EventArgs e)
        {
            if (sender == moduToolStripMenuItem)
            {
                kartenZeichner.setLayers("module", moduToolStripMenuItem.Checked);
            }
            else if (sender == punkteToolStripMenuItem)
            {
                kartenZeichner.setLayers("module_punkte", punkteToolStripMenuItem.Checked);
            }
            else if (sender == namenToolStripMenuItem)
            {
                kartenZeichner.setLayers("module_namen", namenToolStripMenuItem.Checked);
            }
            else if (sender == verbindungenToolStripMenuItem)
            {
                kartenZeichner.setLayers("module_verbindungen", verbindungenToolStripMenuItem.Checked);
            }
            else if (sender == modulgrenzenToolStripMenuItem)
            {
                kartenZeichner.setLayers("module_grenzen", modulgrenzenToolStripMenuItem.Checked);
            }
            else if (sender == mToolStripMenuItem)
            {
                kartenZeichner.setLayers("strecke", mToolStripMenuItem.Checked);
            }
            else if (sender == fahrstraenToolStripMenuItem)
            {
                kartenZeichner.setLayers("fahrstr", fahrstraenToolStripMenuItem.Checked);
            }

            mMap.Image = kartenZeichner.draw();
        }

        private void mMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && !(updatingMap))
            {
                updatingMap = true;

                int movement = Math.Abs(e.X - mouseDownX_abs) + Math.Abs(e.Y - mouseDownY_abs);
                if (movement > 3) mouseMoved = true;

                int stepsize = Math.Abs(e.X - mouseDownX_rel) + Math.Abs(e.Y - mouseDownY_rel);
                if (stepsize > 50)
                {
                    
                }

                int deltaX = e.X - mouseDownX_rel;
                int deltaY = e.Y - mouseDownY_rel;
                mouseDownX_rel = e.X;
                mouseDownY_rel = e.Y;

                var frameTime = new System.Diagnostics.Stopwatch();
                frameTime.Start();
                kartenZeichner.move(deltaY, deltaX);

                //this.Invalidate();
                //Application.DoEvents();
                mMap.Image = kartenZeichner.draw();

                frameTime.Stop();
                toolStripMenuItem1.Text = frameTime.ElapsedMilliseconds + " ms";

                updatingMap = false;                
            }
        }
    }
}