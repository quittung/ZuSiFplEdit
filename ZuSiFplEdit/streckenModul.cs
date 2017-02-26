using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    public class streckenModul
    {
        /// <summary>
        /// Relativer Pfad zum Modul in Zusi-Bibliothek
        /// </summary>
        public string modPath;
        /// <summary>
        /// Name des Moduls
        /// </summary>
        public string modName;

        /// <summary>
        /// UMT-Nordwert von Modul
        /// </summary>
        public int UTM_NS;
        /// <summary>
        /// UMT-Ostwert von Modul
        /// </summary>
        public int UTM_WE;
        /// <summary>
        /// UMT-Meridianzone von Modul
        /// </summary>
        public int UTM_Z1;
        /// <summary>
        /// UMT Latitude Band von Modul
        /// </summary>
        public char UTM_Z2;

        /// <summary>
        /// Pixel-Position auf X-Achse auf Karte beim letzten Zeichenvorgang
        /// </summary>
        public int PIX_X;
        /// <summary>
        /// Pixel-Position auf Y-Achse auf Karte beim letzten Zeichenvorgang
        /// </summary>
        public int PIX_Y;

        /// <summary>
        /// Enthält die umliegenden Module als String. Nach Verlinkung nicht mehr aktuell.
        /// </summary>
        public List<string> VerbindungenStr;
        /// <summary>
        /// Enthält Pointer zu den umliegenden Modulen.
        /// </summary>
        public List<streckenModul> Verbindungen;

        /// <summary>
        /// Wahr, wenn Modul weniger als 2 existierende Verbindungen im Datenbestand hat.
        /// </summary>
        public bool NetzGrenze;
        public bool wichtig;
        /// <summary>
        /// Wahr, wenn Modul für Fahrplan ausgewählt wurde.
        /// </summary>
        public bool selected;

        public streckenModul(string modulePath)
        {
            modPath = modulePath.Replace('/', '\\');
            modPath = modPath.Substring(modPath.IndexOf("Routes"));
            modName = modContainer.speicherortZuName(modPath, '\\');
            VerbindungenStr = new List<string>();
            NetzGrenze = false;
            wichtig = false;
            selected = false;
        }
    }
}
