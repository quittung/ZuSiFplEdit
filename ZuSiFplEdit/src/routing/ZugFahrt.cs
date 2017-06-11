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

            /// <summary>
            /// Setzt Strecke zum übergebenen Zielsignal
            /// </summary>
            public void streckeBerechnen(streckenModul.referenzElement Zielsignal)
            {
                teilRoute = fstrRouteSearchStart(Zielsignal);
            }

            /// <summary>
            /// Gibt Strecke zu Zielsignal zurück
            /// </summary>
            List<streckenModul.fahrStr> fstrRouteSearchStart(streckenModul.referenzElement ZielSignal)
            {
                var Besucht = new List<streckenModul.fahrStr>();
                foreach (var start_fstr in Signal.abgehendeFahrstraßen)
                {
                    for (int i = -1; i < 10; i++)
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
                    if (Aktuell.wendesignale.Count > 0)
                    {
                        foreach (var wSignal in Aktuell.wendesignale)
                        {
                            foreach (var wSFstr in wSignal.abgehendeFahrstraßen)
                            {
                                var rList = fstrRouteSearch(wSFstr, ZielSignal, Besucht, rekursionstiefe + 1, wendetiefe); if (!(rList == null))
                                {
                                    rList.Insert(0, Aktuell);
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
                WayPoints[i].streckeBerechnen(WayPoints[i + 1].Signal);
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

                    route_ankunft[i] = letzteZeit.AddSeconds(zeit_cur);

                    if ((i < (route.Count - 1)) && (route[i].Ziel != route[i + 1].Start)) //Wendeerkennung
                    {
                        route_abfahrt[i] = route_ankunft[i].AddSeconds(30);
                    }
                }
            }
        }

    }
}