using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuSiFplEdit
{
    /// <summary>
    /// UTM-Punkt und zugehörige Funktionen
    /// </summary>
    public class PunktUTM
    {
        public double rel_X;
        public double rel_Y;
        public double WE;
        public double NS;
        public int Z1;
        public char Z2;

        public PunktUTM(double ref_X, double ref_Y, double rel_X, double rel_Y)
        {
            this.rel_X = rel_X;
            this.rel_Y = rel_Y;

            WE = ref_X + (rel_X / 1000f);
            NS = ref_Y + (rel_Y / 1000f);
        }

        public PunktUTM(double abs_X, double abs_Y)
        {
            this.WE = abs_X;
            this.NS = abs_Y;
        }

        public PunktUTM(double WE, double NS, int Z1, char Z2)
        {
            this.WE = WE;
            this.NS = NS;

            this.Z1 = Z1;
            this.Z2 = Z2;
        }

        public double distanceTo(PunktUTM target)
        {
            double distX = target.WE - WE;
            double distY = target.NS - NS;

            return (Math.Sqrt(distX * distX + distY * distY));
        }
    }
}
