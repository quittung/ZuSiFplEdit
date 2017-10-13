using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    public class VzG_Strecke
    {
        public class streckenPunkt
        {
            public VzG_Strecke strecke;
            public double km;
            public bool aufwärts;

            public streckenPunkt(VzG_Strecke strecke, bool aufwärts, double km)
            {
                this.strecke = strecke;
                this.aufwärts = aufwärts;
                this.km = km;
            }

            public streckenPunkt(VzG_Strecke strecke, bool aufwärts)
            {
                this.strecke = strecke;
                this.aufwärts = aufwärts;
            }

            public override string ToString()
            {
                return strecke.ToString() + " Km " + km.ToString("f1");
            }

            public override bool Equals(object obj)
            {
                return strecke == ((streckenPunkt)obj).strecke;
            }

            public override int GetHashCode()
            {
                return strecke.GetHashCode();
            }
        }

        public int nummer;
        public List<Betriebsstelle> betriebsstellen;
        public List<streckenModul.Fahrstraße> fahrstraßen;

        public VzG_Strecke(int nummer)
        {
            this.nummer = nummer;

            betriebsstellen = new List<Betriebsstelle>();
            fahrstraßen = new List<streckenModul.Fahrstraße>();
        }

        public void betriebsstellenEintragen(Betriebsstelle betriebsstelle)
        {
            if (!betriebsstellen.Contains(betriebsstelle))
                betriebsstellen.Add(betriebsstelle);

            betriebsstelle.streckeEintragen(this);
        }

        public void fahrstraßeEintragen(streckenModul.Fahrstraße fahrstraße)
        {
            if (!fahrstraßen.Contains(fahrstraße))
            {
                fahrstraßen.Add(fahrstraße);
                foreach (var betriebsstelle in fahrstraße.betriebsstellen)
                {
                    betriebsstellenEintragen(betriebsstelle);
                }
                fahrstraße.vzgStrecke = this;
            }
        }

        public override string ToString()
        {
            return "VzG " + nummer;
        }
    }
}
