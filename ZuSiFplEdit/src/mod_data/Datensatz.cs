using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Beinhaltet die vollständig formatierten Streckenmodule und zugehörige lookup-Funktionen
    /// </summary>
    public class Datensatz
    {
        /// <summary>
        /// Metadaten aller Module sind zur Darstellung bereit
        /// </summary>
        public bool moduleBereit = false;
        public bool elementeBereit = false;
        public bool signaleBereit = false;
        public bool fahrstraßenBereit = false;
        public List<streckenModul> module;
        public List<Betriebsstelle> betriebsstellen;
        public List<VzG_Strecke> strecken;
        public string datenVerzeichnis;

        public Datensatz(string datenVerzeichnis)
        {
            module = new List<streckenModul>();
            betriebsstellen = new List<Betriebsstelle>();
            strecken = new List<VzG_Strecke>();

            this.datenVerzeichnis = datenVerzeichnis;
        }

        public streckenModul sucheModul(string name)
        {
            if (module.Count == 0)
                return null;

            foreach (var modul in module)
                if (modul.name == name) return modul;

            return null;
        }

        public Betriebsstelle sucheBetriebsstelle(string name)
        {
            foreach (var betriebsstelle in betriebsstellen)
            {
                if (betriebsstelle.name == name)
                    return betriebsstelle;
            }
            return null;
        }

        public VzG_Strecke sucheStrecke(int nummer)
        {
            foreach (var strecke in strecken)
            {
                if (strecke.nummer == nummer)
                    return strecke;
            }
            var strecke_neu = new VzG_Strecke(nummer);
            strecken.Add(strecke_neu);
            return strecke_neu;
        }

        public Betriebsstelle sucheBetriebsstelle(string name, streckenModul modul)
        {
            var betriebsstelle = sucheBetriebsstelle(name);
            if (betriebsstelle == null)
            {
                betriebsstelle = new Betriebsstelle(name, modul);
                betriebsstellen.Add(betriebsstelle);
            }
            return betriebsstelle;
        }

        public streckenModul.Fahrstraße sucheFahrstraße(string name)
        {
            foreach (var modul in module)
            {
                foreach (var fahrstraße in modul.fahrstraßen)
                {
                    if (fahrstraße.name == name)
                    {
                        return fahrstraße;
                    }
                }
            }
            return null;
        }

        public streckenModul.Signal sucheSignal(string betriebststelle, string name)
        {
            foreach (var modul in module)
            {
                foreach (var signal in modul.signale)
                {
                    if (signal.betriebsstelle != null && signal.betriebsstelle.name == betriebststelle && signal.name == name)
                    {
                        return signal;
                    }
                }
            }
            return null;
        }
    }
}
