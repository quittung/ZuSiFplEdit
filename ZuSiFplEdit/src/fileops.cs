﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    class fileops
    {
        Fahrplan fahrplan;
        List<streckenModul> mSammlung;
        List<ZugFahrt> Zugfahrten;
        string basePath;
        string fpnFullPath;
        string fpnRelPath;
        string fpnSubDir;
        string fpnRelSubDir;

        public fileops(Fahrplan fahrplan, string path, string basePath)
        {
            this.fahrplan = fahrplan;
            mSammlung = fahrplan.module;
            Zugfahrten = fahrplan.zugFahrten;

            
            if (basePath[basePath.Length - 1] != '\\')
            {
                basePath += "\\";
            }
            Uri basePath_uri = new Uri(basePath);

            Uri fpnFullPath_uri = new Uri(path);
            if (!basePath_uri.IsBaseOf(fpnFullPath_uri))
            {
                MessageBox.Show("Fahrplan ist nicht im Zusi-Datenverzeichnis: \n" + basePath_uri + "\n" + fpnFullPath_uri);
                return;
            }
            Uri fpnRelPath_uri = basePath_uri.MakeRelativeUri(fpnFullPath_uri);

            Uri fpnSubDir_uri = new Uri(Path.Combine(Path.GetDirectoryName(fpnFullPath_uri.LocalPath), Path.GetFileNameWithoutExtension(fpnFullPath_uri.LocalPath) + "\\"));
            Uri fpnRelSubDir_uri = basePath_uri.MakeRelativeUri(fpnSubDir_uri);

            this.basePath = basePath_uri.LocalPath;
            this.fpnFullPath = fpnFullPath_uri.LocalPath;
            this.fpnRelPath = fpnRelPath_uri.ToString().Replace('/', '\\');
            this.fpnSubDir = fpnSubDir_uri.LocalPath;
            this.fpnRelSubDir = fpnRelSubDir_uri.ToString().Replace('/', '\\');




            if (!Directory.Exists(fpnSubDir))
            {
                Directory.CreateDirectory(fpnSubDir);
            }

            writeFPN();
            foreach (var zug in Zugfahrten)
            {
                if (zug.route != null && zug.route.Count > 1)
                    writeTRN(zug);
                else
                {
                    MessageBox.Show("Zug " + zug.gattung + zug.zugnummer + " hat keine Route.");
                    break;
                }
            }
            
        }

        /// <summary>
        /// Erzeugt .fpn-Fragment aus aktuell ausgewählten Modulen.
        /// </summary>
        void writeFPN()
        {
            int mod_Count = 0;
            int UTM_NS_avg = 0;
            int UTM_WE_avg = 0;

            int UTM_Z1 = 0;
            char UTM_Z2 = ' ';

            if (fpnFullPath == "") fpnFullPath = "Rohling.fpn";
            var fpn_file = new StreamWriter(fpnFullPath, false);

            fpn_file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            fpn_file.WriteLine("<Zusi>");
            fpn_file.WriteLine("<Info DateiTyp=\"Fahrplan\" Version=\"A.1\" MinVersion=\"A.1\">");
            fpn_file.WriteLine("<AutorEintrag/>");
            fpn_file.WriteLine("</Info>");
            fpn_file.WriteLine("<Fahrplan  AnfangsZeit=\"" + fahrplan.anfangszeit.ToString("yy-MM-dd HH:mm:ss") + "\">");
            fpn_file.WriteLine("<BefehlsKonfiguration Dateiname=\"" + fahrplan.befehlsKonfig + "\"/>");
            fpn_file.WriteLine("<Begruessungsdatei/>");

            foreach (var zug in Zugfahrten)
            {
                fpn_file.WriteLine("<Zug>");
                fpn_file.WriteLine("<Datei Dateiname=\"" + Path.Combine(fpnRelSubDir, zug.gattung + zug.zugnummer + ".trn") + "\"/>");
                fpn_file.WriteLine("</Zug>");
            }

            foreach (var mod in mSammlung)
            {
                //Vorbereitung zur Berechnung vom Referenzpunkt
                mod_Count++;
                UTM_NS_avg += (int)mod.ursprung.NS;
                UTM_WE_avg += (int)mod.ursprung.WE;

                UTM_Z1 = mod.ursprung.Z1;
                UTM_Z2 = mod.ursprung.Z2;

                //Schreibe XML für aktuelles Modul
                fpn_file.WriteLine("<StrModul>");
                fpn_file.WriteLine("<Datei Dateiname=\"" + mod.pfad + "\"/>");
                fpn_file.WriteLine("<p/>");
                fpn_file.WriteLine("<phi/>");
                fpn_file.WriteLine("</StrModul>");
            }

            //Abbrechen, wenn nichts ausgewählt.
            if (mod_Count == 0)
            {
                fpn_file.WriteLine("<UTM/>");
                fpn_file.WriteLine("</Fahrplan>");
                fpn_file.WriteLine("</Zusi>");

                fpn_file.Close();

                return;

            }

            //Berechne UTM-Referenzpunkt
            UTM_NS_avg = UTM_NS_avg / mod_Count;
            UTM_WE_avg = UTM_WE_avg / mod_Count;

            //Schreibe UTM-Referenzpunkt
            fpn_file.WriteLine("<UTM UTM_WE=\"" + UTM_WE_avg + "\" UTM_NS=\"" + UTM_NS_avg + "\" UTM_Zone=\"" + UTM_Z1 + "\" UTM_Zone2=\"" + UTM_Z2 + "\"/>\"");
            fpn_file.WriteLine("</Fahrplan>");
            fpn_file.WriteLine("</Zusi>");

            fpn_file.Close();
        }

        void writeTRN(ZugFahrt zug)
        {
            string path = Path.Combine(fpnSubDir, zug.gattung + zug.zugnummer + ".trn");
            var trn_file = new StreamWriter(path, false);

            trn_file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            trn_file.WriteLine("<Zusi>");
            trn_file.WriteLine("<Info DateiTyp=\"Zug\" Version=\"A.1\" MinVersion=\"A.1\">");
            trn_file.WriteLine("<AutorEintrag/>");
            trn_file.WriteLine("</Info>");
            trn_file.WriteLine("<Zug Gattung=\"" + zug.gattung + "\" Nummer=\"" + zug.zugnummer + "\" Prio=\"5000\" Bremsstellung=\"4\" Rekursionstiefe=\"5\" FahrstrName=\"" + zug.route[0].fahrstraße.name.Replace(">", "&gt;") + "\" Zugtyp=\"1\" Buchfahrplandll=\"_InstSetup\\lib\\timetable\\Buchfahrplan_DB_2006.dll\">");
            trn_file.WriteLine("<Datei Dateiname=\"" + fpnRelPath + "\" NurInfo=\"1\"/>");

            for (int i = 0; i < zug.route.Count; i++)
            {
                if (zug.route[i].relevant)
                {
                    trn_file.Write("<FahrplanEintrag");
                    if ((zug.route[i].ankunft != null) && (zug.route[i].ankunft != new DateTime()))
                        trn_file.Write(" Ank=\"" + zug.route[i].ankunft.ToString("yy-MM-dd HH:mm:ss") + "\"");
                    if ((zug.route[i].abfahrt != null) && (zug.route[i].abfahrt != new DateTime()))
                        trn_file.Write(" Abf=\"" + zug.route[i].abfahrt.ToString("yy-MM-dd HH:mm:ss") + "\"");
                    trn_file.Write(" Betrst=\"" + zug.route[i].signal.betriebsstelle + "\"");
                    if ((i < (zug.route.Count - 1)) && (zug.route[i + 1].wende))
                        trn_file.Write(" FzgVerbandAktion=\"2\" FzgVerbandWendeSignalabstand=\"400\"");
                    trn_file.WriteLine(">");

                    trn_file.WriteLine("<FahrplanSignalEintrag FahrplanSignal=\"" + zug.route[i].signal.name + "\"/>");
                    trn_file.WriteLine("</FahrplanEintrag>"); 
                }
            }

            if(zug.zugVerbandXML == null)
            {
                trn_file.WriteLine("<FahrzeugVarianten Bezeichnung=\"default\" ZufallsWert=\"1\">");
                trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\" DotraModus=\"1\">");
                trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Dieseltriebwagen\\LINT\\LINT41_A-Wagen.rv.fzg\"/>");
                trn_file.WriteLine("</FahrzeugInfo>");
                trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\" DotraModus=\"1\">");
                trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Dieseltriebwagen\\LINT\\LINT41_B-Wagen.rv.fzg\"/>");
                trn_file.WriteLine("</FahrzeugInfo>");
                trn_file.WriteLine("</FahrzeugVarianten>");
            }
            else
            {
                trn_file.WriteLine(zug.zugVerbandXML);
            }
            
            trn_file.WriteLine("</Zug>");
            trn_file.WriteLine("</Zusi>");

            trn_file.Close();
        }
    }
}
