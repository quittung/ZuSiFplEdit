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

            setInitialBorders();
        }

        int coordToPix(double coord, bool isNS)
        {
            if (isNS)
            {
                return (int)((border_north - coord) / (border_north - border_south) * map_height_p);
            } else
            {
                return (int)((coord - border_west) / (border_east - border_west) * map_width_p);
            }
        }

        void setInitialBorders()
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
            
            if (map_width_p > map_height_p)
            {
                double coord_length = (border_north - border_south) / 2;
                coord_length = coord_length / (double)map_height_p * (double)map_width_p;
                double coord_center = (border_west + border_east) / 2;
                border_west = coord_center - coord_length;
                border_east = coord_center + coord_length;
            } else
            {
                double coord_length = (border_east - border_west) / 2;
                coord_length = coord_length / (double)map_height_p * (double)map_width_p;
                double coord_center = (border_south + border_north) / 2;
                border_south = coord_center - coord_length;
                border_north = coord_center + coord_length;
            }
        }
         
        bool isVisible (modContainer.streckenModul mod)
        {
            return ((mod.UTM_NS < border_north) && (mod.UTM_NS > border_south) && (mod.UTM_WE < border_east) && (mod.UTM_WE > border_west));
        }

        public void draw()
        {
            map.Clear(Color.White);

            Pen pn = new Pen(Color.Blue);

            foreach (modContainer.streckenModul mod in modulList)
            {
                if (isVisible(mod)) {
                    map.DrawEllipse(pn, coordToPix(mod.UTM_WE, false), coordToPix(mod.UTM_NS, true), 5, 5);
                }
            }
        }
    }
}
