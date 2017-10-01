using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Sammelklasse für den Fahrplan mit allen zugehörigen Informationen
    /// </summary>
    class Fahrplan
    {
        /// <summary>
        /// Simulationsstartpunkt
        /// </summary>
        public DateTime anfangszeit;
        /// <summary>
        /// Link zu Befehlskonfiguration
        /// </summary>
        public string befehlsKonfig;

        /// <summary>
        /// Liste mit allen Zügen
        /// </summary>
        public List<ZugFahrt> zugFahrten;
        /// <summary>
        /// Liste mit allen für den Fahrplan ausgewählten Modulen
        /// </summary>
        public List<streckenModul> module;
        
        /// <summary>
        /// Bereitet die Listen vor und gibt Standardwerte vor
        /// </summary>
        public Fahrplan()
        {
            anfangszeit = DateTime.Now;
            befehlsKonfig = "Signals\\Deutschland\\Befehle\\408_2003.authority.xml";

            zugFahrten = new List<ZugFahrt>();
            module = new List<streckenModul>();
        }

        /// <summary>
        /// Sammelt finale Daten für den Export
        /// </summary>
        public void exportVorbereiten()
        {
            //Anfangszeit bestimmen
            if (zugFahrten.Count == 0)
            {
                anfangszeit = DateTime.Now;
                return;
            }
            anfangszeit = zugFahrten[0].route[0].ankunft;
            foreach (var zugFahrt in zugFahrten)
            {
                if (zugFahrt.route[0].ankunft < anfangszeit)
                    anfangszeit = zugFahrt.route[0].ankunft;
            }

            //Module auswählen
            module.Clear();
            foreach (var zugFahrt in zugFahrten)
            {
                foreach (var routenPunkt in zugFahrt.route)
                {
                    modulHinzufügen(routenPunkt.fahrstraße.startSignal.modul);
                    foreach (var nachbar in routenPunkt.fahrstraße.startSignal.modul.nachbarn)
                    {
                        modulHinzufügen(nachbar);

                    }

                    modulHinzufügen(routenPunkt.fahrstraße.zielSignal.modul);
                    foreach (var nachbar in routenPunkt.fahrstraße.zielSignal.modul.nachbarn)
                    {
                        modulHinzufügen(nachbar);
                    }
                } 
            }
            //Nur für Legacy-Support
            foreach (var modul in module)
            {
                modul.selected = true;
            }
        }

        /// <summary>
        /// Fügt Module zur Liste hinzu, wenn es noch nicht in der Liste vorhanden ist
        /// </summary>
        /// <param name="modul"></param>
        public void modulHinzufügen(streckenModul modul)
        {
            if (!module.Contains(modul))
                module.Add(modul);
        }

        /// <summary>
        /// Ist eine Zugnummer bereits vergeben?
        /// </summary>
        /// <param name="zugnummer"></param>
        /// <returns></returns>
        public bool zugnummerVergeben(long zugnummer)
        {
            bool ZugNummerBesetzt = false;
            foreach (ZugFahrt zug in zugFahrten)
            {
                if (zug.zugnummer == zugnummer)
                {
                    ZugNummerBesetzt = true;
                    break;
                }
            }
            return ZugNummerBesetzt;
        }
    }
}
