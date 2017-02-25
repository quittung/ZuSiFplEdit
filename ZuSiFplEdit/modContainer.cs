using System;
using System.Collections.Generic;

using System.Xml;
using System.IO;

namespace ZuSiFplEdit
{
    class modContainer
    {
        public class streckenModul
        {
            public string modPath;
            public string modName;

            public int UTM_NS;
            public int UTM_WE;
            public int UTM_Z1;
            public char UTM_Z2;

            /// <summary>
            /// Pixel-Position auf X-Achse auf Karte beim letzten Zeichenvorgang.
            /// </summary>
            public int PIX_X;
            /// <summary>
            /// Pixel-Position auf Y-Achse auf Karte beim letzten Zeichenvorgang.
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
                selected = false;
            }
        }

        public List<streckenModul> mSammlung = new List<streckenModul>();
        public int grenzeN;
        public int grenzeS;
        public int grenzeW;
        public int grenzeE;


        public modContainer()
        {
            //Durchläuft das Streckenverzeichnis und sucht nach allen .st3-Dateien
            string BaseDir = "C:\\games\\Zusi3\\Routes\\Deutschland\\";

            foreach (string grid in Directory.GetDirectories(BaseDir))
            {
                foreach (string mod in Directory.GetDirectories(grid))
                {
                    foreach (string st3 in Directory.GetFiles(mod, "*.st3"))
                    {
                        if (! st3.Contains("ummy"))
                        {
                            streckenModul aktModul = modulEinlesen(st3);
                            grenzenEinlesen(aktModul);
                            mSammlung.Add(aktModul);
                        }
                    }
                }
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
            XmlReader aktModXml = XmlReader.Create(Speicherort);
            

            streckenModul aktMod = new streckenModul(aktModXml.BaseURI);

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
            }
        }


        streckenModul sucheMod(string modName)
        {
            foreach (var mod in mSammlung)
            {
                if (mod.modName == modName) return mod;
            }
            return null;
        }


        //Extrahiert den Namen eines Moduls aus dem Speicherort. 
        static string speicherortZuName(string Speicherort, char DirSeparator)
        {
            //MessageBox.Show(Speicherort, DirSeparator.ToString(), MessageBoxButtons.OK);
            string[] modNameAr = Speicherort.Split(DirSeparator);
            string modName = modNameAr[modNameAr.Length - 1];
            modName = modName.Substring(0, modName.Length - 4);
            return (modName);
        }

        public void writeToFile()
        {
            int mod_Count = 0;
            int UTM_NS_avg = 0;
            int UTM_WE_avg = 0;

            int UTM_Z1 = 0;
            char UTM_Z2 = ' ';

            var fpn_file = new System.IO.StreamWriter("Fragment.fpn", false);
            foreach (var mod in mSammlung)
            {
                if (mod.selected)
                {
                    mod_Count++;
                    UTM_NS_avg += mod.UTM_NS;
                    UTM_WE_avg += mod.UTM_WE;

                    UTM_Z1 = mod.UTM_Z1;
                    UTM_Z2 = mod.UTM_Z2;

                    fpn_file.WriteLine("<StrModul>");
                    fpn_file.WriteLine("<Datei Dateiname=\"" + mod.modPath + "\"/>");
                    fpn_file.WriteLine("<p/>");
                    fpn_file.WriteLine("<phi/>");
                    fpn_file.WriteLine("</StrModul>");
                }
            }
            UTM_NS_avg = UTM_NS_avg / mod_Count;
            UTM_WE_avg = UTM_WE_avg / mod_Count;

            fpn_file.WriteLine("<UTM UTM_WE=\"" + UTM_WE_avg + "\" UTM_NS=\"" + UTM_NS_avg + "\" UTM_Zone=\"" + UTM_Z1 + "\" UTM_Zone2=\"" + UTM_Z2 + "\"/>\"");

            fpn_file.Close();
        }
    }
}
