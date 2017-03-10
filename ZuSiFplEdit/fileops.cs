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
        List<modSelForm.ZugFahrt> Zugfahrten;
        string basePath;
        string fpnFullPath;
        string fpnRelPath;
        string fpnSubDir;
        string fpnRelSubDir;

        public fileops(List<streckenModul> mSammlung, List<modSelForm.ZugFahrt> Zugfahrten, string path, string basePath)
        {
            this.mSammlung = mSammlung;
            this.Zugfahrten = Zugfahrten;

            this.basePath = basePath;
            fpnFullPath = path;
            fpnRelPath = fpnFullPath.Replace(basePath, "");
            fpnSubDir = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            fpnRelSubDir = fpnFullPath.Replace(fpnSubDir, "");

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

        void writeTRN(modSelForm.ZugFahrt zug)
        {
            string path = Path.Combine(fpnSubDir, zug.Gattung + zug.Zugnummer + ".trn");
            var trn_file = new StreamWriter(path, false);

            trn_file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            trn_file.WriteLine("<Zusi>");
            trn_file.WriteLine("<Info DateiTyp=\"Zug\" Version=\"A.1\" MinVersion=\"A.1\">");
            trn_file.WriteLine("<AutorEintrag/>");
            trn_file.WriteLine("</Info>");
            trn_file.WriteLine("<Zug Gattung=\"" + zug.Gattung + "\" Nummer=\"" + zug.Zugnummer + "\" Prio=\"1500\" Bremsstellung=\"4\" Rekursionstiefe=\"5\" FahrstrName=\"" + zug.route[0].FahrstrName.Replace(">", "&gt;") + "\" Zugtyp=\"1\" Buchfahrplandll=\"_InstSetup\\lib\\timetable\\Buchfahrplan_DB_1979.dll\">");
            trn_file.WriteLine("<Datei Dateiname=\"" + fpnRelPath + "\" NurInfo=\"1\"/>");

            for (int i = 0; i < zug.route.Count; i++)
            {
                streckenModul.Signal nextSignal = null;
                if (zug.route[i].Ziel.StrElement.SignalNorm != null)
                {
                    nextSignal = zug.route[i].Ziel.StrElement.SignalNorm;
                }
                else
                {
                    nextSignal = zug.route[i].Ziel.StrElement.SignalGegen;
                }

                if (i == 0)
                {
                    trn_file.WriteLine("<FahrplanEintrag Ank=\"2017-02-27 12:00:00\" Abf=\"2017-02-27 12:01:00\" Betrst=\"" + nextSignal.Betriebstelle + "\">");
                }
                else
                {
                    trn_file.WriteLine("<FahrplanEintrag Betrst=\"" + nextSignal.Betriebstelle + "\">");
                }
                trn_file.WriteLine("<FahrplanSignalEintrag FahrplanSignal=\"" + nextSignal.Name + "\"/>");
                trn_file.WriteLine("</FahrplanEintrag>");
            }

            trn_file.WriteLine("<FahrzeugVarianten Bezeichnung=\"default\" ZufallsWert=\"1\">");
            trn_file.WriteLine("<FahrzeugInfo IDHaupt=\"1\" IDNeben=\"1\">");
            trn_file.WriteLine("<Datei Dateiname=\"RollingStock\\Deutschland\\Epoche5\\Dieseltriebwagen\\RegioShuttle\\RS1.rv.fzg\"/>");
            trn_file.WriteLine("</FahrzeugInfo>");
            trn_file.WriteLine("</FahrzeugVarianten>");
            trn_file.WriteLine("</Zug>");
            trn_file.WriteLine("</Zusi>");

            trn_file.Close();
        }
    }
}
