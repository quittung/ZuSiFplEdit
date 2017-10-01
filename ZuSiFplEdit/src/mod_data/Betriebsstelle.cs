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

        public Betriebsstelle(string name, streckenModul modul)
        {
            this.name = name;
            this.modul = modul;

            signale = new List<streckenModul.Signal>();

            modul.betriebsstellen.Add(this);
        }
    }
}
