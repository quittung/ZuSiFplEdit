﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;
using System.Windows.Forms;

namespace ZuSiFplEdit
{
    class mapDraw
    {
        public class PunktPix
        {
            public int X, Y;

            public PunktPix(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        class textField
        {
            public int X;
            public int Y;
            public string text;

            public int len;

            public textField(int X, int Y, string text)
            {
                this.X = X;
                this.Y = Y;
                this.text = text;
            }
        }

        List<streckenModul> modList;
        List<streckenModul> modVisible;
        ListBox ZFBox;
        Datensatz datensatz;
        Fahrplan fahrplan;
        PictureBox map;
        public Bitmap frame;
        Graphics framebuffer;

        int map_width_p;
        int map_height_p;

        double center_NS;
        double center_WE;
        PunktUTM center;
        double pixPerGrad;

        double border_north;
        double border_east;
        double border_south;
        double border_west;

        //Drawing layers:
        public bool drawModule = true;
        public bool drawModule_Punkte = true;
        public bool drawModule_Namen = true;
        public bool drawModule_Verbindungen = true;
        public bool drawModule_Grenzen = false;

        public bool drawStrecke = false;
        public bool drawSignal_Start = true;
        public bool drawSignal_Ziel = true;
        public bool drawSignal_Namen = false;

        public bool drawFahrstrassen = false;

        public bool drawRoute = true;

        /// <summary>
        /// Zusätzlicher String, der in der Karte angezeigt wird.
        /// </summary>
        public string message = "";




        public mapDraw(PictureBox map, Datensatz datensatz, Fahrplan fahrplan, ListBox ZFBox)
        {
            this.datensatz = datensatz;
            this.fahrplan = fahrplan;
            this.map = map;
            map_width_p = map.Width;
            map_height_p = map.Height;
            modList = datensatz.module;
            modVisible = new List<streckenModul>();
            this.ZFBox = ZFBox; 

            frame = new Bitmap(map_width_p, map_height_p);
            framebuffer = Graphics.FromImage(frame);

            setInitPos();
        }

        public void updateMapSize(int width, int height)
        {
            double factorX = width / map_width_p;
            double factorY = height / map_height_p;

            map_width_p = width;
            map_height_p = height;

            

            frame = new Bitmap(width, height);
            framebuffer = Graphics.FromImage(frame);
            updateBorders();
        }

        /// <summary>
        /// Rechnet UTM-Koordinate in Pixelkoordinate um
        /// </summary>
        /// <param name="coord">Koordinate in UTM-Format</param>
        /// <param name="isNS">Koordinate ist auf NS-Achse</param>
        /// <returns></returns>
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

        /// <summary>
        /// Rechnet Pixelkoordinate in UTM-Koordinate um
        /// </summary>
        /// <param name="pix"></param>
        /// <param name="isNS"></param>
        /// <returns></returns>
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

        public PunktPix UtmToPix(PunktUTM UTM)
        {
            int X = coordToPix(UTM.WE, false);
            int Y = coordToPix(UTM.NS, true);

            return (new PunktPix(X, Y));
        }

        public PunktUTM PixToUtm(PunktPix pix)
        {
            double WE = pixToCoord(pix.X, false);
            double NS = pixToCoord(pix.Y, true);

            return (new PunktUTM(WE, NS));
        }



        void setInitPos()
        {
            border_north = modList[0].ursprung.NS;
            border_south = modList[0].ursprung.NS;
            border_east = modList[0].ursprung.WE;
            border_west = modList[0].ursprung.WE;
            foreach (streckenModul mod in modList)
            {
                if (mod.ursprung.NS > border_north) border_north = mod.ursprung.NS;
                if (mod.ursprung.NS < border_south) border_south = mod.ursprung.NS;
                if (mod.ursprung.WE > border_east) border_east = mod.ursprung.WE;
                if (mod.ursprung.WE < border_west) border_west = mod.ursprung.WE;
            }
            
            center = new PunktUTM((border_west + border_east) / 2f, (border_north + border_south) / 2f);


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
            center.NS += (double)pix_NS / pixPerGrad;
            center.WE -= (double)pix_WE / pixPerGrad;

            updateBorders();
        }

        public void moveTo(PunktUTM center)
        {
            this.center = center;
        }

        void updateBorders()
        {
            double halfLen_NS = (double)map_height_p / (pixPerGrad * 2f);
            double halfLen_WE = (double)map_width_p / (pixPerGrad * 2f);

            border_north = center.NS + halfLen_NS;
            border_east = center.WE + halfLen_WE;
            border_south = center.NS - halfLen_NS;
            border_west = center.WE - halfLen_WE;

            modVisible.Clear();
            foreach (streckenModul mod in modList)
            {
                if (shouldBeDrawn(mod))
                {
                    modVisible.Add(mod);
                }
            }
        }
         
        /// <summary>
        /// Ermittelt, ob ein Modul gezeichnet werden soll
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        bool shouldBeDrawn(streckenModul mod)
        {
            if (isVisible(mod))
                return true;

            foreach (var verbindung in mod.nachbarn)
            {
                if (isVisible(verbindung))
                    return true;
            }

            if (mod.ursprung.distanceTo(center) < 15)
            {
                return true;
            }
                

            return false;
        }

        /// <summary>
        /// Ermittelt, ob der Modulursprung im Zeichenfeld liegt
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        bool isVisible (streckenModul mod)
        {
            return !((mod.ursprung.NS > border_north) || (mod.ursprung.NS < border_south) || (mod.ursprung.WE > border_east) || (mod.ursprung.WE < border_west));
        }

        public void drawMap()
        {
            map.Image = generateMap();
        }

        /// <summary>
        /// Zeichnet die Karte mit den aktuellen Parametern
        /// </summary>
        /// <returns>Fertige Karte als Bitmap</returns>
        public Bitmap generateMap()
        {
            var frameTime = new System.Diagnostics.Stopwatch();
            frameTime.Start();


            updateBorders();

            framebuffer.Clear(Color.White);

            var textManager = new List<textField>();

            Pen pen_unselected = new Pen(Color.Black);
            Pen pen_selected = new Pen(Color.Red);

            if (drawModule)
            {
                foreach (streckenModul mod in modVisible)
                {
                    if ((drawModule_Verbindungen) && (mod.nachbarn.Count != 0))
                    {
                        var pixPos = UtmToPix(mod.ursprung);
                        foreach (streckenModul connection in mod.nachbarn)
                        {
                            framebuffer.DrawLine(pen_unselected, pixPos.X, pixPos.Y, coordToPix(connection.ursprung.WE, false), coordToPix(connection.ursprung.NS, true));
                        } 
                    }
                }
                if (drawModule_Punkte)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        var pixPos = UtmToPix(mod.ursprung);
                        int circleSize = 8; //Größe der Modulkreise
                        if ((pixPerGrad > 2) || mod.knotenpunkt || mod.selected)
                        {
                            if (fahrplan.module.Contains(mod)) //TODO: Effizienter machen
                            {
                                framebuffer.FillEllipse(Brushes.Red, pixPos.X - circleSize / 2, pixPos.Y - circleSize / 2, circleSize, circleSize);
                                framebuffer.DrawEllipse(pen_unselected, pixPos.X - circleSize / 2, pixPos.Y - circleSize / 2, circleSize, circleSize);
                            }
                            else if (mod.knotenpunkt)
                            {
                                framebuffer.FillEllipse(Brushes.DarkGray, pixPos.X - circleSize / 2, pixPos.Y - circleSize / 2, circleSize, circleSize);
                                framebuffer.DrawEllipse(pen_unselected, pixPos.X - circleSize / 2, pixPos.Y - circleSize / 2, circleSize, circleSize);
                            }
                            else //Normale Module
                            {
                                framebuffer.FillEllipse(Brushes.LightGray, pixPos.X - circleSize / 2, pixPos.Y - circleSize / 2, circleSize, circleSize);
                                framebuffer.DrawEllipse(pen_unselected, pixPos.X - circleSize / 2, pixPos.Y - circleSize / 2, circleSize, circleSize);
                            }
                        }
                    }
                }
                if (drawModule_Namen && pixPerGrad > 11)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        var pixPos = UtmToPix(mod.ursprung);
                        textManager.Add(new textField(pixPos.X, pixPos.Y, mod.name));
                    }
                }
                if (drawModule_Grenzen && pixPerGrad > 11)
                {
                    foreach (streckenModul mod in modVisible)
                    {
                        for (int i = 0; i < mod.hüllkurve.Count; i++)
                        {
                            var x1 = mod.hüllkurve[i].WE;
                            var y1 = mod.hüllkurve[i].NS;
                            var x2 = mod.hüllkurve[(i + 1) % mod.hüllkurve.Count].WE;
                            var y2 = mod.hüllkurve[(i + 1) % mod.hüllkurve.Count].NS;

                            framebuffer.DrawLine(pen_selected, coordToPix(x1, false), coordToPix(y1, true), coordToPix(x2, false), coordToPix(y2, true));
                        }
                    }
                }
            }
            if (drawStrecke)
            {
                foreach (streckenModul mod in modVisible)
                {
                    foreach (var strE in mod.elemente)
                    {
                        var pixB = UtmToPix(strE.endpunkte[0]);
                        var pixG = UtmToPix(strE.endpunkte[1]);

                        Pen pen = Pens.Black;
                        if (strE.funktion == 2)
                        {
                            pen = Pens.LightGray;
                        }
                        //if (strE.betriebstellen[0] != null || strE.betriebstellen[1] != null)
                        //{
                        //    pen = Pens.Red;
                        //    if (strE.betriebstellen[0] != null)
                        //        textManager.Add(new textField(pixB.X, pixB.Y, strE.betriebstellen[0].name));
                        //    if (strE.betriebstellen[1] != null)
                        //        textManager.Add(new textField(pixG.X, pixG.Y, strE.betriebstellen[1].name));
                        //}
                        if (strE.streckenmarkierung[0] != "" || strE.streckenmarkierung[1] != "")
                        {
                            pen = Pens.Red;
                            if (strE.streckenmarkierung[0] != "")
                                textManager.Add(new textField(pixB.X, pixB.Y, strE.streckenmarkierung[0]));
                            if (strE.streckenmarkierung[1] != "")
                                textManager.Add(new textField(pixG.X, pixG.Y, strE.streckenmarkierung[1]));
                        }

                        framebuffer.DrawLine(pen, pixB.X, pixB.Y, pixG.X, pixG.Y);
                    }

                    if (pixPerGrad > 75)
                    {
                        if (drawSignal_Start && drawSignal_Ziel)
                        {
                            foreach (var startZielSig in mod.signale)
                            {
                                drawSignal(textManager, startZielSig);
                            }
                        }
                        else
                        {
                            if (drawSignal_Start)
                            {
                                foreach (var startSig in mod.signaleStart)
                                {
                                    drawSignal(textManager, startSig);
                                }
                            }

                            if (drawSignal_Ziel)
                            {
                                foreach (var zielSig in mod.signaleZiel)
                                {
                                    drawSignal(textManager, zielSig);
                                }
                            }
                        }
                    }
                }
            }
            if (drawFahrstrassen)
            {
                foreach (streckenModul mod in modVisible)
                {
                    foreach (var fstr in mod.fahrstraßen)
                    {
                        if (fstr.zielSignal != null)
                        {
                            var start = UtmToPix(fstr.startSignal.streckenelement.endpunkte[fstr.startSignal.richtung]);
                            var ziel = UtmToPix(fstr.zielSignal.streckenelement.endpunkte[fstr.zielSignal.richtung]);
                            
                            var pen = Pens.Violet;

                            if (fstr.RglGgl == 0)
                                pen = Pens.Green; 
                            if (fstr.RglGgl == 1)
                                pen = Pens.Blue;
                            if (fstr.RglGgl == 2)
                                pen = Pens.YellowGreen;
                            if (fstr.RglGgl == 3)
                                pen = Pens.Red; 

                            framebuffer.DrawLine(pen, start.X, start.Y, ziel.X, ziel.Y); 
                        }
                        else
                            MessageBox.Show("Fahrstraße ohne Ziel: " + fstr.name);
                    }
                }
            }
            if (drawRoute && ZFBox.SelectedItem != null)
            {
                var aktZugFahrt = (ZugFahrt)ZFBox.SelectedItem;
                if (aktZugFahrt.route != null)
                {
                    foreach (var step in aktZugFahrt.route)
                    {
                        var start = UtmToPix(step.fahrstraße.startSignal.position);
                        var ziel = UtmToPix(step.fahrstraße.zielSignal.position);

                        framebuffer.DrawLine(Pens.Red, start.X, start.Y, ziel.X, ziel.Y);
                    }
                }
            }

