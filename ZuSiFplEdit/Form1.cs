using System;
using System.Drawing;
using System.Windows.Forms;


namespace ZuSiFplEdit
{
    public partial class Form1 : Form
    {
        modContainer Module;
        mapDraw kartenZeichner;

        int mouseDownX = 0;
        int mouseDownY = 0;
        bool mouseDown = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void ModulButton_Click(object sender, EventArgs e)
        {
            //Module einlesen
            Module = new modContainer();

            //Module ausgeben
            foreach (modContainer.streckenModul modul in Module.mSammlung)
            {
                ModText.Text += modul.modName + "\r\n";
            }

            //Modulekarte vorbereiten
            kartenZeichner = new mapDraw(mMap.CreateGraphics(), mMap.Width, mMap.Height, Module.mSammlung);
            kartenZeichner.draw();
        }

        private void ScaleButton_Click(object sender, EventArgs e)
        {
            if (sender == MapScaleUp)
                kartenZeichner.updateScale(1.25);
            else
                kartenZeichner.updateScale(0.8);
        }

        private void mMap_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownX = e.X;
            mouseDownY = e.Y;
        }

        private void mMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                mouseDown = false;
                int deltaX = e.X - mouseDownX;
                int deltaY = e.Y - mouseDownY;

                kartenZeichner.move(deltaY, deltaX);
            }
        }
    }
}
