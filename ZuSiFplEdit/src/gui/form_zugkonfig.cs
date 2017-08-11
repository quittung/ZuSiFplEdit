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
        }

        /// <summary>
        /// Lädt den aktuellen Zustand des Zuges in das Formular
        /// </summary>
        public void Laden()
        {
            tbZugGattung.Text = Zug.Gattung;
            tbZugNummer.Text = Zug.Zugnummer.ToString();
            tbVmax.Text = (Zug.vMax * 3.6f).ToString("f");
            
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            foreach (var WP in Zug.WayPoints)
            {
                listBox1.Items.Add(WP);
            }
            foreach (var RP in Zug.route)
            {
                listBox2.Items.Add(RP);
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
        public void setSignal(streckenModul.referenzElement Signal)
        {
            Zug.WayPoints.Add(new ZugFahrt.WayPoint(Signal));
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

            if (listBox1.SelectedItem == null)
            {
                listBox1.Items.Add(platzHalter);
            }
            else
            {
                listBox1.Items.Insert(listBox1.SelectedIndex + 1, platzHalter);
            }
            listBox1.SelectedItem = platzHalter;

            if (listBox1.SelectedIndex == 0)
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
            if (listBox1.SelectedIndex == 0)
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
            if (listBox1.SelectedItem == null)
                return;
            Zug.WayPoints.RemoveAt(listBox1.SelectedIndex);
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
    }
}
