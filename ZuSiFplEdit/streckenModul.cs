using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ZuSiFplEdit
{
    public class streckenModul
    {
        public class streckenElement
        {
            public int Nr;
            public float spTrass;
            public int Anschluss;
            public string Oberbau;

            public float g_X;
            public float g_Y;
            public float b_X;
            public float b_Y;

            public int AnschlussNorm;
            public int AnschlussGegen;

            public streckenElement(int Nr, float spTrass, int Anschluss, string Oberbau, float g_X, float g_Y, float b_X, float b_Y, int AnschlussNorm, int AnschlussGegen)
            {
                this.Nr = Nr;
                this.spTrass = spTrass;
                this.Anschluss = Anschluss;
                this.Oberbau = Oberbau;

                this.g_X = g_X;
                this.g_Y = g_Y;
                this.b_X = b_X;
                this.b_Y = b_Y;

                this.AnschlussNorm = AnschlussNorm;
                this.AnschlussGegen = AnschlussGegen;
        }
        }

        public class referenzElement
        {
            public int ReferenzNr;
            public int StrElement;
            public bool StrNorm;
            public int RefTyp;
            public string Info;

            public referenzElement(int ReferenzNr, int StrElement, bool StrNorm, int RefTyp, string Info)
            {
                this.ReferenzNr = ReferenzNr;
                this.StrElement = StrElement;
                this.StrNorm = StrNorm;
                this.RefTyp = RefTyp;
                this.Info = Info;
            }
        }

        public class fahrStr
        {
            public string FahrstrName;
            public string FahrstrStrecke;
            public int RglGgl;
            public string FahrstrTyp;
            public float Laenge;

            public referenzElement Start;
            public referenzElement Ziel;

            public fahrStr(string FahrstrName, string FahrstrStrecke, int RglGgl, string FahrstrTyp, float Laenge, referenzElement Start, referenzElement Ziel)
            {
                this.FahrstrName = FahrstrName;
                this.FahrstrStrecke = FahrstrStrecke;
                this.RglGgl = RglGgl;
                this.FahrstrTyp = FahrstrTyp;
                this.Laenge = Laenge;

                this.Start = Start;
                this.Ziel = Ziel;
            }
        }

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

        public List<streckenElement> StreckenElemente;
        public List<referenzElement> ReferenzElemente;
        public List<fahrStr> FahrStr;

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
            StreckenElemente = new List<streckenElement>();
            ReferenzElemente = new List<referenzElement>();
            FahrStr = new List<fahrStr>();
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

            XmlReader modXml = XmlReader.Create(Speicherort);

            modXml.Read();
            if (modXml.NodeType == XmlNodeType.XmlDeclaration) modXml.Read();
            while (!(modXml.NodeType == XmlNodeType.Element && modXml.Name == "Strecke")) modXml.Read();

            if (isDetailed)
            {
                int progress = 0; //mit switch möglichkeiten begrenzen
                //Vorgeplänkel einlesen
                while (modXml.Read())
                {
                    if (modXml.NodeType == XmlNodeType.Element)
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
                        if (modXml.Name == "UTM")
                        {
                            UTM_NS = Convert.ToInt32(modXml.GetAttribute("UTM_NS"));
                            UTM_WE = Convert.ToInt32(modXml.GetAttribute("UTM_WE"));
                            UTM_Z1 = Convert.ToInt32(modXml.GetAttribute("UTM_Zone"));
                            UTM_Z2 = modXml.GetAttribute("UTM_Zone2")[0];
                        }

                        ////Vielleicht später:
                        //if (modXml.Name == "Huellkurve")
                        //{
                        //    //MessageBox.Show("Lese Huellkurve ein.", "XML: " + modName, MessageBoxButtons.OK);
                        //}
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
                        if (modXml.Name == "ReferenzElemente")
                        {
                            //RefType == "1" - Modulgrenzen
                            //RefType == "2" - Register
                            //RefType == "3" - Weichen
                            //RefType == "4" - Signale und andere streckengebundene Objekte
                            //RefType == "5" - Auflösepunkte
                            //RefType == "6" - Signalhaltfall

                            
                            int ReferenzNr = Convert.ToInt32(modXml.GetAttribute("ReferenzNr"));
                            int StrElement = Convert.ToInt32(modXml.GetAttribute("StrElement"));
                            bool StrNorm;
                            if (modXml.GetAttribute("StrNorm") == "1")  StrNorm = true;
                            else  StrNorm = false;
                            int RefTyp = Convert.ToInt32(modXml.GetAttribute("RefTyp"));
                            string Info = modXml.GetAttribute("Info");

                            ReferenzElemente.Add(new referenzElement(ReferenzNr, StrElement, StrNorm, RefTyp, Info));
                            if (RefTyp == 1)
                            {
                                

                                string verbindungsName = Info;
                                verbindungsName = modContainer.speicherortZuName(verbindungsName, '\\');
                                if (!(VerbindungenStr.Contains(verbindungsName)))
                                    VerbindungenStr.Add(verbindungsName);
                            }
                        }
                        //Sehr detailliert:
                        if (modXml.Name == "StrElement")
                        {
                            int Nr = Convert.ToInt32(modXml.GetAttribute("Nr"));
                            float spTrass = Convert.ToSingle(modXml.GetAttribute("spTrass"), CultureInfo.InvariantCulture.NumberFormat);
                            int Anschluss = Convert.ToInt32(modXml.GetAttribute("Anschluss"));
                            string Oberbau = modXml.GetAttribute("Oberbau");

                            //if (Oberbau != null && Oberbau.Contains("B55")) continue;

                            while (!(modXml.Name == "g")) modXml.Read();
                            float g_X = Convert.ToSingle(modXml.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                            float g_Y = Convert.ToSingle(modXml.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);
                            while (!(modXml.Name == "b")) modXml.Read();
                            float b_X = Convert.ToSingle(modXml.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                            float b_Y = Convert.ToSingle(modXml.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);

                            while ((!(modXml.Name == "NachNorm")) && modXml.Read()) { }
                            int AnschlussNorm = Convert.ToInt32(modXml.GetAttribute("Nr"));
                            while ((!(modXml.Name == "NachGegen")) && modXml.Read()) { }
                            int AnschlussGegen = Convert.ToInt32(modXml.GetAttribute("Nr"));
                        

                            StreckenElemente.Add(new streckenElement(Nr, spTrass, Anschluss, Oberbau, g_X, g_Y, b_X, b_Y, AnschlussNorm, AnschlussGegen));
                            while ((!(modXml.NodeType == XmlNodeType.EndElement && modXml.Name == "StrElement")) && modXml.Read()) { }
                        }
                        if (modXml.Name == "Fahrstrasse")
                        {
                            string FahrstrName = modXml.GetAttribute("FahrstrName");
                            string FahrstrStrecke = modXml.GetAttribute("FahrstrStrecke");
                            int RglGgl = Convert.ToInt32(modXml.GetAttribute("RglGgl"));
                            string FahrstrTyp = modXml.GetAttribute("FahrstrTyp");
                            float Laenge = Convert.ToSingle(modXml.GetAttribute("Laenge"));

                            while (!(modXml.Name == "FahrstrStart")) modXml.Read();
                            int startRef = Convert.ToInt32(modXml.GetAttribute("Ref"));
                            while (!(modXml.Name == "FahrstrZiel")) modXml.Read();
                            int zielRef = Convert.ToInt32(modXml.GetAttribute("Ref"));

                            referenzElement Start = sucheReferenz(startRef);
                            referenzElement Ziel = sucheReferenz(zielRef);

                            FahrStr.Add(new fahrStr(FahrstrName, FahrstrStrecke, RglGgl, FahrstrTyp, Laenge, Start, Ziel));
                        }
                    }
                    //else if (modXml.NodeType == XmlNodeType.EndElement && modXml.Name == "Strecke") break;
                }
            } else
            {
                while (modXml.Read())
                {
                    if ((modXml.NodeType == XmlNodeType.Element) && (modXml.Name == "UTM"))
                    {
                        UTM_NS = Convert.ToInt32(modXml.GetAttribute("UTM_NS"));
                        UTM_WE = Convert.ToInt32(modXml.GetAttribute("UTM_WE"));
                        UTM_Z1 = Convert.ToInt32(modXml.GetAttribute("UTM_Zone"));
                        UTM_Z2 = modXml.GetAttribute("UTM_Zone2")[0];
                    }

                    if ((modXml.NodeType == XmlNodeType.Element) && (modXml.Name == "ModulDateien"))
                    {
                        modXml.Read();
                        while (modXml.Name != "Datei") modXml.Read();
                        string Dateiname = modXml.GetAttribute("Dateiname");

                        if (!(Dateiname == null || VerbindungenStr.Contains(Dateiname)))
                            VerbindungenStr.Add(modContainer.speicherortZuName(Dateiname, '\\'));
                    }
                }
            }

            timeKeeper.Stop();
            //List<string> oberbauTypen = new List<string>();
            //string OT = "";
            //foreach (var se in StreckenElemente)
            //{
            //    if (!(oberbauTypen.Contains(se.Oberbau)))
            //    {
            //        oberbauTypen.Add(se.Oberbau);
            //        OT += "- " + se.Oberbau + "\n";
            //    }
            //}
            //MessageBox.Show(OT, "Oberbautypen: " + modName, MessageBoxButtons.OK);
            //MessageBox.Show("Einlesen hat " + timeKeeper.ElapsedMilliseconds + " ms gedauert.", "XML: " + modName, MessageBoxButtons.OK);
        }

        public referenzElement sucheReferenz(int ReferenzNr)
        {
            foreach (var rE in ReferenzElemente)
            {
                if (rE.ReferenzNr == ReferenzNr)
                {
                    return (rE);
                }
            }

            return null;
        }
    }
}
