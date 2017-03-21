using System;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Sammelt, vernetzt und verwaltet Streckenmodule
    /// </summary>
    class modContainer
    {
        

        public List<streckenModul> mSammlung = new List<streckenModul>();
        public string DirBase = "";
        public string DirRoute = "";

        public long loadTime;


        public modContainer()
        {
            var timeGesamt = new System.Diagnostics.Stopwatch();
            timeGesamt.Start();
            
            var ladeAnzeige = new form_lade();
            ladeAnzeige.Show();
            ladeAnzeige.Beschreibung.Text = "Suche Datenverzeichnis...";
            ladeAnzeige.Update();

            FindeDatenVerzeichnis();
            
            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 1, "Suche Module...");

            if (DirBase[DirBase.Length - 1] != '\\') DirBase += '\\';
            DirRoute = DirBase + "Routes\\Deutschland\\";

            List<string> modulPaths = new List<string>();
            foreach (string grid in Directory.GetDirectories(DirRoute))
            {
                foreach (string mod in Directory.GetDirectories(grid))
                {
                    foreach (string st3 in Directory.GetFiles(mod, "*.st3"))
                    {
                        modulPaths.Add(st3);
                    }
                }
            }

            ladeAnzeige.progressBar2.Maximum = modulPaths.Count - 1;
            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 2, "Lese Module...");

            List<string> st3Fehler = new List<string>();
            for (int i = 0; i < modulPaths.Count; i++)
            {
                ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, i, "Lese Module [" + (i + 1) + "/" + modulPaths.Count + "] - " + modulPaths[i].Split('\\').Last());
                ModulEinlesen(modulPaths[i], st3Fehler);
            }

            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 3, "Verlinke Module...");

            moduleVerlinken();

            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 4, "Finalisiere...");

            timeGesamt.Stop();
            loadTime = timeGesamt.ElapsedMilliseconds;
            //MessageBox.Show("Einlesen hat " + timeGesamt.ElapsedMilliseconds + " ms gedauert.", "Gesamtdauer des Einlesens", MessageBoxButtons.OK);




            if (st3Fehler.Count > 0)
            {
                string errMsg = "Fehler beim Laden der folgenden Module:";
                foreach (var st3 in st3Fehler)
                {
                    errMsg += "\n - " + st3;
                }
                errMsg += "\n\nDie betroffenen Module wurden ignoriert.";
                MessageBox.Show(errMsg, "Fehler in .st3-Dateien", MessageBoxButtons.OK);
            }

            ladeAnzeige.Dispose();
        }


        /// <summary>
        /// Liest ein Modul ein und hängt es der passenden Liste an
        /// </summary>
        private void ModulEinlesen(string Speicherort, List<string> st3Fehler)
        {
            streckenModul aktMod = new streckenModul(Speicherort);

            if (aktMod.isSane == true)
            {
                mSammlung.Add(aktMod);
            }
            else
            {
                st3Fehler.Add(Speicherort);
            }
        }


        /// <summary>
        /// Liest das Datenverzeichnis aus der Registry ein
        /// </summary>
        private void FindeDatenVerzeichnis()
        {
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
                    MessageBox.Show("Datenverzeichnis konnte nicht gefunden werden.", "Fataler Fehler", MessageBoxButtons.OK);
                    Environment.Exit(1);
                }
            }
        }
        
    
        //Wandelt die als String gespeicherten Verbindungen in Pointer um.
        void moduleVerlinken()
        {
            //MessageBox.Show("Module werden jetzt verlinkt.", "Debugnachricht", MessageBoxButtons.OK);
            foreach (streckenModul aktModul in mSammlung)
            {
                aktModul.Verbindungen = new List<streckenModul>();
                foreach (string connectionString in aktModul.VerbindungenStr)
                {
                    streckenModul connectionObj = sucheMod(connectionString);
                    if (!(connectionObj == null))
                    {
                        aktModul.Verbindungen.Add(connectionObj);
                    } else
                    {
                        aktModul.NetzGrenze = true;
                        //aktModul.VerbindungenStr.Remove(connectionString);
                    }   
                }
            }

            //Delete one-sided connections.
            foreach (streckenModul mod in mSammlung)
            {
                foreach (var connection in mod.Verbindungen.ToArray())
                {
                    //if (! connection.Verbindungen.Contains(mod))
                    //{
                    //    mod.Verbindungen.Remove(connection);
                    //}
                }

                if (mod.Verbindungen.Count < 2)
                {
                    mod.NetzGrenze = true;
                    //if (true || mod.Verbindungen.Count != mod.VerbindungenStr.Count)
                    //{
                    //    string msgString = "Eingelesene Verbindungen:";
                    //    foreach (var item in mod.VerbindungenStr)
                    //    {
                    //        msgString += "\n" + item;
                    //    }
                    //    msgString += "\nVerbliebene Verbindungen:";
                    //    foreach (var item in mod.Verbindungen)
                    //    {
                    //        msgString += "\n" + item.modName;
                    //    }
                    //    MessageBox.Show(msgString, "Grenzreport für " + mod.modName, MessageBoxButtons.OK);
                    //}
                }
                if ((mod.Verbindungen.Count > 2) || (mod.NetzGrenze)) mod.wichtig = true;
                //DrawDist eintragen.
                double dist = 0;
                foreach (var con in mod.Verbindungen)
                {
                    double dx = mod.UTM_WE - con.UTM_WE;
                    double dy = mod.UTM_NS - con.UTM_NS;

                    double modDist = Math.Sqrt(dx * dx + dy * dy);

                    if (modDist > dist) dist = modDist;
                }
                mod.drawDist = dist;
            }

            //string problemstellen = "";
            //verlinke fahrstraßen mit referenzen.
            var unvollständigeFahrstraßen = new List<streckenModul.fahrStr>();
            foreach (var mod in mSammlung)
            {
                foreach (var fstr in mod.FahrStr)
                {
                    fstr.StartMod = mod;
                    fstr.Start = mod.sucheReferenz(fstr.StartRef);
                    //if (fstr.Start == null || fstr.Start.Signal == null)
                    if (fstr.Start == null)
                    {
                        unvollständigeFahrstraßen.Add(fstr);
                        continue;
                    }
                    else
                    {
                        fstr.Start.abgehendeFahrstraßen.Add(fstr);
                    }
                    fstr.ZielMod = sucheMod(fstr.ZielMod_Str);
                    if (fstr.ZielMod == null)
                    {
                        unvollständigeFahrstraßen.Add(fstr);
                        continue;
                    }
                    else
                    {
                        fstr.Ziel = fstr.ZielMod.sucheReferenz(fstr.ZielRef);
                        //if (fstr.Ziel == null ||fstr.Ziel.Signal == null)
                        if (fstr.Ziel == null)
                        {
                            unvollständigeFahrstraßen.Add(fstr);
                            continue;
                        }
                    }
                }
                foreach (var ufstr in unvollständigeFahrstraßen) //Entfernen von unvollständigen Fahrstraßen.
                {
                    mod.FahrStr.Remove(ufstr);
                }
            }

            //Folgestraßen eintragen
            foreach (var mod in mSammlung)
            {
                foreach (var fstr in mod.FahrStr)
                {
                    fstr.folgeStraßen = new List<streckenModul.fahrStr>();

                    if (!(fstr.Ziel == null))
                    {
                        foreach (var fort in fstr.ZielMod.FahrStr)
                        {
                            if (fstr.Ziel == fort.Start)
                            {
                                fstr.folgeStraßen.Add(fort);
                            }
                        }
                    }
                }
            }
            

            //Sammle abgehende Fahrstraßen zu Signalen in Modul.
            foreach (var mod in mSammlung)
            {
                foreach (var fstr in mod.FahrStr) //Start- und Zielsignale in den entsprechenden Modulen ein.
                {
                    fstr.Start.istStart = true;
                    if (!(mod.StartSignale.Contains(fstr.Start)))
                        mod.StartSignale.Add(fstr.Start);
                    if (!(mod.StartUndZielSignale.Contains(fstr.Start)))
                        mod.StartUndZielSignale.Add(fstr.Start);

                    fstr.Ziel.istZiel = true;
                    if (!(fstr.ZielMod.ZielSignale.Contains(fstr.Ziel)))
                        fstr.ZielMod.ZielSignale.Add(fstr.Ziel);
                    if (!(fstr.ZielMod.StartUndZielSignale.Contains(fstr.Ziel)))
                        fstr.ZielMod.StartUndZielSignale.Add(fstr.Ziel);
                }
            }
        }

        /// <summary>
        /// Gibt Modul-Objekt für einen Modulnamen zurück
        /// </summary>
        /// <param name="modName">Name des Moduls</param>
        public streckenModul sucheMod(string modName) 
        {
            modName = modName.Substring(0, modName.Length - 5); //HACK: Jahreszahl wird ignoriert
            foreach (var mod in mSammlung)
            {
                if (mod.modName.Contains(modName)) return mod;
            }
            return null;
        }


        /// <summary>
        /// Extrahiert den Namen eines Moduls aus seinem Pfad
        /// </summary>
        /// <param name="Speicherort">Pfad zum Modul</param>
        /// /// <param name="DirSeparator">Verzeichnisseparator</param>
        public static string speicherortZuName(string Speicherort, char DirSeparator)
        {
            string[] modNameAr = Speicherort.Split(DirSeparator);
            string modName = modNameAr[modNameAr.Length - 1];
            modName = modName.Substring(0, modName.Length - 4);
            return (modName);
        }
    }
}
