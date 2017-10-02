using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Rohform eines Streckenmoduls, direkt eingelesen aus ST3-Datei
    /// </summary>
    public class st3Modul
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
            public PunktXY(double abs_X, double abs_Y)
            {
                this.abs_X = abs_X;
                this.abs_Y = abs_Y;
            }

            public double distanceTo(PunktXY target)
            {
                double distX = target.abs_X - abs_X;
                double distY = target.abs_Y - abs_Y;

                return (Math.Sqrt(distX * distX + distY * distY));
            }
        }

        public class Signal
        {
            public string Name;
            public string Stellwerk;
            public string Betriebstelle;
            public int Signaltyp;
            public bool Gegengleis;
            public List<float> vSig;

            [Obsolete]
            public Signal(string Name, string Stellwerk, string Betriebstelle, int Signaltyp, bool Gegengleis)
            {
                this.Name = Name;
                this.Stellwerk = Stellwerk;
                this.Betriebstelle = Betriebstelle;
                this.Signaltyp = Signaltyp;
                this.Gegengleis = Gegengleis;
            }

            public Signal(XmlReader XML)
            {
                XML.Read();

                Name = XML.GetAttribute("Signalname");
                if (Name == null)
                    Name = "";
                Stellwerk = XML.GetAttribute("Stellwerk");
                Betriebstelle = XML.GetAttribute("NameBetriebsstelle");
                Signaltyp = Convert.ToInt32(XML.GetAttribute("SignalTyp"));

                vSig = new List<float>();

                while (XML.Read())
                {
                    if (XML.Name == "HsigBegriff")
                        vSig.Add(Convert.ToSingle(XML.GetAttribute("HsigGeschw"), CultureInfo.InvariantCulture.NumberFormat));
                }

                XML.Close();
            }

            public override string ToString()
            {
                return "Signal " + Name + " in " + Betriebstelle;
            }
        }

        public class Anschluss
        {
            public int element;
            public string modul;

            public Anschluss(int element, string modul)
            {
                this.element = element;
                this.modul = modul;
            }
        }

        public class streckenElement
        {
            public st3Modul Modul;
            public bool verlinkt;

            public int Nr;

            public float spTrass;
            public float SigVmax;
            public float km;

            public int Anschluss;
            public int Funktion;
            public string Oberbau;
            public referenzElement signalReferenz;

            public double g_X;
            public double g_Y;
            public double b_X;
            public double b_Y;

            public List<Anschluss> AnschlussNorm;
            public List<Anschluss> AnschlussGegen;

            public string betriebstStelleNorm;
            public string betriebstStelleGegen;


            public Signal SignalNorm;
            public Signal SignalGegen;

            public streckenElement(XmlReader partialXmlReader, st3Modul Modul)
            {
                this.Modul = Modul;

                partialXmlReader.Read();

                Nr = Convert.ToInt32(partialXmlReader.GetAttribute("Nr"));

                spTrass = Convert.ToSingle(partialXmlReader.GetAttribute("spTrass"), CultureInfo.InvariantCulture.NumberFormat);
                SigVmax = spTrass;

                Anschluss = Convert.ToInt32(partialXmlReader.GetAttribute("Anschluss"));
                Funktion = Convert.ToInt32(partialXmlReader.GetAttribute("Fkt"));
                Oberbau = partialXmlReader.GetAttribute("Oberbau");


                AnschlussNorm = new List<Anschluss>();
                AnschlussGegen = new List<Anschluss>();

                betriebstStelleNorm = "";
                betriebstStelleGegen = "";


                while (partialXmlReader.Read())
                {
                    if ((partialXmlReader.NodeType != XmlNodeType.Whitespace) && (partialXmlReader.NodeType != XmlNodeType.EndElement))
                    {
                        if (partialXmlReader.Name == "g")
                        {
                            g_X = Convert.ToDouble(partialXmlReader.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                            g_Y = Convert.ToDouble(partialXmlReader.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);

                            g_X = Modul.UTM_WE + (g_X / 1000);
                            g_Y = Modul.UTM_NS + (g_Y / 1000);
                        }
                        else if (partialXmlReader.Name == "b")
                        {
                            b_X = Convert.ToDouble(partialXmlReader.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                            b_Y = Convert.ToDouble(partialXmlReader.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);

                            b_X = Modul.UTM_WE + (b_X / 1000);
                            b_Y = Modul.UTM_NS + (b_Y / 1000);
                        }
                        else if (partialXmlReader.Name == "InfoNormRichtung")
                        {
                            SignalNorm = leseRichtungsInfo(partialXmlReader.ReadSubtree(), out betriebstStelleNorm);
                        }
                        else if (partialXmlReader.Name == "InfoGegenRichtung")
                        {
                            SignalGegen = leseRichtungsInfo(partialXmlReader.ReadSubtree(), out betriebstStelleGegen);
                        }
                        else if (partialXmlReader.Name == "NachNorm")
                        {
                            AnschlussNorm.Add(new Anschluss(Convert.ToInt32(partialXmlReader.GetAttribute("Nr")), ""));
                        }
                        else if (partialXmlReader.Name == "NachGegen")
                        {
                            AnschlussGegen.Add(new Anschluss(Convert.ToInt32(partialXmlReader.GetAttribute("Nr")), ""));
                        }
                        //else if (partialXmlReader.Name == "NachNormModul")
                        //{
                        //    int element = Convert.ToInt32(partialXmlReader.GetAttribute("Nr"));
                        //    partialXmlReader.Read();
                        //    partialXmlReader.Read();
                        //    string modul = partialXmlReader.GetAttribute("Dateiname");
                        //    AnschlussNorm.Add(new Anschluss(element, modul));
                        //}
                        //else if (partialXmlReader.Name == "NachGegenModul")
                        //{
                        //    int element = Convert.ToInt32(partialXmlReader.GetAttribute("Nr"));
                        //    partialXmlReader.Read();
                        //    partialXmlReader.Read();
                        //    string modul = partialXmlReader.GetAttribute("Dateiname");
                        //    if (modul.Contains("Olsberg"))
                        //    {

                        //    }
                        //    AnschlussGegen.Add(new Anschluss(element, modul));
                        //}
                    }
                }

                partialXmlReader.Close();
            }

            private Signal leseRichtungsInfo(XmlReader partialXmlReader, out string betriebstelle)
            {
                partialXmlReader.Read();
                Signal tmpSignal = null;
                betriebstelle = "";

                SigVmax = Convert.ToSingle(partialXmlReader.GetAttribute("vMax"), CultureInfo.InvariantCulture.NumberFormat);
                km = Convert.ToSingle(partialXmlReader.GetAttribute("km"), CultureInfo.InvariantCulture.NumberFormat);
                

                while (partialXmlReader.Read())
                {
                    if (partialXmlReader.Name == "Ereignis" && partialXmlReader.IsStartElement())
                    {
                        if ((partialXmlReader.GetAttribute("Er") == "1000007" || partialXmlReader.GetAttribute("Er") == "1000010") && partialXmlReader.GetAttribute("Beschr") != "")
                        {
                            betriebstelle = partialXmlReader.GetAttribute("Beschr");
                            if (betriebstelle == null /*|| betriebstelle.Contains('@')*/)
                                betriebstelle = "";

                            //if (betriebstelle != "")
                            //    Console.WriteLine(betriebstelle);
                        }
                    }
                    if (partialXmlReader.Name == "Signal" && partialXmlReader.IsStartElement())
                    {
                        tmpSignal = new Signal(partialXmlReader.ReadSubtree());
                        //string Name = partialXmlReader.GetAttribute("Signalname");
                        //if (Name == null)
                        //    Name = "";
                        //string Stellwerk = partialXmlReader.GetAttribute("Stellwerk");
                        //string Betriebstelle = partialXmlReader.GetAttribute("NameBetriebsstelle");
                        //int Signaltyp = Convert.ToInt32(partialXmlReader.GetAttribute("SignalTyp"));
                        //tmpSignal = new Signal(Name, Stellwerk, Betriebstelle, Signaltyp, false);
                    } 
                }

                partialXmlReader.Close();

                return (tmpSignal);
            }

            public override string ToString()
            {

                return ("StrEl " + Nr);
            }
        }

        public class referenzElement
        {
            public st3Modul Modul;

            public int ReferenzNr;
            public int StrElementNr;
            public bool StrNorm;
            public int RefTyp;
            public string Info;
            public streckenElement StrElement;

            public Signal Signal;
            public PunktXY SignalCoord;
            public bool istStart;
            public bool istZiel;

            public List<fahrStr> abgehendeFahrstraßen;
            public List<referenzElement> wendeSignale;
            public int fsAnzahl = 0;

            [Obsolete]
            public referenzElement(st3Modul Modul, int ReferenzNr, int StrElement, bool StrNorm, int RefTyp, string Info)
            {
                this.Modul = Modul;

                this.ReferenzNr = ReferenzNr;
                StrElementNr = StrElement;
                this.StrNorm = StrNorm;
                this.RefTyp = RefTyp;
                this.Info = Info;

                abgehendeFahrstraßen = new List<fahrStr>();
            }

            public referenzElement(XmlReader partialXmlReader, st3Modul Modul)
            {
                //RefType == "1" - Modulgrenzen
                //RefType == "2" - Register
                //RefType == "3" - Weichen
                //RefType == "4" - Signale und andere streckengebundene Objekte
                //RefType == "5" - Auflösepunkte
                //RefType == "6" - Signalhaltfall

                abgehendeFahrstraßen = new List<fahrStr>();

                this.Modul = Modul;

                partialXmlReader.Read();

                ReferenzNr = Convert.ToInt32(partialXmlReader.GetAttribute("ReferenzNr"));
                StrElementNr = Convert.ToInt32(partialXmlReader.GetAttribute("StrElement"));

                if (partialXmlReader.GetAttribute("StrNorm") == "1") StrNorm = true;
                else StrNorm = false;
                RefTyp = Convert.ToInt32(partialXmlReader.GetAttribute("RefTyp"));
                Info = partialXmlReader.GetAttribute("Info");
                if (RefTyp == 1)
                {
                    string verbindungsName = Info;
                    verbindungsName = DataConstructor.speicherortZuName(verbindungsName, '\\');
                    if (!(Modul.VerbindungenStr.Contains(verbindungsName)))
                        Modul.VerbindungenStr.Add(verbindungsName);
                }
            }

            public override string ToString()
            {
                string prefix = "";
                if (Signal != null)
                    prefix = Signal.Signaltyp.ToString() + " - ";

                switch (RefTyp)
                {
                    case 0:
                        return (prefix + "AGP " + Info);
                    case 1:
                        return (prefix + "MDG " + Info);
                    case 2:
                        return (prefix + "REG " + Info);
                    case 3:
                        return (prefix + "WEI " + Info);
                    case 4:
                        return (prefix + "SIG " + Info);
                    case 5:
                        return (prefix + "ALP " + Info);
                    default:
                        return (prefix + "N/A " + Info);
                }
            }
        }

        public class fahrStr
        {
            public class fstrSignal
            {
                public int RefInt;
                public string RefModString;
                public referenzElement Ref;
                public int SignalZeile;


                public fstrSignal(XmlReader partialXmlReader)
                {

                    while (partialXmlReader.Read())
                    {
                        if (partialXmlReader.NodeType != XmlNodeType.EndElement && partialXmlReader.Name == "FahrstrSignal")
                        {
                            RefInt = Convert.ToInt32(partialXmlReader.GetAttribute("Ref"));
                            SignalZeile = Convert.ToInt32(partialXmlReader.GetAttribute("FahrstrSignalZeile")); 
                        }
                        
                        if (partialXmlReader.Name == "Datei")
                            RefModString = DataConstructor.speicherortZuName(partialXmlReader.GetAttribute("Dateiname"), '\\');
                    }
                }
            }

            public class fstrWeiche
            {
                public int referenzIndex;
                public string RefModString;
                public referenzElement Ref;
                public int strEInt = -1;

                public int weichenlage;


                public fstrWeiche(XmlReader partialXmlReader)
                {
                    while (partialXmlReader.Read())
                    {
                        if (partialXmlReader.NodeType != XmlNodeType.EndElement && partialXmlReader.Name == "FahrstrWeiche")
                        {
                            referenzIndex = Convert.ToInt32(partialXmlReader.GetAttribute("Ref"));
                            weichenlage = Convert.ToInt32(partialXmlReader.GetAttribute("FahrstrWeichenlage")); 
                        }
                        
                        if (partialXmlReader.Name == "Datei")
                            RefModString = DataConstructor.speicherortZuName(partialXmlReader.GetAttribute("Dateiname"), '\\');
                    }
                }


                public override string ToString()
                {
                    if (strEInt == -1)
                        return "Wr" + referenzIndex + "|" + weichenlage;
                    else
                        return "W" + strEInt + "|" + weichenlage;
                }
            }

            public string FahrstrName;
            public string FahrstrStrecke;
            public int RglGgl;
            public string FahrstrTyp;
            public float Laenge;
            public double LaengeGewichtet;
            public double wichtung;

            public int StartRef;
            public string StartMod_Str;
            public int ZielRef;
            public string ZielMod_Str;

            public referenzElement Start;
            public st3Modul StartMod;
            public referenzElement Ziel;
            public st3Modul ZielMod;

            public List<fahrStr> folgestraßen;
            public List<referenzElement> wendesignale;

            public bool wendeSignaleBestimmt = false;

            public List<fstrSignal> signale;
            public List<fstrWeiche> weichen;

            public float Durchschnittsgeschwindigkeit;

            /// <summary>
            /// Liste aller Referenzen auf Fahrstraße. Wird zur Erkennung von feindlichen Fahrstraßen benutzt. Wird extern bestückt.
            /// </summary>
            public List<int> referenzenInFahrstraße;

            
            //Sollte nur für nicht aus XML eingelesene Fahrstraßen (also Wendehilfsfahrstraßen) eingesetzt werden
            public fahrStr(string FahrstrName, string FahrstrStrecke, int RglGgl, string FahrstrTyp, float Laenge, int StartRef, string StartMod_Str, int ZielRef, string ZielMod_Str)
            {
                this.FahrstrName = FahrstrName;
                this.FahrstrStrecke = FahrstrStrecke;
                this.RglGgl = RglGgl;
                this.FahrstrTyp = FahrstrTyp;
                this.Laenge = Laenge;
                this.LaengeGewichtet = gewichteLänge();

                this.StartRef = StartRef;
                this.StartMod_Str = StartMod_Str;
                this.ZielRef = ZielRef;
                this.ZielMod_Str = ZielMod_Str;

            }

            public fahrStr(XmlReader partialXmlReader)
            {
                partialXmlReader.Read();
                FahrstrName = partialXmlReader.GetAttribute("FahrstrName");
                FahrstrStrecke = partialXmlReader.GetAttribute("FahrstrStrecke");
                RglGgl = Convert.ToInt32(partialXmlReader.GetAttribute("RglGgl"));
                FahrstrTyp = partialXmlReader.GetAttribute("FahrstrTyp");
                Laenge = Convert.ToSingle(partialXmlReader.GetAttribute("Laenge"), CultureInfo.InvariantCulture.NumberFormat);
                LaengeGewichtet = gewichteLänge();

                while (!(partialXmlReader.Name == "FahrstrStart")) partialXmlReader.Read();
                StartRef = Convert.ToInt32(partialXmlReader.GetAttribute("Ref"));
                while (!(partialXmlReader.Name == "Datei")) partialXmlReader.Read();
                StartMod_Str = DataConstructor.speicherortZuName(partialXmlReader.GetAttribute("Dateiname"), '\\');


                while (!(partialXmlReader.Name == "FahrstrZiel")) partialXmlReader.Read();
                ZielRef = Convert.ToInt32(partialXmlReader.GetAttribute("Ref"));
                while (!(partialXmlReader.Name == "Datei")) partialXmlReader.Read();
                ZielMod_Str = DataConstructor.speicherortZuName(partialXmlReader.GetAttribute("Dateiname"), '\\');

                signale = new List<fstrSignal>();
                weichen = new List<fstrWeiche>();
                while (partialXmlReader.Read())
                {
                    if (partialXmlReader.Name == "FahrstrWeiche")
                        weichen.Add(new fstrWeiche(partialXmlReader.ReadSubtree()));

                    if (partialXmlReader.Name == "FahrstrSignal")
                        signale.Add(new fstrSignal(partialXmlReader.ReadSubtree()));
                }
            }

            /// <summary>
            /// Erstellt Wendehilfsfahrstraße
            /// </summary>
            public fahrStr(referenzElement wendeStart, referenzElement wendeZiel)
            {
                FahrstrName = "Wendehilfsfahrstraße";
                FahrstrStrecke = "0";
                RglGgl = 4;
                FahrstrTyp = "TypWende";
                Laenge = (float)wendeStart.SignalCoord.distanceTo(wendeZiel.SignalCoord) * 1000f;

                StartRef = wendeStart.ReferenzNr;
                StartMod_Str = wendeStart.Modul.modName;
                StartMod = wendeStart.Modul;
                Start = wendeStart;

                ZielRef = wendeZiel.ReferenzNr;
                ZielMod_Str = wendeZiel.Modul.modName;
                ZielMod = wendeZiel.Modul;
                Ziel = wendeZiel;

                folgestraßen = wendeZiel.abgehendeFahrstraßen;
            }

            double gewichteLänge()
            {
                //TODO: Weichen mit in Gewichtung aufnehmen?
                wichtung = 1;
                if (RglGgl == 0) // Bahnhofsgleis
                    wichtung *= 1f;
                if (RglGgl == 1) // Eingleisige Strecke
                    wichtung *= 1f;
                if (RglGgl == 2) // Endet im Regelgleis
                    wichtung *= 1f;
                if (RglGgl == 3) // Endet im Gegengleis
                    wichtung *= 1.2f;
                if (FahrstrTyp == "TypWende")
                    wichtung *= 1f;

                return (Laenge * wichtung);
            }

            ////TODO: Geschwindigkeit genauer berechnen
            //public float berechneFahrdauerGewichtet(float vMax)
            //{
            //    vMax = vMaxBestimmen(vMax);

            //    //if (FahrstrName.Contains("Warburg"))
            //    //    Console.WriteLine(FahrstrName + "  " + vMax + " " + RglGgl);

            //    //if (vMax > 45)
            //    //    MessageBox.Show(this.ToString() + " - " + vMax);


            //    return (LaengeGewichtet / vMax);

            //    //Ganz einfach:
            //    //return (LaengeGewichtet / vMax);
            //}

            public float berechneFahrdauer(float vMax)
            {
                vMax = vMaxBestimmen(vMax);

                //Console.WriteLine(this.ToString() + " - " + vMax);

                return (Laenge / vMax);

                //Ganz einfach:
                //return (LaengeGewichtet / vMax);
            }

            public float vMaxBestimmen(float vMax)
            {
                float vMaxSig = -1f;

                if (signale != null)
                {
                    foreach (var fstrSignal in signale)
                    {
                        if ((fstrSignal.SignalZeile != 0))
                            vMaxSig = fstrSignal.Ref.Signal.vSig[fstrSignal.SignalZeile - 1];
                    }
                }

                if (vMaxSig <= 0)
                    vMaxSig = this.Start.StrElement.SigVmax;

                

                if (FahrstrTyp == "TypWende")
                    vMaxSig = 5; //Für Wende wird 20km/h angenommen.
                if ((RglGgl == 3) && (vMaxSig > 20 ))
                    vMaxSig = 5; //Für schnelle Gegengleisfahrten wird 20km/h angenommen.

                if (vMaxSig <= 0)
                    vMaxSig = 5;

                //if ((FahrstrTyp != "TypLZB") && (vMaxSig > 45)) //Bei Fahrstraßen mit zu wenig Infos ausserhalb der LZB wird vMax 160km/h angenommen.
                //    vMaxSig = 45;

                if (vMaxSig > 0)
                    vMax = Math.Min(vMax, vMaxSig);
                else
                    Console.WriteLine("NLF: " + FahrstrName);



                return vMax;
            }

            

            public override string ToString()
            {
                return FahrstrName;
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

        public double drawDist;

        public List<PunktUTM> Huellkurve;
        public List<streckenElement> StreckenElemente;
        public streckenElement[] StreckenElementeNachNr;
        public List<referenzElement> ReferenzElemente;
        public referenzElement[] ReferenzElementeNachNr;
        public List<string> löschFahrstraßen;
        public List<fahrStr> FahrStraßen;
        public List<referenzElement> Signale;
        public List<int> AlleSignale;
        public List<int> StartSignale;
        public List<int> ZielSignale;

        /// <summary>
        /// Enthält die umliegenden Module als String. Nach Verlinkung nicht mehr aktuell.
        /// </summary>
        public List<string> VerbindungenStr;
        /// <summary>
        /// Enthält Pointer zu den umliegenden Modulen.
        /// </summary>
        public List<st3Modul> Verbindungen;

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

        public st3Modul(string modulePath)
        {
            modPath = modulePath.Replace('/', '\\');
            modPath = modPath.Substring(modPath.IndexOf("Routes"));
            modName = DataConstructor.speicherortZuName(modPath, '\\');

            Huellkurve = new List<PunktUTM>();

            StreckenElemente = new List<streckenElement>();
            

            ReferenzElemente = new List<referenzElement>();
            löschFahrstraßen = new List<string>();
            FahrStraßen = new List<fahrStr>();
            Signale = new List<referenzElement>();
            AlleSignale = new List<int>();
            StartSignale = new List<int>();
            ZielSignale = new List<int>();
            VerbindungenStr = new List<string>();
            NetzGrenze = false;
            wichtig = false;
            selected = false;
            isDetailed = true;

            isSane = true;
            readData(modulePath);
            //try
            //{
            //    readData(modulePath);
            //}
            //catch (Exception)
            //{
            //    //MessageBox.Show(e.Message, "XML Error: " + modName, MessageBoxButtons.OK);
            //    isSane = false;
            //}
        }

        void readData(string Speicherort)
        {
            var timeKeeper = new System.Diagnostics.Stopwatch();
            timeKeeper.Start();

            XmlReader modXml = XmlReader.Create(Speicherort);

            modXml.Read();
            if (modXml.NodeType == XmlNodeType.XmlDeclaration) modXml.Read();
            while (!(modXml.NodeType == XmlNodeType.Element && modXml.Name == "Strecke")) modXml.Read();


            int größteStreckenelementNummer = 0;
            int größteReferenzelementNummer = 0;


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
                    if (modXml.Name == "LoeschFahrstrasse")
                    {
                        löschFahrstraßen.Add(modXml.GetAttribute("FahrstrName"));
                    }
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

                        while (modXml.Read() && !(modXml.Name == "Huellkurve"))
                        {
                            if (modXml.Name == "PunktXYZ")
                            {
                                double rel_X = Convert.ToSingle(modXml.GetAttribute("X"), CultureInfo.InvariantCulture.NumberFormat);
                                double rel_Y = Convert.ToSingle(modXml.GetAttribute("Y"), CultureInfo.InvariantCulture.NumberFormat);

                                Huellkurve.Add(new PunktUTM(UTM_WE, UTM_NS, rel_X, rel_Y));
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
                        var RefEl = new referenzElement(modXml.ReadSubtree(), this);

                        if (RefEl.ReferenzNr > größteReferenzelementNummer)
                            größteReferenzelementNummer = RefEl.ReferenzNr;

                        ReferenzElemente.Add(RefEl);

                        ////RefType == "1" - Modulgrenzen
                        ////RefType == "2" - Register
                        ////RefType == "3" - Weichen
                        ////RefType == "4" - Signale und andere streckengebundene Objekte
                        ////RefType == "5" - Auflösepunkte
                        ////RefType == "6" - Signalhaltfall


                        //int ReferenzNr = Convert.ToInt32(modXml.GetAttribute("ReferenzNr"));
                        //int StrElement = Convert.ToInt32(modXml.GetAttribute("StrElement"));
                        //bool StrNorm;
                        //if (modXml.GetAttribute("StrNorm") == "1")  StrNorm = true;
                        //else  StrNorm = false;
                        //int RefTyp = Convert.ToInt32(modXml.GetAttribute("RefTyp"));
                        //string Info = modXml.GetAttribute("Info");

                        //ReferenzElemente.Add(new referenzElement(this, ReferenzNr, StrElement, StrNorm, RefTyp, Info));
                        //if (RefTyp == 1)
                        //{
                        //    string verbindungsName = Info;
                        //    verbindungsName = modContainer.speicherortZuName(verbindungsName, '\\');
                        //    if (!(VerbindungenStr.Contains(verbindungsName)))
                        //        VerbindungenStr.Add(verbindungsName);
                        //}
                    }

                    if (modXml.Name == "StrElement")
                    {
                        var tmpStrElement = new streckenElement(modXml.ReadSubtree(), this);

                        if (tmpStrElement.Nr > größteStreckenelementNummer)
                            größteStreckenelementNummer = tmpStrElement.Nr;
                        
                        StreckenElemente.Add(tmpStrElement);

                    }

                    if (modXml.Name == "Fahrstrasse")
                    {
                        var fahrstraße = new fahrStr(modXml.ReadSubtree());
                        if (!löschFahrstraßen.Contains(fahrstraße.FahrstrName))
                            FahrStraßen.Add(fahrstraße);
                    }
                }
                //else if (modXml.NodeType == XmlNodeType.EndElement && modXml.Name == "Strecke") break;

                
            }

            //Erstelle Array mit Streckenelementen
            StreckenElementeNachNr = new streckenElement[größteStreckenelementNummer + 1];
            foreach (var tmpStrElement in StreckenElemente)
            {
                StreckenElementeNachNr[tmpStrElement.Nr] = tmpStrElement;
            }

            
            //Verlinke Referenzelemente mit Streckenelementen 
            var schlechteReferenzen = new List<referenzElement>();
            foreach (var refEl in ReferenzElemente)
            {
                refEl.StrElement = sucheStrElement(refEl.StrElementNr);
                if (refEl.StrElement == null)
                {
                    schlechteReferenzen.Add(refEl);
                }
            }

            foreach (var badRef in schlechteReferenzen)
            {
                ReferenzElemente.Remove(badRef);
            }


            //Erstelle Array mit Referenzelementen
            ReferenzElementeNachNr = new referenzElement[größteReferenzelementNummer + 1];
            foreach (var tmpRefElement in ReferenzElemente)
            {
                ReferenzElementeNachNr[tmpRefElement.ReferenzNr] = tmpRefElement;
            }


            //Signale zum einfacheren Zugriff standardisieren.
            foreach (var refEl in ReferenzElemente)
            {
                if (refEl.RefTyp == 4)
                {

                    refEl.StrElement.signalReferenz = refEl;

                    if (refEl.StrElement.SignalNorm == null && refEl.StrElement.SignalGegen == null) //Wenn kein Signal existiert
                    {
                        continue;
                    }

                    if (refEl.StrElement.SignalNorm == null ^ refEl.StrElement.SignalGegen == null) //Wenn nur eins der Signale existiert
                    {
                        if (refEl.StrElement.SignalGegen == null)
                        {
                            refEl.Signal = refEl.StrElement.SignalNorm;
                            refEl.StrNorm = true;
                        }
                        else
                        {
                            refEl.Signal = refEl.StrElement.SignalGegen;
                            refEl.StrNorm = false;
                        }
                    }
                    else if (refEl.StrElement.SignalNorm.Signaltyp == 0 ^ refEl.StrElement.SignalGegen.Signaltyp == 0) //Wenn eins der Signale kein anerkanntes Signal ist
                    {
                        if (refEl.StrElement.SignalGegen.Signaltyp == 0)
                        {
                            refEl.Signal = refEl.StrElement.SignalNorm;
                            refEl.StrNorm = true;
                        }
                        else
                        {
                            refEl.Signal = refEl.StrElement.SignalGegen;
                            refEl.StrNorm = false;
                        }
                    }
                    else if (refEl.StrElement.SignalNorm != null && refEl.StrElement.SignalGegen != null) //Wenn beide Signale existieren
                    {
                        if (refEl.StrElement.SignalNorm.Name != "" && refEl.Info.Contains(refEl.StrElement.SignalNorm.Name))
                        {
                            refEl.Signal = refEl.StrElement.SignalNorm;
                            refEl.StrNorm = true;
                        }
                        else if (refEl.StrElement.SignalGegen.Name != "" && refEl.Info.Contains(refEl.StrElement.SignalGegen.Name))
                        {
                            refEl.Signal = refEl.StrElement.SignalGegen;
                            refEl.StrNorm = false;
                        }
                        else if (refEl.StrNorm)
                        {
                            refEl.Signal = refEl.StrElement.SignalNorm;
                        }
                        else
                        {
                            refEl.Signal = refEl.StrElement.SignalGegen;
                        }
                    }



                    //Zum Index hinzufügen
                    if (refEl.Signal != null)
                    {
                        Signale.Add(refEl);
                    }
                }

                //Signalkoordinaten setzen
                if (refEl.StrNorm)
                {
                    refEl.SignalCoord = new PunktXY(refEl.StrElement.b_X, refEl.StrElement.b_Y);
                }
                else
                {
                    refEl.SignalCoord = new PunktXY(refEl.StrElement.g_X, refEl.StrElement.g_Y);
                }
            }



            timeKeeper.Stop();
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
            return (StreckenElementeNachNr[StreckenElementNr]);
        }

        public override string ToString()
        {
            return ("Modul " + modName);
        }
    }
}
