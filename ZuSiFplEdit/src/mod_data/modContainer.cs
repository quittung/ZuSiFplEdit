using System;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Sammelt, vernetzt und verwaltet Streckenmodule
    /// </summary>
    class modContainer
    {
        

        public List<streckenModul> mSammlung = new List<streckenModul>();
        public string DirBase = "";
        public string DirRoute = "";

        public long loadTime;


        public modContainer()
        {
            var timeGesamt = new System.Diagnostics.Stopwatch();
            timeGesamt.Start();

            var ladeAnzeige = new form_lade();
            ladeAnzeige.Show();
            ladeAnzeige.Beschreibung.Text = "Suche Datenverzeichnis...";
            ladeAnzeige.Update();

            FindeDatenVerzeichnis();



            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 1, "Suche Module...");

            List<string> modulPaths = erzeugeST3Liste();

            ladeAnzeige.progressBar2.Maximum = modulPaths.Count - 1;



            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 2, "Lese Module...");

            List<string> st3Fehler = new List<string>();
            for (int i = 0; i < modulPaths.Count; i++)
            {
                ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, i, "Lese Module [" + (i + 1) + "/" + modulPaths.Count + "] - " + modulPaths[i].Split('\\').Last());
                ModulEinlesen(modulPaths[i], st3Fehler);
            }



            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 3, "Verlinke Module...");

            moduleVerlinken(ladeAnzeige);



            ladeAnzeige.instantProgress(ladeAnzeige.progressBar1, 4, "Finalisiere...");

            timeGesamt.Stop();
            loadTime = timeGesamt.ElapsedMilliseconds;
            //MessageBox.Show("Einlesen hat " + timeGesamt.ElapsedMilliseconds + " ms gedauert.", "Gesamtdauer des Einlesens", MessageBoxButtons.OK);


            //Einlesefehler an Nutzer melden
            if (st3Fehler.Count > 0)
            {
                string errMsg = "Fehler beim Laden der folgenden Module:";
                foreach (var st3 in st3Fehler)
                {
                    errMsg += "\n - " + st3;
                }
                errMsg += "\n\nDie betroffenen Module wurden ignoriert.";
                MessageBox.Show(errMsg, "Fehler in .st3-Dateien", MessageBoxButtons.OK);
            }

            ladeAnzeige.Dispose();
        }

        private List<string> erzeugeST3Liste()
        {
            List<string> modulPaths = new List<string>();
            foreach (string grid in Directory.GetDirectories(DirRoute))
            {
                foreach (string mod in Directory.GetDirectories(grid))
                {
                    foreach (string st3 in Directory.GetFiles(mod, "*.st3"))
                    {
                        modulPaths.Add(st3);
                    }
                }
            }

            return modulPaths;
        }


        /// <summary>
        /// Liest ein Modul ein und hängt es der passenden Liste an
        /// </summary>
        private void ModulEinlesen(string Speicherort, List<string> st3Fehler)
        {
            streckenModul aktMod = new streckenModul(Speicherort);

            if (aktMod.isSane == true)
            {
                mSammlung.Add(aktMod);
            }
            else
            {
                st3Fehler.Add(Speicherort);
            }
        }


        /// <summary>
        /// Liest das Datenverzeichnis aus der Registry ein
        /// </summary>
        private void FindeDatenVerzeichnis()
        {
            try
            {
                //Registry-Key auf win64
                DirBase = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Zusi3", "DatenVerzeichnis", "").ToString();
            }
            catch (Exception)
            {
                try
                {
                    //Registry-Key auf win32
                    DirBase = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3", "DatenVerzeichnis", "").ToString();
                }
                catch (Exception)
                {
                    MessageBox.Show("Datenverzeichnis konnte nicht gefunden werden.", "Fataler Fehler", MessageBoxButtons.OK);
                    Environment.Exit(1);
                }
            }

            if (DirBase[DirBase.Length - 1] != '\\') DirBase += '\\';
            DirRoute = DirBase + "Routes\\Deutschland\\";
        }


        //Wandelt die als String gespeicherten Verbindungen in Pointer um.
        void moduleVerlinken(form_lade ladeAnzeige)
        {
            ladeAnzeige.progressBar2.Maximum = 4;
            ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 0, "Verlinke Module...");

            //MessageBox.Show("Module werden jetzt verlinkt.", "Debugnachricht", MessageBoxButtons.OK);
            foreach (streckenModul aktModul in mSammlung)
            {
                aktModul.Verbindungen = new List<streckenModul>();
                foreach (string connectionString in aktModul.VerbindungenStr)
                {
                    streckenModul connectionObj = sucheMod(connectionString);
                    if (!(connectionObj == null))
                    {
                        aktModul.Verbindungen.Add(connectionObj);
                    } else
                    {
                        aktModul.NetzGrenze = true;
                        //aktModul.VerbindungenStr.Remove(connectionString);
                    }
                }
            }

            //Delete one-sided connections.
            foreach (streckenModul mod in mSammlung)
            {
                if (mod.Verbindungen.Count < 2)
                {
                    mod.NetzGrenze = true;
                }
                if ((mod.Verbindungen.Count > 2) || (mod.NetzGrenze)) mod.wichtig = true;
                //DrawDist eintragen.
                double dist = 0;
                foreach (var con in mod.Verbindungen)
                {
                    double dx = mod.UTM_WE - con.UTM_WE;
                    double dy = mod.UTM_NS - con.UTM_NS;

                    double modDist = Math.Sqrt(dx * dx + dy * dy);

                    if (modDist > dist) dist = modDist;
                }
                mod.drawDist = dist;
            }


            //ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 0, "Verlinke Streckenelemente...");
            ////Verlinke Streckenelemente
            //foreach (var mod in mSammlung)
            //{
            //    foreach (var strE in mod.StreckenElemente)
            //    {
            //        strE.AnschlussNorm = new List<streckenModul.streckenElement>();
            //        if (strE.AnschlussNormInt.Count > 0)
            //        {
            //            foreach (var normElement in strE.AnschlussNormInt)
            //            {
            //                strE.AnschlussNorm.Add(mod.sucheStrElement(normElement));
            //            }
            //        }

            //        strE.AnschlussGegen = new List<streckenModul.streckenElement>();
            //        if (strE.AnschlussGegenInt.Count > 0)
            //        {
            //            foreach (var gegenElement in strE.AnschlussGegenInt)
            //            {
            //                strE.AnschlussGegen.Add(mod.sucheStrElement(gegenElement));
            //            }
            //        }
            //    }
            //}


            ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 1, "Verlinke Fahrstraßen mit Signalen...");
            //string problemstellen = "";
            //verlinke fahrstraßen mit referenzen.
            var unvollständigeFahrstraßen = new List<streckenModul.fahrStr>();
            foreach (var mod in mSammlung)
            {
                foreach (var fstr in mod.FahrStr)
                {
                    fstr.StartMod = mod;
                    fstr.Start = mod.sucheReferenz(fstr.StartRef);
                    //if (fstr.Start == null || fstr.Start.Signal == null)
                    if (fstr.Start == null)
                    {
                        unvollständigeFahrstraßen.Add(fstr);
                        continue;
                    }
                    else
                    {
                        fstr.Start.abgehendeFahrstraßen.Add(fstr);
                    }
                    fstr.ZielMod = sucheMod(fstr.ZielMod_Str);

                    foreach (var wegpunkt in fstr.wegpunkte)
                    {
                        var refMod = sucheMod(wegpunkt.RefModString);
                        if (refMod != null)
                            wegpunkt.complete(refMod.sucheReferenz(wegpunkt.RefInt));
                    }

                    if (fstr.ZielMod == null)
                    {
                        unvollständigeFahrstraßen.Add(fstr);
                        continue;
                    }
                    else
                    {
                        fstr.Ziel = fstr.ZielMod.sucheReferenz(fstr.ZielRef);
                        //if (fstr.Ziel == null ||fstr.Ziel.Signal == null)
                        if (fstr.Ziel == null)
                        {
                            unvollständigeFahrstraßen.Add(fstr);
                            continue;
                        }
                    }
                }
                foreach (var ufstr in unvollständigeFahrstraßen) //Entfernen von unvollständigen Fahrstraßen.
                {
                    mod.FahrStr.Remove(ufstr);
                }
            }




            ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 2, "Verlinke Fahrstraßen mit Folgestraßen...");
            //Folgestraßen eintragen
            foreach (var mod in mSammlung)
            {
                foreach (var fstr in mod.FahrStr)
                {
                    fstr.folgestraßen = new List<streckenModul.fahrStr>();

                    if (!(fstr.Ziel == null))
                    {
                        foreach (var fort in fstr.ZielMod.FahrStr)
                        {
                            if (fstr.Ziel == fort.Start)
                            {
                                fstr.folgestraßen.Add(fort);
                            }
                        }
                    }
                }
            }


            ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 3, "Sammle Start- und Zielsignale...");
            //Sammle abgehende Fahrstraßen zu Signalen in Modul.
            foreach (var mod in mSammlung)
            {
                foreach (var fstr in mod.FahrStr) //Start- und Zielsignale in den entsprechenden Modulen ein.
                {
                    fstr.Start.istStart = true;
                    if (!(mod.StartSignale.Contains(fstr.Start)))
                        mod.StartSignale.Add(fstr.Start);
                    if (!(mod.StartUndZielSignale.Contains(fstr.Start)))
                        mod.StartUndZielSignale.Add(fstr.Start);

                    fstr.Ziel.istZiel = true;
                    if (!(fstr.ZielMod.ZielSignale.Contains(fstr.Ziel)))
                        fstr.ZielMod.ZielSignale.Add(fstr.Ziel);
                    if (!(fstr.ZielMod.StartUndZielSignale.Contains(fstr.Ziel)))
                        fstr.ZielMod.StartUndZielSignale.Add(fstr.Ziel);
                }
            }

            ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 4, "Finde Wendeziele...");

            foreach (var mod in mSammlung)
            {
                List<streckenModul.fahrStr> neueFahrstraßen = new List<streckenModul.fahrStr>();
                foreach (var fstr in mod.FahrStr)
                {

                    ladeAnzeige.instantProgress(ladeAnzeige.progressBar2, 4, "Finde Wendeziele (" + mod.modName + " - " + fstr.Ziel.Info + ")...");
                    fstr.wendesignale = findeWendeziele(fstr.Ziel);
                    fstr.Ziel.wendeSignale = fstr.wendesignale; //HACK: Manche Signale erhalten keine Wendesignale.
                    
                    //Füge WendeHilfsFahrStraßen hinzu
                    foreach (var wendeSignal in fstr.wendesignale)
                    {
                        var wendeHilfsStraße = erstelleWendehilfsfahrstraße(fstr.Ziel, wendeSignal);

                        fstr.folgestraßen.Add(wendeHilfsStraße);
                        fstr.Ziel.abgehendeFahrstraßen.Add(wendeHilfsStraße);
                        neueFahrstraßen.Add(wendeHilfsStraße);
                    }
                }
                mod.FahrStr.AddRange(neueFahrstraßen);
            }
        }

        private static streckenModul.fahrStr erstelleWendehilfsfahrstraße(streckenModul.referenzElement StartSignal, streckenModul.referenzElement ZielSignal)
        {
            var distanz = (float)StartSignal.SignalCoord.distanceTo(ZielSignal.SignalCoord) * 1000f;
            var hilfsfstr = new streckenModul.fahrStr("Wendehilfsfahrstraße", "0", 4, "TypWende", distanz, StartSignal.ReferenzNr, StartSignal.Modul.modName, ZielSignal.ReferenzNr, ZielSignal.Modul.modName); //TODO: RglGgl
            hilfsfstr.StartMod = StartSignal.Modul;
            hilfsfstr.ZielMod = ZielSignal.Modul;
            hilfsfstr.Start = StartSignal;
            hilfsfstr.Ziel = ZielSignal;
            hilfsfstr.folgestraßen = ZielSignal.abgehendeFahrstraßen;
            return hilfsfstr;
        }

        /// <summary>
        /// Gibt Modul-Objekt für einen Modulnamen zurück
        /// </summary>
        /// <param name="modName">Name des Moduls</param>
        public streckenModul sucheMod(string modName) 
        {
            modName = modName.Substring(0, modName.Length - 5); //HACK: Jahreszahl wird ignoriert
            foreach (var mod in mSammlung)
            {
                if (mod.modName.Contains(modName)) return mod;
            }
            return null;
        }


        /// <summary>
        /// Extrahiert den Namen eines Moduls aus seinem Pfad
        /// </summary>
        /// <param name="Speicherort">Pfad zum Modul</param>
        /// /// <param name="DirSeparator">Verzeichnisseparator</param>
        public static string speicherortZuName(string Speicherort, char DirSeparator)
        {
            string[] modNameAr = Speicherort.Split(DirSeparator);
            string modName = modNameAr[modNameAr.Length - 1];
            modName = modName.Substring(0, modName.Length - 4);
            return (modName);
        }


        List<streckenModul.referenzElement> findeWendeziele(streckenModul.referenzElement wendeStart)
        {
            verbindeStreckenelement(wendeStart.StrElement);

            //Wähle richtige Richtung zum Suchen
            List<streckenModul.streckenElement> nächsteElemente;
            if (wendeStart.StrNorm)
            {
                nächsteElemente = wendeStart.StrElement.AnschlussGegen;
            }
            else
            {
                nächsteElemente = wendeStart.StrElement.AnschlussNorm;
            }

            //Ziele suchen
            List<streckenModul.referenzElement> Zielsignale = new List<streckenModul.referenzElement>();

            if (nächsteElemente.Count > 0)
            {
                foreach (var element in nächsteElemente)
                {
                    var ZielsignaleTeil = WendesignalTraverse(element, wendeStart.StrElement);
                    if (ZielsignaleTeil != null)
                    {
                        Zielsignale.AddRange(ZielsignaleTeil);
                    }
                }
            }

            //if (Zielsignale.Count == 0)
            //{
            //    string report = "Habe " + Zielsignale.Count + " Wendeziele für " + wendeStart.Info + " gefunden: \n";
            //    foreach (var Zielsignal in Zielsignale)
            //    {
            //        report += Zielsignal.ToString() + "\n";
            //    }
            //    MessageBox.Show(report); 
            //}

            return Zielsignale;
        }

        List<streckenModul.referenzElement> WendesignalTraverse(streckenModul.streckenElement aktuellesStreckenElement, streckenModul.streckenElement letztesStreckenelement)
        {
            verbindeStreckenelement(aktuellesStreckenElement);

            bool bewegungsrichtungIstNorm = true;

            var nächsteElemente = findeNächsteStreckenelemente(aktuellesStreckenElement, letztesStreckenelement, out bewegungsrichtungIstNorm);


            var Zielsignale = new List<streckenModul.referenzElement>();

            //Ist Zielsignal in diesem Element vorhanden?
            if (aktuellesStreckenElement.signalReferenz != null)
            {
                if (aktuellesStreckenElement.signalReferenz.istStart && (aktuellesStreckenElement.signalReferenz.StrNorm == bewegungsrichtungIstNorm))
                {
                    Zielsignale.Add(aktuellesStreckenElement.signalReferenz);
                    return Zielsignale;
                }
            }
            
            //Zum nächsten Element weiterziehen.
            if (nächsteElemente.Count > 0)
            {
                foreach (var element in nächsteElemente)
                {
                    Zielsignale.AddRange(WendesignalTraverse(element, aktuellesStreckenElement));
                }
            }

            return Zielsignale;
        }

        void verbindeStreckenelement(streckenModul.streckenElement verbindungsElement)
        {
            verbindeStreckenelement(verbindungsElement, null);
        }

        void verbindeStreckenelement(streckenModul.streckenElement verbindungsElement, streckenModul.streckenElement herkunftsElement)
        {
            //TODO: Herkunftselement einbeziehen
            //TODO: Neue Verbindungen zurückverbinden
            //TODO: Verbindungen zu anderen Modulen ermöglichen
            //TODO: Codverdoppelung zurückbauen
            if (!verbindungsElement.verlinkt)
            {
                verbindungsElement.AnschlussNorm = new List<streckenModul.streckenElement>();
                if (verbindungsElement.AnschlussNormInt.Count > 0)
                {
                    foreach (var normElement in verbindungsElement.AnschlussNormInt)
                    {
                        verbindungsElement.AnschlussNorm.Add(verbindungsElement.Modul.sucheStrElement(normElement));
                    }
                }

                verbindungsElement.AnschlussGegen = new List<streckenModul.streckenElement>();
                if (verbindungsElement.AnschlussGegenInt.Count > 0)
                {
                    foreach (var gegenElement in verbindungsElement.AnschlussGegenInt)
                    {
                        verbindungsElement.AnschlussGegen.Add(verbindungsElement.Modul.sucheStrElement(gegenElement));
                    }
                }

                verbindungsElement.verlinkt = true; 
            }
        }

        List<streckenModul.streckenElement> findeNächsteStreckenelemente(streckenModul.streckenElement aktuellesStreckenElement, streckenModul.streckenElement letztesStreckenelement, out bool bewegungsrichtungIstNorm)
        {
            verbindeStreckenelement(aktuellesStreckenElement);

            //Finde das richte Ende
            if (aktuellesStreckenElement.AnschlussNorm.Count > 0)
            {
                if (aktuellesStreckenElement.AnschlussNorm.Contains(letztesStreckenelement))
                {
                    bewegungsrichtungIstNorm = false;
                    return aktuellesStreckenElement.AnschlussGegen;
                }
            }

            if (aktuellesStreckenElement.AnschlussGegen.Count > 0)
            {
                if (aktuellesStreckenElement.AnschlussGegen.Contains(letztesStreckenelement))
                {
                    bewegungsrichtungIstNorm = true;
                    return aktuellesStreckenElement.AnschlussNorm;
                }
            }

            bewegungsrichtungIstNorm = false;
            return new List<streckenModul.streckenElement>(); ;
        }
    }
}
