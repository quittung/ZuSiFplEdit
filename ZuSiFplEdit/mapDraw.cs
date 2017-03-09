using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    class mapDraw
    {
        List<streckenModul> modList;
        List<streckenModul> modVisible;
        ListBox ZFBox;
        public Bitmap frame;
        Graphics framebuffer;

        int map_width_p;
        int map_height_p;

        double center_NS;
        double center_WE;
        double pixPerGrad;

        double border_north;
        double border_east;
        double border_south;
        double border_west;

        //Drawing layers:
        bool drawModule = true;
        bool drawModule_Punkte = true;
        bool drawModule_Namen = true;
        bool drawModule_Verbindungen = true;
        bool drawModule_Grenzen = false;

        bool drawStrecke = false;
        bool drawSignal_Start = true;
        bool drawSignal_Ziel = true;
        bool drawSignal_Namen = true;

        bool drawFahrstrassen = false;

        bool drawRoute = true;




        public mapDraw(int width, int height, List<streckenModul> mList, ListBox ZFBox)
        {
            map_width_p = width;
            map_height_p = height;
            modList = mList;
            modVisible = new List<streckenModul>();
            this.ZFBox = ZFBox; 

            frame = new Bitmap(map_width_p, map_height_p);
            framebuffer = Graphics.FromImage(frame);

            setInitPos();
        }

        public void updateMapSize(int width, int height)
        {
            map_width_p = width;
            map_height_p = height;
            frame = new Bitmap(width, height);
            framebuffer = Graphics.FromImage(frame);
            updateBorders();
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
            foreach (streckenModul mod in modList)
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
        }

        public void move (int pix_NS, int pix_WE)
        {
            center_NS += (double)pix_NS / pixPerGrad;
            center_WE -= (double)pix_WE / pixPerGrad;

            updateBorders();
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
            foreach (streckenModul mod in modList)
            {
                if (isVisible(mod))
                {
                    mod.PIX_X = coordToPix(mod.UTM_WE, false);
                    mod.PIX_Y = coordToPix(mod.UTM_NS, true);

                    modVisible.Add(mod);
                }
            }
        }
         
        bool isVisible (streckenModul mod)
        {
            double ax = border_east - border_west;
            double ay = border_north - border_south;
            double halfDiag = Math.Sqrt(ax * ax + ay * ay) * 0.5;
            double dx = mod.UTM_WE - center_WE;
            double dy = mod.UTM_NS - center_NS;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            return (dist < (mod.drawDist + halfDiag));
        }



        public Bitmap draw()
        {
            var frameTime = new System.Diagnostics.Stopwatch();
            frameTime.Start();

            framebuffer.Clear(Color.White);

            Pen pen_unselected = new Pen(Color.Black);
            Pen pen_selected = new Pen(Color.Red);

            if (drawModule)
            {
                if (drawModule_Verbindungen)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        foreach (streckenModul connection in mod.Verbindungen)
                        {
                            framebuffer.DrawLine(pen_unselected, mod.PIX_X, mod.PIX_Y, coordToPix(connection.UTM_WE, false), coordToPix(connection.UTM_NS, true));
                        }
                    }
                }
                if (drawModule_Punkte)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        int circleSize = 8; //Größe der Modulkreise
                        if ((pixPerGrad > 2) || mod.NetzGrenze || mod.selected)
                        {
                            if (mod.selected)
                            {
                                framebuffer.FillEllipse(Brushes.Red, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                                framebuffer.DrawEllipse(pen_unselected, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                            }
                            else if (mod.NetzGrenze)
                            {
                                framebuffer.FillEllipse(Brushes.DarkGray, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                                framebuffer.DrawEllipse(pen_unselected, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                            }
                            else //Normale Module
                            {
                                framebuffer.FillEllipse(Brushes.LightGray, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                                framebuffer.DrawEllipse(pen_unselected, mod.PIX_X - circleSize / 2, mod.PIX_Y - circleSize / 2, circleSize, circleSize);
                            }
                        }
                    }
                }
                if (drawModule_Namen && pixPerGrad > 11)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        framebuffer.DrawString(mod.modName, new Font("Verdana", 8), Brushes.Black, mod.PIX_X + 6, mod.PIX_Y);
                    }
                }
                if (drawModule_Grenzen && pixPerGrad > 11)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        for (int i = 0; i < mod.Huellkurve.Count; i++)
                        {
                            var x1 = mod.Huellkurve[i].abs_X;
                            var y1 = mod.Huellkurve[i].abs_Y;
                            var x2 = mod.Huellkurve[(i + 1) % mod.Huellkurve.Count].abs_X;
                            var y2 = mod.Huellkurve[(i + 1) % mod.Huellkurve.Count].abs_Y;

                            framebuffer.DrawLine(pen_selected, coordToPix(x1, false), coordToPix(y1, true), coordToPix(x2, false), coordToPix(y2, true));
                        }
                    }
                }
            }
            if (drawStrecke)
            {
                int[] gewünschteSignale = new int[] { 7, 8, 9, 10, 12 }; //5 Können zielsignale sein-
                //int[] gewünschteSignale = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

                foreach (streckenModul mod in modVisible)
                {
                    foreach (var strE in mod.StreckenElemente)
                    {
                        if (strE.Funktion != 2)
                        {
                            framebuffer.DrawLine(Pens.Black, coordToPix(strE.b_X, false), coordToPix(strE.b_Y, true), coordToPix(strE.g_X, false), coordToPix(strE.g_Y, true));
                        }
                        else
                        {
                            framebuffer.DrawLine(Pens.LightGray, coordToPix(strE.b_X, false), coordToPix(strE.b_Y, true), coordToPix(strE.g_X, false), coordToPix(strE.g_Y, true));
                        }
                        
                    }

                    foreach (var startSig in mod.StartSignale)
                    {
                        if (drawSignal_Start)
                        {
                            if (drawSignal_Namen && startSig.RefTyp == 4)
                                framebuffer.DrawString(startSig.Signal.Signaltyp.ToString() + ":" + startSig.Signal.Name, new Font("Verdana", 8), Brushes.Black, coordToPix(startSig.StrElement.b_X, false) + 3, coordToPix(startSig.StrElement.b_Y, true) + 3);
                            if (drawSignal_Namen && startSig.RefTyp == 0)
                                framebuffer.DrawString(startSig.ToString(), new Font("Verdana", 8), Brushes.Black, coordToPix(startSig.StrElement.b_X, false) + 3, coordToPix(startSig.StrElement.b_Y, true) + 3);
                            int circleSize = 4;
                            framebuffer.FillEllipse(Brushes.LightGreen, coordToPix(startSig.StrElement.b_X, false) - circleSize / 2, coordToPix(startSig.StrElement.b_Y, true) - circleSize / 2, circleSize, circleSize); 
                        }
                    }

                    foreach (var zielSig in mod.ZielSignale)
                    {
                        if (drawSignal_Ziel)
                        {
                            if (drawSignal_Namen && zielSig.RefTyp == 4)
                                framebuffer.DrawString(zielSig.Signal.Signaltyp.ToString() + ":" + zielSig.Signal.Name, new Font("Verdana", 8), Brushes.Black, coordToPix(zielSig.StrElement.b_X, false) + 3, coordToPix(zielSig.StrElement.b_Y, true) + 3);
                            int circleSize = 4;
                            framebuffer.FillEllipse(Brushes.Red, coordToPix(zielSig.StrElement.b_X, false) - circleSize / 2, coordToPix(zielSig.StrElement.b_Y, true) - circleSize / 2, circleSize, circleSize); 
                        }
                    }
                }
            }
            if (drawFahrstrassen)
            {
                foreach (streckenModul mod in modVisible)
                {
                    foreach (var fstr in mod.FahrStr)
                    {
                        if (fstr.Ziel != null)
                        {
                            int start_x = coordToPix(fstr.Start.StrElement.b_X, false);
                            int start_y = coordToPix(fstr.Start.StrElement.b_Y, true);
                            int ziel_x = coordToPix(fstr.Ziel.StrElement.b_X, false);
                            int ziel_y = coordToPix(fstr.Ziel.StrElement.b_Y, true);

                            //framebuffer.DrawString("Fstr:" + fstr.FahrstrName, new Font("Verdana", 8), Brushes.Black, start_x + 3, start_y + 3);

                            framebuffer.DrawLine(Pens.Orange, start_x, start_y, ziel_x, ziel_y); 
                        }
                    }
                }
            }
            if (drawRoute && ZFBox.SelectedItem != null)
            {
                var aktZugFahrt = (modSelForm.ZugFahrt)ZFBox.SelectedItem;
                if (aktZugFahrt.route != null)
                {
                    foreach (var step in aktZugFahrt.route)
                    {
                        framebuffer.DrawLine(Pens.Red, coordToPix(step.Start.StrElement.b_X, false), coordToPix(step.Start.StrElement.b_Y, true), coordToPix(step.Ziel.StrElement.b_X, false), coordToPix(step.Ziel.StrElement.b_Y, true));
                    } 
                }
            }
            frameTime.Stop();
            framebuffer.DrawString("N" + center_NS.ToString("F2") + " - E" + center_WE.ToString("F2") + " - " + pixPerGrad.ToString("F1") + "pix/km - " + frameTime.ElapsedMilliseconds + " ms/frame", new Font("Verdana", 10), new SolidBrush(Color.Red), 20, map_height_p - 20);

            return (frame);
        }

        public streckenModul getNearestStation(int X, int Y)
        {
            double dist = -1;
            streckenModul nearestMod = null;
            foreach (streckenModul mod in modVisible)
            {
                double modDist = getStationDistance(mod, X, Y);
                if (((dist > modDist) || (dist == -1)) && !(modDist < 0))
                {
                    dist = modDist;
                    nearestMod = mod;
                }                
            }
            return (nearestMod);
        }

        public int getStationDistance(streckenModul mod, int X, int Y)
        {
            int deltaX = mod.PIX_X - X;
            int deltaY = mod.PIX_Y - Y;

            int dist = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return dist;
        }

        public streckenModul.referenzElement getNearestSignalStartZiel(int X, int Y, bool start)
        {
            double closestDist = -1;
            List<streckenModul.referenzElement> Ergebnis = new List<streckenModul.referenzElement>();

            foreach (streckenModul mod in modVisible)
            {
                List<streckenModul.referenzElement> Signalliste;
                if (start)
                {
                    Signalliste = mod.StartSignale;
                }
                else
                {
                    Signalliste = mod.ZielSignale;
                }
                foreach (var Signal in Signalliste)
                {
                    double sigDist = getSigDistance(Signal, X, Y);
                    if ((closestDist > sigDist) || (closestDist == -1) && !(sigDist < 0))
                    {
                        Ergebnis.Clear();
                        Ergebnis.Add(Signal);
                        closestDist = sigDist;
                    }
                    else if (closestDist == sigDist)
                    {
                        Ergebnis.Add(Signal);
                    }
                }
            }

            if (Ergebnis.Count > 1)
            {
                var selectForm = new FormSignalSelect();
                
                foreach (var erg in Ergebnis)
                {
                    selectForm.listBox1.Items.Add(erg);
                }
                selectForm.ShowDialog();
                return ((streckenModul.referenzElement)selectForm.listBox1.SelectedItem);
            }

            return (Ergebnis[0]);
        }
            

        public streckenModul.referenzElement getNearestSignal(int X, int Y)
        {
            double dist = -1;
            streckenModul.referenzElement nearestMod = null;
            foreach (streckenModul mod in modVisible)
            {
                foreach (var Signal in mod.StartSignale)
                {
                    double modDist = getSigDistance(Signal, X, Y);
                    if (((dist > modDist) || (dist == -1)) && (modDist >= 0))
                    {
                        dist = modDist;
                        nearestMod = Signal;
                    } 
                }
            }
            return (nearestMod);
        }

        public double getSigDistance(streckenModul.referenzElement Signal, int X, int Y)
        {
            double deltaX = Signal.StrElement.b_X - pixToCoord(X, false);
            double deltaY = Signal.StrElement.b_Y - pixToCoord(Y, true);

            double dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return dist;
        }

        public void setLayers(string layer, bool drawLayer)
        {
            if (layer == "module")
            {
                drawModule = drawLayer;
            }
            else if (layer == "module_punkte")
            {
                drawModule_Punkte = drawLayer;
            }
            else if (layer == "module_namen")
            {
                drawModule_Namen = drawLayer;
            }
            else if (layer == "module_verbindungen")
            {
                drawModule_Verbindungen = drawLayer;
            }
            else if (layer == "module_grenzen")
            {
                drawModule_Grenzen = drawLayer;
            }
            else if (layer == "strecke")
            {
                drawStrecke = drawLayer;
            }
            else if (layer == "signal_namen")
            {
                drawSignal_Namen = drawLayer;
            }
            else if (layer == "signal_start")
            {
                drawSignal_Start = drawLayer;
            }
            else if (layer == "signal_ziel")
            {
                drawSignal_Ziel = drawLayer;
            }
            else if (layer == "fahrstr")
            {
                drawFahrstrassen = drawLayer;
            }
            else if (layer == "route")
            {
                drawRoute = drawLayer;
            }
        }
    }
}
