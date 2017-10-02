using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    public class Betriebsstelle
    {
        public string name;
        public streckenModul modul;
        public List<streckenModul.Signal> signale;
        public List<VzG_Strecke> strecken;

        public Betriebsstelle(string name, streckenModul modul)
        {
            this.name = name;
            this.modul = modul;

            signale = new List<streckenModul.Signal>();
            strecken = new List<VzG_Strecke>();

            modul.betriebsstellen.Add(this);
        }

        public void streckeEintragen(VzG_Strecke strecke)
        {
            if (!strecken.Contains(strecke))
                strecken.Add(strecke);
        }

        public override string ToString()
        {
            return "Bst " + name;
        }
    }
}
