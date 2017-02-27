using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ZuSiFplEdit
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
        public bool isSane;
        public bool isDetailed;

        public streckenModul(string modulePath)
        {
            modPath = modulePath.Replace('/', '\\');
            modPath = modPath.Substring(modPath.IndexOf("Routes"));
            modName = modContainer.speicherortZuName(modPath, '\\');
            VerbindungenStr = new List<string>();
            NetzGrenze = false;
            wichtig = false;
            selected = false;
            isDetailed = true;

            isSane = true;
            try
            {
                readData(modulePath);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "XML Error: " + modName, MessageBoxButtons.OK);
                isSane = false;
            }
        }

        void readData(string Speicherort)
        {
            var timeKeeper = new System.Diagnostics.Stopwatch();
            timeKeeper.Start();

            XmlReader aktModXml = XmlReader.Create(Speicherort);

            aktModXml.Read();
            if (aktModXml.NodeType == XmlNodeType.XmlDeclaration) aktModXml.Read();
            while (!(aktModXml.NodeType == XmlNodeType.Element && aktModXml.Name == "Strecke")) aktModXml.Read();

            if (isDetailed)
            {
                int progress = 0; //mit switch möglichkeiten begrenzen
                //Vorgeplänkel einlesen
                while (aktModXml.Read())
                {
                    if (aktModXml.NodeType == XmlNodeType.Element)
                    {
                        //Bereits bekannt:
                        //if (aktModXml.Name == "Datei")
                        //{
                        //}
                        //Funktion unbekannt:
                        //if (aktModXml.Name == "HintergrundDatei")
                        //{
                        //}
                        //Vielleicht später:
                        //if (aktModXml.Name == "BefehlsKonfiguration")
                        //{
                        //}
                        //Funktion unbekannt:
                        //if (aktModXml.Name == "Kachelpfad")
                        //{
                        //}
                        //Vielleicht später:
                        //if (aktModXml.Name == "Beschreibung")
                        //{
                        //}
                        if (aktModXml.Name == "UTM")
                        {
                            UTM_NS = Convert.ToInt32(aktModXml.GetAttribute("UTM_NS"));
                            UTM_WE = Convert.ToInt32(aktModXml.GetAttribute("UTM_WE"));
                            UTM_Z1 = Convert.ToInt32(aktModXml.GetAttribute("UTM_Zone"));
                            UTM_Z2 = aktModXml.GetAttribute("UTM_Zone2")[0];
                        }
                        if (aktModXml.Name == "Huellkurve")
                        {
                            //MessageBox.Show("Lese Huellkurve ein.", "XML: " + modName, MessageBoxButtons.OK);
                        }
                        //Vielleicht später:
                        //if (aktModXml.Name == "Skybox")
                        //{
                        //}
                        //Vielleicht später:
                        //if (aktModXml.Name == "SkyDome")
                        //{
                        //}
                        //Vielleicht später:
                        //if (aktModXml.Name == "StreckenStandort")
                        ////{
                        //}
                        //Funktion soll ersetzt werden:
                        //if (aktModXml.Name == "ModulDateien")
                        //{
                        //}
                        if (aktModXml.Name == "ReferenzElemente")
                        {
                            if (aktModXml.GetAttribute("RefTyp") == "1")
                            {
                                string verbindungsName = aktModXml.GetAttribute("Info");
                                verbindungsName = modContainer.speicherortZuName(verbindungsName, '\\');
                                if (!(VerbindungenStr.Contains(verbindungsName)))
                                    VerbindungenStr.Add(verbindungsName);
                            }
                        }
                        //Sehr detailliert:
                        if (aktModXml.Name == "StrElement")
                        {
                            //MessageBox.Show("Lese StrElement ein.", "XML: " + modName, MessageBoxButtons.OK);
                        }
                        if (aktModXml.Name == "Fahrstrasse")
                        {
                            //MessageBox.Show("Lese Fahrstrasse ein.", "XML: " + modName, MessageBoxButtons.OK);
                        }
                    }
                    else if (aktModXml.NodeType == XmlNodeType.EndElement && aktModXml.Name == "Strecke") break;
                }
            } else
            {
                while (aktModXml.Read())
                {
                    if ((aktModXml.NodeType == XmlNodeType.Element) && (aktModXml.Name == "UTM"))
                    {
                        UTM_NS = Convert.ToInt32(aktModXml.GetAttribute("UTM_NS"));
                        UTM_WE = Convert.ToInt32(aktModXml.GetAttribute("UTM_WE"));
                        UTM_Z1 = Convert.ToInt32(aktModXml.GetAttribute("UTM_Zone"));
                        UTM_Z2 = aktModXml.GetAttribute("UTM_Zone2")[0];
                    }

                    if ((aktModXml.NodeType == XmlNodeType.Element) && (aktModXml.Name == "ModulDateien"))
                    {
                        aktModXml.Read();
                        while (aktModXml.Name != "Datei") aktModXml.Read();
                        string Dateiname = aktModXml.GetAttribute("Dateiname");

                        if (!(Dateiname == null || VerbindungenStr.Contains(Dateiname)))
                            VerbindungenStr.Add(modContainer.speicherortZuName(Dateiname, '\\'));
                    }
                }
            }

            timeKeeper.Stop();
            //MessageBox.Show("Einlesen hat " + timeKeeper.ElapsedMilliseconds + " ms gedauert.", "XML: " + modName, MessageBoxButtons.OK);
        }
    }
}
