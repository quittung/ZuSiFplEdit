using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    public partial class ZugForm : Form
    {
        ZugFahrt FormZugFahrt;
        modSelForm Hauptmenü;

        public static event SignalSelectEventHandler signalSelectionEvent;

        public ZugForm()
        {
            InitializeComponent();
        }

        public void ZugFahrtLaden(ZugFahrt ZF)
        {
            this.FormZugFahrt = ZF;

            listBox1.Items.Clear();
            listBox2.Items.Clear();

            foreach (var WP in ZF.WayPoints)
            {
                listBox1.Items.Add(WP);
            }
            foreach (var RP in ZF.route)
            {
                listBox2.Items.Add(RP);
            }
        }

        private void Sync()
        {
            ZugFahrtLaden(FormZugFahrt);
        }

        private void button1_Click(object sender, EventArgs e)
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

        public void setSignal(streckenModul.referenzElement Signal)
        {
            FormZugFahrt.WayPoints.Add(new ZugFahrt.WayPoint(Signal));
            if (FormZugFahrt.WayPoints.Count > 1)
            {
                FormZugFahrt.routeBerechnen();
            }
            Sync();
        }

        private void button2_Click(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;
            FormZugFahrt.WayPoints.RemoveAt(listBox1.SelectedIndex);
            if (FormZugFahrt.WayPoints.Count > 1)
            {
                FormZugFahrt.routeBerechnen();
            }
            Sync();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Höchstgeschwindigkeit wurde verändert.
            try
            {
                textBox1.BackColor = Color.White;
                FormZugFahrt.vMax = (float)Convert.ToDouble(textBox1.Text) / 3.6f;
            }
            catch (Exception)
            {
                textBox1.BackColor = Color.PaleVioletRed;
            }
        }
    }
}
