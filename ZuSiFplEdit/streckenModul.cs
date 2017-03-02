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
        public class PunktXY
        {
            public double rel_X;
            public double rel_Y;
            public double abs_X;
            public double abs_Y;

            public PunktXY(double ref_X, double ref_Y, double rel_X, double rel_Y)
            {
                this.rel_X = rel_X;
                this.rel_Y = rel_Y;

                abs_X = ref_X + (rel_X / 1000f);
                abs_Y = ref_Y + (rel_Y / 1000f);
            }
        }

        public class streckenElement
        {
            public int Nr;
            public float spTrass;
            public int Anschluss;
            public int Funktion;
            public string Oberbau;

            public float g_X;
            public float g_Y;
            public float b_X;
            public float b_Y;

            public int AnschlussNorm;
            public int AnschlussGegen;

            public streckenElement(int Nr, float spTrass, int Anschluss, int Funktion, string Oberbau, float g_X, float g_Y, float b_X, float b_Y, int AnschlussNorm, int AnschlussGegen)
            {
                this.Nr = Nr;
                this.spTrass = spTrass;
                this.Anschluss = Anschluss;
                this.Funktion = Funktion;
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
            public int StrElementNr;
            public bool StrNorm;
            public int RefTyp;
            public string Info;
            public streckenElement StrElement;

            public referenzElement(int ReferenzNr, int StrElement, bool StrNorm, int RefTyp, string Info)
            {
                this.ReferenzNr = ReferenzNr;
                StrElementNr = StrElement;
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

            public int StartRef;
            public string StartMod;
            public int ZielRef;
            public string ZielMod;

            public referenzElement Start;
            public referenzElement Ziel;

            public fahrStr(string FahrstrName, string FahrstrStrecke, int RglGgl, string FahrstrTyp, float Laenge, int StartRef, string StartMod, int ZielRef, string ZielMod)
            {
                this.FahrstrName = FahrstrName;
                this.FahrstrStrecke = FahrstrStrecke;
                this.RglGgl = RglGgl;
                this.FahrstrTyp = FahrstrTyp;
                this.Laenge = Laenge;


                this.StartRef = StartRef;
                this.StartMod = StartMod;
                this.ZielRef = ZielRef;
                this.ZielMod = ZielMod;
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

        public List<PunktXY> Huellkurve;
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

            Huellkurve = new List<PunktXY>();
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
                MessageBox.Show(e.Message, "XML Error: " + modName, MessageBoxButtons.OK);
                isSane = false;
            }

            if (modName == "Aulfingen2005")
            {

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
                    if (modXml.Name == "Huellkurve")
                    {
                        modXml.Read();

                        while (modXml.Read() && !(modXml.Name == "Huellkurve")){
                            if (modXml.Name == "PunktXYZ")
                            {
                                double rel_X = Convert.ToSingle(modXml.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                                double rel_Y = Convert.ToSingle(modXml.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);

                                Huellkurve.Add(new PunktXY(UTM_WE, UTM_NS, rel_X, rel_Y));
                            }
                        }
                    }
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
                        if (modName == "Aulfingen2005" && Nr == 9133)
                        {

                        }
                        float spTrass = Convert.ToSingle(modXml.GetAttribute("spTrass"), CultureInfo.InvariantCulture.NumberFormat);
                        int Anschluss = Convert.ToInt32(modXml.GetAttribute("Anschluss"));
                        int Funktion = Convert.ToInt32(modXml.GetAttribute("Fkt"));
                        string Oberbau = modXml.GetAttribute("Oberbau");

                        //if (Oberbau != null && Oberbau.Contains("B55")) continue;

                        while (!(modXml.Name == "g")) modXml.Read();
                        float g_X = Convert.ToSingle(modXml.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                        float g_Y = Convert.ToSingle(modXml.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);
                        while (!(modXml.Name == "b")) modXml.Read();
                        float b_X = Convert.ToSingle(modXml.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                        float b_Y = Convert.ToSingle(modXml.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);

                        g_X = UTM_WE + (g_X / 1000f);
                        g_Y = UTM_NS + (g_Y / 1000f);
                        b_X = UTM_WE + (b_X / 1000f);
                        b_Y = UTM_NS + (b_Y / 1000f);

                        while ((!(modXml.Name == "NachNorm")) && (!(modXml.Name == "StrElement")) && modXml.Read()) { }
                        int AnschlussNorm = Convert.ToInt32(modXml.GetAttribute("Nr"));
                        while ((!(modXml.Name == "NachGegen")) && (!(modXml.Name == "StrElement")) && modXml.Read()) { }
                        int AnschlussGegen = Convert.ToInt32(modXml.GetAttribute("Nr"));
                        
                        if(!(Funktion == 2))
                            StreckenElemente.Add(new streckenElement(Nr, spTrass, Anschluss, Funktion, Oberbau, g_X, g_Y, b_X, b_Y, AnschlussNorm, AnschlussGegen));
                        while ((!(modXml.NodeType == XmlNodeType.EndElement && modXml.Name == "StrElement")) && modXml.Read()) { }

                        
                    }
                    
                    if (modXml.Name == "Fahrstrasse")
                    {
                        if (modName == "Aulfingen2005")
                        {

                        }

                        string FahrstrName = modXml.GetAttribute("FahrstrName");
                        string FahrstrStrecke = modXml.GetAttribute("FahrstrStrecke");
                        int RglGgl = Convert.ToInt32(modXml.GetAttribute("RglGgl"));
                        string FahrstrTyp = modXml.GetAttribute("FahrstrTyp");
                        float Laenge = Convert.ToSingle(modXml.GetAttribute("Laenge"));

                        while (!(modXml.Name == "FahrstrStart")) modXml.Read();
                        int StartRef = Convert.ToInt32(modXml.GetAttribute("Ref"));
                        while (!(modXml.Name == "Datei")) modXml.Read();
                        string StartMod = modContainer.speicherortZuName(modXml.GetAttribute("Dateiname"), '\\');


                        while (!(modXml.Name == "FahrstrZiel")) modXml.Read();
                        int ZielRef = Convert.ToInt32(modXml.GetAttribute("Ref"));
                        while (!(modXml.Name == "Datei")) modXml.Read();
                        string ZielMod = modContainer.speicherortZuName(modXml.GetAttribute("Dateiname"), '\\');
                    

                        FahrStr.Add(new fahrStr(FahrstrName, FahrstrStrecke, RglGgl, FahrstrTyp, Laenge, StartRef, StartMod, ZielRef, ZielMod));
                    }
                }
                //else if (modXml.NodeType == XmlNodeType.EndElement && modXml.Name == "Strecke") break;
            }

            //Verlinke Referenzelemente mit Streckenelementen 
            foreach (var refEl in ReferenzElemente)
            {
                refEl.StrElement = sucheStrElement(refEl.StrElementNr);
            }

            timeKeeper.Stop();
            //List<int> oberbauTypen = new List<int>();
            //string OT = "";
            //foreach (var se in StreckenElemente)
            //{
            //    if (!(oberbauTypen.Contains(se.Funktion)))
            //    {
            //        oberbauTypen.Add(se.Funktion);
            //        OT += "- " + se.Funktion + "\n";
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

        public streckenElement sucheStrElement(int StreckenElementNr)
        {
            foreach (var strEl in StreckenElemente)
            {
                if (strEl.Nr == StreckenElementNr)
                {
                    return (strEl);
                }
            }

            return null;
        }
    }
}
