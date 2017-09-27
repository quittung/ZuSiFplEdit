using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    /// <summary>
    /// Hauptdialog zum Bearbeiten einzelner Zugfahrten
    /// </summary>
    public partial class ZugForm : Form
    {
        ZugFahrt Zug;

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
            LB_signal.Items.Clear();

            foreach (var WP in Zug.WayPoints)
            {
                LB_waypoint.Items.Add(WP);
            }
            foreach (var RP in Zug.route)
            {
                LB_signal.Items.Add(RP);
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
            if(Zug.route != null && Zug.route.Count > 0)
            {
                ZugFahrt.RoutenPunkt routenPunkt = (ZugFahrt.RoutenPunkt)LB_signal.SelectedItem;
                label_signal.Text = routenPunkt.ToString();
                label_fahrstraße.Text = routenPunkt.fahrstraße.name;

                label_dauer.Text = routenPunkt.fahrdauer.ToString("f0") + "s für " + (routenPunkt.fahrstraße.länge / 1000).ToString("f1") + "km";
                label_vMin.Text = (routenPunkt.fahrstraße.vStart * 3.6).ToString("f0") + "km/h für " + (routenPunkt.fahrstraße.längeWeichenBereich / 1000).ToString("f1") + "km";
                if (routenPunkt.fahrstraße.längeWeichenBereich == routenPunkt.fahrstraße.länge / 2)
                    label_vMin.Text += "*";
                    
                label_vMax.Text = (routenPunkt.fahrstraße.vZiel * 3.6).ToString("f0") + "km/h";

                checkBox_Ankunft.Checked = routenPunkt.ankunft != new DateTime();
                checkBox_Abfahrt.Checked = routenPunkt.abfahrt != new DateTime();

                dateTimePicker_Ankunft.Visible = routenPunkt.ankunft != new DateTime();
                dateTimePicker_Abfahrt.Visible = routenPunkt.abfahrt != new DateTime();

                if(dateTimePicker_Ankunft.Visible)
                    dateTimePicker_Ankunft.Value = routenPunkt.ankunft;
                if (dateTimePicker_Abfahrt.Visible)
                    dateTimePicker_Abfahrt.Value = routenPunkt.abfahrt;
            }
        }
    }
}
