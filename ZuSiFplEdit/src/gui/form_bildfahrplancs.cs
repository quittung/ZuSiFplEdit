using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    public partial class form_bildfahrplancs : Form
    {
        public class StreckenAbschnitt
        {
            public VzG_Strecke strecke;
            public streckenModul.Fahrstraße fahrstraße;

            public double längeKm;

            public double kmStart;
            public double kmEnde;

            public int pixStart;
            public int pixEnde;
            
            public int pixAusKm(double km)
            {
                if (pixEnde == 0)
                    return 0;

                return pixStart + (int)(((km - kmStart) / längeKm) * (pixEnde - pixStart));
            }

            public bool erweitern(double km)
            {
                if (km < kmStart)
                    kmStart = km;

                if (km > kmEnde)
                    kmEnde = km;

                längeKm = kmEnde - kmStart;

                return true;
            }

            public bool erweitern(streckenModul.Fahrstraße fahrstraße)
            {
                if (strecke == null)
                    return false;

                var fstr_start = fahrstraße.startSignal.streckenelement.kilometer;
                var fstr_ende = fahrstraße.zielSignal.streckenelement.kilometer;
                if (fstr_start > fstr_ende)
                {
                    var tmp = fstr_start;
                    fstr_start = fstr_ende;
                    fstr_ende = tmp;
                }

                if (kmStart == kmEnde)
                {
                    strecke = fahrstraße.vzgStrecke;
                    kmStart = fstr_start;
                    kmEnde = fstr_ende;
                    längeKm = kmEnde - kmStart;
                    return true;
                }
                
                if (istImAbschnitt(fstr_start) && istImAbschnitt(fstr_ende))
                    return true;
                
                if (!istImAbschnitt(fstr_start) && !istImAbschnitt(fstr_ende))
                    return false;

                if (istImAbschnitt(fstr_start))
                {
                    kmEnde = fstr_ende;
                    längeKm = kmEnde - kmStart;
                    return true;
                }

                if (istImAbschnitt(fstr_ende))
                {
                    kmStart = fstr_start;
                    längeKm = kmEnde - kmStart;
                    return true;
                }

                return false;
            }

            public bool enthält(streckenModul.Fahrstraße fahrstraße)
            {
                return (istImAbschnitt(fahrstraße.startSignal.streckenelement.kilometer) || istImAbschnitt(fahrstraße.zielSignal.streckenelement.kilometer));
            }

            public bool istImAbschnitt(double km)
            {
                return km >= kmStart && km <= kmEnde;
            }


            public override string ToString()
            {
                if (strecke == null)
                {
                    return "Streckenabschnitt";
                }
                else
                {
                    return strecke + " von " + kmStart.ToString("f1") + " bis " + kmEnde.ToString("f1");
                }
            }
        }

        public ZugFahrt referenzZug;
        Fahrplan fahrplan;
        List<StreckenAbschnitt> streckenAbschnitte;

        DateTime startZeit;
        DateTime endZeit;

        public double pixProSekunde;
        public double pixProMeter;

        public double länge;
        public double dauer;

        public form_bildfahrplancs(ZugFahrt zug, Fahrplan fahrplan)
        {
            InitializeComponent();

            this.referenzZug = zug;
            this.fahrplan = fahrplan;
            erneuern();
        }

        public void erneuern()
        {
            if (referenzZug == null || referenzZug.route.Count() == 0)
            {
                return;
            }

            startZeit = referenzZug.route[0].ankunft;
            endZeit = startZeit.AddHours(1);
            länge = 0;

            streckenAbschnitteErstellen();
            xAchseAufbauen();

            foreach (var routenPunkt in referenzZug.route)
            {
                if (routenPunkt.ankunft != new DateTime())
                    endZeit = routenPunkt.ankunft;
                if (routenPunkt.abfahrt != new DateTime())
                    endZeit = routenPunkt.abfahrt;

                länge += routenPunkt.fahrstraße.länge;
            }

            dauer = (endZeit - startZeit).TotalSeconds;

            pixProSekunde = BFP.Height / dauer;

            Bitmap frame = new Bitmap(BFP.Width, BFP.Height);
            Graphics framebuffer = Graphics.FromImage(frame);
            framebuffer.Clear(Color.White);

            //Hilfslinien zeichnen
            zeichneHilfslinien(framebuffer);
            
            zugZeichnen(framebuffer, referenzZug, Pens.Blue);

            foreach (var zug in fahrplan.zugFahrten)
            {
                if (zug != referenzZug)
                {
                    zugZeichnen(framebuffer, zug, Pens.Black);
                }
            }

            BFP.Image = frame;
        }

        private void zugZeichnen(Graphics framebuffer, ZugFahrt zug, Pen pen)
        {
            for (int i = 0; i < zug.route.Count; i++)
            {
                var routenPunkt = zug.route[i];
                var streckenPunktStart = routenPunkt.fahrstraße.startSignal.findeStreckenPunkt(streckenAbschnitte);
                var streckenPunktZiel = routenPunkt.fahrstraße.zielSignal.findeStreckenPunkt(streckenAbschnitte);
                if (streckenPunktZiel == null || streckenPunktZiel.km == 0)
                    continue;


                if (i != 0 && !(streckenPunktStart == null || streckenPunktStart.km == 0))
                {
                    var letzterPunkt = zug.route[i - 1];

                    var z_start = letzterPunkt.ankunft;
                    if (letzterPunkt.abfahrt != new DateTime())
                        z_start = letzterPunkt.abfahrt;
                    int t_start = yAusZeit(z_start);

                    var z_ziel = routenPunkt.abfahrt;
                    if (routenPunkt.ankunft != new DateTime())
                        z_ziel = routenPunkt.ankunft;
                    int t_ziel = yAusZeit(z_ziel);

                    int pos_start = xAusKmAufStrecke(streckenPunktStart.km, streckenPunktStart.strecke);
                    int pos_ziel = xAusKmAufStrecke(streckenPunktZiel.km, streckenPunktZiel.strecke);

                    framebuffer.DrawLine(pen, pos_start, t_start, pos_ziel, t_ziel);
                }

                //Wartezeiten eintragen
                if (routenPunkt.ankunft != new DateTime() && routenPunkt.abfahrt != new DateTime())
                {
                    int t_start = yAusZeit(routenPunkt.ankunft);
                    int t_ziel = yAusZeit(routenPunkt.abfahrt);
                    int pos = xAusKmAufStrecke(streckenPunktZiel.km, streckenPunktZiel.strecke);
                    framebuffer.DrawLine(pen, pos, t_start, pos, t_ziel);
                }
            }
        }

        /// <summary>
        /// Erstellt alle Streckenabschnitte, die für eine eindimensionale Darstellung der Strecke benötigt werden
        /// </summary>
        private void streckenAbschnitteErstellen()
        {
            streckenAbschnitte = new List<StreckenAbschnitt>();
            foreach (var routenPunkt in referenzZug.route)
            {
                var Signal = routenPunkt.fahrstraße.startSignal;
                foreach (var streckenPunkt in Signal.streckenPunkte)
                {
                    if (streckenPunkt.km != 0)
                    {
                        var streckenAbschnitt = sucheStreckenAbschnitt(streckenPunkt.strecke, streckenPunkt.km);
                        streckenAbschnitt.erweitern(streckenPunkt.km);
                    }
                }
                Signal = routenPunkt.fahrstraße.zielSignal;
                foreach (var streckenPunkt in Signal.streckenPunkte)
                {
                    if (streckenPunkt.km != 0)
                    {
                        var streckenAbschnitt = sucheStreckenAbschnitt(streckenPunkt.strecke, streckenPunkt.km);
                        streckenAbschnitt.erweitern(streckenPunkt.km);
                    }
                }

            }
        }

        private void xAchseAufbauen()
        {
            int gesamtBreite = BFP.Width;
            double gesamtLänge = 0;
            foreach (var abschnitt in streckenAbschnitte)
            {
                gesamtLänge += abschnitt.längeKm;
            }

            int pixX = 0;
            foreach (var abschnitt in streckenAbschnitte)
            {
                abschnitt.pixStart = pixX;
                pixX += (int)((abschnitt.längeKm / gesamtLänge) * gesamtBreite);
                abschnitt.pixEnde = pixX;
            }
        }
        
        private void zeichneHilfslinien(Graphics framebuffer)
        {
            var z_hilf = startZeit;
            z_hilf = z_hilf.AddMinutes(-z_hilf.Minute);
            z_hilf = z_hilf.AddSeconds(-z_hilf.Second);

            while (z_hilf < endZeit)
            {
                int t_hilf = yAusZeit(z_hilf);

                framebuffer.DrawLine(Pens.LightGray, 0, t_hilf, BFP.Width, t_hilf);
                framebuffer.DrawString(z_hilf.ToString("hh:mm"), new Font("Verdana", 8), Brushes.LightGray, 6, t_hilf);
                z_hilf = z_hilf.AddMinutes(30);
            }
            
            foreach (var routenPunkt in referenzZug.route)
            {
                foreach (var streckenPunkt in routenPunkt.fahrstraße.zielSignal.streckenPunkte)
                {
                    if (sucheStreckenAbschnitt(streckenPunkt.strecke) == null)//TODO: Einzelne Fahrstraßen vernünftig einbinden
                        continue;

                    var signal = routenPunkt.fahrstraße.zielSignal;
                    int pos_signal = xAusKmAufStrecke(streckenPunkt.km, streckenPunkt.strecke);
                    framebuffer.DrawLine(Pens.LightGray, pos_signal, 0, pos_signal, BFP.Height);
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                    framebuffer.DrawString(streckenPunkt.km.ToString("f1") + " - " + signal.betriebsstelle.name + " " + signal.name, new Font("Verdana", 8), Brushes.LightGray, pos_signal, 6, stringFormat); 
                }
            }
        }
        
        private StreckenAbschnitt sucheStreckenAbschnitt(VzG_Strecke strecke)
        {
            foreach (var abschnitt in streckenAbschnitte)
            {
                if (abschnitt.strecke == strecke)
                {
                    return abschnitt;
                }
            }

            return null;
        }

        private StreckenAbschnitt sucheStreckenAbschnitt(VzG_Strecke strecke, double km)
        {
            foreach (var abschnitt in streckenAbschnitte)
            {
                if (abschnitt.strecke == strecke)
                {
                    return abschnitt;
                }
            }

            var abschnittNeu = new StreckenAbschnitt();
            abschnittNeu.strecke = strecke;
            abschnittNeu.kmStart = km;
            abschnittNeu.kmEnde = km;
            streckenAbschnitte.Add(abschnittNeu);
            return abschnittNeu;
        }

        private StreckenAbschnitt sucheStreckenAbschnitt(VzG_Strecke strecke, streckenModul.Fahrstraße fahrstraße)
        {
            foreach (var abschnitt in streckenAbschnitte)
            {
                if (abschnitt.strecke == strecke && abschnitt.enthält(fahrstraße))
                {
                    return abschnitt;
                }
            }

            var abschnittNeu = new StreckenAbschnitt();
            abschnittNeu.strecke = strecke;
            streckenAbschnitte.Add(abschnittNeu);
            return abschnittNeu;
        }

        private int yAusZeit(DateTime zeit)
        {
            return (int)((zeit - startZeit).TotalSeconds * pixProSekunde);
        }

        private int xAusKmAufStrecke(double km, VzG_Strecke strecke)
        {
            var abschnitt = sucheStreckenAbschnitt(strecke);
            return abschnitt.pixAusKm(km);
        }

        private void BFP_Click(object sender, EventArgs e)
        {
            erneuern();
        }
    }
}
