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
        bool guiBereit = true;

        DataConstructor dataConstructor;
        Datensatz datenFertig;
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

        Fahrplan fahrplan;
        List<ZugFahrt> ZugFahrten = new List<ZugFahrt>();
        ZugForm ZugKonfigForm;

        int debugX = 0;
        int debugY = 0;

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
            
            FindeDatenVerzeichnis();

            datenFertig = new Datensatz(DirBase);

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
            
            //Fahrplan initialisieren
            fahrplan = new Fahrplan();

            //Modulekarte vorbereiten
            debugX = mMap.Width;
            debugY = mMap.Height;
            kartenZeichner = new mapDraw(mMap.Width, mMap.Height, datenFertig, fahrplan, ZugFahrtBox);

            //Module ausgeben
            foreach (streckenModul modul in datenFertig.module)
            {
                modListBox.Items.Add(modul);
            }
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

        /// <summary>
        /// Reagiert auf das Klicken des "Fahrplan speichern"-Knopfes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fahrplanAusgeben(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = DirBase + "Timetables\\";
            saveFileDialog1.Filter = "Fahrplandateien (*.fpn)|*.fpn|Alle Dateiformate (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fahrplan.exportVorbereiten();
                
                this.Invalidate();
                Application.DoEvents();

                new fileops(fahrplan, saveFileDialog1.FileName, DirBase);
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


                //Auswahl von Signalen für die Routenerstellung
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
                    //Modulauswahl
                    if (moduToolStripMenuItem.Checked && punkteToolStripMenuItem.Checked)
                    {
                        var nächstesModul = kartenZeichner.getNearestStation(e.X, e.Y);
                        if (kartenZeichner.getModulDistance(nächstesModul, e.X, e.Y) < 10)
                        {
                            nächstesModul.selected = !nächstesModul.selected;
                            if (fahrplan.module.Contains(nächstesModul))
                            {
                                fahrplan.module.Remove(nächstesModul);
                                modListBox.SelectedItems.Remove(nächstesModul);
                                nächstesModul.selected = false;
                            }
                            else
                            {
                                fahrplan.module.Add(nächstesModul);
                                modListBox.SelectedItems.Add(nächstesModul);
                                nächstesModul.selected = true;
                            }

                            kartenZeichner.message = dataConstructor.fortschrittMeldung;
                            mMap.Image = kartenZeichner.draw();
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

        /// <summary>
        /// Überträgt die Auswahl der Modul-Liste auf das interne Datenmodell und die Liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!guiBereit)
                return;

            fahrplan.module.Clear();
            foreach (streckenModul modul in modListBox.SelectedItems)
            {
                fahrplan.modulHinzufügen(modul);
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

            tmpZugfahrt.gattung = "RB";
            
            int ZugNummer = 0;
            bool ZugNummerBesetzt = true;
            while (ZugNummerBesetzt)
            {
                ZugNummer++;
                ZugNummerBesetzt = ZNBesetzt(ZugNummer);
            }
            tmpZugfahrt.zugnummer = ZugNummer;

            fahrplan.zugFahrten.Add(tmpZugfahrt);
            ZugFahrtBox.Items.Add(tmpZugfahrt);
            ZugFahrtBox.SelectedItem = tmpZugfahrt;
        }
        
        bool ZNBesetzt(int ZN)
        {
            bool ZugNummerBesetzt = false;
            foreach (ZugFahrt zug in fahrplan.zugFahrten)
            {
                if (zug.zugnummer == ZN)
                {
                    ZugNummerBesetzt = true;
                    break;
                }
            }
            return ZugNummerBesetzt;
        }
        
        private void ZugFahrtBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!guiBereit || ZugFahrtBox.SelectedItem == null)
                return;

            guiBereit = false;
            ZugFahrtBox.Items[ZugFahrtBox.SelectedIndex] = ZugFahrtBox.SelectedItem;
            guiBereit = true;
        }

        private void zugLöschen_click(object sender, EventArgs e)
        {
            ZugFahrt zug = (ZugFahrt)ZugFahrtBox.SelectedItem;
            if (zug == null)
                return;

            ZugFahrtBox.Items.Remove(zug);
            fahrplan.zugFahrten.Remove(zug);            
        }

        private void zugBearbeite_click(object sender, EventArgs e)
        {
            if (ZugFahrtBox.SelectedItem == null)
                return;

            ZugKonfigForm = new ZugForm();
            ZugKonfigForm.Owner = this;
            ZugKonfigForm.setZug((ZugFahrt)ZugFahrtBox.SelectedItem);
            ZugKonfigForm.Show();
        }

        private void ladenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FahrplanLaden.fahrplanÖffnen(fahrplan, datenFertig);

            guiBereit = false;
            modListBox.SelectedItems.Clear();
            foreach (var modul in fahrplan.module)
            {
                modListBox.SelectedItems.Add(modul);
            }

            ZugFahrtBox.Items.Clear();
            foreach (var zug in fahrplan.zugFahrten)
            {
                ZugFahrtBox.Items.Add(zug);
            }
            guiBereit = true;
        }
    }
}