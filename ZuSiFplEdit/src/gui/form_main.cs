using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace ZuSiFplEdit
{
    public delegate void SignalSelectEventHandler(string text);

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

        bool selectRouteStart = false;
        bool selectRouteEnd = false;

        bool ZLBready = true;
        List<ZugFahrt> ZugFahrten = new List<ZugFahrt>();
        ZugForm ZugKonfigForm;

        int debugX = 0;
        int debugY = 0;
        streckenModul horribleHackVariableThatHoldsRightClickModule;
        streckenModul.referenzElement tmpSignal;
        streckenModul.referenzElement startSignal;
        streckenModul.referenzElement zielSignal;
        List<streckenModul.fahrStr> fstrRoute;

        public modSelForm()
        {
            InitializeComponent();

            ZugKonfigForm = new ZugForm();
            ZugKonfigForm.Owner = this;
            ZugForm.signalSelectionEvent += new SignalSelectEventHandler(signalSelect);

            this.MouseWheel += new MouseEventHandler(mMap_MouseWheel);
            modListBox.SelectedIndexChanged += new EventHandler(modListBox_SelectedValueChanged);

#if DEBUG
            this.Text += "  -  DEBUG";
#endif

            appInit();
        }


        private void signalSelect(string signalSelectType)
        {
            if (signalSelectType == "start")
            {
                selectRouteStart = true;
                kartenZeichner.setLayers("signal_ziel", false);
            }
            if (signalSelectType == "ziel")
            {
                selectRouteEnd = true;
                kartenZeichner.setLayers("signal_start", false);
            }

            this.Invalidate();
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
                List<ZugFahrt> zList = new List<ZugFahrt>();
                foreach (var Zufa in ZugFahrtBox.Items)
                {
                    ZugFahrt Zug = (ZugFahrt)Zufa;
                    foreach (var fstr in Zug.route)
                    {
                        fstr.StartMod.selected = true;
                        fstr.ZielMod.selected = true;
                    }
                    zList.Add(Zug);
                }
                this.Invalidate();
                Application.DoEvents();
                new fileops(Module.mSammlung, zList, saveFileDialog1.FileName, Module.DirBase);
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
            kartenZeichner = new mapDraw(mMap.Width, mMap.Height, Module.mSammlung, ZugFahrtBox);
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

                if ((selectRouteStart || selectRouteEnd) && !mouseMoved)
                {
                    var act = (ZugFahrt)ZugFahrtBox.SelectedItem;
                    if (selectRouteStart)
                    {
                        ZugKonfigForm.setSignal(kartenZeichner.getNearestSignalStartZiel(e.X, e.Y, true));
                        kartenZeichner.setLayers("signal_ziel", true);
                        this.Invalidate();
                        selectRouteStart = false;
                    }
                    else
                    {
                        ZugKonfigForm.setSignal(kartenZeichner.getNearestSignalStartZiel(e.X, e.Y, false));
                        kartenZeichner.setLayers("signal_start", true); 
                        this.Invalidate();
                        selectRouteEnd = false;
                    }

                    if (act.ZstartSignal != null && act.ZzielSignal != null)
                    {
                        act.route = fstrRouteSearchStart(act.ZstartSignal, act.ZzielSignal);
                    }

                    updateZugFields();
                    mMap.Image = kartenZeichner.draw();
                    return;
                }

                int movementThreshold = 3;

                if ((Math.Abs(deltaX) > movementThreshold) || (Math.Abs(deltaY) > movementThreshold))
                {
                    kartenZeichner.move(deltaY, deltaX);
                    mMap.Image = kartenZeichner.draw();
                } else if (!mouseMoved) 
                {
                    if (moduToolStripMenuItem.Checked && punkteToolStripMenuItem.Checked)
                    {
                        var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                        if (kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y) < 10)
                        {
                            nächsteStation.selected = !nächsteStation.selected;
                            mMap.Image = kartenZeichner.draw();
                            modListBox.SetSelected(Module.mSammlung.IndexOf(nächsteStation), nächsteStation.selected);
                        }
                    }
                }

                mouseMoved = false;
            }

            if (e.Button == MouseButtons.Right) //HACK: Horrible Right Click Menu
            {
                var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                horribleHackVariableThatHoldsRightClickModule = nächsteStation;
                tmpSignal = kartenZeichner.getNearestSignal(e.X, e.Y);
                if (nächsteStation == null || tmpSignal == null)
                    return;

                string Signame = tmpSignal.Info;

                //MessageBox.Show(horribleHackVariableThatHoldsRightClickSignal.Info, "Nächstes Signal:", MessageBoxButtons.OK);
                
                

                    MenuItem[] menuItems = new MenuItem[]{new MenuItem("Pixel: X" + e.X + " - Y" + e.Y),
                new MenuItem("Koordinaten: X" + kartenZeichner.pixToCoord(e.X, false).ToString("F1") + " - Y" + kartenZeichner.pixToCoord(e.Y, true).ToString("F1")),
                new MenuItem("Nächste Station: " + nächsteStation.modName + "; Distanz: " + kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y).ToString()),
                new MenuItem("Nächstes Signal: " + tmpSignal.Info + "-" + Signame + "; Distanz: " + (1000 * kartenZeichner.getSigDistance(tmpSignal, e.X, e.Y)).ToString("F2")),
                new MenuItem(nächsteStation.modName + " im Explorer anzeigen", new EventHandler(showMod))};
            
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

            var fertige_fstr_route = fstrRouteSearchStart(startSignal, zielSignal);
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

                fstrRoute = fertige_fstr_route;
            }
        }

        List<streckenModul.fahrStr> fstrRouteSearchStart(streckenModul.referenzElement StartSignal, streckenModul.referenzElement ZielSignal)
        {
            var Besucht = new List<streckenModul.fahrStr>();
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
                foreach (var folge in Aktuell.folgestraßen)
                {
                    if ((!(Besucht.Contains(folge))) && (folge.Ziel != null))
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
            else if (sender == signalnamenToolStripMenuItem)
            {
                kartenZeichner.setLayers("signal_namen", signalnamenToolStripMenuItem.Checked);
            }
            else if (sender == fahrstraenToolStripMenuItem)
            {
                kartenZeichner.setLayers("fahrstr", fahrstraenToolStripMenuItem.Checked);
            }
            else if (sender == routeToolStripMenuItem)
            {
                kartenZeichner.setLayers("route", routeToolStripMenuItem.Checked);
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
                toolStripMenuItem1.Text = "Zeichendauer: " + frameTime.ElapsedMilliseconds + " ms";

                updatingMap = false;                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            var tmpZugfahrt = new ZugFahrt();

            tmpZugfahrt.Gattung = "RB";
            
            int ZugNummer = 0;
            bool ZugNummerBesetzt = true;
            while (ZugNummerBesetzt)
            {
                ZugNummer++;
                ZugNummerBesetzt = ZNBesetzt(ZugNummer);
            }
            tmpZugfahrt.Zugnummer = ZugNummer;

            ZugFahrtBox.Items.Add(tmpZugfahrt);
            ZugFahrtBox.SelectedItem = tmpZugfahrt;
        }
        

        bool ZNBesetzt(int ZN)
        {
            bool ZugNummerBesetzt = false;
            foreach (ZugFahrt zug in ZugFahrtBox.Items)
            {
                if (zug.Zugnummer == ZN)
                {
                    ZugNummerBesetzt = true;
                    break;
                }
            }
            return ZugNummerBesetzt;
        }

        private void textBox_Gattung_TextChanged(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            var act = (ZugFahrt)ZugFahrtBox.SelectedItem;
            act.Gattung = textBox_Gattung.Text;
            zlbUpdate();
        }

        void zlbUpdate()
        {
            ZLBready = false;
            ZugFahrtBox.Items[ZugFahrtBox.SelectedIndex] = ZugFahrtBox.SelectedItem;
            ZLBready = true;
        }

        private void textBox_ZNummer_TextChanged(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            textBox_ZNummer.BackColor = Color.White;

            int ZN = -1;
            try
            {
                ZN = Convert.ToInt32(textBox_ZNummer.Text);
            }
            catch (Exception)
            {
                textBox_ZNummer.BackColor = Color.Red;
                //HACK: Bei Backspace soll keine Fehlermeldung kommen.
                MessageBox.Show("Keine gültige Zugnummer", "Fehler", MessageBoxButtons.OK);
                return;
            }

            
            var act = (ZugFahrt)ZugFahrtBox.SelectedItem;
            if (ZNBesetzt(ZN))
            {
                if (ZN != act.Zugnummer)
                    textBox_ZNummer.BackColor = Color.Red;
            } 
            else
            {
                act.Zugnummer = ZN;

                zlbUpdate();
            }
        }

        private void ZugFahrtBox_SelectedValueChanged(object sender, EventArgs e)
        {
            updateZugFields();
        }

        private void updateZugFields()
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            if (ZLBready)
            {
                var act = (ZugFahrt)ZugFahrtBox.SelectedItem;

                textBox_Gattung.Text = act.Gattung;
                textBox_ZNummer.Text = act.Zugnummer.ToString();

                if (act.ZstartSignal == null)
                {
                    label_StartSig.Text = "Startsignal unbekannt";
                }
                else
                {
                    label_StartSig.Text = act.ZstartSignal.ToString();
                }

                if (act.ZzielSignal == null)
                {
                    label_ZielSig.Text = "Zielsignal unbekannt";
                }
                else
                {
                    label_ZielSig.Text = act.ZzielSignal.ToString();
                }

                if (act.route == null)
                {
                    if (act.ZstartSignal == null || act.ZzielSignal == null)
                    {
                        label_Fstr.Text = "Start- und/oder Zielsignal nicht gesetzt";
                    } 
                    else
                    {
                        label_Fstr.Text = "Fahrweg konnte nicht gefunden werden";
                    }
                }
                else
                {
                    double fstr_len = 0;
                    foreach (var fstr in act.route)
                    {
                        fstr_len += fstr.Laenge;
                    }
                    label_Fstr.Text = "Fahrweglänge ist " + (fstr_len/1000).ToString("F2") + " km";
                }
            }

            this.Invalidate();
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            selectRouteStart = true;
            label_StartSig.Text = "Startsignal auf Karte wählen";
            kartenZeichner.setLayers("signal_ziel", false); 
            this.Invalidate();
            //Startsignal wird bei MouseUp && MouseButton == Links gesetzt.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            selectRouteEnd = true;
            label_ZielSig.Text = "Zielsignal auf Karte wählen";
            kartenZeichner.setLayers("signal_start", false);
            this.Invalidate();
            //Zielsignal wird bei MouseUp && MouseButton == Links gesetzt.
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            int löschZug = ZugFahrtBox.SelectedIndex;
            if(ZugFahrtBox.Items.Count > 1)
            {
                if (löschZug == 0)
                {
                    ZugFahrtBox.SelectedIndex = 1;
                }
                else
                {
                    ZugFahrtBox.SelectedIndex = löschZug - 1;
                }
            }
            ZugFahrtBox.Items.RemoveAt(löschZug);

            updateZugFields();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;
            ZugKonfigForm.ZugFahrtLaden((ZugFahrt)ZugFahrtBox.SelectedItem);
            ZugKonfigForm.Show();
        }
    }
}