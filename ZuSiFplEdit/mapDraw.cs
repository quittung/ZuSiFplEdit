using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;

namespace ZuSiFplEdit
{
    class mapDraw
    {
        List<modContainer.streckenModul> modList;
        List<modContainer.streckenModul> modVisible;
        Graphics map;

        int map_width_p;
        int map_height_p;

        double center_NS;
        double center_WE;
        double pixPerGrad;

        double border_north;
        double border_east;
        double border_south;
        double border_west;

        public mapDraw(Graphics map_gr, int width, int height, List<modContainer.streckenModul> mList)
        {
            map = map_gr;
            map_width_p = width;
            map_height_p = height;
            modList = mList;
            modVisible = new List<modContainer.streckenModul>();

            setInitPos();
        }

        public int coordToPix(double coord, bool isNS)
        {
            if (isNS)
            {
                return (int)((border_north - coord) / (border_north - border_south) * map_height_p);
            } else
            {
                return (int)((coord - border_west) / (border_east - border_west) * map_width_p);
            }
        }

        public double pixToCoord(int pix, bool isNS)
        {
            if (isNS)
            {
                return (border_north - ((double)pix * (border_north - border_south) / (double)map_height_p));
            }
            else
            {
                return (((double)pix * (border_east - border_west) / (double)map_width_p) + border_west);
            }
        }

        void setInitPos()
        {
            border_north = modList[0].UTM_NS;
            border_south = modList[0].UTM_NS;
            border_east = modList[0].UTM_WE;
            border_west = modList[0].UTM_WE;
            foreach (modContainer.streckenModul mod in modList)
            {
                if (mod.UTM_NS > border_north) border_north = mod.UTM_NS;
                if (mod.UTM_NS < border_south) border_south = mod.UTM_NS;
                if (mod.UTM_WE > border_east) border_east = mod.UTM_WE;
                if (mod.UTM_WE < border_west) border_west = mod.UTM_WE;
            }

            center_NS = (border_north + border_south) / 2f;
            center_WE = (border_west + border_east) / 2f;

            if (map_width_p > map_height_p) pixPerGrad = map_height_p / (border_north - border_south);
            else pixPerGrad = map_width_p / (border_east - border_west);

            pixPerGrad = pixPerGrad * 0.9;

            updateBorders();
        }

        public void updateScale (double factor)
        {
            pixPerGrad = pixPerGrad * factor;
            updateBorders();

            draw();
        }

        public void move (int pix_NS, int pix_WE)
        {
            center_NS += (double)pix_NS / pixPerGrad;
            center_WE -= (double)pix_WE / pixPerGrad;

            updateBorders();
            draw();
        }

        void updateBorders()
        {
            double halfLen_NS = (double)map_height_p / (pixPerGrad * 2f);
            double halfLen_WE = (double)map_width_p / (pixPerGrad * 2f);

            border_north = center_NS + halfLen_NS;
            border_east = center_WE + halfLen_WE;
            border_south = center_NS - halfLen_NS;
            border_west = center_WE - halfLen_NS;

            modVisible.Clear();
            foreach (modContainer.streckenModul mod in modList)
            {
                if (isVisible(mod))
                {
                    mod.PIX_X = coordToPix(mod.UTM_WE, false);
                    mod.PIX_Y = coordToPix(mod.UTM_NS, true);

                    modVisible.Add(mod);
                }
            }
        }
         
        bool isVisible (modContainer.streckenModul mod)
        {
            return ((mod.UTM_NS < border_north) && (mod.UTM_NS > border_south) && (mod.UTM_WE < border_east) && (mod.UTM_WE > border_west));
        }


        //Zeichnet die Karte. 
        public void draw()
        {
            map.Clear(Color.White);

            map.DrawString("N" + center_NS.ToString("F2") + " - E" + center_WE.ToString("F2") + " - " + pixPerGrad.ToString("F1") + "pix/km", new Font("Verdana", 10), new SolidBrush(Color.Red), 20, map_height_p - 20);

            Pen pen_unselected = new Pen(Color.Black);
            Pen pen_selected = new Pen(Color.Green);
            Pen pen_act;
            
            //First layer (lines + names) + data processing
            foreach (modContainer.streckenModul mod in modVisible)
            {
                if (mod.selected)
                {
                    pen_act = pen_selected;
                } else
                {
                    pen_act = pen_unselected; 
                }

                foreach (modContainer.streckenModul connection in mod.Verbindungen)
                {
                    map.DrawLine(pen_unselected, mod.PIX_X, mod.PIX_Y, coordToPix(connection.UTM_WE, false), coordToPix(connection.UTM_NS, true));
                }
                if (pixPerGrad > 11)
                {
                    //TODO: Text richtig ausrichten.
                    map.DrawString(mod.modName, new Font("Verdana", 8), Brushes.Black, mod.PIX_X + 10, mod.PIX_Y + 10);
                }  
            }

            //Second layer (station circles)
            foreach (var mod in modVisible)
            {
                int circleSize = 8;

                if (pixPerGrad > 2)
                {
                    if (mod.selected)
                    {
                        map.FillEllipse(Brushes.Red, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                        map.DrawEllipse(pen_selected, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                    }
                    else
                    {
                        map.FillEllipse(Brushes.LightGray, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                        map.DrawEllipse(pen_unselected, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                    }
                }
            }
        }

        public modContainer.streckenModul getNearestStation(int X, int Y)
        {
            double dist = -1;
            modContainer.streckenModul nearestMod = null;
            foreach (modContainer.streckenModul mod in modVisible)
            {
                double modDist = getStationDistance(mod, X, Y);
                if ((dist > modDist) || (dist == -1))
                {
                    dist = modDist;
                    nearestMod = mod;
                }                
            }
            return (nearestMod);
        }

        public int getStationDistance(modContainer.streckenModul mod, int X, int Y)
        {
            int deltaX = mod.PIX_X - X;
            int deltaY = mod.PIX_Y - Y;

            int dist = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return dist;
        }
    }
}
