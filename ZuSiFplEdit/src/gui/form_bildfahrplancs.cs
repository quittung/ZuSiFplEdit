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
        public ZugFahrt zug;

        DateTime startZeit;
        DateTime endZeit;

        public double pixProSekunde;
        public double pixProMeter;

        public double länge;
        public double dauer;

        public form_bildfahrplancs(ZugFahrt zug)
        {
            InitializeComponent();

            this.zug = zug;
            zeichnen();
        }

        public void zeichnen()
        {
            if (zug.route.Count() == 0)
            {
                return;
            }

            startZeit = zug.route[0].ankunft;
            endZeit = startZeit.AddHours(1);
            länge = 0;

            foreach (var routenPunkt in zug.route)
            {
                if (routenPunkt.ankunft != new DateTime())
                    endZeit = routenPunkt.ankunft;
                if (routenPunkt.abfahrt != new DateTime())
                    endZeit = routenPunkt.abfahrt;

                länge += routenPunkt.fahrstraße.länge;
            }

            dauer = (endZeit - startZeit).TotalSeconds;

            pixProSekunde = BFP.Height / dauer;
            pixProMeter = BFP.Width / länge;

            double position = 0;

            Bitmap frame = new Bitmap(BFP.Width, BFP.Height);
            Graphics framebuffer = Graphics.FromImage(frame);
            framebuffer.Clear(Color.White);

            //Hilfslinien zeichnen
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


            for (int i = 0; i < zug.route.Count; i++)
            {
                var routenPunkt = zug.route[i];
                position += routenPunkt.fahrstraße.länge;

                //Wartezeiten eintragen
                if (routenPunkt.ankunft != new DateTime() && routenPunkt.abfahrt != new DateTime())
                {
                    int t_start = yAusZeit(routenPunkt.ankunft);
                    int t_ziel  = yAusZeit(routenPunkt.abfahrt);
                    int pos     = (int)(position * pixProMeter);
                    framebuffer.DrawLine(Pens.Black, pos, t_start, pos, t_ziel);
                }


                if (i != 0)
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

                    int pos_start = xAusPosition(position - routenPunkt.fahrstraße.länge);
                    int pos_ziel = xAusPosition(position);

                    framebuffer.DrawLine(Pens.Black, pos_start, t_start, pos_ziel, t_ziel);
                }
            }

            BFP.Image = frame;
        }

        private int yAusZeit(DateTime zeit)
        {
            return (int)((zeit - startZeit).TotalSeconds * pixProSekunde);
        }

        private int xAusPosition(double position)
        {
            return (int)(position * pixProMeter);
        }

        private void BFP_Click(object sender, EventArgs e)
        {
            zeichnen();
        }
    }
}
