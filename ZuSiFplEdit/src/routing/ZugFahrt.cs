﻿using System;
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
            public List<streckenModul.Fahrstraße> teilRoute;

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
                while (true)
                {
                    if (CurrentNode.PreviousNode == null)
                        break;
                    teilRoute.Insert(0, CurrentNode.PreviousVertex);

                    route.Insert(legStartIndex, routenPunktErzeugen(CurrentNode.PreviousVertex));

                    //CurrentNode.PreviousVertex.Durchschnittsgeschwindigkeit = CurrentNode.PreviousVertex.vMaxBestimmen(1000);
                    //Console.WriteLine(CurrentNode.PreviousVertex.FahrstrName + " " + CurrentNode.PreviousVertex.RglGgl + " " + CurrentNode.PreviousVertex.Durchschnittsgeschwindigkeit);

                    CurrentNode = CurrentNode.PreviousNode;
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

                var routenPunkt = new RoutenPunkt(signal, fahrstraße, relevant, wende, fahrdauer);

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

            public DateTime ankunft;
            public DateTime abfahrt;
            public bool wende;
            public WayPoint wegPunkt;


            public RoutenPunkt(streckenModul.Signal signal, streckenModul.Fahrstraße anfahrt, bool relevant, bool wende, double fahrdauer)
            {
                this.signal = signal;
                this.fahrstraße = anfahrt;

                this.relevant = relevant;
                this.wende = wende;
                this.fahrdauer = fahrdauer;
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
        /// Veraltet: Enthält Startsignal der Zugfahrt. Ersetzt durch Wegpunkt-System.
        /// </summary>
        [Obsolete]
        public st3Modul.referenzElement ZstartSignal;
        /// <summary>
        /// Veraltet: Enthält Zielsignal der Zugfahrt. Ersetzt durch Wegpunkt-System.
        /// </summary>
        [Obsolete]
        public st3Modul.referenzElement ZzielSignal;
        /// <summary>
        /// Enthält alle Wegpunkte der Zugfahrt
        /// </summary>
        public List<WayPoint> WayPoints;

        Datensatz datensatz;

        public float vMax;

        /// <summary>
        /// Länge der gesamten Route
        /// </summary>
        public double route_länge;
        /// <summary>
        /// Erwartete Fahrtdauer auf der gesamten Route
        /// </summary>
        public long route_dauer;

        public List<RoutenPunkt> route;

        

        /// <summary>
        /// </summary>
        public DateTime[] route_ankunft;
        /// <summary>
        /// </summary>
        public DateTime[] route_abfahrt;

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

        public void route_metadaten_berechen()
        {
            if (route == null || route.Count == 0)
                return;
            
            route_dauer = 0;
            route_länge = 0;

            route[0].ankunft = Convert.ToDateTime("2017-02-27 12:00:00");
            //route_abfahrt[0] = Convert.ToDateTime("2017-02-27 12:00:00");

            for (int i = 1; i < route.Count; i++)
            {
                //Strecke suchen
                double strecke_cur = route[i].fahrstraße.länge;
                //Zeit berechnen
                long zeit_cur = (long)route[i].fahrstraße.berechneFahrdauer(vMax);
                //Strecke aufaddieren
                route_länge += strecke_cur;
                //Zeit aufaddieren
                route_dauer += zeit_cur;


                //Zeiten eintragen
                if (i != 0)
                {
                    DateTime letzteZeit;
                    if ((route[i - 1].abfahrt == null) || (route[i - 1].abfahrt == new DateTime()))
                    {
                        letzteZeit = route[i - 1].ankunft;
                    }
                    else
                    {
                        letzteZeit = route[i - 1].abfahrt;
                    }

                    if (route[i].wende)
                    {
                        route[i].ankunft = letzteZeit.AddSeconds(zeit_cur);
                        route[i].abfahrt = route[i].ankunft.AddSeconds(30);
                    }
                    else
                    {
                        route[i].abfahrt = letzteZeit.AddSeconds(zeit_cur);
                    }
                }
            }
        }

    }
}