            //textmanagement:
            int fontSize = 8;
            int lastCount = 0;
            while (!(lastCount == textManager.Count))
            {
                lastCount = textManager.Count;
                textManager = textReiniger(textManager);
            }

            foreach (var txt in textManager)
            {
                framebuffer.DrawString(txt.text, new Font("Verdana", fontSize), Brushes.Blue, txt.X + 6, txt.Y);
            }


            frameTime.Stop();
            framebuffer.DrawString("N" + center.NS.ToString("F2") + " - E" + center.WE.ToString("F2") + " - " + pixPerGrad.ToString("F1") + "pix/km - " + frameTime.ElapsedMilliseconds + " ms/frame " + message, new Font("Verdana", 10), new SolidBrush(Color.Red), 20, map_height_p - 20);

            return (frame);
        }

        private void drawSignal(List<textField> textManager, streckenModul.Signal signal)
        {

            int signalRichtung = signal.richtung;
            int antiSignalRichtung = 1 - signalRichtung;

            double p1Xcoord = signal.streckenelement.endpunkte[signalRichtung].WE;
            double p1Ycoord = signal.streckenelement.endpunkte[signalRichtung].NS;

            
            //Signaldreieck berechnen
            //Spitze
            int p1X = coordToPix(p1Xcoord, false);
            if (p1X > map_width_p || p1X < 0)
                return;
            int p1Y = coordToPix(p1Ycoord, true);
            if (p1Y > map_width_p || p1Y < 0)
                return;

            //Namen zeichnen
            string vzgString = "";
            foreach (var streckenPunkt in signal.streckenPunkte)
            {
                vzgString += streckenPunkt.strecke.ToString() + " Km " + streckenPunkt.km.ToString("f1") + "   ";
            }
            if (vzgString == "")
                vzgString += "VzG #### Km " + signal.streckenelement.kilometer.ToString("f1") + "   ";
            textManager.Add(new textField(p1X, p1Y, vzgString));

            if (drawSignal_Namen)
                textManager.Add(new textField(p1X, p1Y, signal.ToString()));

            //Vorbereitung für andere Punkte:

            //Vektor zeigt zum anderen Ende
            double VX = signal.streckenelement.endpunkte[antiSignalRichtung].WE - p1Xcoord;
            double VY = signal.streckenelement.endpunkte[antiSignalRichtung].NS - p1Ycoord;
            
            //Normieren des Vektors
            double vlen = (Math.Sqrt(VX * VX + VY * VY)) * pixPerGrad;
            double VXnorm = VX / vlen;
            double VYnorm = VY / vlen;
            //Normalvektor berechnen
            double VXnormnorm = VYnorm;
            double VYnormnorm = -VXnorm;
            //"Unterer" Punkt
            double p2Xcoord = p1Xcoord + 8 * VXnorm + 4 * VXnormnorm;
            double p2Ycoord = p1Ycoord + 8 * VYnorm + 4 * VYnormnorm;
            int p2X = coordToPix(p2Xcoord, false);
            int p2Y = coordToPix(p2Ycoord, true);
            //"Oberer" Punkt
            double p3Xcoord = p1Xcoord + 8 * VXnorm - 4 * VXnormnorm;
            double p3Ycoord = p1Ycoord + 8 * VYnorm - 4 * VYnormnorm;
            int p3X = coordToPix(p3Xcoord, false);
            int p3Y = coordToPix(p3Ycoord, true);
            //Sammeln in Array:
            var points = new Point[3];
            points[0] = new Point(p1X, p1Y);
            points[1] = new Point(p2X, p2Y);
            points[2] = new Point(p3X, p3Y);

            //Farbe aussuchen
            Brush SigCol = Brushes.Black;
            if (signal.istStart && signal.istZiel)
                SigCol = Brushes.Orange;
            else
            {
                if (signal.istStart)
                    SigCol = Brushes.Green;
                if (signal.istZiel)
                    SigCol = Brushes.Red;
            }

            framebuffer.FillPolygon(SigCol, points);//framebuffer.FillEllipse(SigCol, coordToPix(Signal.SignalCoord.abs_X, false) - circleSize / 2, coordToPix(Signal.SignalCoord.abs_Y, true) - circleSize / 2, circleSize, circleSize);
        }

        List<textField> textReiniger(List<textField> textManager)
        {
            int fontSize = 10;
            var textManaged = new List<textField>();
            foreach (var txt in textManager)
            {
                bool toBeAdded = true;
                txt.len = (int)framebuffer.MeasureString(txt.text, new Font("Verdana", fontSize)).Width;
                foreach (var txtM in textManaged)
                {
                    if ((Math.Abs(txt.Y - txtM.Y) < fontSize))
                    {
                        if (((txt.X >= txtM.X) && (txt.X <= txtM.X + txtM.len)) || ((txt.X + txt.len >= txtM.X) && (txt.X + txt.len <= txtM.X + txtM.len)))
                        {
                            //txtM.X = (txt.X + txtM.X) / 2;
                            txtM.Y = (txt.Y + txtM.Y) / 2;
                            txtM.text += " | " + txt.text;
                            txtM.len = (int)framebuffer.MeasureString(txtM.text, new Font("Verdana", fontSize)).Width;
                            toBeAdded = false;
                            break;
                        }
                    }
                }
                if (toBeAdded)
                {
                    textManaged.Add(txt);
                }
            }
            return textManaged;
        }

        /// <summary>
        /// Sucht das nächste Signal aus einer bestimmten Gruppe zu einem Punkt 
        /// </summary>
        /// <param name="punktPix">Punkt im Pixel-Koordinatensystem der Karte</param>
        /// <param name="signalAuswahl">0 = alle, 1 = start, 2 = ziel, 3 = zwischen</param>
        /// <returns></returns>
        public streckenModul.Signal findeNächstesSignal(PunktPix punktPix, int signalAuswahl)
        {
            var punktUTM = PixToUtm(punktPix);
            double kleinsteDistanz = modList[0].signale[0].streckenelement.endpunkte[modList[0].signale[0].richtung].distanceTo(punktUTM);
            var nächstesSignal = modList[0].signale[0];

            foreach (var modul in modList)
            {
                List<streckenModul.Signal> signalListe;
                switch (signalAuswahl)
                {
                    case 1:
                        signalListe = modul.signaleStart;
                        break;
                    case 2:
                        signalListe = modul.signaleZiel;
                        break;
                    case 3:
                        signalListe = modul.signaleZwischen;
                        break;
                    default:
                        signalListe = modul.signale;
                        break;
                }

                foreach (var signal in signalListe)
                {
                    var distanz = signal.streckenelement.endpunkte[signal.richtung].distanceTo(punktUTM);
                    if (distanz < kleinsteDistanz)
                    {
                        kleinsteDistanz = distanz;
                        nächstesSignal = signal;
                    }
                }
            }
            return nächstesSignal;
        }

        [Obsolete]
        public streckenModul getNearestStation(int X, int Y)
        {
            double dist = -1;
            streckenModul nearestMod = null;
            if (modVisible.Count == 0)
                return null;
            foreach (streckenModul mod in modVisible)
            {
                double modDist = getModulDistance(mod, X, Y);
                if (((dist > modDist) || (dist == -1)) && !(modDist < 0))
                {
                    dist = modDist;
                    nearestMod = mod;
                }                
            }
            return (nearestMod);
        }

        [Obsolete]
        public int getModulDistance(streckenModul mod, int X, int Y)
        {
            var pixPos = UtmToPix(mod.ursprung);

            int deltaX = pixPos.X - X;
            int deltaY = pixPos.Y - Y;

            int dist = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return dist;
        }

        //public streckenModul.Signal getNearestSignalStartZiel(int X, int Y, bool start) //TODO: Genauigkeit verbessern, Mehr Auswahl bei nahen Signalen
        //{
        //    double closestDist = -1;
        //    List<streckenModul.Signal> Ergebnis = new List<streckenModul.Signal>();

        //    foreach (streckenModul mod in modVisible)
        //    {
        //        List<streckenModulLegacy.referenzElement> Signalliste;
        //        if (start)
        //        {
        //            Signalliste = mod.StartSignale;
        //        }
        //        else
        //        {
        //            Signalliste = mod.ZielSignale;
        //        }
        //        foreach (var Signal in Signalliste)
        //        {
        //            double sigDist = getSigDistance(Signal, X, Y);
        //            if ((closestDist > sigDist) || (closestDist == -1) && !(sigDist < 0))
        //            {
        //                Ergebnis.Clear();
        //                Ergebnis.Add(Signal);
        //                closestDist = sigDist;
        //            }
        //            else if (closestDist == sigDist)
        //            {
        //                Ergebnis.Add(Signal);
        //            }
        //        }
        //    }

        //    if (Ergebnis.Count > 1)
        //    {
        //        var selectForm = new FormSignalSelect();

        //        foreach (var erg in Ergebnis)
        //        {
        //            selectForm.listBox1.Items.Add(erg);
        //        }
        //        selectForm.ShowDialog();
        //        return ((streckenModul.Signal)selectForm.listBox1.SelectedItem);
        //    }

        //    return (Ergebnis[0]);
        //}


        //public streckenModul.Signal getNearestSignal(int X, int Y)
        //{
        //    double dist = -1;
        //    streckenModulLegacy.referenzElement nearestMod = null;
        //    foreach (streckenModul mod in modVisible)
        //    {
        //        foreach (var Signal in mod.StartSignale)
        //        {
        //            double modDist = getSigDistance(Signal, X, Y);
        //            if (((dist > modDist) || (dist == -1)) && (modDist >= 0))
        //            {
        //                dist = modDist;
        //                nearestMod = Signal;
        //            }
        //        }
        //    }
        //    return (nearestMod);
        //}

        [Obsolete]
        public double getSigDistance(st3Modul.referenzElement Signal, int X, int Y)
        {
            double deltaX = 999;
            double deltaY = 999;
            try
            {
                deltaX = Signal.SignalCoord.abs_X - pixToCoord(X, false);
                deltaY = Signal.SignalCoord.abs_Y - pixToCoord(Y, true);
            }
            catch (Exception)
            {
                deltaX = Signal.StrElement.b_X - pixToCoord(X, false);
                deltaY = Signal.StrElement.b_Y - pixToCoord(Y, true);
            }
            

            double dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return dist;
        }
    }
}
