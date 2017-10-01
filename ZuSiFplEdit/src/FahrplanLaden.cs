using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ZuSiFplEdit
{
    class FahrplanLaden
    {
        public static void fahrplanÖffnen(Fahrplan fahrplan, Datensatz datensatz)
        {
            OpenFileDialog fpnDateiDialog = new OpenFileDialog();

            fpnDateiDialog.InitialDirectory = datensatz.datenVerzeichnis + "Timetables\\";
            fpnDateiDialog.Filter = "Fahrplandateien (*.fpn;*.trn)|*.fpn;*.trn|Alle Dateiformate (*.*)|*.*";
            fpnDateiDialog.FilterIndex = 1;
            fpnDateiDialog.RestoreDirectory = true;

            if (fpnDateiDialog.ShowDialog() == DialogResult.OK)
            {
                string speicherort = fpnDateiDialog.FileName;
                string endung = speicherort.Substring(speicherort.Length - 3, 3);

                if (endung == "fpn")
                    fpnLaden(speicherort, fahrplan, datensatz);
                if (endung == "trn")
                    trnLaden(speicherort, fahrplan, datensatz);

                //TODO: Falsch deklarierte Dateien erkennen
            }
        }

        public static void fpnLaden(string speicherort, Fahrplan fahrplan, Datensatz datensatz)
        {
            XmlReader xmlReader = XmlReader.Create(speicherort);

            while (xmlReader.Read())
            {
                //Grundinformationen einlesen
                if (xmlReader.Name == "Fahrplan" && xmlReader.IsStartElement())
                {
                    var anfagszeit_str = xmlReader.GetAttribute("AnfangsZeit");
                    fahrplan.anfangszeit = Convert.ToDateTime(anfagszeit_str);
                }

                if (xmlReader.Name == "BefehlsKonfiguration")
                    fahrplan.befehlsKonfig = xmlReader.GetAttribute("Dateiname");

                if (xmlReader.Name == "Zug" && xmlReader.IsStartElement())
                {
                    xmlReader.Read();
                    while (xmlReader.Read())
                    {
                        if (xmlReader.Name == "Datei")
                        {
                            trnLaden(datensatz.datenVerzeichnis + xmlReader.GetAttribute("Dateiname"), fahrplan, datensatz); 
                        }
                        if (xmlReader.Name == "StrModul")
                        {
                            xmlReader.Read();
                            break;
                        }
                    }
                }

                if (xmlReader.Name == "StrModul" && xmlReader.IsStartElement())
                {
                    xmlReader.Read();
                    while (xmlReader.Read())
                    {
                        if (xmlReader.Name == "Datei")
                        {
                            string modul_str = xmlReader.GetAttribute("Dateiname");
                            modul_str = DataConstructor.speicherortZuName(modul_str, '\\');
                            var modul = datensatz.sucheModul(modul_str);

                            if (modul != null)
                                fahrplan.modulHinzufügen(modul);
                        }

                        if (xmlReader.Name == "StrModul")
                        {
                            xmlReader.Read();
                            break;
                        }
                    }
                }
            }
        }

        public static void trnLaden(string speicherort, Fahrplan fahrplan, Datensatz datensatz)
        {
            XmlReader xmlReader = XmlReader.Create(speicherort);

            var zug = new ZugFahrt(datensatz);

            while (xmlReader.Read())
            {
                //Grundinfos einlesen
                if (xmlReader.Name == "Zug" && xmlReader.IsStartElement())
                {
                    zug.gattung = xmlReader.GetAttribute("Gattung");
                    var zugnummer_str = xmlReader.GetAttribute("Nummer");
                    for (int i = 0; i < zugnummer_str.Length; i++)
                    {
                        if (!char.IsDigit(zugnummer_str[i]))
                        {
                            zugnummer_str = zugnummer_str.Substring(0, i);
                            break;
                        }
                    }
                    if (zugnummer_str == "")
                        return;
                    zug.zugnummer = Convert.ToInt64(zugnummer_str);
                    while (fahrplan.zugnummerVergeben(zug.zugnummer))
                        zug.zugnummer += 1000000;
                    zug.prio = Convert.ToInt32(xmlReader.GetAttribute("Prio"));

                    var startFahrstraße_str = xmlReader.GetAttribute("FahrstrName");
                    var startFahrstraße = datensatz.sucheFahrstraße(startFahrstraße_str);
                    if(startFahrstraße == null)
                    {
                        Console.WriteLine("Fahrstraße nicht gefunden: " + zug.gattung + zug.zugnummer);
                        return;
                    }

                    var wegPunkt = new ZugFahrt.WegPunkt(startFahrstraße.startSignal, zug);
                    zug.wegPunkte.Add(wegPunkt);
                }

                //Fahrstraßen
                if (xmlReader.Name == "FahrplanEintrag" && xmlReader.IsStartElement())
                {
                    string ankunft_str = xmlReader.GetAttribute("Ank");
                    string abfahrt_str = xmlReader.GetAttribute("Abf");
                    string betriebststelle = xmlReader.GetAttribute("Betrst");
                    while(xmlReader.Read() && xmlReader.Name != "FahrplanSignalEintrag") { }
                    string name = xmlReader.GetAttribute("FahrplanSignal");

                    if (betriebststelle == "" || name == "")
                        return;

                    var signal = datensatz.sucheSignal(betriebststelle, name);
                    if (signal == null)
                        return;

                    var wegPunkt = new ZugFahrt.WegPunkt(signal, zug);

                    DateTime letzteZeit = zug.wegPunkte.Last().ankunft;
                    if (zug.wegPunkte.Last().abfahrt_gesetzt)
                        letzteZeit = zug.wegPunkte.Last().abfahrt;

                    if (ankunft_str != null && ankunft_str != "")
                    {
                        wegPunkt.ankunft = Convert.ToDateTime(ankunft_str);
                        if (wegPunkt.ankunft >= letzteZeit)
                            wegPunkt.ankunft_gesetzt = true;
                        else
                        {
                        }
                    }
                    if (abfahrt_str != null && abfahrt_str != "")
                    {
                        wegPunkt.abfahrt = Convert.ToDateTime(abfahrt_str);
                        if (wegPunkt.ankunft_gesetzt)
                            letzteZeit = wegPunkt.ankunft;
                        if (wegPunkt.abfahrt >= letzteZeit)
                            wegPunkt.abfahrt_gesetzt = true;
                        else
                        {
                        }
                    }

                    zug.wegPunkte.Add(wegPunkt);
                }

                    //Zugverband
                    if (xmlReader.Name == "FahrzeugVarianten" && xmlReader.IsStartElement())
                {
                    zug.zugVerbandXML = xmlReader.ReadOuterXml();

                    zug.zugVerbandName = "fpn-definiert";
                }
            }

            zug.routeBerechnen();
            fahrplan.zugFahrten.Add(zug);
            Console.WriteLine("Zug geladen: " + zug.gattung + zug.zugnummer);
        }
    }
}
