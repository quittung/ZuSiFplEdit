using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace ZuSiFplEdit
{
    public delegate void SignalSelectEventHandler(string text);

    public partial class modSelForm : Form
    {

        DataConstructor dataConstructor;
        Datensatz datenFertig;
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
        st3Modul horribleHackVariableThatHoldsRightClickModule;
        st3Modul.referenzElement tmpSignal;
        st3Modul.referenzElement startSignal;
        st3Modul.referenzElement zielSignal;
        List<st3Modul.fahrStr> fstrRoute;

        string DirBase;

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

            var ladeAnzeige = new form_lade();
            ladeAnzeige.Show();
            ladeAnzeige.Text = "Suche Datenverzeichnis...";
            ladeAnzeige.Update();

            datenFertig = new Datensatz();

            FindeDatenVerzeichnis();

            string DirRoute = DirBase + "Routes\\Deutschland\\";
            dataConstructor = new DataConstructor(DirRoute, datenFertig);

            ThreadPool.QueueUserWorkItem(dataConstructor.datenEinlesen);

            while (!datenFertig.moduleBereit)
            {
                ladeAnzeige.instantProgress(dataConstructor.fortschrittNumerisch, dataConstructor.fortschrittMaximal, dataConstructor.fortschrittMeldung);
                Thread.Sleep(100);
                Application.DoEvents();
            }

            ladeAnzeige.Hide();
            ladeAnzeige = null;

            //Modulekarte vorbereiten
            debugX = mMap.Width;
            debugY = mMap.Height;
            kartenZeichner = new mapDraw(mMap.Width, mMap.Height, datenFertig.module, ZugFahrtBox);

            //Module ausgeben
            foreach (streckenModul modul in datenFertig.module)
            {
                modListBox.Items.Add(modul.name);
            }
        }

        /// <summary>
        /// Started den DatenKonstruktor im Hintergrund und wartet auf das vollständige Einlesen der Module.
        /// </summary>
        private void appInit2()
        {
            
        }


        /// <summary>
        /// Liest das Datenverzeichnis aus der Registry ein
        /// </summary>
        private void FindeDatenVerzeichnis()
        {
            //TODO: Suche nach typischen Verzeichnissen für Datenverzeichnis im aktuellen Verzeichnis. Wenn gefunden, nutze diese.
            var lokaleOrdner = Directory.GetDirectories(Directory.GetCurrentDirectory());
            foreach (var Ordner in lokaleOrdner)
            {
                if (Ordner == "Routes")
                {
                    DirBase = Directory.GetCurrentDirectory();
                    return;
                }
            }

            try
            {
                //Registry-Key auf win64
                DirBase = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Zusi3", "DatenVerzeichnis", "").ToString();
            }
            catch (Exception)
            {
                try
                {
                    //Registry-Key auf win32
                    DirBase = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3", "DatenVerzeichnis", "").ToString();
                }
                catch (Exception)
                {   
                    MessageBox.Show("Datenverzeichnis konnte nicht in der Registry gefunden werden. Starte die Applikation alternativ direkt im Zusi-Datenordner.", "Fataler Fehler", MessageBoxButtons.OK);
                    Environment.Exit(1);
                }
            }

            if (DirBase[DirBase.Length - 1] != '\\') DirBase += '\\';
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

        /// <summary>
        /// Verarbeitet das Bewegen des Mausrades
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                kartenZeichner.updateScale(1.25);
            if (e.Delta < 0)
                kartenZeichner.updateScale(0.8);

            kartenZeichner.message = dataConstructor.fortschrittMeldung;
            mMap.Image = kartenZeichner.draw();
        }

        private void ModulButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = DirBase + "Timetables\\";
            saveFileDialog1.Filter = "Fahrplandateien (*.fpn)|*.fpn|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<ZugFahrt> zList = new List<ZugFahrt>();
                foreach (var Zufa in ZugFahrtBox.Items)
                {
                    ZugFahrt Zug = (ZugFahrt)Zufa;
                    foreach (var routenPunkt in Zug.route) //TODO: Auch alle Module dazwischen markieren
                    {
                        routenPunkt.fahrstraße.startSignal.modul.selected = true;
                        foreach (var nachbar in routenPunkt.fahrstraße.startSignal.modul.nachbarn)
                        {
                            nachbar.selected = true;
                        }

                        routenPunkt.fahrstraße.zielSignal.modul.selected = true;
                        foreach (var nachbar in routenPunkt.fahrstraße.zielSignal.modul.nachbarn)
                        {
                            nachbar.selected = true;
                        }
                    }
                    zList.Add(Zug);
                }
                this.Invalidate();
                Application.DoEvents();
                new fileops(datenFertig.module, zList, saveFileDialog1.FileName, DirBase);
            }
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
                    if (selectRouteStart)
                    {
                        ZugKonfigForm.setSignal(kartenZeichner.findeNächstesSignal(new mapDraw.PunktPix(e.X, e.Y), 1));
                        kartenZeichner.setLayers("signal_ziel", true);
                        this.Invalidate();
                        selectRouteStart = false;
                    }
                    else
                    {
                        ZugKonfigForm.setSignal(kartenZeichner.findeNächstesSignal(new mapDraw.PunktPix(e.X, e.Y), 2));
                        kartenZeichner.setLayers("signal_start", true); 
                        this.Invalidate();
                        selectRouteEnd = false;
                    }

                    kartenZeichner.message = dataConstructor.fortschrittMeldung;
                    mMap.Image = kartenZeichner.draw();
                    return;
                }

                int movementThreshold = 3;

                if ((Math.Abs(deltaX) > movementThreshold) || (Math.Abs(deltaY) > movementThreshold))
                {
                    kartenZeichner.move(deltaY, deltaX);
                    kartenZeichner.message = dataConstructor.fortschrittMeldung;
                    mMap.Image = kartenZeichner.draw();
                } else if (!mouseMoved) 
                {
                    if (moduToolStripMenuItem.Checked && punkteToolStripMenuItem.Checked)
                    {
                        var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                        if (kartenZeichner.getModulDistance(nächsteStation, e.X, e.Y) < 10)
                        {
                            nächsteStation.selected = !nächsteStation.selected;
                            kartenZeichner.message = dataConstructor.fortschrittMeldung;
                            mMap.Image = kartenZeichner.draw();
                            //modListBox.SetSelected(Module.mSammlung.IndexOf(nächsteStation), nächsteStation.selected);
                        }
                    }
                }

                mouseMoved = false;
            }

            //Debug-Menü
            if (e.Button == MouseButtons.Right) 
            {
                var punktPix = new mapDraw.PunktPix(e.X, e.Y);
                var punktUTM = kartenZeichner.PixToUtm(punktPix);

                var tmpSignal = kartenZeichner.findeNächstesSignal(new mapDraw.PunktPix(e.X, e.Y), 0);
                if (tmpSignal == null)
                    return;
                
                MenuItem[] menuItems = new MenuItem[]{
                    new MenuItem("Pixel: X" + punktPix.X + " - Y" + punktPix.Y),
                    new MenuItem("Koordinaten: X" + punktUTM.WE.ToString("F1") + " - Y" + punktUTM.NS.ToString("F1")),
                    
                    new MenuItem("Nächstes Signal: " + tmpSignal.name)
                };

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
            var fertige_route = routeSearch(horribleHackVariableThatHoldsRightClickModule, Module.sucheMod("Meschede_1980"), new List<st3Modul>());

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

        List<st3Modul.fahrStr> fstrRouteSearchStart(st3Modul.referenzElement StartSignal, st3Modul.referenzElement ZielSignal)
        {
            var Besucht = new List<st3Modul.fahrStr>();
            foreach (var start_fstr in StartSignal.abgehendeFahrstraßen)
            {
                var rList = fstrRouteSearch(start_fstr, ZielSignal, Besucht);
                if (rList != null) return rList;
            }

            return null;
        }

        List<st3Modul.fahrStr> fstrRouteSearch(st3Modul.fahrStr Aktuell, st3Modul.referenzElement ZielSignal, List<st3Modul.fahrStr> Besucht)
        {
            Besucht.Add(Aktuell);
            if (Aktuell.Ziel.StrElement == ZielSignal.StrElement)
            {
                var Liste = new List<st3Modul.fahrStr>();
                Liste.Add(Aktuell);
                return (Liste);
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

        List<st3Modul> routeSearch(st3Modul Aktuell, st3Modul Ziel, List<st3Modul> Besucht)
        {
            Besucht.Add(Aktuell);
            if (Aktuell == Ziel)
            {
                var Lizte = new List<st3Modul>();
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
            kartenZeichner.message = dataConstructor.fortschrittMeldung;
            mMap.Image = kartenZeichner.draw();
        }

        private void modSelForm_Paint(object sender, PaintEventArgs e)
        {
            Application.DoEvents();
            kartenZeichner.message = dataConstructor.fortschrittMeldung;
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

            kartenZeichner.message = dataConstructor.fortschrittMeldung;
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

        private void Neuer_Zug_button_Click(object sender, EventArgs e)
        {
            var tmpZugfahrt = new ZugFahrt(datenFertig);

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

        void zlbUpdate()
        {
            ZLBready = false;
            ZugFahrtBox.Items[ZugFahrtBox.SelectedIndex] = ZugFahrtBox.SelectedItem;
            ZLBready = true;
        }

        
        private void ZugFahrtBox_SelectedValueChanged(object sender, EventArgs e)
        {
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
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;

            ZugKonfigForm = new ZugForm();
            ZugKonfigForm.Owner = this;
            ZugKonfigForm.setZug((ZugFahrt)ZugFahrtBox.SelectedItem);
            ZugKonfigForm.Show();
        }
    }
}