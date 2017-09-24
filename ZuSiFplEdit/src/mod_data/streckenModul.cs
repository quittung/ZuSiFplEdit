using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Beinhaltet ein Streckenmodul in endgültiger Form sowie Informationen über seine Vollständigkeit
    /// </summary>
    public class streckenModul
    {
        /// <summary>
        /// Streckenelement
        /// </summary>
        public class Element
        {
            public int nummer, funktion;
            public float kilometer, vMax;
            public Signal[] signale;
            public PunktUTM[] endpunkte;
            public List<Element>[] anschlüsse;

            public Element(int nummer, int funktion, float kilometer, float vMax, PunktUTM endpunktB, PunktUTM endpunktG)
            {
                this.nummer = nummer;
                this.funktion = funktion; //TODO: Direktere Unterscheidung von Schiene und Rest speichern
                this.kilometer = kilometer;
                this.vMax = vMax;

                signale = new Signal[2];
                endpunkte = new PunktUTM[2];
                endpunkte[0] = endpunktB;
                endpunkte[1] = endpunktG;
                anschlüsse = new List<Element>[2];
                anschlüsse[0] = new List<Element>();
                anschlüsse[1] = new List<Element>();
            }

            public override string ToString()
            {
                return nummer.ToString();
            }
        }

        /// <summary>
        /// Einzelnes Signal - Referenzelement, dass Anfang oder Ende einer Fahrstraße ist
        /// </summary>
        public class Signal
        {
            public int nummer;
            public string info;
            public string name;
            public string betriebsstelle;
            public int typ;

            /// <summary>
            /// Zugehöriges Streckenelement
            /// </summary>
            public Element streckenelement;
            /// <summary>
            /// Signal ist in Norm- oder Gegenrichtung des Streckenelementes ausgerichtet
            /// </summary>
            public int richtung;
            public PunktUTM position;

            public streckenModul modul;

            public List<Fahrstraße> abgehendeFahrstraßen;

            /// <summary>
            /// Signal ist Start einer Fahrstraße
            /// </summary>
            public bool istStart = false;
            /// <summary>
            /// Signal ist Ziel einer Fahrstraße
            /// </summary>
            public bool istZiel = false;

            public Signal(int nummer, string info, string name, string betriebsstelle, int typ, streckenModul modul, Element streckenelement, int richtung) //TODO: Informationsumfang für Navigation ausweiten? Vl. besser in Fahrstraßen...
            {
                this.nummer = nummer;
                this.info = info;
                this.name = name;
                this.betriebsstelle = betriebsstelle;
                this.typ = typ;

                this.modul = modul;
                this.streckenelement = streckenelement;
                this.richtung = richtung;

                streckenelement.signale[richtung] = this;
                position = streckenelement.endpunkte[richtung];

                abgehendeFahrstraßen = new List<Fahrstraße>();
            }

            public override string ToString()
            {
                return info;
            }
        }

        /// <summary>
        /// Vollständige Fahrstraße - Verbindet zwei Elemente
        /// </summary>
        public class Fahrstraße
        {
            public Signal startSignal;
            public Signal zielSignal;
            public List<Fahrstraße> folgeStraßen;

            public string name;
            public string typ;
            public int RglGgl; //TODO: Information anders darstellen

            public double länge;
            public double wichtung;
            public double vMax;
            
            public Fahrstraße(Signal startSignal, Signal zielSignal, string name, string typ, int RglGgl, double länge, double wichtung, double vMax)
            {
                folgeStraßen = new List<Fahrstraße>();

                this.startSignal = startSignal;
                this.zielSignal = zielSignal;

                this.name = name;
                this.typ = typ;
                this.RglGgl = RglGgl;

                this.länge = länge;
                this.wichtung = wichtung;
                this.vMax = vMax;
            }

            public double berechneFahrdauer(double geschwindigkeit)
            {
                if (geschwindigkeit > vMax)
                    geschwindigkeit = vMax;

                double fahrdauer = länge / geschwindigkeit;
                if (typ == "TypWende")
                    fahrdauer += 60;

                return fahrdauer;
            }

            public override string ToString()
            {
                return name;
            }
        }



        public string pfad, name;
        public PunktUTM ursprung;
        public bool knotenpunkt = false; //TODO: Bearbeiten
        public bool selected = false; //TODO: Bearbeiten

        public List<streckenModul> nachbarn;

        public List<PunktUTM> hüllkurve;

        public bool elementeBereit = false;
        public Element[] elementeLookup;
        public List<Element> elemente;

        public bool signaleBereit = false;
        public Signal[] signaleLookup;
        public List<Signal> signale;
        public List<Signal> signaleStart;
        public List<Signal> signaleZwischen;
        public List<Signal> signaleZiel;

        public bool fahrstraßenBereit = false;
        public List<Fahrstraße> fahrstraßen;


        public streckenModul(string pfad, string name, PunktUTM ursprung, List<PunktUTM> hüllkurve)
        {
            this.pfad = pfad;
            this.name = name;
            this.ursprung = ursprung;
            this.hüllkurve = hüllkurve;

            elemente = new List<Element>();
            signale = new List<Signal>();
            signaleStart = new List<Signal>();
            signaleZwischen = new List<Signal>();
            signaleZiel = new List<Signal>();
            fahrstraßen = new List<Fahrstraße>();

            nachbarn = new List<streckenModul>();
        }

        public override string ToString()
        {
            return name;
        }
    }
}
