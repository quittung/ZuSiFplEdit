using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Vollständige Fahrstraße - Verbindet zwei Elemente
    /// </summary>
    class DataConstructor
    {
        string pfad;
        Datensatz datenFertig;
        List<st3Modul> datenRohform;

        public string fortschrittMeldung = "Initialisiere...";
        public int fortschrittNumerisch = 0;
        public int fortschrittMaximal = 100;

        public DataConstructor(string pfad, Datensatz datensatz)
        {
            this.pfad = pfad;
            this.datenFertig = datensatz;
        }

        /// <summary>
        /// Startet das Einlesen der st3-Dateien und ihre Verarbeitung. Sollte in neuem Thread aufgerufen werden
        /// </summary>
        public void datenEinlesen(Object threadContext)
        {
            st3Einlesen();
            verbindeModule();
            datenFertig.moduleBereit = true;

            erzeugeElemente();
            verbindeElemente();
            datenFertig.elementeBereit = true;

            sammleSignale();
            erzeugeSignale();
            datenFertig.signaleBereit = true;

            erzeugeFahrstraßen();
            erzeugeWendefahrstraßen();
            verlinkeFolgestraßen();
            datenFertig.fahrstraßenBereit = true;

            fortschrittMeldung = "";
        }

        /// <summary>
        /// Liest st3-Dateien ein und erzeugt die zugehörigen umformatierten Streckenmodule.
        /// </summary>
        void st3Einlesen()
        {
            fortschrittMeldung = "Suche nach Modulen in " + pfad;
            List<string> modulPfade = erzeugeST3Liste(pfad);
            fortschrittMaximal = modulPfade.Count;

            fortschrittMeldung = "Lese Module...";
            datenRohform = new List<st3Modul>();
            foreach (var modulPfad in modulPfade)
            {
                fortschrittMeldung = "Lese Modul " + speicherortZuName(modulPfad, '\\');
                datenRohform.Add(new st3Modul(modulPfad));
                fortschrittNumerisch++;
            }

            fortschrittMeldung = "Erzeuge Module...";
            foreach (var modul in datenRohform)
            {
                var modulFertig = new streckenModul(modul.modPath, modul.modName, new PunktUTM(modul.UTM_WE, modul.UTM_NS, modul.UTM_Z1, modul.UTM_Z2), modul.Huellkurve);
                datenFertig.module.Add(modulFertig);
            }
        }
        
        /// <summary>
        /// Trägt Verbindungen und zugehörige Metadaten in umformatierte Streckenmodule ein.
        /// </summary>
        void verbindeModule()
        {
            fortschrittMeldung = "Verbinde Module...";
            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);
                
                foreach (var verbindung_str in modulRoh.VerbindungenStr)
                {
                    var verbindung = datenFertig.sucheModul(verbindung_str);
                    if (verbindung != null)
                    {
                        if (!modulFertig.nachbarn.Contains(verbindung))
                            modulFertig.nachbarn.Add(verbindung);

                        if (!verbindung.nachbarn.Contains(modulFertig))
                            verbindung.nachbarn.Add(modulFertig);
                    }
                }
            }

            //Erkenne Knoten- und Endpunkte
            foreach (var modulFertig in datenFertig.module)
            {
                if (modulFertig.nachbarn.Count != 2)
                    modulFertig.knotenpunkt = true;
            }
        }

        /// <summary>
        /// Trägt Streckenelemente in umformatierte Streckenmodule ein.
        /// </summary>
        void erzeugeElemente()
        {
            fortschrittMeldung = "Verarbeite Streckenelemente...";
            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);

                int refNrMax = 0;

                foreach (var elementRoh in modulRoh.StreckenElemente)
                {
                    var endpunktB = new PunktUTM(elementRoh.b_X, elementRoh.b_Y);
                    var endpunktG = new PunktUTM(elementRoh.g_X, elementRoh.g_Y);

                    var element = new streckenModul.Element(elementRoh.Nr, elementRoh.Funktion, elementRoh.km, elementRoh.spTrass, endpunktB, endpunktG);
                    modulFertig.elemente.Add(element);
                    
                    if (element.nummer > refNrMax)
                        refNrMax = element.nummer;
                }

                modulFertig.elementeLookup = new streckenModul.Element[refNrMax + 1];
                foreach (var element in modulFertig.elemente)
                {
                    modulFertig.elementeLookup[element.nummer] = element;
                }
            }
        }

        /// <summary>
        /// Verbindet alle Streckenelemente miteinander.
        /// </summary>
        void verbindeElemente()
        {
            fortschrittMeldung = "Verbinde Streckenelemente...";
            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);

                foreach (var elementRoh in modulRoh.StreckenElemente)
                {
                    if (elementRoh.AnschlussNormInt.Count != 0)
                        foreach (var anschluss in elementRoh.AnschlussNormInt)
                        {
                            modulFertig.elementeLookup[elementRoh.Nr].anschlüsse[0].Add(modulFertig.elementeLookup[anschluss]);
                        }

                    if (elementRoh.AnschlussGegenInt.Count != 0)
                        foreach (var anschluss in elementRoh.AnschlussGegenInt)
                        {
                            modulFertig.elementeLookup[elementRoh.Nr].anschlüsse[1].Add(modulFertig.elementeLookup[anschluss]);
                        }
                }

                modulFertig.elementeBereit = true;
            }
        }

        /// <summary>
        /// Stellt Listen mit Signalen zusammen
        /// </summary>
        void sammleSignale()
        {
            fortschrittMeldung = "Sammle Signale...";

            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);

                //Erstelle Liste mit allen verwendeten Signalen
                //TODO: Fahrstraßen, die in anderen Modulen starten und hier komplett enden, werden nicht eingelesen.
                foreach (var fahrstraßeRoh in modulRoh.FahrStr)
                {
                    var signalRef = fahrstraßeRoh.StartRef;
                    var signalModul = findeModulRoh(fahrstraßeRoh.StartMod_Str);
                    List<int> signalListe;

                    if (signalModul != null)
                    {
                        signalListe = signalModul.AlleSignale;
                        if (!signalListe.Contains(signalRef))
                            signalListe.Add(signalRef);

                        signalListe = signalModul.StartSignale;
                        if (!signalListe.Contains(signalRef))
                            signalListe.Add(signalRef);
                    }
                    

                    signalRef = fahrstraßeRoh.ZielRef;
                    signalModul = findeModulRoh(fahrstraßeRoh.ZielMod_Str);
                    if (signalModul != null)
                    {
                        signalListe = signalModul.AlleSignale;
                        if (!signalListe.Contains(signalRef))
                            signalListe.Add(signalRef);

                        signalListe = signalModul.ZielSignale;
                        if (!signalListe.Contains(signalRef))
                            signalListe.Add(signalRef);
                    }
                }
            }
        }
        
        /// <summary>
        /// Erzeugt Signale aus vorher erstellten Listen
        /// </summary>
        void erzeugeSignale()
        {
            fortschrittMeldung = "Erzeuge Signale...";

            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);

                //Erzeuge Signal-Instanzen
                int signalRefMax = 0;
                foreach (var signalReferenz in modulRoh.AlleSignale)
                {
                    if (signalReferenz > signalRefMax)
                        signalRefMax = signalReferenz;

                    var signalRoh = modulRoh.ReferenzElementeNachNr[signalReferenz];
                    var name = signalRoh.ToString();
                    var betriebsstelle = "";
                    if (signalRoh.Signal != null)
                    {
                        betriebsstelle = signalRoh.Signal.Betriebstelle;
                        name = signalRoh.Signal.Name;
                    }
                        

                    var signalFertig = new streckenModul.Signal(signalReferenz, signalRoh.ToString(), name, betriebsstelle, modulFertig, modulFertig.elementeLookup[signalRoh.StrElementNr], 1 - Convert.ToInt32(signalRoh.StrNorm));
                    
                    modulFertig.signale.Add(signalFertig);
                }

                //Signale in andere Listen und Arrays eintragen
                modulFertig.signaleLookup = new streckenModul.Signal[signalRefMax + 1];
                foreach (var signal in modulFertig.signale)
                {
                    modulFertig.signaleLookup[signal.nummer] = signal;
                }

                foreach (var signalReferenz in modulRoh.StartSignale)
                {
                    var signal = modulFertig.signaleLookup[signalReferenz];
                    signal.istStart = true;
                    modulFertig.signaleStart.Add(signal);
                }

                foreach (var signalReferenz in modulRoh.ZielSignale)
                {
                    var signal = modulFertig.signaleLookup[signalReferenz];
                    signal.istZiel = true;
                    modulFertig.signaleZiel.Add(signal);
                }

                foreach (var signal in modulFertig.signale)
                {
                    if (signal.istStart && signal.istZiel)
                        modulFertig.signaleZwischen.Add(signal);
                }

                modulFertig.signaleBereit = true;
            }
        }

        /// <summary>
        /// Erzeugt normale Fahrstraßen aus dem Rohdatenbestand
        /// </summary>
        void erzeugeFahrstraßen()
        {
            fortschrittMeldung = "Erzeuge Fahrstraßen...";

            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);

                foreach (var fahrstraßeRoh in modulRoh.FahrStr)
                {
                    var startModul = datenFertig.sucheModul(fahrstraßeRoh.StartMod_Str);
                    if (startModul == null)
                        continue;
                    var startSignal = startModul.signaleLookup[fahrstraßeRoh.StartRef];

                    var zielModul = datenFertig.sucheModul(fahrstraßeRoh.ZielMod_Str);
                    if (zielModul == null)
                        continue;
                    var zielSignal = zielModul.signaleLookup[fahrstraßeRoh.ZielRef];

                    double vMax = startSignal.streckenelement.vMax;

                    var fahrstraßeFertig = new streckenModul.Fahrstraße(startSignal, zielSignal, fahrstraßeRoh.FahrstrName, fahrstraßeRoh.FahrstrTyp, fahrstraßeRoh.RglGgl, fahrstraßeRoh.Laenge, fahrstraßeRoh.wichtung, vMax);
                    modulFertig.fahrstraßen.Add(fahrstraßeFertig);
                }
            }
        }

        /// <summary>
        /// Erzeugt zu den Fahrstraßen passende Wendefahrstraßen
        /// </summary>
        void erzeugeWendefahrstraßen()
        {
            fortschrittMeldung = "Erzeuge Wendefahrstraßen...";

            foreach (var modulFertig in datenFertig.module)
            {
                //Erstellen von externer Liste, um foreach-Enumeration nicht zu stören 
                var wendeFahrstraßen = new List<streckenModul.Fahrstraße>();

                foreach (var fahrstraße in modulFertig.fahrstraßen)
                {
                    //Orientierung
                    var startElement = fahrstraße.zielSignal.streckenelement;
                    int suchRichtung = 0;
                    if (startElement.signale[0] == fahrstraße.zielSignal)
                        suchRichtung = 1;
                    
                    //Wendeziele sammeln
                    var wendeZiele = new List<streckenModul.Signal>();
                    foreach (var folgeElement in startElement.anschlüsse[suchRichtung])
                    {
                        wendeZiele.AddRange(sucheWendesignal(folgeElement, startElement));
                    }

                    //Wendeziele in Fahrstraßen umwandeln
                    foreach (var wendeZiel in wendeZiele)
                    {
                        double länge = fahrstraße.zielSignal.streckenelement.endpunkte[0].distanceTo(wendeZiel.streckenelement.endpunkte[0]);
                        string name = "WF: " + fahrstraße.zielSignal.name + " -> " + wendeZiel.name;
                        var wendeFahrstraße = new streckenModul.Fahrstraße(fahrstraße.zielSignal, wendeZiel, name, "TypWende", 4, länge, 5, 25 / 3.6);

                        wendeFahrstraßen.Add(wendeFahrstraße);
                    }
                }

                //Wendefahrstraßen zu Modulliste hinzufügen
                foreach (var wendeFahrstraße in wendeFahrstraßen)
                {
                    modulFertig.fahrstraßen.Add(wendeFahrstraße);
                }
            }
        }

        /// <summary>
        /// Verlinkt alle erzeugten Fahrstraßen mit ihren Folgestraßen
        /// </summary>
        void verlinkeFolgestraßen() //TODO: Algorithmus überarbeiten
        {
            fortschrittMeldung = "Verlinke Fahrstraßen...";

            //Durchlaufe alle Fahrstraßen
            foreach (var endModul in datenFertig.module)
            {
                //Sammle alle Streckenmodule, in denen eine Fahrstraße enden könnte
                var folgeModule = new List<streckenModul>();
                folgeModule.Add(endModul);
                folgeModule.AddRange(endModul.nachbarn);

                foreach (var endFahrstraße in endModul.fahrstraßen)
                {
                    endFahrstraße.startSignal.abgehendeFahrstraßen.Add(endFahrstraße);

                    //Durchlaufe alle möglichen Folgestraßen
                    foreach (var folgeModul in folgeModule)
                    {
                        foreach (var folgeFahrstraße in folgeModul.fahrstraßen)
                        {
                            //Verlinke passende Fahrstraßen
                            if (endFahrstraße.zielSignal == folgeFahrstraße.startSignal)
                            {
                                endFahrstraße.folgeStraßen.Add(folgeFahrstraße);
                            }
                        }
                    }
                }

                endModul.fahrstraßenBereit = true;
            }
        }

        /// <summary>
        /// Rekursive Funktion zur Suche nach Wendezielen
        /// </summary>
        /// <param name="aktuellesElement"></param>
        /// <param name="letztesElement"></param>
        /// <param name="wendeZiele"></param>
        /// <returns></returns>
        List<streckenModul.Signal> sucheWendesignal(streckenModul.Element aktuellesElement, streckenModul.Element letztesElement)
        {
            //Orientierung
            int suchRichtung = 0;
            if (aktuellesElement.anschlüsse[0].Contains(letztesElement))
                suchRichtung = 1;

            //Liste initialisieren
            var wendeZiele = new List<streckenModul.Signal>();

            //Enthält das aktuelle Element ein Wendeziel?
            if (aktuellesElement.signale[suchRichtung] != null)
            {
                wendeZiele = new List<streckenModul.Signal>();
                wendeZiele.Add(aktuellesElement.signale[suchRichtung]);
                return wendeZiele;
            }

            //Aufhören, wenn Ende der Strecke erreicht wurde
            if (aktuellesElement.anschlüsse[suchRichtung].Count == 0)
                return wendeZiele;

            //In nächsten Elementen weitersuchen
            foreach (var folgeElement in aktuellesElement.anschlüsse[suchRichtung])
            {
                wendeZiele.AddRange(sucheWendesignal(folgeElement, aktuellesElement));
            }
            return wendeZiele;
        }

        /// <summary>
        /// Erstelle Liste mit allen .st3-Dateien in einem Pfad
        /// </summary>
        /// <returns></returns>
        List<string> erzeugeST3Liste(string DirRoute)
        {
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

            return modulPaths;
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

        /// <summary>
        /// Sucht nach einem Modul in der Liste der Rohmodule
        /// </summary>
        /// <param name="name">Exakter Name des Moduls</param>
        /// <returns></returns>
        public st3Modul findeModulRoh(string name)
        {
            foreach (var modul in datenRohform)
                if (modul.modName == name) return modul;

            return null;
        }
    }
}
