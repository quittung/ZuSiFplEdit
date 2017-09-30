using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Hauptdialog zum Bearbeiten einzelner Zugfahrten
    /// </summary>
    public partial class ZugForm : Form
    {
        ZugFahrt Zug;
        bool guiBereit = true;

        public static event SignalSelectEventHandler signalSelectionEvent;

        public ZugForm()
        {
            InitializeComponent();

            LB_waypoint.AllowDrop = true;
        }

        /// <summary>
        /// Lädt den aktuellen Zustand des Zuges in das Formular
        /// </summary>
        public void Laden()
        {
            tbZugGattung.Text = Zug.Gattung;
            tbZugNummer.Text = Zug.Zugnummer.ToString();
            tbVmax.Text = (Zug.vMax * 3.6f).ToString("f0");
            
            LB_waypoint.Items.Clear();
            LB_routenPunkte.Items.Clear();

            foreach (var WP in Zug.WayPoints)
            {
                LB_waypoint.Items.Add(WP);
            }
            foreach (var RP in Zug.route)
            {
                LB_routenPunkte.Items.Add(RP);
            }
        }

        /// <summary>
        /// Lädt einen neuen Zug in das Formular
        /// </summary>
        public void setZug(ZugFahrt neuerZug)
        {
            this.Zug = neuerZug;
            Laden();
        }
        
        /// <summary>
        /// Fügt einen neuen Wegpunkt ein? 
        /// </summary>
        public void setSignal(streckenModul.Signal signal)
        {
            Zug.WayPoints.Add(new ZugFahrt.WayPoint(signal, Zug));
            if (Zug.WayPoints.Count > 1)
            {
                Zug.routeBerechnen();
            }
            Laden();
        }
        
        /// <summary>
        /// Fügt einen neuen Wegpunkt ein
        /// </summary>
        private void punktEinfügen_Click(object sender, EventArgs e)
        {
            string platzHalter = "Signal auf der Karte wählen...";

            if (LB_waypoint.SelectedItem == null)
            {
                LB_waypoint.Items.Add(platzHalter);
            }
            else
            {
                LB_waypoint.Items.Insert(LB_waypoint.SelectedIndex + 1, platzHalter);
            }
            LB_waypoint.SelectedItem = platzHalter;

            if (LB_waypoint.SelectedIndex == 0)
            {
                signalSelectionEvent.Invoke("start");
            }
            else
            {
                signalSelectionEvent.Invoke("ziel");
            }
        }

        /// <summary>
        /// Verändert einen bestehenden Wegpunkt
        /// </summary>
        private void punktBearbeiten_Click(object sender, EventArgs e)
        {
            if (LB_waypoint.SelectedIndex == 0)
            {
                signalSelectionEvent.Invoke("start");
            }
            else
            {
                signalSelectionEvent.Invoke("ziel");
            }
        }

        /// <summary>
        /// Löscht einen Wegpunkt
        /// </summary>
        private void punktLöschen_Click(object sender, EventArgs e)
        {
            if (LB_waypoint.SelectedItem == null)
                return;
            Zug.WayPoints.RemoveAt(LB_waypoint.SelectedIndex);
            if (Zug.WayPoints.Count > 1)
            {
                Zug.routeBerechnen();
            }
            Laden();
        }

        /// <summary>
        /// Lässt ein Textfeld zur Bestätigung 0.5s grün aufleuchten
        /// </summary>
        public void tbColorAck(object textBox)
        {
            TextBox tb = (TextBox)textBox;
            setColorTS(tb, Color.PaleGreen);
            string tbText = tb.Text;

            Thread.Sleep(500);

            if (tb.Text == tbText)
                setColorTS(tb, Color.White);
        }

        /// <summary>
        /// Callback für setColorTS()
        /// </summary>
        public delegate void SetColorCallback(TextBox tb, Color clr);

        /// <summary>
        /// Verändert die Farbe eines Textfeldes im ursprünglichen Thread
        /// </summary>
        private void setColorTS(TextBox tb, Color clr)
        {
            try
            {
                Invoke(new SetColorCallback(setColor), new object[] { tb, clr });
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Verändert die Farbe eines Textfeldes
        /// </summary>
        private void setColor(TextBox tb, Color clr)
        {
            tb.BackColor = clr;
        }

        /// <summary>
        /// Liest eine veränderte Zuggattung ein
        /// </summary>
        private void tbZugGattung_TextChanged(object sender, EventArgs e)
        {
            Zug.Gattung = tbZugGattung.Text;
            ThreadPool.QueueUserWorkItem(tbColorAck, tbZugGattung);
        }

        /// <summary>
        /// Liest eine veränderte Zugnummer ein
        /// </summary>
        private void tbZugNummer_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(tbZugNummer.Text, out Zug.Zugnummer))
            {
                ThreadPool.QueueUserWorkItem(tbColorAck, tbZugNummer);
            }
            else
            {
                tbZugNummer.BackColor = Color.LightPink;
            }
        }

        /// <summary>
        /// Liest eine veränderte Höchstgeschwindigkeit ein
        /// </summary>
        private void tbVmax_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(tbVmax.Text, out Zug.vMax))
            {
                Zug.vMax /= 3.6f;
                ThreadPool.QueueUserWorkItem(tbColorAck, tbVmax);
            }
            else
            {
                tbVmax.BackColor = Color.LightPink;
            }
        }

        private void LB_waypoint_MouseDown(object sender, MouseEventArgs e)
        {
            if (LB_waypoint.SelectedItem == null) return;
            LB_waypoint.DoDragDrop(LB_waypoint.SelectedItem, DragDropEffects.Move);
        }

        private void LB_waypoint_DragDrop(object sender, DragEventArgs e)
        {
            Point point = LB_waypoint.PointToClient(new Point(e.X, e.Y));
            int index = LB_waypoint.IndexFromPoint(point);
            if (index < 0) index = LB_waypoint.Items.Count - 1;
            object data = e.Data.GetData(typeof(ZugFahrt.WayPoint));
            LB_waypoint.Items.Remove(data);
            LB_waypoint.Items.Insert(index, data);

            Zug.WayPoints.Clear();
            foreach(var WP in LB_waypoint.Items)
            {
                Zug.WayPoints.Add((ZugFahrt.WayPoint)WP);
            }

            if (Zug.WayPoints.Count > 1)
            {
                Zug.routeBerechnen();
            }
            Laden();
        }

        private void LB_waypoint_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// Updated Signalparameter, wenn neues Signal ausgewählt wurde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LB_signal_SelectedIndexChanged(object sender, EventArgs e)
        {
            routenPunktLaden();
        }

        private void routenPunktLaden()
        {
            if (Zug.route == null || Zug.route.Count == 0 || LB_routenPunkte.SelectedItem == null || !guiBereit)
            {
                return;
            }

            guiBereit = false;

            ZugFahrt.RoutenPunkt routenPunkt = (ZugFahrt.RoutenPunkt)LB_routenPunkte.SelectedItem;
            var wegPunkt = routenPunkt.wegPunkt;

            label_signal.Text = routenPunkt.ToString();
            label_fahrstraße.Text = routenPunkt.fahrstraße.name;

            label_dauer.Text = routenPunkt.fahrdauer.ToString("f0") + "s für " + (routenPunkt.fahrstraße.länge / 1000).ToString("f1") + "km";
            label_vMin.Text = (routenPunkt.fahrstraße.vStart * 3.6).ToString("f0") + "km/h für " + (routenPunkt.fahrstraße.längeWeichenBereich / 1000).ToString("f1") + "km";
            if (routenPunkt.fahrstraße.längeWeichenBereich == routenPunkt.fahrstraße.länge / 2)
                label_vMin.Text += "*";

            label_vMax.Text = (routenPunkt.fahrstraße.vZiel * 3.6).ToString("f0") + "km/h";

            bool istWegPunkt = routenPunkt.wegPunkt != null;
            checkBox_Ankunft.Enabled = istWegPunkt;
            checkBox_Abfahrt.Enabled = istWegPunkt;
            button_WpUmwandlung.Visible = !istWegPunkt;

            if (istWegPunkt)
            {
                checkBox_Ankunft.Checked = wegPunkt.ankunft_gesetzt;
                checkBox_Abfahrt.Checked = wegPunkt.abfahrt_gesetzt;
            }
            else
            {
                checkBox_Ankunft.Checked = false;
                checkBox_Abfahrt.Checked = false;
            }


            dateTimePicker_Ankunft.Enabled = checkBox_Ankunft.Enabled && checkBox_Ankunft.Checked;
            dateTimePicker_Abfahrt.Enabled = checkBox_Abfahrt.Enabled && checkBox_Abfahrt.Checked;

            dateTimePicker_Ankunft.Visible = routenPunkt.ankunft != new DateTime();
            dateTimePicker_Abfahrt.Visible = routenPunkt.abfahrt != new DateTime();

            if (dateTimePicker_Ankunft.Visible)
                dateTimePicker_Ankunft.Value = routenPunkt.ankunft;
            if (dateTimePicker_Abfahrt.Visible)
                dateTimePicker_Abfahrt.Value = routenPunkt.abfahrt;

            
            if (dateTimePicker_Ankunft.Visible)
            {
                signalText.Text = "Planhalt";
            }
            else
            {
                signalText.Text = "Durchfahrt";
            }

            guiBereit = true;
        }

        /// <summary>
        /// Wandelt RoutenPunkt in bearbeitbaren Wegpunkt um
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_WpUmwandlung_Click(object sender, EventArgs e)
        {
            ZugFahrt.RoutenPunkt routenPunkt = (ZugFahrt.RoutenPunkt)LB_routenPunkte.SelectedItem;
            if (routenPunkt == null)
                return;

            var wegPunkt = new ZugFahrt.WayPoint(routenPunkt.signal, Zug);
            //wegPunkt.ankunft = routenPunkt.ankunft;
            //wegPunkt.abfahrt = routenPunkt.abfahrt;

            //if (wegPunkt.ankunft != new DateTime())
            //    wegPunkt.ankunft_gesetzt = true;
            //if (wegPunkt.abfahrt != new DateTime())
            //    wegPunkt.abfahrt_gesetzt = true;

            Zug.WayPoints.Insert(Zug.WayPoints.IndexOf(routenPunkt.letzterWegPunkt) + 1, wegPunkt);

            neuBerechnen(wegPunkt);
        }

        private void fahrplandaten_verändert(object sender, EventArgs e)
        {
            routenPunktSpeichern();
        }

        private void routenPunktSpeichern()
        {
            if (Zug.route == null || Zug.route.Count == 0 || !guiBereit)
                return;

            guiBereit = false;

            ZugFahrt.RoutenPunkt routenPunkt = (ZugFahrt.RoutenPunkt)LB_routenPunkte.SelectedItem;
            if (routenPunkt == null)
                return;
            var wegPunkt = routenPunkt.wegPunkt;

            if (checkBox_Ankunft.Checked)
            {
                if (wegPunkt.ankunft != new DateTime())
                {
                    //Ankunft wurde bereits in das Feld eingetragen und muss nur noch zurückgelesen werden.
                    wegPunkt.ankunft = dateTimePicker_Ankunft.Value;
                }
                else
                {
                    //Ankunft muss noch bestimmt werden; Box wurde gerade umgeschaltet
                    wegPunkt.ankunft = routenPunkt.abfahrt.AddSeconds(streckenModul.zeitverlustDurchBeschleunigung(routenPunkt.fahrstraße.vZiel, 0, 0.5) * (1 + Zug.reserve));
                }
                wegPunkt.ankunft_gesetzt = true;
            }
            else
            {
                wegPunkt.ankunft = new DateTime();
                wegPunkt.ankunft_gesetzt = false;
            }

            if (checkBox_Abfahrt.Checked)
            {
                if (wegPunkt.abfahrt != new DateTime())
                {
                    //Abfahrt wurde bereits in das Feld eingetragen und muss nur noch zurückgelesen werden.
                    wegPunkt.abfahrt = dateTimePicker_Abfahrt.Value;
                }
                else
                {
                    //Abfahrt muss noch bestimmt werden; Box wurde gerade umgeschaltet
                    if (dateTimePicker_Abfahrt.Visible)
                    {
                        //Vorberechnete Abfahrzeit kann übernommen werden
                        wegPunkt.abfahrt = dateTimePicker_Abfahrt.Value;
                    }
                    else
                    {
                        //Halt muss aus Ankunft berechnet werden
                        wegPunkt.abfahrt = wegPunkt.ankunft.AddSeconds(30);
                    }
                }
                wegPunkt.abfahrt_gesetzt = true;
            }
            else
            {
                wegPunkt.abfahrt_gesetzt = false;
                wegPunkt.abfahrt = new DateTime();
            }

            neuBerechnen(wegPunkt);

            guiBereit = true;

            routenPunktLaden();
        }

        private void neuBerechnen(ZugFahrt.WayPoint auswahl)
        {
            if (Zug.WayPoints.Count > 1)
            {
                Zug.routeBerechnen();
            }
            Laden();

            var routenPunkt = auswahl.routenPunkt;
            LB_routenPunkte.SelectedItem = routenPunkt;
        }

        private DateTime sucheZeit(ZugFahrt.WayPoint wegPunkt, bool ankunft)
        {

            var zeit = wegPunkt.abfahrt;
            if (zeit == new DateTime())
            {
                zeit = wegPunkt.ankunft;
                zeit = zeit.AddSeconds(30);
            }
            else
            {
                zeit = zeit.AddSeconds(-30);
            }

            return zeit;
        }

        /// <summary>
        /// Liest neuen Zugverband ein
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_zugVerband_Click(object sender, EventArgs e)
        {
            OpenFileDialog zugVerbandDialog = new OpenFileDialog();

            zugVerbandDialog.InitialDirectory = Zug.datensatz.datenVerzeichnis + "RollingStock\\";
            zugVerbandDialog.Filter = "Zugverband (*.trn.xml)|*.trn.xml|Alle Dateiformate (*.*)|*.*";
            zugVerbandDialog.FilterIndex = 1;
            zugVerbandDialog.RestoreDirectory = true;

            if (zugVerbandDialog.ShowDialog() == DialogResult.OK)
            {
                string speicherort = zugVerbandDialog.FileName;

                string[] speicherOrtArray = zugVerbandDialog.FileName.Split('\\');
                string zugVerbandName = speicherOrtArray[speicherOrtArray.Length - 1];
                zugVerbandName = zugVerbandName.Substring(0, zugVerbandName.Length - 8);



                var xmlReader = XmlReader.Create(speicherort);

                while (xmlReader.Read())
                {
                    if (xmlReader.Name == "FahrzeugVarianten")
                    {
                        Zug.zugVerbandXML = xmlReader.ReadOuterXml();

                        Zug.zugVerbandName = zugVerbandName;
                        button_zugVerband.Text = zugVerbandName + " - Ändern";

                        break;
                    }
                }
            }
        }
    }
}
