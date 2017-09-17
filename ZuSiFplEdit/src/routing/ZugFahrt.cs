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
            public streckenModul.Signal signal;
            /// <summary>
            /// Enthält die Strecke zum nächsten zuletzt übergebenen Zielsignal, wenn möglich
            /// </summary>
            public List<streckenModul.Fahrstraße> teilRoute;

            /// <summary>
            /// Konstruktor für Wegpunkt von Signal
            /// </summary>
            public WayPoint(streckenModul.Signal signal)
            {
                this.signal = signal;
            }


            public class aStarNode
            {
                public streckenModul.Signal Node;
                public double DistanceFromStart;
                public double HeuristicDistanceToTarget;
                public double OverallDistance;
                public aStarNode PreviousNode;
                public streckenModul.Fahrstraße PreviousVertex;

                public float vMax = 160f / 3.6f;

                public aStarNode(streckenModul.Signal Node, aStarNode PreviousNode, streckenModul.Fahrstraße PreviousVertex, streckenModul.Signal target)
                {
                    this.Node = Node;
                    this.PreviousNode = PreviousNode;
                    this.PreviousVertex = PreviousVertex;

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
            /// Setzt Strecke zum übergebenen Zielsignal
            /// </summary>
            public void streckeBerechnen(streckenModul.Signal Startsignal, streckenModul.Signal Zielsignal)
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
                        if ((AdjacentVertex.zielSignal != null) && (findDuplicateAStarByNode(AdjacentVertex.zielSignal, ClosedNodes) == null))
                        {
                            var DuplicateNode = findDuplicateAStarByNode(AdjacentVertex.zielSignal, OpenNodes);
                            if (DuplicateNode == null)
                                OpenNodes.Add(new aStarNode(AdjacentVertex.zielSignal, CurrentNode, AdjacentVertex, Zielsignal)); 
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

                while (true)
                {
                    if (CurrentNode.PreviousNode == null)
                        break;
                    teilRoute.Insert(0, CurrentNode.PreviousVertex);

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

            /// <summary>
            /// Gibt Strecke zu Zielsignal zurück
            /// </summary>
            //[Obsolete]
            //List<st3Modul.fahrStr> fstrRouteSearchStart(st3Modul.referenzElement ZielSignal)
            //{
            //    var Besucht = new List<st3Modul.fahrStr>();
            //    for (int i = -1; i < 10; i++)
            //    {
            //        Besucht = new List<st3Modul.fahrStr>();
            //        foreach (var start_fstr in signal.abgehendeFahrstraßen)
            //        {
            //            var rList = fstrRouteSearch(start_fstr, ZielSignal, Besucht, 0, i);
            //            if (rList != null) return rList; 
            //        }
            //    }

            //    return null;
            //}

            /// <summary>
            /// Rekursiver Teil der Streckensuche
            /// </summary>
            // [Obsolete]
            //List<st3Modul.fahrStr> fstrRouteSearch(st3Modul.fahrStr Aktuell, st3Modul.referenzElement ZielSignal, List<st3Modul.fahrStr> Besucht, int rekursionstiefe, int wendetiefe)
            //{
            //    Besucht.Add(Aktuell);
            //    //Sofort aufhören, wenn Zielsignal gefunden.
            //    if (Aktuell.Ziel.StrElement == ZielSignal.StrElement)
            //    {
            //        var Lizte = new List<st3Modul.fahrStr>();
            //        Lizte.Add(Aktuell);
            //        return (Lizte);
            //    }


            //    if (rekursionstiefe == wendetiefe)
            //    {
            //        if ((Aktuell.Start.wendeSignale != null) && (Aktuell.Start.wendeSignale.Count > 0))
            //        {
            //            foreach (var wSignal in Aktuell.Start.wendeSignale)
            //            {
            //                if (wSignal == ZielSignal)
            //                {
            //                    st3Modul.fahrStr hilfsfstr = erstelleWendehilfsfahrstraße(Aktuell.Ziel, ZielSignal);
            //                    var Lizte = new List<st3Modul.fahrStr>();
            //                    Lizte.Add(hilfsfstr);
            //                    return (Lizte);
            //                }
            //            }
            //        }
                    
            //        if (Aktuell.wendesignale.Count > 0)
            //        {
                        
            //            foreach (var wSignal in Aktuell.wendesignale)
            //            {
            //                if (wSignal == ZielSignal)
            //                {
            //                    st3Modul.fahrStr hilfsfstr = erstelleWendehilfsfahrstraße(Aktuell.Ziel, ZielSignal);
            //                    var Lizte = new List<st3Modul.fahrStr>();
            //                    Lizte.Add(Aktuell);
            //                    Lizte.Add(hilfsfstr);
            //                    return (Lizte);
            //                }
            //                foreach (var wSFstr in wSignal.abgehendeFahrstraßen)
            //                {
            //                    var rList = fstrRouteSearch(wSFstr, ZielSignal, Besucht, rekursionstiefe + 1, wendetiefe);
            //                    if (!(rList == null))
            //                    {
            //                        //TODO: Implement commented stuff.
            //                        //streckenModul.fahrStr hilfsfstr = erstelleWendehilfsfahrstraße(Aktuell, ZielSignal);
            //                        rList.Insert(0, Aktuell);
            //                        //rList.Add(hilfsfstr);
            //                        return rList;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            return null;
            //        }
            //    }


            //    //Weiter vorwärts suchen.
            //    foreach (var folge in Aktuell.folgestraßen)
            //    {
            //        if ((!(Besucht.Contains(folge))) && (folge.Ziel != null))
            //        {
            //            var rList = fstrRouteSearch(folge, ZielSignal, Besucht, rekursionstiefe + 1, wendetiefe);
            //            if (!(rList == null))
            //            {
            //                //MessageBox.Show("Weg gefunden nach " + rekursionstiefe + " Rekursionen.");
            //                rList.Insert(0, Aktuell);
            //                return rList;
            //            }
            //            Besucht.Add(Aktuell);
            //        }
            //    }
                

            //    //MessageBox.Show("Gebe auf nach " + rekursionstiefe + " Rekursionen.");
            //    return null;
            //}

            private st3Modul.fahrStr erstelleWendehilfsfahrstraße(st3Modul.referenzElement StartSignal, st3Modul.referenzElement ZielSignal)
            {
                var distanz = (float) StartSignal.SignalCoord.distanceTo(ZielSignal.SignalCoord);
                var hilfsfstr = new st3Modul.fahrStr("Wendehilfsfahrstraße", "0", 2, "TypWende", distanz, StartSignal.ReferenzNr, StartSignal.Modul.modName, ZielSignal.ReferenzNr, ZielSignal.Modul.modName); //TODO: RglGgl
                hilfsfstr.StartMod = StartSignal.Modul;
                hilfsfstr.ZielMod = ZielSignal.Modul;
                hilfsfstr.Start = StartSignal;
                hilfsfstr.Ziel = ZielSignal;
                return hilfsfstr;
            }

            public override string ToString()
            {
                return (signal.info);
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

        public float vMax;

        /// <summary>
        /// </summary>
        public double route_länge;
        /// <summary>
        /// </summary>
        public long route_dauer;
        /// <summary>
        /// Enthält die Route der Zugfahrt als Liste von Fahrstraßen, wenn berechnet. 
        /// </summary>
        public List<streckenModul.Fahrstraße> route;
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
        public ZugFahrt()
        {
            WayPoints = new List<WayPoint>();
            route = new List<streckenModul.Fahrstraße>();
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
            route.Clear();
            includeSignal.Clear();


            if (WayPoints.Count < 2)
            {
                MessageBox.Show("Weniger als zwei Wegpunkte sind bekannt.\nFahrweg kann nicht berechnet werden.");
                return;
            }

            for (int i = 0; i < WayPoints.Count - 1; i++)
            {
                WayPoints[i].streckeBerechnen(WayPoints[i].signal, WayPoints[i + 1].signal);
                if (WayPoints[i].teilRoute != null)
                {
                    route.AddRange(WayPoints[i].teilRoute);
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
                        MessageBox.Show("Die Route konnte nur bis \"" + route[route.Count - 1].zielSignal.name + "\" bestimmt werden.");
                        break;
                    }
                }
            }
            includeSignal[0] = true;
            route_metadaten_berechen();
        }

        public void route_metadaten_berechen()
        {
            

            route_ankunft = new DateTime[route.Count];
            route_abfahrt = new DateTime[route.Count];
            
            route_dauer = 0;
            route_länge = 0;

            //route_ankunft[0] = Convert.ToDateTime("2017-02-27 12:00:00");
            route_abfahrt[0] = Convert.ToDateTime("2017-02-27 12:00:00");

            for (int i = 1; i < route.Count; i++)
            {
                //Strecke suchen
                double strecke_cur = route[i].länge;
                //Zeit berechnen
                long zeit_cur = (long)route[i].berechneFahrdauer(vMax);
                //Strecke aufaddieren
                route_länge += strecke_cur;
                //Zeit aufaddieren
                route_dauer += zeit_cur;


                //Zeiten eintragen
                if (i != 0)
                {
                    if ((i != route.Count - 1) && (route[i + 1].folgeStraßen.Count > 1))
                        includeSignal[i] = true;

                    DateTime letzteZeit;
                    if ((route_abfahrt[i - 1] == null) || (route_abfahrt[i - 1] == new DateTime()))
                    {
                        letzteZeit = route_ankunft[i - 1];
                    }
                    else
                    {
                        letzteZeit = route_abfahrt[i - 1];
                    }

                    if (((i < (route.Count - 1)) && (route[i + 1].typ == "TypWende")) || (i == route.Count - 1)) //Wendeerkennung
                    {
                        includeSignal[i] = true;
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