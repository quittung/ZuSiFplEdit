using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Drawing.Drawing2D;


namespace ZuSiFplEdit
{
    public partial class Form1 : Form
    {
        modContainer Module;
        Graphics ModulKarte;

        float mapScale = 1;
        float pixelScale = 1;
        float verschubX = 0;
        float verschubY = 0;

        int mausDaunX = 0;
        int mausDaunY = 0;
        bool mausDaun = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void ModulButton_Click(object sender, EventArgs e)
        {
            ModulKarte = mMap.CreateGraphics();
            Module = new modContainer();
            foreach (modContainer.streckenModul modul in Module.mSammlung)
            {
                ModText.Text += modul.modName + "\r\n";
            }
            karteZeichnen();
        }



        void karteZeichnen()
        {
            ModulKarte.Clear(Color.White);

            int AusdehnungNS = Module.grenzeN - Module.grenzeS;
            int AusdehnungWE = Module.grenzeE - Module.grenzeW;

            float verhältnisKarte = (float)AusdehnungWE / (float)AusdehnungNS;
            float verhältnisZeichenfläche = (float)mMap.Width / (float)mMap.Height;

            float faktorX = mapScale;
            float faktorY = mapScale;
            if (verhältnisKarte < verhältnisZeichenfläche)
            {
                faktorX = mapScale * verhältnisKarte / verhältnisZeichenfläche;
                pixelScale = (float)AusdehnungNS / (float)mMap.Height;
            }
                
            else
            {
                faktorY = mapScale * verhältnisZeichenfläche / verhältnisKarte;
                pixelScale = (float)AusdehnungWE / (float)mMap.Width;
            }
                
            
            foreach (modContainer.streckenModul aktModul in Module.mSammlung)
            {
                int posRelN = Module.grenzeN - aktModul.UTM_NS + (int)verschubY;
                int posRelW = aktModul.UTM_WE - Module.grenzeW + (int)verschubX;

                float posX = (float)posRelW / (float)AusdehnungWE * faktorX * mMap.Width;
                float posY = (float)posRelN / (float)AusdehnungNS * faktorY * mMap.Height;
                
                ModulKarte.DrawEllipse(new Pen(Color.Blue), posX, posY, 5, 5);

                if (mapScale > 15)
                {
                    ModulKarte.DrawString(aktModul.modName, new Font("Verdana", 8), new SolidBrush(Color.Red), posX + 5, posY + 5);
                }
            }
        }

        private void ScaleButton_Click(object sender, EventArgs e)
        {
            if (sender == MapScaleUp)
                mapScale = mapScale * 1.25F;
            else
                mapScale = mapScale * 0.8F;
            
            karteZeichnen();
        }

        private void mMap_MouseDown(object sender, MouseEventArgs e)
        {
            mausDaun = true;
            mausDaunX = e.X;
            mausDaunY = e.Y;
        }

        private void mMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (mausDaun)
            {
                mausDaun = false;
                int deltaX = e.X - mausDaunX;
                int deltaY = e.Y - mausDaunY;

                verschubX += deltaX * pixelScale / mapScale;
                verschubY += deltaY * pixelScale / mapScale;

                karteZeichnen();
            }
        }
    }
}
