using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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


        int debugX = 0;
        int debugY = 0;
        streckenModul horribleHackVariableThatHoldsRightClickModule;
        streckenModul.referenzElement tmpSignal;
        streckenModul.referenzElement startSignal;
        streckenModul.referenzElement zielSignal;

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
            debugX = mMap.Width;
            debugY = mMap.Height;
            kartenZeichner = new mapDraw(mMap.Width, mMap.Height, Module.mSammlung);
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

            if (e.Button == MouseButtons.Right) //HACK: Horrible Right Click Menu
            {
                var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                horribleHackVariableThatHoldsRightClickModule = nächsteStation;
                tmpSignal = kartenZeichner.getNearestSignal(e.X, e.Y);
                string Signame = "";
                if (tmpSignal.StrElement.SignalNorm != null)
                {
                    Signame = tmpSignal.StrElement.SignalNorm.Name;
                } else
                {
                    Signame = tmpSignal.StrElement.SignalGegen.Name;
                }

                //MessageBox.Show(horribleHackVariableThatHoldsRightClickSignal.Info, "Nächstes Signal:", MessageBoxButtons.OK);

                string startText = tmpSignal.Info + " als Startsignal festlegen";

                string zielText = "Route zu " + tmpSignal.Info;
                if (startSignal != null)
                {
                    zielText = "Route von " + startSignal.Info + " zu " + tmpSignal.Info;
                }

                

                    MenuItem[] menuItems = new MenuItem[]{new MenuItem("Pixel: X" + e.X + " - Y" + e.Y),
                new MenuItem("Koordinaten: X" + kartenZeichner.pixToCoord(e.X, false).ToString("F1") + " - Y" + kartenZeichner.pixToCoord(e.Y, true).ToString("F1")),
                new MenuItem("Nächste Station: " + nächsteStation.modName + "; Distanz: " + kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y).ToString()),
                new MenuItem("Nächstes Signal: " + tmpSignal.Info + "-" + Signame + "; Distanz: " + (1000 * kartenZeichner.getSigDistance(tmpSignal, e.X, e.Y)).ToString("F2")),
                new MenuItem(nächsteStation.modName + " im Explorer anzeigen", new EventHandler(showMod)),
                new MenuItem(startText, new EventHandler(setStartSig)),
                new MenuItem(zielText, new EventHandler(setZielSig))};
            
                ContextMenu buttonMenu = new ContextMenu(menuItems);
                buttonMenu.Show(mMap, new Point(e.X, e.Y));
            }
        }

        private void setStartSig(object sender, EventArgs e)
        {
            startSignal = tmpSignal;
        }

        private void setZielSig(object sender, EventArgs e)
        {
            zielSignal = tmpSignal;
            if( startSignal == null)
            {
                MessageBox.Show("Startsignal nicht gesetzt!", "Fehler", MessageBoxButtons.OK);
            }
            route();
        }

        private void showMod(object sender, EventArgs e)
        {
            Process.Start(Module.DirBase + horribleHackVariableThatHoldsRightClickModule.modPath.Substring(0, horribleHackVariableThatHoldsRightClickModule.modPath.LastIndexOf('\\'))); //HACK: HACKHACKHACKHACKHACK Shame!
        }

        private void route()
        {
            var fertige_route = routeSearch(horribleHackVariableThatHoldsRightClickModule, Module.sucheMod("Meschede_1980"), new List<streckenModul>());

            if (fertige_route == null)
            {
                MessageBox.Show("Strecke nicht gefunden.", "Strecke nach Meschede", MessageBoxButtons.OK);
            }
            else
            {
                string stregge = "";
                foreach (var mod in fertige_route)
                {
                    stregge += mod.modName + "\n";
                }
                MessageBox.Show(stregge, "Strecke nach Meschede", MessageBoxButtons.OK);
            }

            var fertige_fstr_route = fstrRouteSearchStart(startSignal, zielSignal, new List<streckenModul.fahrStr>());
            if (fertige_fstr_route == null)
            {
                MessageBox.Show("Fahrstraßen nicht gefunden.", "Strecke nach Meschede", MessageBoxButtons.OK);
            }
            else
            {
                double länge = 0;
                string stregge = "";
                foreach (var fstr in fertige_fstr_route)
                {
                    stregge += fstr.FahrstrName + "\n";
                    länge += fstr.Laenge;
                }
                stregge += "Länge: " + (länge/1000).ToString("F1") + "km";
                MessageBox.Show(stregge, "Fahrstraßen nach Meschede", MessageBoxButtons.OK);

                string path = "RB999.trn";
                var trn_file = new StreamWriter(path, false);

                trn_file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                trn_file.WriteLine("<Zusi>");
                trn_file.WriteLine("<Info DateiTyp=\"Zug\" Version=\"A.1\" MinVersion=\"A.1\">");
                trn_file.WriteLine("<AutorEintrag/>");
                trn_file.WriteLine("</Info>");
                trn_file.WriteLine("<Zug Gattung=\"RB\" Nummer=\"999\" Prio=\"1500\" Bremsstellung=\"4\" Rekursionstiefe=\"5\" FahrstrName=\"" + fertige_fstr_route[0].FahrstrName.Replace(">", "&gt;") + "\" Zugtyp=\"1\" Buchfahrplandll=\"_InstSetup\\lib\\timetable\\Buchfahrplan_DB_1979.dll\">");
                trn_file.WriteLine("<Datei Dateiname=\"Timetables\\Custom\\test.fpn\" NurInfo=\"1\"/>");

                for (int i = 0; i < fertige_fstr_route.Count; i++)
                {
                    streckenModul.Signal nextSignal = null;
                    if (fertige_fstr_route[i].Ziel.StrElement.SignalNorm != null)
                    {
                        nextSignal = fertige_fstr_route[i].Ziel.StrElement.SignalNorm;
                    } 
                    else
                    {
                        nextSignal = fertige_fstr_route[i].Ziel.StrElement.SignalGegen;
                    }

                    if (i == 0)
                    {
                        trn_file.WriteLine("<FahrplanEintrag Ank=\"2017-02-27 12:00:00\" Abf=\"2017-02-27 12:01:00\" Betrst=\"" + nextSignal.Betriebstelle + "\">");
                    }
                    else
                    {
                        trn_file.WriteLine("<FahrplanEintrag Betrst=\"" + nextSignal.Betriebstelle + "\">");
                    }
                    trn_file.WriteLine("<FahrplanSignalEintrag FahrplanSignal=\"" + nextSignal.Name + "\"/>");
                    trn_file.WriteLine("</FahrplanEintrag>");
                }

                trn_file.WriteLine("<FahrzeugVarianten Bezeichnung=\"default\" ZufallsWert=\"1\">");
                trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
                trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Dieseltriebwagen\\RegioShuttle\\RS1.rv.fzg\"/>");
                trn_file.WriteLine("</FahrzeugInfo>");
                trn_file.WriteLine("</FahrzeugVarianten>");
                trn_file.WriteLine("</Zug>");
                trn_file.WriteLine("</Zusi>");

                trn_file.Close();
            }
        }

        List<streckenModul.fahrStr> fstrRouteSearchStart(streckenModul.referenzElement StartSignal, streckenModul.referenzElement ZielSignal, List<streckenModul.fahrStr> Besucht)
        {
            foreach (var start_fstr in StartSignal.abgehendeFahrstraßen)
            {
                var rList = fstrRouteSearch(start_fstr, ZielSignal, Besucht);
                if (rList != null) return rList;
            }

            return null;
        }

        List<streckenModul.fahrStr> fstrRouteSearch(streckenModul.fahrStr Aktuell, streckenModul.referenzElement ZielSignal, List<streckenModul.fahrStr> Besucht)
        {
            Besucht.Add(Aktuell);
            if (Aktuell.Ziel.StrElement == ZielSignal.StrElement)
            {
                var Lizte = new List<streckenModul.fahrStr>();
                Lizte.Add(Aktuell);
                return (Lizte);
            }
            else
            {
                foreach (var folge in Aktuell.folgeStraßen)
                {
                    if (!(Besucht.Contains(folge)))
                    {
                        var rList = fstrRouteSearch(folge, ZielSignal, Besucht);
                        if (!(rList == null))
                        {
                            rList.Insert(0, Aktuell);
                            return rList;
                        } Besucht.Add(Aktuell);
                    }
                }
            }

            return null;
        }

        List<streckenModul> routeSearch(streckenModul Aktuell, streckenModul Ziel, List<streckenModul> Besucht)
        {
            Besucht.Add(Aktuell);
            if (Aktuell == Ziel)
            {
                var Lizte = new List<streckenModul>();
                Lizte.Add(Aktuell);
                return (Lizte);
            }
            else
            {
                foreach (var con in Aktuell.Verbindungen)
                {
                    if (!(Besucht.Contains(con)))
                    {
                        var rList = routeSearch(con, Ziel, Besucht); 
                        if (!(rList == null))
                        {
                            rList.Insert(0, Aktuell);
                            return rList;
                        }
                    }
                }
            }

            return null;
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
            kartenZeichner.updateMapSize(mMap.Width, mMap.Height);
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

                this.Invalidate();
                Application.DoEvents();//Zeichnen

                frameTime.Stop();
                toolStripMenuItem1.Text = frameTime.ElapsedMilliseconds + " ms";

                updatingMap = false;                
            }
        }
    }
}