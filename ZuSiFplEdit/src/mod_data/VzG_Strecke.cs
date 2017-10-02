using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    public class VzG_Strecke
    {
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
            }
        }

        public override string ToString()
        {
            return "VzG " + nummer;
        }
    }
}
