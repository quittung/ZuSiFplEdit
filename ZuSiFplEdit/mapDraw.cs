using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace ZuSiFplEdit
{
    class mapDraw
    {
        List<modContainer.streckenModul> modulList;

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
            modulList = mList;

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
            border_north = modulList[0].UTM_NS;
            border_south = modulList[0].UTM_NS;
            border_east = modulList[0].UTM_WE;
            border_west = modulList[0].UTM_WE;
            foreach (modContainer.streckenModul mod in modulList)
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
        }
         
        bool isVisible (modContainer.streckenModul mod)
        {
            return ((mod.UTM_NS < border_north) && (mod.UTM_NS > border_south) && (mod.UTM_WE < border_east) && (mod.UTM_WE > border_west));
        }


        public void draw()
        {
            map.Clear(Color.White);

            map.DrawString("N" + center_NS.ToString("F2") + " - E" + center_WE.ToString("F2") + " - 1:" + pixPerGrad.ToString("F1"), new Font("Verdana", 10), new SolidBrush(Color.Red), 20, map_height_p - 20);

            Pen pn = new Pen(Color.Blue);

            foreach (modContainer.streckenModul mod in modulList)
            {
                if (isVisible(mod)) {
                    if (pixPerGrad > 1.5)
                    {
                        foreach (modContainer.streckenModul connection in mod.Verbindungen)
                        {
                            map.DrawLine(pn, coordToPix(mod.UTM_WE, false), coordToPix(mod.UTM_NS, true), coordToPix(connection.UTM_WE, false), coordToPix(connection.UTM_NS, true));
                        }

                        if (pixPerGrad > 12)
                        {
                            map.DrawString(mod.modName, new Font("Verdana", 8), Brushes.Black, coordToPix(mod.UTM_WE, false) + 10, coordToPix(mod.UTM_NS, true) + 10);
                        }
                    }
                    int circleSize = 6;
                    map.DrawEllipse(pn, coordToPix(mod.UTM_WE, false) - circleSize / 2, coordToPix(mod.UTM_NS, true) - circleSize / 2, circleSize, circleSize);
                }
            }
        }
    }
}
