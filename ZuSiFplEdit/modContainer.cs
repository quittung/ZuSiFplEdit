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
            public string modName;

            public int UTM_NS;
            public int UTM_WE;
            public int UTM_Z1;
            public char UTM_Z2;

            public int PIX_X;
            public int PIX_Y;

            public List<string> VerbindungenStr;
            public streckenModul[] Verbindungen;
            public bool NetzGrenze;

            public streckenModul(string moduleName)
            {
                modName = moduleName;
                VerbindungenStr = new List<string>();
                NetzGrenze = false;
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
            

            streckenModul aktMod = new streckenModul(speicherortZuName(aktModXml.BaseURI, '/'));

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
                        aktMod.VerbindungenStr.Add(Dateiname);
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
                int missingConnections = 0;
                aktModul.Verbindungen = new streckenModul[aktModul.VerbindungenStr.Count];
                for (int i = 0; i < aktModul.VerbindungenStr.Count; i++)
                {
                    aktModul.VerbindungenStr[i] = speicherortZuName(aktModul.VerbindungenStr[i], '\\');
                    for (int n = 0; n < mSammlung.Count; n++)
                    {
                        if (mSammlung[n].modName.Equals(aktModul.VerbindungenStr[i]))
                        {
                            aktModul.Verbindungen[i] = mSammlung[n];
                            break;
                        }
                    }
                    if (aktModul.Verbindungen[i] == null)
                    {
                        aktModul.NetzGrenze = true;
                        missingConnections++;
                    }
                }

                //Fehlende Module aus Links löschen:
                if (aktModul.NetzGrenze)
                {
                    streckenModul[] tmp_Verbindungen = new streckenModul[aktModul.Verbindungen.Length - missingConnections];
                    int verbindungen_count = 0;
                    for (int i = 0; i < aktModul.Verbindungen.Length; i++)
                    {
                        if (aktModul.Verbindungen[i] != null)
                        {
                            tmp_Verbindungen[verbindungen_count] = aktModul.Verbindungen[i];
                            verbindungen_count++;
                        }
                    }
                    aktModul.Verbindungen = tmp_Verbindungen;
                }
            }
        }


        //Extrahiert den Namen eines Moduls aus dem Speicherort. 
        string speicherortZuName(string Speicherort, char DirSeparator)
        {
            //MessageBox.Show(Speicherort, DirSeparator.ToString(), MessageBoxButtons.OK);
            string[] modNameAr = Speicherort.Split(DirSeparator);
            string modName = modNameAr[modNameAr.Length - 1];
            modName = modName.Substring(0, modName.Length - 4);
            return (modName);
        }
    }
}
