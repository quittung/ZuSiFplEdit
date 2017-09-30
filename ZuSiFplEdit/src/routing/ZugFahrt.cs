using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Enthält Informationen zu einer Zugfahrt inkl. Zugnummer, Gattung und Route
    /// </summary>
    public class ZugFahrt
    {
        /// <summary>
        /// Enthält Informationen zu einem Wegpunkt und Funktionen zur Wegfindung
        /// </summary>
        public class WayPoint
        {
            public class aStarNode
            {
                public streckenModul.Signal Node;
                public double DistanceFromStart;
                public double HeuristicDistanceToTarget;
                public double OverallDistance;
                public aStarNode PreviousNode;
                public streckenModul.Fahrstraße PreviousVertex;

                public float vMax;

                public aStarNode(streckenModul.Signal Node, aStarNode PreviousNode, streckenModul.Fahrstraße PreviousVertex, streckenModul.Signal target, float vMax)
                {
                    this.Node = Node;
                    this.PreviousNode = PreviousNode;
                    this.PreviousVertex = PreviousVertex;
                    this.vMax = vMax;

                    if (PreviousNode == null)
                        DistanceFromStart = 0;
                    else
                        DistanceFromStart = PreviousNode.DistanceFromStart + PreviousVertex.berechneFahrdauer(vMax) * PreviousVertex.wichtung;

                    HeuristicDistanceToTarget = Node.position.distanceTo(target.position) * 1000f / vMax;

                    OverallDistance = DistanceFromStart + HeuristicDistanceToTarget;
                }

                public void updateNode(aStarNode PreviousNode, streckenModul.Fahrstraße PreviousVertex)
                {
                    var alternativeDistanceFromStart = PreviousNode.DistanceFromStart + PreviousVertex.berechneFahrdauer(vMax) * PreviousVertex.wichtung;
                    if (DistanceFromStart > alternativeDistanceFromStart)
                    {
                        this.PreviousNode = PreviousNode;
                        this.PreviousVertex = PreviousVertex;

                        DistanceFromStart = alternativeDistanceFromStart;
                        OverallDistance = DistanceFromStart + HeuristicDistanceToTarget;
                    }
                }

                // Default comparer
                public int CompareTo(aStarNode compareNode)
                {
                    return OverallDistance.CompareTo(compareNode.OverallDistance);
                }

                public override string ToString()
                {
                    return (Node.ToString() + "; " + OverallDistance.ToString("N2"));
                }
            }

            /// <summary>
            /// Signal des Wegpunkts
            /// </summary>
            public streckenModul.Signal signal;
            /// <summary>
            /// Enthält die Strecke zum nächsten zuletzt übergebenen Zielsignal, wenn möglich
            /// </summary>
            [Obsolete]
            public List<streckenModul.Fahrstraße> teilRoute;

            public RoutenPunkt routenPunkt;
            public DateTime ankunft;
            public DateTime abfahrt;
            public bool ankunft_gesetzt;
            public bool abfahrt_gesetzt;

            public ZugFahrt zug;
            
            /// <summary>
            /// Konstruktor für Wegpunkt von Signal
            /// </summary>
            public WayPoint(streckenModul.Signal signal, ZugFahrt zug)
            {
                this.signal = signal;
                this.zug = zug;
            }
            
            

            /// <summary>
            /// Setzt Strecke zum übergebenen Zielsignal
            /// </summary>
            public void streckeBerechnen(streckenModul.Signal Startsignal, streckenModul.Signal Zielsignal, List<RoutenPunkt> route)
            {
                //teilRoute = fstrRouteSearchStart(Zielsignal);

                var OpenNodes = new List<aStarNode>();
                var ClosedNodes = new List<aStarNode>();

                var CurrentNode = new aStarNode(Startsignal, null, null, Zielsignal, zug.vMax);
                

                while (true)
                {
                    ClosedNodes.Add(CurrentNode);

                    foreach (var AdjacentVertex in CurrentNode.Node.abgehendeFahrstraßen)
                    {
                        if ((AdjacentVertex.zielSignal != null) && (findDuplicateAStarByNode(AdjacentVertex.zielSignal, ClosedNodes) == null))
                        {
                            var DuplicateNode = findDuplicateAStarByNode(AdjacentVertex.zielSignal, OpenNodes);
                            if (DuplicateNode == null)
                                OpenNodes.Add(new aStarNode(AdjacentVertex.zielSignal, CurrentNode, AdjacentVertex, Zielsignal, zug.vMax)); 
                            else
                            {
                                DuplicateNode.updateNode(CurrentNode, AdjacentVertex);
                            }
                        }
                    }
                    
                    OpenNodes.Sort(CompareAStarByOverallLength);

                    if (OpenNodes.Count == 0)
                        return;

                    CurrentNode = OpenNodes[0];
                    OpenNodes.RemoveAt(0);

                    if (CurrentNode.Node == Zielsignal)
                        break;
                }


                teilRoute = new List<streckenModul.Fahrstraße>();

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Geschätzte Fahrzeit: " + new DateTime().AddSeconds(CurrentNode.OverallDistance).ToString("HH:mm:ss"));


                int legStartIndex = route.Count;
                int routenPunktZähler = 0;

                while (CurrentNode.PreviousNode != null)
                {
                    //Routenpunkt erzeugen
                    var tmp_routenPunkt = routenPunktErzeugen(CurrentNode.PreviousVertex);
                    route.Insert(legStartIndex, tmp_routenPunkt);

                    //Endpunkte mit nächstem Waypoint verlinken
                    if(routenPunktZähler == 0)
                    {
                        int eigenerIndex = zug.WayPoints.IndexOf(this);
                        var nächsterWegPunkt = zug.WayPoints[eigenerIndex + 1];
                        tmp_routenPunkt.wegPunkt = nächsterWegPunkt;
                        nächsterWegPunkt.routenPunkt = tmp_routenPunkt;
                    }

                    //Zum nächsten Knoten gehen
                    CurrentNode = CurrentNode.PreviousNode;
                    routenPunktZähler++;

                    //Ersten Startpunkt verlinken
                    if (CurrentNode.PreviousNode == null && zug.WayPoints.IndexOf(this) == 0)
                    {
                        tmp_routenPunkt.wegPunkt = this;
                        routenPunkt = tmp_routenPunkt;
                    }
                }
            }

            int CompareAStarByOverallLength(aStarNode x, aStarNode y)
            {
                return (x.OverallDistance.CompareTo(y.OverallDistance));
            } 

            aStarNode findDuplicateAStarByNode(streckenModul.Signal Node, List<aStarNode> List)
            {
                foreach (var ListNode in List)
                {
                    if (ListNode.Node == Node)
                        return ListNode;
                }
                return null;
            }

            public RoutenPunkt routenPunktErzeugen(streckenModul.Fahrstraße fahrstraße)
            {
                var signal = fahrstraße.zielSignal;

                bool relevant = (fahrstraße.startSignal.abgehendeFahrstraßen.Count > 1);

                bool wende = fahrstraße.typ == "TypWende";

                double fahrdauer = fahrstraße.berechneFahrdauer(zug.vMax);

                var routenPunkt = new RoutenPunkt(signal, fahrstraße, relevant, wende, fahrdauer, this);

                return routenPunkt;
            }

            public override string ToString()
            {
                return (signal.info);
            }
        }

        public class RoutenPunkt
        {
            public streckenModul.Signal signal;
            public streckenModul.Fahrstraße fahrstraße;
            
            /// <summary>
            /// Routenpunkt muss in Fahrplan eingetragen werden
            /// </summary>
            public bool relevant;
            public double fahrdauer;
            public double fahrzeitÜberschuss;

            public DateTime ankunft;
            public DateTime abfahrt;
            public bool wende;

            /// <summary>
            /// Wenn zutreffend, verlinkt den Wegpunkt, der diesem Routenpunkt exakt entspricht 
            /// </summary>
            public WayPoint wegPunkt;
            /// <summary>
            /// Wegpunkt, nach dem dieser Punkt nach einer Umwandlung einsortiert wird
            /// </summary>
            public WayPoint letzterWegPunkt;


            public RoutenPunkt(streckenModul.Signal signal, streckenModul.Fahrstraße anfahrt, bool relevant, bool wende, double fahrdauer, WayPoint letzterWegPunkt)
            {
                this.signal = signal;
                this.fahrstraße = anfahrt;

                this.relevant = relevant;
                this.wende = wende;
                this.fahrdauer = fahrdauer;

                this.letzterWegPunkt = letzterWegPunkt;
            }

            public override string ToString()
            {
                string beschreibung = "- ";
                if (relevant)
                    beschreibung = "> ";
                if (wegPunkt != null)
                    beschreibung = "= ";
                if (wende)
                    beschreibung = "< ";

                beschreibung += signal.betriebsstelle + " " + signal.name;

                return (beschreibung);
            }
        }

        /// <summary>
        /// Enthält die Zuggattung (z.B. RB, IC)
        /// </summary>
        public string Gattung;
        /// <summary>
        /// Enthält eine im Fahplankontext einmalige Zugnummer
        /// </summary>
        public int Zugnummer;
        /// <summary>
        /// Enthält alle Wegpunkte der Zugfahrt
        /// </summary>
        public List<WayPoint> WayPoints;

        public Datensatz datensatz;

        public float vMax;
        public double reserve = 0.1;

        public string zugVerbandName;
        public string zugVerbandXML;

        /// <summary>
        /// Länge der gesamten Route
        /// </summary>
        public double route_länge;
        /// <summary>
        /// Erwartete Fahrtdauer auf der gesamten Route
        /// </summary>
        public long route_dauer;

        public List<RoutenPunkt> route;

        

        public List<bool> includeSignal; //Signal in Fahrplan aufnehmen in Abhängigkeit von Wichtigkeit/abgehenden Fahrstraßen etc.

        /// <summary>
        /// Konstruktor der Klasse ZugFahrt
        /// </summary>
        public ZugFahrt(Datensatz datensatz)
        {
            this.datensatz = datensatz;

            WayPoints = new List<WayPoint>();
            route = new List<RoutenPunkt>();
            includeSignal = new List<bool>();
            vMax = 160f / 3.6f;
        }

        public override string ToString()
        {
            return (Gattung + " " + Zugnummer.ToString());
        }

        /// <summary>
        /// Berechnet die Route der Zugfahrt aus Wegpunkten soweit wie möglich
        /// </summary>
        public void routeBerechnen()
        {
            includeSignal.Clear();
            route.Clear();


            if (WayPoints.Count < 2)
            {
                MessageBox.Show("Weniger als zwei Wegpunkte sind bekannt.\nFahrweg kann nicht berechnet werden.");
                return;
            }

            if (!datensatz.fahrstraßenBereit)
            {
                MessageBox.Show("Datensatz wurde noch nicht vollständig verarbeitet. Bitte in ein paar Sekunden erneut versuchen.");
                return;
            }

            for (int i = 0; i < WayPoints.Count - 1; i++)
            {
                WayPoints[i].streckeBerechnen(WayPoints[i].signal, WayPoints[i + 1].signal, route);
                if (WayPoints[i].teilRoute != null) //TODO: Abbruch anders erkennen
                {
                    for (int b = 1; b < WayPoints[i].teilRoute.Count; b++)
                    {
                        includeSignal.Add(false);
                    }
                    includeSignal.Add(true);
                }
                else
                {
                    if(route.Count == 0)
                    {
                        MessageBox.Show("Vom Startsignal aus konnte keine Route gefunden werden.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Die Route konnte nur bis \"" + route[route.Count - 1].fahrstraße.zielSignal.name + "\" bestimmt werden.");
                        break;
                    }
                }
            }
            includeSignal[0] = true;
            route_metadaten_berechen();
        }

        /// <summary>
        /// Berechnet Metadaten wie Fahrzeiten und Routenlänge
        /// </summary>
        public void route_metadaten_berechen()
        {
            if (route == null || route.Count == 0)
                return;
                           
            route_dauer = 0;
            route_länge = 0;

            //Werte aus Wegpunkte eintragen
            foreach (var wegPunkt in WayPoints)
            {
                var routenPunkt = wegPunkt.routenPunkt;

                routenPunkt.relevant = true;

                if (wegPunkt.ankunft_gesetzt)
                    routenPunkt.ankunft = wegPunkt.ankunft;

                if (wegPunkt.abfahrt_gesetzt)
                    routenPunkt.abfahrt = wegPunkt.abfahrt;
            }

            if (route[0].ankunft == new DateTime())
                route[0].ankunft = Convert.ToDateTime("2017-02-27 12:00:00");

            for (int i = 1; i < route.Count; i++)
            {
                //Informationen sammeln
                bool wende = (i < (route.Count - 1)) && (route[i + 1].wende);
                
                DateTime letzteZeit;
                if (route[i - 1].abfahrt == new DateTime())
                    letzteZeit = route[i - 1].ankunft;
                else
                    letzteZeit = route[i - 1].abfahrt;
                
                double strecke_cur = route[i].fahrstraße.länge;
                route_länge += strecke_cur;


                //Fahrzeit für Halte anpassen
                if (route[i - 1].ankunft != new DateTime())
                {
                    route[i].fahrdauer += zeitverlustDurchBeschleunigung(0, route[i].fahrstraße.vStart, 0.5);
                }
                if ((route[i].wegPunkt != null && route[i].wegPunkt.ankunft_gesetzt) || wende)
                {
                    route[i].fahrdauer += zeitverlustDurchBeschleunigung(route[i].fahrstraße.vZiel, 0, 0.5);
                }
                route[i].fahrdauer = route[i].fahrdauer * (1 + reserve);

                //Zeiten berechnen
                if (wende)
                {
                    if(route[i].ankunft == new DateTime())
                    {
                        if(route[i].abfahrt == new DateTime())
                        {
                            route[i].ankunft = letzteZeit.AddSeconds(route[i].fahrdauer);
                            route[i].abfahrt = route[i].ankunft.AddSeconds(30);
                        }
                        else
                        {
                            route[i].ankunft = route[i].abfahrt.AddSeconds(-30);
                        }
                    }
                    else
                    {
                        if (route[i].abfahrt == new DateTime())
                        {
                            route[i].abfahrt = route[i].ankunft.AddSeconds(30);
                        }
                    }
                }
                else
                {
                    if (route[i].ankunft == new DateTime() && route[i].abfahrt == new DateTime())
                        route[i].abfahrt = letzteZeit.AddSeconds(route[i].fahrdauer);
                }

                //Zeit aufaddieren
                double fahrdauer = 0;
                if (route[i].ankunft != new DateTime())
                {
                    fahrdauer += (route[i].ankunft - letzteZeit).TotalSeconds;
                    if (route[i].abfahrt != new DateTime())
                    {
                        fahrdauer += (route[i].abfahrt - route[i].ankunft).TotalSeconds;
                    }
                }
                else
                {
                    if (route[i].abfahrt != new DateTime())
                    {
                        fahrdauer += (route[i].abfahrt - letzteZeit).TotalSeconds;
                    }
                }

                route_dauer += (long)fahrdauer;
                route[i].fahrzeitÜberschuss = fahrdauer - route[i].fahrdauer;
            }
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

    }
}