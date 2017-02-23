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

            this.MouseWheel += new MouseEventHandler(mMap_MouseWheel);

            appInit();
        }

        private void mMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                kartenZeichner.updateScale(1.25);
            if (e.Delta < 0)
                kartenZeichner.updateScale(0.8);
        }

        private void ModulButton_Click(object sender, EventArgs e)
        {
            appInit();
        }

        private void appInit()
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

        private void mMap_MouseDown(object sender, MouseEventArgs e)
        {
            mMap.Focus();

            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                mouseDownX = e.X;
                mouseDownY = e.Y;
            }

            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void mMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown && (e.Button == MouseButtons.Left))
            {
                mouseDown = false;
                int deltaX = e.X - mouseDownX;
                int deltaY = e.Y - mouseDownY;

                kartenZeichner.move(deltaY, deltaX);
            }

            if (e.Button == MouseButtons.Right)
            {
                MenuItem[] menuItems = new MenuItem[]{new MenuItem("X: " + e.X),
                new MenuItem("Y: " + e.Y)};

                ContextMenu buttonMenu = new ContextMenu(menuItems);
                buttonMenu.Show(mMap, new System.Drawing.Point(e.X, e.Y));
            }
        }
    }
}
