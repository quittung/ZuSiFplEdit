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
            public Betriebsstelle[] betriebstellen;
            public PunktUTM[] endpunkte;
            public List<Element>[] anschlüsse;

            //public bool hervorheben;
            //public DateTime hervorhebenBis;

            public Element(int nummer, int funktion, float kilometer, float vMax, PunktUTM endpunktB, PunktUTM endpunktG)
            {
                this.nummer = nummer;
                this.funktion = funktion; //TODO: Direktere Unterscheidung von Schiene und Rest speichern
                this.kilometer = kilometer;
                this.vMax = vMax;

                signale = new Signal[2];
                betriebstellen = new Betriebsstelle[2];
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
            public Betriebsstelle betriebsstelle;
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

            public Signal(int nummer, string info, string name, Betriebsstelle betriebsstelle, int typ, streckenModul modul, Element streckenelement, int richtung) //TODO: Informationsumfang für Navigation ausweiten? Vl. besser in Fahrstraßen...
            {
                this.nummer = nummer;
                this.info = info;
                this.name = name;
                this.betriebsstelle = betriebsstelle;
                if (betriebsstelle != null)
                    betriebsstelle.signale.Add(this);
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
            public List<Betriebsstelle> betriebsstellen;

            public string name;
            public string typ;
            public VzG_Strecke vzgStrecke;
            public bool richtung;
            public int RglGgl; //TODO: Information anders darstellen

            public double länge;
            public double vStart;
            public double vZiel;
            public double längeWeichenBereich;
            public double wichtung;
            
            public Fahrstraße(Signal startSignal, Signal zielSignal, string name, string typ, int RglGgl, double länge, double längeWeichenBereich, double vMin, double vMax, double wichtung)
            {
                folgeStraßen = new List<Fahrstraße>();

                this.startSignal = startSignal;
                this.zielSignal = zielSignal;

                this.name = name;
                this.typ = typ;
                this.RglGgl = RglGgl;

                this.länge = länge;
                this.längeWeichenBereich = längeWeichenBereich;
                this.vStart = vMin;
                this.vZiel = vMax;
                this.wichtung = wichtung;

                betriebsstellen = new List<Betriebsstelle>();
            }

            public double berechneFahrdauer(double geschwindigkeit)
            {
                double zugVmin = vStart;
                if (zugVmin > geschwindigkeit)
                    zugVmin = geschwindigkeit;
                double zugVmax = vZiel;
                if (zugVmax > geschwindigkeit)
                    zugVmax = geschwindigkeit;

                double fahrdauerImWeichenbereich = längeWeichenBereich / vStart;
                double fahrdauerNachWeichenbereich = (länge - längeWeichenBereich) / vZiel;
                double fahrdauerBeschleunigung = zeitverlustDurchBeschleunigung(zugVmin, zugVmax, 0.5);

                double fahrdauer = fahrdauerImWeichenbereich + fahrdauerNachWeichenbereich;
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

        public List<Betriebsstelle> betriebsstellen;

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
            betriebsstellen = new List<Betriebsstelle>();
            signale = new List<Signal>();
            signaleStart = new List<Signal>();
            signaleZwischen = new List<Signal>();
            signaleZiel = new List<Signal>();
            fahrstraßen = new List<Fahrstraße>();

            nachbarn = new List<streckenModul>();
        }

        public static double zeitverlustDurchBeschleunigung(double vStart, double vZiel, double beschleunigung)
        {
            if (vZiel > vStart)
            {
                var tmp = vZiel;
                vZiel = vStart;
                vStart = tmp;
            }

            var vDiff = vStart - vZiel;
            var zeitBeschleunigung = vDiff / beschleunigung;
            var strecke = 0.5 * beschleunigung * zeitBeschleunigung * zeitBeschleunigung;
            var zeitDurchfahrt = strecke / vStart;

            double zeitverlust = zeitBeschleunigung - zeitDurchfahrt;
            return zeitverlust;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
