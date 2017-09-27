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

            datenRohform.Clear();
            datenRohform = null;

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
            var endstücke = new List<streckenModul.Element>();

            foreach (var modulFertig in datenFertig.module)
            {
                var modulRoh = findeModulRoh(modulFertig.name);

                foreach (var elementRoh in modulRoh.StreckenElemente)
                {
                    var elementFertig = modulFertig.elementeLookup[elementRoh.Nr];
                    if (elementRoh.AnschlussNorm.Count != 0)
                        foreach (var anschluss in elementRoh.AnschlussNorm)
                        {
                            var anschlussModul = modulFertig;
                            if (anschluss.modul != "")
                            {
                                //Console.WriteLine(anschluss.element);
                                anschlussModul = datenFertig.sucheModul(speicherortZuName(anschluss.modul, '\\'));
                            }
                            if (anschlussModul == null)
                                continue;
                            var anschlussElement = anschlussModul.elementeLookup[anschluss.element];
                            if (anschlussElement == null)
                                continue;
                            elementFertig.anschlüsse[0].Add(anschlussElement);
                        }

                    if (elementRoh.AnschlussGegen.Count != 0)
                        foreach (var anschluss in elementRoh.AnschlussGegen)
                        {
                            var anschlussModul = modulFertig;
                            if (anschluss.modul != "")
                                anschlussModul = datenFertig.sucheModul(speicherortZuName(anschluss.modul, '\\'));
                            if (anschlussModul == null)
                                continue;
                            var anschlussElement = anschlussModul.elementeLookup[anschluss.element];
                            if (anschlussElement == null)
                                continue;
                            elementFertig.anschlüsse[1].Add(anschlussElement);
                        }

                    if ((elementFertig.anschlüsse[0].Count() == 0 || elementFertig.anschlüsse[1].Count() == 0) && elementFertig.funktion != 2)
                    {
                        endstücke.Add(elementFertig);
                    }
                }
            }

            //Endstücke zusammensetzen
            fortschrittMeldung = "Verbinde Endelemente...";
            var abstandMax = 0.1 / 1000;
            foreach (var endstückA in endstücke)
            {
                //Elemente ausfiltern, die keine Endstücke mehr sind
                if (endstückA.anschlüsse[0].Count() > 0 && endstückA.anschlüsse[1].Count() > 0)
                    continue;

                foreach (var endstückB in endstücke)
                {
                    //Eigenvergleiche ausfiltern
                    if (endstückA == endstückB)
                        continue;

                    //Elemente ausfiltern, die keine Endstücke mehr sind
                    if (endstückB.anschlüsse[0].Count() > 0 && endstückB.anschlüsse[1].Count() > 0)
                        continue;

                    //Alle Endepunkte miteinander vergleichen
                    for (int a = 0; a <= 1; a++)
                    {
                        for (int b = 0; b <= 1; b++)
                        {
                            var abstand = endstückA.endpunkte[a].distanceTo(endstückB.endpunkte[b]);
                            if (abstand < abstandMax)
                            {
                                endstückA.anschlüsse[a].Add(endstückB);
                                endstückB.anschlüsse[b].Add(endstückA);
                            }
                        }
                    }
                }
            }

            foreach (var modulFertig in datenFertig.module)
            {
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
                    var name = signalRoh.Info;
                    var betriebsstelle = "";
                    int typ = 0;
                    if (signalRoh.Signal != null)
                    {
                        betriebsstelle = signalRoh.Signal.Betriebstelle;
                        name = signalRoh.Signal.Name;
                        typ = signalRoh.Signal.Signaltyp;
                    }

                    if (typ == 13) //Unsichtbare Signale aussortieren
                        continue;

                    var signalFertig = new streckenModul.Signal(signalReferenz, signalRoh.ToString(), name, betriebsstelle, typ, modulFertig, modulFertig.elementeLookup[signalRoh.StrElementNr], 1 - Convert.ToInt32(signalRoh.StrNorm));
                    
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
                    if (signal == null)
                        continue;
                    signal.istStart = true;
                    modulFertig.signaleStart.Add(signal);
                }

                foreach (var signalReferenz in modulRoh.ZielSignale)
                {
                    var signal = modulFertig.signaleLookup[signalReferenz];
                    if (signal == null)
                        continue;
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
                    if (startSignal == null)
                        continue;

                    var zielModul = datenFertig.sucheModul(fahrstraßeRoh.ZielMod_Str);
                    if (zielModul == null)
                        continue;
                    var zielSignal = zielModul.signaleLookup[fahrstraßeRoh.ZielRef];
                    if (zielSignal == null)
                        continue;

                    double länge = fahrstraßeRoh.Laenge;

                    double längeWeichenBereich = ermittleLängeAWB(fahrstraßeRoh, modulRoh, startSignal, länge);

                    //vStart bestimmen
                    double vStart = 40 / 3.6;

                    int signalZeile = 0;
                    foreach (var fstrSignal in fahrstraßeRoh.signale)
                    {
                        if (fstrSignal.RefInt == fahrstraßeRoh.StartRef)
                        {
                            signalZeile = fstrSignal.SignalZeile;
                            break;
                        }
                    }

                    var startModulRoh = findeModulRoh(fahrstraßeRoh.StartMod_Str);
                    if (startModulRoh == null)
                        continue;
                    var startSignalRoh = startModulRoh.ReferenzElementeNachNr[fahrstraßeRoh.StartRef];
                    if (startSignalRoh == null)
                        continue;

                    if (startSignalRoh.Signal != null && startSignalRoh.Signal.Betriebstelle != null)
                        vStart = startSignalRoh.Signal.vSig[signalZeile];

                    if (vStart == -1)
                        vStart = startSignal.streckenelement.vMax;


                    double vZiel = zielSignal.streckenelement.vMax;

                    var fahrstraßeFertig = new streckenModul.Fahrstraße(startSignal, zielSignal, fahrstraßeRoh.FahrstrName, fahrstraßeRoh.FahrstrTyp, fahrstraßeRoh.RglGgl, länge, längeWeichenBereich, vStart, vZiel, fahrstraßeRoh.wichtung);
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
                        wendeZiele.AddRange(sucheWendesignal(folgeElement, startElement, 0));
                    }

                    //Wendeziele in Fahrstraßen umwandeln
                    foreach (var wendeZiel in wendeZiele)
                   { 
                        double länge = fahrstraße.zielSignal.streckenelement.endpunkte[0].distanceTo(wendeZiel.streckenelement.endpunkte[0]) * 1000;
                        string name = "WF: " + fahrstraße.zielSignal.name + " -> " + wendeZiel.name;
                        var wendeFahrstraße = new streckenModul.Fahrstraße(fahrstraße.zielSignal, wendeZiel, name, "TypWende", 4, länge, 0, 25/3.6, 25 / 3.6, 2);

                        wendeFahrstraßen.Add(wendeFahrstraße);
                    }
                }

                //Wendefahrstraßen zu Modulliste hinzufügen
                foreach (var wendeFahrstraße in wendeFahrstraßen)
                {
                    if (!istWendefahrstraßeVorhanden(wendeFahrstraße.name, modulFertig))
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
            foreach (var ausgangsModul in datenFertig.module)
            {
                //Sammle alle Streckenmodule, in denen eine Fahrstraße enden könnte
                var weiterführendeModule = new List<streckenModul>();
                weiterführendeModule.Add(ausgangsModul);
                weiterführendeModule.AddRange(ausgangsModul.nachbarn);

                foreach (var ausgangsFahrstraße in ausgangsModul.fahrstraßen)
                {
                    ausgangsFahrstraße.startSignal.abgehendeFahrstraßen.Add(ausgangsFahrstraße);

                    //Durchlaufe alle möglichen Folgestraßen
                    foreach (var weiterführendesModul in weiterführendeModule)
                    {
                        foreach (var weiterführendeFahrstraße in weiterführendesModul.fahrstraßen)
                        {
                            //Verlinke passende Fahrstraßen
                            if (ausgangsFahrstraße.zielSignal == weiterführendeFahrstraße.startSignal)
                            {
                                ausgangsFahrstraße.folgeStraßen.Add(weiterführendeFahrstraße);
                            }
                        }
                    }
                }

                ausgangsModul.fahrstraßenBereit = true;
            }
        }

        /// <summary>
        /// Ermittelt die Länge des anschließenden Weichenbereichs durch Ablaufen der Fahrstraße
        /// </summary>
        /// <param name="fahrstraßeRoh"></param>
        /// <param name="startSignal"></param>
        /// <param name="längeFahrstraße"></param>
        /// <returns></returns>
        double ermittleLängeAWB(st3Modul.fahrStr fahrstraßeRoh, st3Modul modulRoh, streckenModul.Signal startSignal, double längeFahrstraße)
        {
            double längeWeichenBereich = 0;
            int suchrichtung = 0;
            if (startSignal.streckenelement.signale[1] == startSignal)
                suchrichtung = 1;

            streckenModul.Element aktuellesElement = startSignal.streckenelement.anschlüsse[suchrichtung][0];
            streckenModul.Element letztesElement = startSignal.streckenelement;

            int weichenÜberfahren = 0;
            double längeBisLetzteWeiche = 0;
            string weichenPfad = "";

            foreach (var weiche in fahrstraßeRoh.weichen)
            {
                weichenvermerkAuflösen(weiche);
            }

            while (true)
            {
                //Wurden alle Weichen des AWBs bereits abgefahren?
                if (weichenÜberfahren == fahrstraßeRoh.weichen.Count)
                {
                    return längeWeichenBereich;
                }

                //Ist der erkannte AWB bereits länger als die Fahrstraße selbst?
                //Dieser Fall tritt häufig aufgrund von (fehlerhaft eingetragenen?) Flankenschutzweichen auf
                if (längeWeichenBereich > längeFahrstraße)
                {
                    //Console.WriteLine("LUe " + fahrstraßeRoh.FahrstrName + " | " + weichenPfad);
                    if (längeBisLetzteWeiche == 0)
                        return längeFahrstraße / 2;
                    else
                        return längeBisLetzteWeiche;
                }


                //Orientierung
                suchrichtung = 0;
                if (aktuellesElement.anschlüsse[0].Contains(letztesElement))
                    suchrichtung = 1;

                //Länge integrieren:
                längeWeichenBereich += aktuellesElement.endpunkte[0].distanceTo(aktuellesElement.endpunkte[1]) * 1000;
                
                                
                //Ist das aktuelle Element eine spitz befahrene Weiche?
                if (aktuellesElement.anschlüsse[suchrichtung].Count() > 1)
                {
                    weichenPfad += "<" + aktuellesElement.nummer + " ";
                    int weichenAbfragen = 0;
                    //In Fahrstraße vermerkte Weichen nach Weichenlage durchsuchen
                    foreach (var weiche in fahrstraßeRoh.weichen)
                    {
                        //Suche Modul des Weichenvermerks
                        var weichenModul = findeModulRoh(weiche.RefModString);
                        if (weichenModul == null)
                        {
                            Console.WriteLine("MU  " + fahrstraßeRoh.FahrstrName);
                            return längeFahrstraße / 2;
                        }

                        //Existiert Referenzelement des Weichenvermerks?
                        if (weiche.referenzIndex >= weichenModul.ReferenzElementeNachNr.Count() || weichenModul.ReferenzElementeNachNr[weiche.referenzIndex] == null)
                        {
                            Console.WriteLine("RU  " + fahrstraßeRoh.FahrstrName);
                            return längeFahrstraße / 2;
                        }
                        weiche.strEInt = weichenModul.ReferenzElementeNachNr[weiche.referenzIndex].StrElementNr;

                        
                        //Wurde ein passender Weicheneintrag gefunden?
                        if (weiche.strEInt == aktuellesElement.nummer)
                        {
                            weichenÜberfahren++;
                            längeBisLetzteWeiche = längeWeichenBereich;

                            letztesElement = aktuellesElement;
                            aktuellesElement = aktuellesElement.anschlüsse[suchrichtung][weiche.weichenlage - 1];
                            break;
                        } 
                        weichenAbfragen++;
                    }
                    //War die letzte Weiche nicht in der Fahrstraße vermerkt?
                    //Dieser Fehler wird durch in falscher Reihenfolge eingelesene Elemente erzeugt
                    if (weichenAbfragen == fahrstraßeRoh.weichen.Count)
                    {
                        //Console.WriteLine("WU" + weichenÜberfahren + " " + fahrstraßeRoh.FahrstrName + " | " + weichenPfad);
                        return längeFahrstraße / 2;
                    }
                }
                else //Element war keine spitz befahrene Weiche
                {
                    //Stumpfes Überfahren von Weiche eintragen
                    if (aktuellesElement.anschlüsse[1 - suchrichtung].Count() > 1)
                    {
                        weichenÜberfahren++;
                        längeBisLetzteWeiche = längeWeichenBereich;
                        weichenPfad += ">" + aktuellesElement.nummer + " ";
                    }

                    
                    //Endet die Strecke mit dem aktuellen Element?
                    if (aktuellesElement.anschlüsse[suchrichtung].Count == 0)
                    {
                        //Console.WriteLine("SE  " + fahrstraßeRoh.FahrstrName);
                        if (längeBisLetzteWeiche == 0)
                            return längeFahrstraße / 2;
                        else
                            return längeBisLetzteWeiche;
                    }

                    //Vorbereiten auf nächsten Durchlauf
                    letztesElement = aktuellesElement;
                    aktuellesElement = aktuellesElement.anschlüsse[suchrichtung][0];
                }
            }
        }

        bool weichenvermerkAuflösen(st3Modul.fahrStr.fstrWeiche weiche)
        {
            //Suche Modul des Weichenvermerks
            var weichenModul = findeModulRoh(weiche.RefModString);
            if (weichenModul == null)
            {
                return false;
            }

            //Existiert Referenzelement des Weichenvermerks?
            if (weiche.referenzIndex >= weichenModul.ReferenzElementeNachNr.Count() || weichenModul.ReferenzElementeNachNr[weiche.referenzIndex] == null)
            {
                return false;
            }
            weiche.strEInt = weichenModul.ReferenzElementeNachNr[weiche.referenzIndex].StrElementNr;
            return true;
        }

        /// <summary>
        /// Rekursive Funktion zur Suche nach Wendezielen
        /// </summary>
        /// <param name="aktuellesElement"></param>
        /// <param name="letztesElement"></param>
        /// <param name="wendeZiele"></param>
        /// <returns></returns>
        List<streckenModul.Signal> sucheWendesignal(streckenModul.Element aktuellesElement, streckenModul.Element letztesElement, double länge)
        {
            //Länge integrieren
            länge += aktuellesElement.endpunkte[0].distanceTo(aktuellesElement.endpunkte[1]);

            //Orientierung
            int suchRichtung = 0;
            if (aktuellesElement.anschlüsse[0].Contains(letztesElement))
                suchRichtung = 1;
            
            //Liste initialisieren
            var wendeZiele = new List<streckenModul.Signal>();

            //Enthält das aktuelle Element ein Wendeziel?
            if ((aktuellesElement.signale[suchRichtung] != null) && (aktuellesElement.signale[suchRichtung].istStart) && (aktuellesElement.signale[suchRichtung].istZiel)) //TODO: Erkennung von AGPs verbessern
            {
                wendeZiele = new List<streckenModul.Signal>();
                wendeZiele.Add(aktuellesElement.signale[suchRichtung]);
                return wendeZiele;
            }

            //Aufhören, wenn Ende der Strecke erreicht wurde
            if (aktuellesElement.anschlüsse[suchRichtung].Count == 0)
                return wendeZiele;

            //Aufhören, wenn Wendefahrstraße übermäßig lang wird
            if (länge > 3)
                return wendeZiele;

            //In nächsten Elementen weitersuchen
            foreach (var folgeElement in aktuellesElement.anschlüsse[suchRichtung])
            {
                wendeZiele.AddRange(sucheWendesignal(folgeElement, aktuellesElement, länge));
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

        /// <summary>
        /// Gibt zurück, ob eine gleichnamige Fahrstraße bereits im Modul existiert
        /// </summary>
        /// <param name="name"></param>
        /// <param name="modul"></param>
        /// <returns></returns>
        bool istWendefahrstraßeVorhanden(string name, streckenModul modul)
        {
            foreach (var fahrstraße in modul.fahrstraßen)
            {
                if (fahrstraße.name == name)
                    return true;
            }

            return false;
        }
    }
}
