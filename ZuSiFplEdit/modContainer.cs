using System;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    class modContainer
    {
        public class streckenModul
        {
            /// <summary>
            /// Relativer Pfad zum Modul in Zusi-Bibliothek
            /// </summary>
            public string modPath;
            /// <summary>
            /// Name des Moduls
            /// </summary>
            public string modName;

            /// <summary>
            /// UMT-Nordwert von Modul
            /// </summary>
            public int UTM_NS;
            /// <summary>
            /// UMT-Ostwert von Modul
            /// </summary>
            public int UTM_WE;
            /// <summary>
            /// UMT-Meridianzone von Modul
            /// </summary>
            public int UTM_Z1;
            /// <summary>
            /// UMT Latitude Band von Modul
            /// </summary>
            public char UTM_Z2;

            /// <summary>
            /// Pixel-Position auf X-Achse auf Karte beim letzten Zeichenvorgang
            /// </summary>
            public int PIX_X;
            /// <summary>
            /// Pixel-Position auf Y-Achse auf Karte beim letzten Zeichenvorgang
            /// </summary>
            public int PIX_Y;

            /// <summary>
            /// Enthält die umliegenden Module als String. Nach Verlinkung nicht mehr aktuell.
            /// </summary>
            public List<string> VerbindungenStr;
            /// <summary>
            /// Enthält Pointer zu den umliegenden Modulen.
            /// </summary>
            public List<streckenModul> Verbindungen;

            /// <summary>
            /// Wahr, wenn Modul weniger als 2 existierende Verbindungen im Datenbestand hat.
            /// </summary>
            public bool NetzGrenze;
            public bool wichtig;
            /// <summary>
            /// Wahr, wenn Modul für Fahrplan ausgewählt wurde.
            /// </summary>
            public bool selected;

            public streckenModul(string modulePath)
            {
                modPath = modulePath.Replace('/', '\\');
                modPath = modPath.Substring(modPath.IndexOf("Routes"));
                modName = speicherortZuName(modPath, '\\');
                VerbindungenStr = new List<string>();
                NetzGrenze = false;
                wichtig = false;
                selected = false;
            }
        }

        public List<streckenModul> mSammlung = new List<streckenModul>();
        public string DirBase = "";
        public string DirRoute = "";
        public int grenzeN;
        public int grenzeS;
        public int grenzeW;
        public int grenzeE;


        public modContainer()
        {
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
                                grenzenEinlesen(aktModul);
                                mSammlung.Add(aktModul);
                            } else
                            {
                                st3Fehler.Add(st3);
                            }
                        }
                    }
                }
            }

            if (st3Fehler.Count > 0)
            {
                string errMsg = "Fehler beim Laden der folgenden Module:";
                foreach (var st3 in st3Fehler)
                {
                    errMsg += "\n - " + st3;
                }
                MessageBox.Show(errMsg, "Fehler in .st3-Dateien", MessageBoxButtons.OK);
            }

            moduleVerlinken();
        }
        

        void grenzenEinlesen(streckenModul aktModul)
        {
            if (mSammlung.Count == 1)
            {
                grenzeN = aktModul.UTM_NS;
                grenzeS = aktModul.UTM_NS;
                grenzeE = aktModul.UTM_WE;
                grenzeW = aktModul.UTM_WE;
            }
            else
            {
                if (aktModul.UTM_NS > grenzeN) grenzeN = aktModul.UTM_NS;
                if (aktModul.UTM_NS < grenzeS) grenzeS = aktModul.UTM_NS;
                if (aktModul.UTM_WE > grenzeE) grenzeE = aktModul.UTM_WE;
                if (aktModul.UTM_WE < grenzeW) grenzeW = aktModul.UTM_WE;
            }
        }

        streckenModul modulEinlesen(string Speicherort)
        {
            
            

            streckenModul aktMod = new streckenModul(Speicherort);

            try
            {
                XmlReader aktModXml = XmlReader.Create(Speicherort);

                while (aktModXml.Read())
                {
                    if ((aktModXml.NodeType == XmlNodeType.Element) && (aktModXml.Name == "UTM"))
                    {
                        aktMod.UTM_NS = Convert.ToInt32(aktModXml.GetAttribute("UTM_NS"));
                        aktMod.UTM_WE = Convert.ToInt32(aktModXml.GetAttribute("UTM_WE"));
                        aktMod.UTM_Z1 = Convert.ToInt32(aktModXml.GetAttribute("UTM_Zone"));
                        aktMod.UTM_Z2 = aktModXml.GetAttribute("UTM_Zone2")[0];
                    }

                    if ((aktModXml.NodeType == XmlNodeType.Element) && (aktModXml.Name == "ModulDateien"))
                    {
                        aktModXml.Read();
                        while (aktModXml.Name != "Datei") aktModXml.Read();
                        string Dateiname = aktModXml.GetAttribute("Dateiname");

                        if (!(Dateiname == null || aktMod.VerbindungenStr.Contains(Dateiname)))
                            aktMod.VerbindungenStr.Add(speicherortZuName(Dateiname, '\\'));
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            

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
                    if (! connection.Verbindungen.Contains(mod))
                    {
                        mod.Verbindungen.Remove(connection);
                    }
                }

                if (mod.Verbindungen.Count < 2) mod.NetzGrenze = true;
                if ((mod.Verbindungen.Count > 2) || (mod.NetzGrenze)) mod.wichtig = true; 
            }
        }

        /// <summary>
        /// Gibt Modul-Objekt für einen Modulnamen zurück
        /// </summary>
        /// <param name="modName">Name des Moduls</param>
        streckenModul sucheMod(string modName)
        {
            foreach (var mod in mSammlung)
            {
                if (mod.modName == modName) return mod;
            }
            return null;
        }


        /// <summary>
        /// Extrahiert den Namen eines Moduls aus seinem Pfad
        /// </summary>
        /// <param name="Speicherort">Pfad zum Modul</param>
        /// /// <param name="DirSeparator">Verzeichnisseparator</param>
        static string speicherortZuName(string Speicherort, char DirSeparator)
        {
            string[] modNameAr = Speicherort.Split(DirSeparator);
            string modName = modNameAr[modNameAr.Length - 1];
            modName = modName.Substring(0, modName.Length - 4);
            return (modName);
        }

        /// <summary>
        /// Erzeugt .fpn-Fragment aus aktuell ausgewählten Modulen.
        /// </summary>
        public void writeToFile(string path)
        {
            int mod_Count = 0;
            int UTM_NS_avg = 0;
            int UTM_WE_avg = 0;

            int UTM_Z1 = 0;
            char UTM_Z2 = ' ';

            if (path == "") path = "Rohling.fpn";
            var fpn_file = new System.IO.StreamWriter(path, false);

            fpn_file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            fpn_file.WriteLine("<Zusi>");
            fpn_file.WriteLine("<Info DateiTyp=\"Fahrplan\" Version=\"A.1\" MinVersion=\"A.1\">");
            fpn_file.WriteLine("<AutorEintrag/>");
            fpn_file.WriteLine("</Info>");
            fpn_file.WriteLine("<Fahrplan>");
            fpn_file.WriteLine("<BefehlsKonfiguration/>");
            fpn_file.WriteLine("<Begruessungsdatei/>");

            foreach (var mod in mSammlung)
            {
                if (mod.selected)
                {
                    //Vorbereitung zur Berechnung vom Referenzpunkt
                    mod_Count++;
                    UTM_NS_avg += mod.UTM_NS;
                    UTM_WE_avg += mod.UTM_WE;

                    UTM_Z1 = mod.UTM_Z1;
                    UTM_Z2 = mod.UTM_Z2;

                    //Schreibe XML für aktuelles Modul
                    fpn_file.WriteLine("<StrModul>");
                    fpn_file.WriteLine("<Datei Dateiname=\"" + mod.modPath + "\"/>");
                    fpn_file.WriteLine("<p/>");
                    fpn_file.WriteLine("<phi/>");
                    fpn_file.WriteLine("</StrModul>");
                }
            }

            //Abbrechen, wenn nichts ausgewählt.
            if (mod_Count == 0)
            {
                fpn_file.WriteLine("<UTM/>");
                fpn_file.WriteLine("</Fahrplan>");
                fpn_file.WriteLine("</Zusi>");

                fpn_file.Close();

                return;

            }

            //Berechne UTM-Referenzpunkt
            UTM_NS_avg = UTM_NS_avg / mod_Count;
            UTM_WE_avg = UTM_WE_avg / mod_Count;

            //Schreibe UTM-Referenzpunkt
            fpn_file.WriteLine("<UTM UTM_WE=\"" + UTM_WE_avg + "\" UTM_NS=\"" + UTM_NS_avg + "\" UTM_Zone=\"" + UTM_Z1 + "\" UTM_Zone2=\"" + UTM_Z2 + "\"/>\"");
            fpn_file.WriteLine("</Fahrplan>");
            fpn_file.WriteLine("</Zusi>");

            fpn_file.Close();
        }
    }
}
