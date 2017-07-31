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
            /// <summary>
            /// Signal des Wegpunkts
            /// </summary>
            public streckenModul.referenzElement Signal;
            /// <summary>
            /// Enthält die Strecke zum nächsten zuletzt übergebenen Zielsignal, wenn möglich
            /// </summary>
            public List<streckenModul.fahrStr> teilRoute;

            /// <summary>
            /// Konstruktor für Wegpunkt von Signal
            /// </summary>
            public WayPoint(streckenModul.referenzElement Signal)
            {
                this.Signal = Signal;
            }


            public class aStarNode
            {
                public streckenModul.referenzElement Node;
                public double DistanceFromStart;
                public double HeuristicDistanceToTarget;
                public double OverallDistance;
                public aStarNode PreviousNode;
                public streckenModul.fahrStr PreviousVertex;

                public aStarNode(streckenModul.referenzElement Node, aStarNode PreviousNode, streckenModul.fahrStr PreviousVertex, streckenModul.referenzElement target)
                {
                    this.Node = Node;
                    this.PreviousNode = PreviousNode;
                    this.PreviousVertex = PreviousVertex;

                    if (PreviousNode == null)
                        DistanceFromStart = 0;
                    else
                        DistanceFromStart = PreviousNode.DistanceFromStart + PreviousVertex.LaengeGewichtet;

                    HeuristicDistanceToTarget = Node.SignalCoord.distanceTo(target.SignalCoord) * 1000f;

                    OverallDistance = DistanceFromStart + HeuristicDistanceToTarget;
                }

                public void updateNode(aStarNode PreviousNode, streckenModul.fahrStr PreviousVertex)
                {
                    var alternativeDistanceFromStart = PreviousNode.DistanceFromStart + PreviousVertex.LaengeGewichtet;
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
            /// Setzt Strecke zum übergebenen Zielsignal
            /// </summary>
            public void streckeBerechnen(streckenModul.referenzElement Startsignal, streckenModul.referenzElement Zielsignal)
            {
                //teilRoute = fstrRouteSearchStart(Zielsignal);

                var OpenNodes = new List<aStarNode>();
                var ClosedNodes = new List<aStarNode>();

                var CurrentNode = new aStarNode(Startsignal, null, null, Zielsignal);
                

                while (true)
                {
                    ClosedNodes.Add(CurrentNode);

                    foreach (var AdjacentVertex in CurrentNode.Node.abgehendeFahrstraßen)
                    {
                        if ((AdjacentVertex.Ziel != null) && (findDuplicateAStarByNode(AdjacentVertex.Ziel, ClosedNodes) == null))
                        {
                            var DuplicateNode = findDuplicateAStarByNode(AdjacentVertex.Ziel, OpenNodes);
                            if (DuplicateNode == null)
                                OpenNodes.Add(new aStarNode(AdjacentVertex.Ziel, CurrentNode, AdjacentVertex, Zielsignal)); 
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


                teilRoute = new List<streckenModul.fahrStr>();
                while (true)
                {
                    if (CurrentNode.PreviousNode == null)
                        break;
                    teilRoute.Insert(0, CurrentNode.PreviousVertex);
                    CurrentNode = CurrentNode.PreviousNode;
                }
            }

            int CompareAStarByOverallLength(aStarNode x, aStarNode y)
            {
                return (x.OverallDistance.CompareTo(y.OverallDistance));
            } 

            aStarNode findDuplicateAStarByNode(streckenModul.referenzElement Node, List<aStarNode> List)
            {
                foreach (var ListNode in List)
                {
                    if (ListNode.Node == Node)
                        return ListNode;
                }
                return null;
            }

            /// <summary>
            /// Gibt Strecke zu Zielsignal zurück
            /// </summary>
            List<streckenModul.fahrStr> fstrRouteSearchStart(streckenModul.referenzElement ZielSignal)
            {
                var Besucht = new List<streckenModul.fahrStr>();
                for (int i = -1; i < 10; i++)
                {
                    Besucht = new List<streckenModul.fahrStr>();
                    foreach (var start_fstr in Signal.abgehendeFahrstraßen)
                    {
                        var rList = fstrRouteSearch(start_fstr, ZielSignal, Besucht, 0, i);
                        if (rList != null) return rList; 
                    }
                }

                return null;
            }

            /// <summary>
            /// Rekursiver Teil der Streckensuche
            /// </summary>
            List<streckenModul.fahrStr> fstrRouteSearch(streckenModul.fahrStr Aktuell, streckenModul.referenzElement ZielSignal, List<streckenModul.fahrStr> Besucht, int rekursionstiefe, int wendetiefe)
            {
                Besucht.Add(Aktuell);
                //Sofort aufhören, wenn Zielsignal gefunden.
                if (Aktuell.Ziel.StrElement == ZielSignal.StrElement)
                {
                    var Lizte = new List<streckenModul.fahrStr>();
                    Lizte.Add(Aktuell);
                    return (Lizte);
                }


                if (rekursionstiefe == wendetiefe)
                {
                    if ((Aktuell.Start.wendeSignale != null) && (Aktuell.Start.wendeSignale.Count > 0))
                    {
                        foreach (var wSignal in Aktuell.Start.wendeSignale)
                        {
                            if (wSignal == ZielSignal)
                            {
                                streckenModul.fahrStr hilfsfstr = erstelleWendehilfsfahrstraße(Aktuell.Ziel, ZielSignal);
                                var Lizte = new List<streckenModul.fahrStr>();
                                Lizte.Add(hilfsfstr);
                                return (Lizte);
                            }
                        }
                    }
                    
                    if (Aktuell.wendesignale.Count > 0)
                    {
                        
                        foreach (var wSignal in Aktuell.wendesignale)
                        {
                            if (wSignal == ZielSignal)
                            {
                                streckenModul.fahrStr hilfsfstr = erstelleWendehilfsfahrstraße(Aktuell.Ziel, ZielSignal);
                                var Lizte = new List<streckenModul.fahrStr>();
                                Lizte.Add(Aktuell);
                                Lizte.Add(hilfsfstr);
                                return (Lizte);
                            }
                            foreach (var wSFstr in wSignal.abgehendeFahrstraßen)
                            {
                                var rList = fstrRouteSearch(wSFstr, ZielSignal, Besucht, rekursionstiefe + 1, wendetiefe);
                                if (!(rList == null))
                                {
                                    //TODO: Implement commented stuff.
                                    //streckenModul.fahrStr hilfsfstr = erstelleWendehilfsfahrstraße(Aktuell, ZielSignal);
                                    rList.Insert(0, Aktuell);
                                    //rList.Add(hilfsfstr);
                                    return rList;
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }


                //Weiter vorwärts suchen.
                foreach (var folge in Aktuell.folgestraßen)
                {
                    if ((!(Besucht.Contains(folge))) && (folge.Ziel != null))
                    {
                        var rList = fstrRouteSearch(folge, ZielSignal, Besucht, rekursionstiefe + 1, wendetiefe);
                        if (!(rList == null))
                        {
                            //MessageBox.Show("Weg gefunden nach " + rekursionstiefe + " Rekursionen.");
                            rList.Insert(0, Aktuell);
                            return rList;
                        }
                        Besucht.Add(Aktuell);
                    }
                }
                

                //MessageBox.Show("Gebe auf nach " + rekursionstiefe + " Rekursionen.");
                return null;
            }

            private static streckenModul.fahrStr erstelleWendehilfsfahrstraße(streckenModul.referenzElement StartSignal, streckenModul.referenzElement ZielSignal)
            {
                var distanz = (float) StartSignal.SignalCoord.distanceTo(ZielSignal.SignalCoord);
                var hilfsfstr = new streckenModul.fahrStr("Wendehilfsfahrstraße", "0", 2, "TypWende", distanz, StartSignal.ReferenzNr, StartSignal.Modul.modName, ZielSignal.ReferenzNr, ZielSignal.Modul.modName); //TODO: RglGgl
                hilfsfstr.StartMod = StartSignal.Modul;
                hilfsfstr.ZielMod = ZielSignal.Modul;
                hilfsfstr.Start = StartSignal;
                hilfsfstr.Ziel = ZielSignal;
                return hilfsfstr;
            }

            public override string ToString()
            {
                return (Signal.Info);
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
        public streckenModul.referenzElement ZstartSignal;
        /// <summary>
        /// Veraltet: Enthält Zielsignal der Zugfahrt. Ersetzt durch Wegpunkt-System.
        /// </summary>
        [Obsolete]
        public streckenModul.referenzElement ZzielSignal;
        /// <summary>
        /// Enthält alle Wegpunkte der Zugfahrt
        /// </summary>
        public List<WayPoint> WayPoints;

        public float vMax;

        /// <summary>
        /// </summary>
        public float route_länge;
        /// <summary>
        /// </summary>
        public long route_dauer;
        /// <summary>
        /// Enthält die Route der Zugfahrt als Liste von Fahrstraßen, wenn berechnet. 
        /// </summary>
        public List<streckenModul.fahrStr> route;
        /// <summary>
        /// </summary>
        public DateTime[] route_ankunft;
        /// <summary>
        /// </summary>
        public DateTime[] route_abfahrt;

        /// <summary>
        /// Konstruktor der Klasse ZugFahrt
        /// </summary>
        public ZugFahrt()
        {
            WayPoints = new List<WayPoint>();
            route = new List<streckenModul.fahrStr>();
            vMax = 20;
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
            route.Clear();

            if (WayPoints.Count < 2)
            {
                MessageBox.Show("Weniger als zwei Wegpunkte sind bekannt.\nFahrweg kann nicht berechnet werden.");
                return;
            }

            for (int i = 0; i < WayPoints.Count - 1; i++)
            {
                WayPoints[i].streckeBerechnen(WayPoints[i].Signal, WayPoints[i + 1].Signal);
                if (WayPoints[i].teilRoute != null)
                {
                    route.AddRange(WayPoints[i].teilRoute);
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
                        MessageBox.Show("Die Route konnte nur bis \"" + route[route.Count - 1].Ziel.Info + "\" bestimmt werden.");
                        break;
                    }
                }
            }
            route_metadaten_berechen();
        }

        public void route_metadaten_berechen()
        {
            route_ankunft = new DateTime[route.Count];
            route_abfahrt = new DateTime[route.Count];

            float v_ms_max = 20;

            route_dauer = 0;
            route_länge = 0;

            route_ankunft[0] = Convert.ToDateTime("2017-02-27 12:00:00");
            route_abfahrt[0] = Convert.ToDateTime("2017-02-27 12:00:20");

            for (int i = 1; i < route.Count; i++)
            {
                //Strecke suchen
                float strecke_cur = route[i].Laenge;
                //Zeit berechnen
                long zeit_cur = (long)(strecke_cur / v_ms_max);
                //Strecke aufaddieren
                route_länge += strecke_cur;
                //Zeit aufaddieren
                route_dauer += zeit_cur;


                //Zeiten eintragen
                if (i != 0)
                {
                    DateTime letzteZeit;
                    if ((route_abfahrt[i - 1] == null) || (route_abfahrt[i - 1] == new DateTime()))
                    {
                        letzteZeit = route_ankunft[i - 1];
                    }
                    else
                    {
                        letzteZeit = route_abfahrt[i - 1];
                    }

                    if (((i < (route.Count - 1)) && (route[i + 1].FahrstrTyp == "TypWende")) || (i == route.Count - 1)) //Wendeerkennung
                    {
                        route_ankunft[i] = letzteZeit.AddSeconds(zeit_cur);
                        route_abfahrt[i] = route_ankunft[i].AddSeconds(30);
                    }
                    else
                    {
                        route_abfahrt[i] = letzteZeit.AddSeconds(zeit_cur);
                    }
                        

                    //route_abfahrt[i] = letzteZeit.AddSeconds(zeit_cur);

                    //if (((i < (route.Count - 1)) && (route[i].Ziel != route[i + 1].Start)) || (route[i].FahrstrName == "Wendehilfsfahrstraße")) //Wendeerkennung
                    //{
                    //    route_ankunft[i] = route_abfahrt[i];
                    //    route_abfahrt[i] = route_ankunft[i].AddSeconds(30);
                    //}
                }
            }

            route_abfahrt[route.Count - 1] = route_ankunft[route.Count - 1].AddSeconds(30);
        }

    }
}