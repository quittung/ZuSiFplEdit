using System;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace ZuSiFplEdit
{
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

            //Durchläuft das Streckenverzeichnis und sucht nach allen .st3-Dateien
            try
            {
                DirBase = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Zusi3", "DatenVerzeichnis", "").ToString();
            }
            catch (Exception)
            {
                try
                {
                    DirBase = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3", "DatenVerzeichnis", "").ToString();
                }
                catch (Exception)
                {
                    MessageBox.Show("Datenverzeichnis konnte nicht gefunden werden.", "Fataler Fehler", MessageBoxButtons.OK);
                    Application.Exit();
                }
            }

            
            if (DirBase[DirBase.Length - 1] != '\\') DirBase += '\\';
            DirRoute = DirBase + "Routes\\Deutschland\\";

            List<string> st3Fehler = new List<string>();

            foreach (string grid in Directory.GetDirectories(DirRoute))
            {
                foreach (string mod in Directory.GetDirectories(grid))
                {
                    foreach (string st3 in Directory.GetFiles(mod, "*.st3"))
                    {
                        if (! st3.Contains("ummy"))
                        {
                            streckenModul aktModul = modulEinlesen(st3);
                            if (!(aktModul == null))
                            {
                                mSammlung.Add(aktModul);
                            } else
                            {
                                st3Fehler.Add(st3);
                            }
                        }
                    }
                }
            }
            
            moduleVerlinken();

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
        }

        streckenModul modulEinlesen(string Speicherort)
        {
            streckenModul aktMod = new streckenModul(Speicherort);

            if (!aktMod.isSane) return null;

            return aktMod;
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
                    if (!(mod.StartSignale.Contains(fstr.Start)))
                        mod.StartSignale.Add(fstr.Start);
                    if (!(fstr.ZielMod.ZielSignale.Contains(fstr.Ziel)))
                        fstr.ZielMod.ZielSignale.Add(fstr.Ziel);
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
