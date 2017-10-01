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
        public string datenVerzeichnis;

        public Datensatz(string datenVerzeichnis)
        {
            module = new List<streckenModul>();

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
                    if (signal.betriebsstelle == betriebststelle && signal.name == name)
                    {
                        return signal;
                    }
                }
            }
            return null;
        }
    }
}
