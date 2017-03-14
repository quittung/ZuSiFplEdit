using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    public class ZugFahrt
    {
        public class WayPoint
        {
            public streckenModul.referenzElement Signal;
            public List<streckenModul.fahrStr> teilRoute;

            public WayPoint(streckenModul.referenzElement Signal)
            {
                this.Signal = Signal;
            }

            public void streckeBerechnen(streckenModul.referenzElement Zielsignal)
            {
                teilRoute = fstrRouteSearchStart(Zielsignal);
            }

            List<streckenModul.fahrStr> fstrRouteSearchStart(streckenModul.referenzElement ZielSignal)
            {
                var Besucht = new List<streckenModul.fahrStr>();
                foreach (var start_fstr in Signal.abgehendeFahrstraßen)
                {
                    var rList = fstrRouteSearch(start_fstr, ZielSignal, Besucht);
                    if (rList != null) return rList;
                }

                return null;
            }

            List<streckenModul.fahrStr> fstrRouteSearch(streckenModul.fahrStr Aktuell, streckenModul.referenzElement ZielSignal, List<streckenModul.fahrStr> Besucht)
            {
                Besucht.Add(Aktuell);
                if (Aktuell.Ziel.StrElement == ZielSignal.StrElement)
                {
                    var Lizte = new List<streckenModul.fahrStr>();
                    Lizte.Add(Aktuell);
                    return (Lizte);
                }
                else
                {
                    foreach (var folge in Aktuell.folgeStraßen)
                    {
                        if ((!(Besucht.Contains(folge))) && (folge.Ziel != null))
                        {
                            var rList = fstrRouteSearch(folge, ZielSignal, Besucht);
                            if (!(rList == null))
                            {
                                rList.Insert(0, Aktuell);
                                return rList;
                            }
                            Besucht.Add(Aktuell);
                        }
                    }
                }

                return null;
            }

            public override string ToString()
            {
                return (Signal.Info);
            }
        }

        public string Gattung;
        public int Zugnummer;
        public streckenModul.referenzElement ZstartSignal;
        public streckenModul.referenzElement ZzielSignal;
        public List<WayPoint> WayPoints;
        public List<streckenModul.fahrStr> route;

        public ZugFahrt()
        {
            WayPoints = new List<WayPoint>();
            route = new List<streckenModul.fahrStr>();
        }

        public override string ToString()
        {
            return (Gattung + " " + Zugnummer.ToString());
        }

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
                        return;
                    }
                }
            }
        }
    }
}
