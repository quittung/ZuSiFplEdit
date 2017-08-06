using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    class fileops
    {
        List<streckenModul> mSammlung;
        List<ZugFahrt> Zugfahrten;
        string basePath;
        string fpnFullPath;
        string fpnRelPath;
        string fpnSubDir;
        string fpnRelSubDir;

        public fileops(List<streckenModul> mSammlung, List<ZugFahrt> Zugfahrten, string path, string basePath)
        {
            this.mSammlung = mSammlung;
            this.Zugfahrten = Zugfahrten;

            
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
                if (zug.route != null)
                    writeTRN(zug);
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
            fpn_file.WriteLine("<Fahrplan  AnfangsZeit=\"2017-02-27 12:00:00\">");
            fpn_file.WriteLine("<BefehlsKonfiguration/>");
            fpn_file.WriteLine("<Begruessungsdatei/>");

            foreach (var zug in Zugfahrten)
            {
                fpn_file.WriteLine("<Zug>");
                fpn_file.WriteLine("<Datei Dateiname=\"" + Path.Combine(fpnRelSubDir, zug.Gattung + zug.Zugnummer + ".trn") + "\"/>");
                fpn_file.WriteLine("</Zug>");
            }

            foreach (var mod in mSammlung)
            {
                if (mod.selected)
                {
                    //Vorbereitung zur Berechnung vom Referenzpunkt
                    mod_Count++;
                    UTM_NS_avg += mod.UTM_NS;
                    UTM_WE_avg += mod.UTM_WE;

                    UTM_Z1 = mod.UTM_Z1;
                    UTM_Z2 = mod.UTM_Z2;

                    //Schreibe XML für aktuelles Modul
                    fpn_file.WriteLine("<StrModul>");
                    fpn_file.WriteLine("<Datei Dateiname=\"" + mod.modPath + "\"/>");
                    fpn_file.WriteLine("<p/>");
                    fpn_file.WriteLine("<phi/>");
                    fpn_file.WriteLine("</StrModul>");
                }
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
            string path = Path.Combine(fpnSubDir, zug.Gattung + zug.Zugnummer + ".trn");
            var trn_file = new StreamWriter(path, false);

            trn_file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            trn_file.WriteLine("<Zusi>");
            trn_file.WriteLine("<Info DateiTyp=\"Zug\" Version=\"A.1\" MinVersion=\"A.1\">");
            trn_file.WriteLine("<AutorEintrag/>");
            trn_file.WriteLine("</Info>");
            trn_file.WriteLine("<Zug Gattung=\"" + zug.Gattung + "\" Nummer=\"" + zug.Zugnummer + "\" Prio=\"5000\" Bremsstellung=\"4\" Rekursionstiefe=\"5\" FahrstrName=\"" + zug.route[0].FahrstrName.Replace(">", "&gt;") + "\" Zugtyp=\"1\" Buchfahrplandll=\"_InstSetup\\lib\\timetable\\Buchfahrplan_DB_2006.dll\">");
            trn_file.WriteLine("<Datei Dateiname=\"" + fpnRelPath + "\" NurInfo=\"1\"/>");

            for (int i = 0; i < zug.route.Count; i++)
            {
                //Richtiges Signal aussuchen
                streckenModul.Signal nextSignal = null;
                if (zug.route[i].Ziel.StrElement.SignalNorm != null)
                {
                    nextSignal = zug.route[i].Ziel.StrElement.SignalNorm;
                }
                else
                {
                    nextSignal = zug.route[i].Ziel.StrElement.SignalGegen;
                }


                trn_file.Write("<FahrplanEintrag");
                if ((zug.route_ankunft[i] != null) && (zug.route_ankunft[i] != new DateTime()))
                    trn_file.Write(" Ank=\"" + zug.route_ankunft[i].ToString("yy-MM-dd HH:mm:ss") + "\"");
                if ((zug.route_abfahrt[i] != null) && (zug.route_abfahrt[i] != new DateTime()))
                    trn_file.Write(" Abf=\"" + zug.route_abfahrt[i].ToString("yy-MM-dd HH:mm:ss") + "\"");
                trn_file.Write(" Betrst=\"" + nextSignal.Betriebstelle + "\"");
                if ((i < (zug.route.Count - 1)) && (zug.route[i + 1].FahrstrTyp == "TypWende")) //Wendeerkennung
                    trn_file.Write(" FzgVerbandAktion=\"2\" FzgVerbandWendeSignalabstand=\"250\"");
                trn_file.WriteLine(">");
                
                trn_file.WriteLine("<FahrplanSignalEintrag FahrplanSignal=\"" + nextSignal.Name + "\"/>");
                trn_file.WriteLine("</FahrplanEintrag>");
            }

            trn_file.WriteLine("<FahrzeugVarianten Bezeichnung=\"default\" ZufallsWert=\"1\">");
            //trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");

            //trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Dieseltriebwagen\\RegioShuttle\\RS1.rv.fzg\"/>");

            //trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektroloks\\TRAXX\\TRAXX_AC2.rv.fzg\"/>");
            
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"2\" IDNeben=\"1\" DotraModus=\"1\" SASchaltung=\"3\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\401.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\802_Bvmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\804_WSmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\803_BSmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\801_Avmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\801_Avmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\801_Avmz.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"2\" IDNeben=\"1\" DotraModus=\"1\" SASchaltung=\"3\" Gedreht=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Elektrotriebwagen\\ICE1_2\\401.rv.fzg\"/>");

            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("</FahrzeugVarianten>");
            trn_file.WriteLine("</Zug>");
            trn_file.WriteLine("</Zusi>");

            trn_file.Close();
        }
    }
}
