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
    public partial class form_lade : Form
    {
        public form_lade()
        {
            InitializeComponent();
        }

        public void instantProgress(ProgressBar pb, int value, string txt)
        {
            pb.Maximum++;
            pb.Value = value + 1;
            pb.Value = value;
            pb.Maximum--;

            if (txt != "")
            {
                Beschreibung.Text = txt;
            }

            Update();           
        }
    }
}
