using System;
using System.Drawing;
using System.Windows.Forms;


namespace ZuSiFplEdit
{
    public partial class modSelForm : Form
    {
        modContainer Module;
        mapDraw kartenZeichner;

        int mouseDownX = 0;
        int mouseDownY = 0;
        bool mouseDown = false;

        public modSelForm()
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(mMap_MouseWheel);
            modListBox.SelectedIndexChanged += new EventHandler(modListBox_SelectedValueChanged);

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
            Module.writeToFile();
        }

        private void appInit()
        {
            //Module einlesen
            Module = new modContainer();

            //Module ausgeben
            foreach (modContainer.streckenModul modul in Module.mSammlung)
            {
                modListBox.Items.Add(modul.modName);
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

                int movementThreshold = 3;

                if ((Math.Abs(deltaX) > movementThreshold) || (Math.Abs(deltaY) > movementThreshold))
                {
                    kartenZeichner.move(deltaY, deltaX);
                } else
                {
                    var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);
                    if (kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y) < 10)
                    {
                        nächsteStation.selected = !nächsteStation.selected;
                        kartenZeichner.draw();
                        modListBox.SetSelected(Module.mSammlung.IndexOf(nächsteStation), nächsteStation.selected);
                    }
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                var nächsteStation = kartenZeichner.getNearestStation(e.X, e.Y);

                MenuItem[] menuItems = new MenuItem[]{new MenuItem("X: " + e.X + " - Y: " + e.Y),
                new MenuItem("X: " + kartenZeichner.pixToCoord(e.X, false).ToString("F1") + " - Y: " + kartenZeichner.pixToCoord(e.Y, true).ToString("F1")),
                new MenuItem("Nächste Station: " + nächsteStation.modName + "; Distanz: " + kartenZeichner.getStationDistance(nächsteStation, e.X, e.Y).ToString())};

                ContextMenu buttonMenu = new ContextMenu(menuItems);
                buttonMenu.Show(mMap, new Point(e.X, e.Y));
            }
        }

        private void modListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Module.mSammlung.Count; i++)
            {
                if (modListBox.SelectedIndices.Contains(i))
                {
                    Module.mSammlung[i].selected = true;
                } else
                {
                    Module.mSammlung[i].selected = false;
                }
            }
            kartenZeichner.draw();
        }
    }
}
