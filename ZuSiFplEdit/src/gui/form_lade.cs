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

        [Obsolete]
        public void instantProgress(bool secondaryBar, int value, string txt)
        {
            instantProgress(value, txt);
        }

        public void instantProgress(int value, string txt)
        {
            instantProgress(value, progressBar1.Maximum, txt);
        }

        public void instantProgress(int value, int valMax, string txt)
        {
            if ((value != progressBar1.Value) || (valMax != progressBar1.Maximum))
            {
                progressBar1.Maximum = valMax + 1;
                progressBar1.Value = value + 1;
                progressBar1.Value = value;
                progressBar1.Maximum--;
            }
            

            if (txt != "")
            {
                Text = txt;
            }

            Update();
        }
    }
}
