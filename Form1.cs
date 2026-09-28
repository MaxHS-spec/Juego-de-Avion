// ************************************************//
// NOMBRE PROYECTO: JUEGO DE AVIONES BASICO        //
// AUTOR: ING. JUAN CARLOS PINTO L.               //
// INSTITUCION: UNAJ                              //
// FECHA DE CREACION: 01/07/2024                  //
// VERSION : 2.0.0.0                              //
// ************************************************//
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Juego_Basic
{
    public partial class Form1 : Form
    {
        //*********** VARIABLES GLOBALES ***********//
        PictureBox naveX = new PictureBox();
        PictureBox naveRival = new PictureBox();
        PictureBox contiene = new PictureBox();
        System.Windows.Forms.Timer tiempo;
        int Dispara = 0;
        bool flag = false;
        float angulo = 0;

        // *** Object Pooling (Reto 2) ***
        PictureBox[] poolMisiles = new PictureBox[3];
        int misilesActivos = 0;
        DateTime ultimoDisparo = DateTime.MinValue;

        //*********** DIAGRAMAR DEL MISIL ***********//
        public void CrearMisil(int AngRotar, Color pintar, string nombre, int x, int y)
        {
            dynamic Balas = new PictureBox();
            int PosX = 1;
            int PosY = 1;
            int largoM = 11;
            int anchoM = 8;
            Point[] myMisil1 = { new Point(4 * PosX, 0 * PosY), new Point(5 * PosX, 1 * PosY), new Point(6 * PosX, 2 * PosY), new Point(6 * PosX, 7 * PosY), new Point(7 * PosX, 8 * PosY), new Point(8 * PosX, 9 * PosY), new Point(7 * PosX, 9 * PosY), new Point(6 * PosX, 10 * PosY), new Point(2 * PosX, 10 * PosY), new Point(1 * PosX, 9 * PosY), new Point(0 * PosX, 9 * PosY), new Point(1 * PosX, 8 * PosY), new Point(2 * PosX, 7 * PosY), new Point(2 * PosX, 2 * PosY), new Point(3 * PosX, 1 * PosY), new Point(4 * PosX, 0 * PosY) };
            Point[] myMisil = new Point[myMisil1.Length];
            for (int i = 0; i < myMisil1.Length; i++)
            {
                myMisil[i].X = myMisil1[i].X;
                if (AngRotar == 180)
                    myMisil[i].Y = largoM - myMisil1[i].Y;
                else
                    myMisil[i].Y = myMisil1[i].Y;
            }
            GraphicsPath ObjGrafico = new GraphicsPath();
            ObjGrafico.AddPolygon(myMisil);
            Balas.Location = new Point(x, y);
            Balas.BackColor = pintar;
            Balas.Size = new Size(anchoM * PosX, largoM * PosY);
            Balas.Region = new Region(ObjGrafico);
            contiene.Controls.Add(Balas);
            Balas.Visible = true;
            Balas.Tag = nombre;
            Bitmap flag = new Bitmap(anchoM, largoM);
            Graphics flagImagen = Graphics.FromImage(flag);
            flagImagen.FillRectangle(Brushes.Orange, 2, 8, 5, 1);
            flagImagen.FillRectangle(Brushes.Yellow, 3, 10, 3, 1);
            Balas.Image = flag;
        }

        //*********** DESTRUCTOR DEL MISIL ***********//
        private void ImpactarTick(object sender, EventArgs e)
        {
            int X = naveRival.Location.X;
            int Y = naveRival.Location.Y;
            int W = naveRival.Width;
            int H = naveRival.Height;
            int PH = 6;
            int X2 = naveX.Location.X;
            int Y2 = naveX.Location.Y;
            int W2 = naveX.Width;
            int H2 = naveX.Height;
            int x = naveRival.Location.X;
            int y = naveRival.Location.Y;
            Dispara++;

            // ACCION DE DISPARAR DEL RIVAL
            if (Dispara == 100 && naveRival.Visible == true)
            {
                int xRival = naveRival.Location.X + (naveRival.Width / 2);
                int yRival = naveRival.Location.Y + (naveRival.Height / 2);
                CrearMisil(180, Color.DarkRed, "Rival", xRival, yRival);
                Dispara = 0;
            }

            // MOVIMIENTO DE LA NAVE RIVAL
            if (flag == false)
            {
                if (contiene.Width == x + naveRival.Width)
                    flag = true;
                x++;
            }
            else
            {
                if (contiene.Location.X == x)
                    flag = false;
                x--;
            }
            naveRival.Location = new Point(x, y);
            naveRival.BorderStyle = BorderStyle.FixedSingle;

            // Actualizar contador real de misiles activos del pool
            misilesActivos = 0;
            for (int i = 0; i < 3; i++)
            {
                if (poolMisiles[i] != null && poolMisiles[i].Visible)
                    misilesActivos++;
            }

            foreach (Control c in contiene.Controls)
            {
                if (c is PictureBox)
                {
                    int X1 = ((PictureBox)c).Location.X;
                    int Y1 = ((PictureBox)c).Location.Y;
                    int W1 = ((PictureBox)c).Width;
                    int H1 = ((PictureBox)c).Height;
                    string nombre = ((PictureBox)c).Tag != null ? ((PictureBox)c).Tag.ToString() : "";

                    // IMPACTO CON LA NAVE RIVAL
                    if (X < X1 && X1 + W1 < X + W && Y < Y1 && Y1 + H1 < Y + H && nombre == "Misil")
                    {
                        // Reciclar misil
                        c.Visible = false;
                        c.Location = new Point(-200, -200);

                        if (X + PH < X1 && X1 + W1 < X + W - PH)
                            naveRival.Tag = int.Parse(naveRival.Tag.ToString()) - 10;
                        else
                            naveRival.Tag = int.Parse(naveRival.Tag.ToString()) - 1;

                        toolStripStatusLabel1.Text = "Vida del Rival : " + naveRival.Tag.ToString();
                    }
                    else if (int.Parse(naveRival.Tag.ToString()) <= 0)
                    {
                        naveRival.Dispose();
                        Bitmap NuevoImg = new Bitmap(contiene.Width, contiene.Height);
                        Graphics flagImagen = Graphics.FromImage(NuevoImg);
                        String drawString = "Felicitaciones Ganaste !";
                        Font drawFont = new Font("Arial", 16);
                        SolidBrush drawBrush = new SolidBrush(Color.Blue);
                        PointF drawPoint = new PointF(40, 150);
                        flagImagen.DrawString(drawString, drawFont, drawBrush, drawPoint);
                        contiene.Image = NuevoImg;
                        tiempo.Stop();
                        System.IO.File.WriteAllText("resultado.txt", "Ganaste");
                    }

                    // IMPACTO CON MI NAVE
                    if (X2 < X1 && X1 + W1 < X2 + W2 && Y2 < Y1 && Y1 + H1 < Y2 + H2 && nombre == "Rival")
                    {
                        if (X2 + PH < X1 && X1 + W1 < X2 + W2 - PH)
                        {
                            ((PictureBox)c).Dispose();
                            naveX.Tag = int.Parse(naveX.Tag.ToString()) - 10;
                        }
                        else
                        {
                            ((PictureBox)c).Dispose();
                            naveX.Tag = int.Parse(naveX.Tag.ToString()) - 1;
                        }
                        toolStripStatusLabel2.Text = "Mi Nave : " + naveX.Tag.ToString();
                    }
                    else if (int.Parse(naveX.Tag.ToString()) <= 0)
                    {
                        naveX.Dispose();
                        Bitmap NuevoImg = new Bitmap(contiene.Width, contiene.Height);
                        Graphics flagImagen = Graphics.FromImage(NuevoImg);
                        String drawString = "Perdiste el Juego";
                        Font drawFont = new Font("Arial", 16);
                        SolidBrush drawBrush = new SolidBrush(Color.Red);
                        PointF drawPoint = new PointF(70, 150);
                        flagImagen.DrawString(drawString, drawFont, drawBrush, drawPoint);
                        contiene.Image = NuevoImg;
                        tiempo.Stop();
                        System.IO.File.WriteAllText("resultado.txt", "Perdiste");
                    }

                    // Misil del jugador sale del cuadro → reciclar
                    if (nombre == "Misil" && c.Visible && ((PictureBox)c).Location.Y <= 0)
                    {
                        c.Visible = false;
                        c.Location = new Point(-200, -200);
                    }

                    // Misil del rival sale por abajo
                    if (nombre == "Rival" && ((PictureBox)c).Location.Y >= contiene.Height)
                    {
                        ((PictureBox)c).Dispose();
                    }

                    // Movimiento lento del misil del jugador
                    if (nombre == "Misil" && c.Visible)
                    {
                        ((PictureBox)c).Top -= 3;
                    }
                    if (nombre == "Rival")
                    {
                        ((PictureBox)c).Top += 6;
                    }

                    if (X >= X2 && H >= Y2 && W2 >= X && H2 >= Y)
                    {
                        naveRival.Dispose();
                        naveX.Dispose();
                    }
                }
                else
                {
                    tiempo.Stop();
                }
            }
        }

        //*********** DIAGRAMAR NAVE ***********//
        public void CrearNave(PictureBox Avion, int AngRotar, int Tipox, Color Pintar, int Vida)
        {
            int largoN = 0;
            int anchoN = 0;
            Point[] myNave1 = { new Point(29, 0), new Point(30, 1), new Point(30, 6), new Point(31, 6), new Point(31, 11), new Point(32, 11), new Point(32, 17), new Point(35, 17), new Point(35, 18), new Point(37, 18), new Point(37, 19), new Point(38, 19), new Point(38, 20), new Point(42, 39), new Point(41, 45), new Point(50, 51), new Point(51, 51), new Point(61, 52), new Point(56, 69), new Point(50, 66), new Point(44, 66), new Point(19, 71), new Point(35, 71), new Point(35, 74), new Point(32, 77), new Point(26, 77), new Point(23, 74), new Point(23, 71), new Point(19, 71), new Point(14, 66), new Point(0, 66), new Point(0, 59), new Point(7, 52), new Point(7, 51), new Point(4, 51), new Point(14, 45), new Point(16, 39), new Point(19, 20), new Point(20, 20), new Point(20, 19), new Point(21, 19), new Point(21, 18), new Point(23, 18), new Point(23, 17), new Point(26, 17), new Point(26, 11), new Point(27, 11), new Point(27, 6), new Point(28, 6), new Point(28, 1), new Point(29, 0) };
            Point[] myNave2 = { new Point(24, 0), new Point(29, 5), new Point(29, 18), new Point(32, 21), new Point(34, 21), new Point(38, 17), new Point(41, 28), new Point(41, 30), new Point(47, 36), new Point(47, 41), new Point(41, 41), new Point(38, 44), new Point(36, 44), new Point(33, 41), new Point(38, 41), new Point(25, 46), new Point(22, 46), new Point(17, 41), new Point(14, 41), new Point(11, 44), new Point(9, 44), new Point(6, 41), new Point(0, 41), new Point(0, 36), new Point(6, 30), new Point(6, 28), new Point(9, 17), new Point(13, 21), new Point(15, 21), new Point(18, 18), new Point(18, 5), new Point(23, 0) };
            Point[] myNave3 = { new Point(25, 54), new Point(26, 54), new Point(26, 50), new Point(26, 50), new Point(27, 50), new Point(28, 50), new Point(29, 50), new Point(30, 51), new Point(31, 51), new Point(32, 52), new Point(32, 49), new Point(31, 48), new Point(30, 47), new Point(29, 46), new Point(28, 45), new Point(27, 44), new Point(27, 36), new Point(28, 35), new Point(28, 25), new Point(29, 25), new Point(30, 25), new Point(31, 25), new Point(32, 26), new Point(33, 26), new Point(34, 27), new Point(35, 28), new Point(36, 28), new Point(37, 29), new Point(38, 30), new Point(39, 30), new Point(40, 31), new Point(41, 32), new Point(42, 32), new Point(43, 33), new Point(44, 34), new Point(45, 35), new Point(46, 36), new Point(47, 36), new Point(48, 36), new Point(49, 37), new Point(50, 37), new Point(51, 38), new Point(51, 37), new Point(51, 36), new Point(51, 35), new Point(50, 35), new Point(37, 22), new Point(37, 15), new Point(36, 14), new Point(35, 14), new Point(34, 15), new Point(34, 21), new Point(28, 15), new Point(28, 7), new Point(27, 6), new Point(26, 5), new Point(25, 5), new Point(24, 6), new Point(23, 7), new Point(23, 15), new Point(17, 21), new Point(17, 15), new Point(16, 14), new Point(15, 14), new Point(15, 15), new Point(14, 22), new Point(1, 35), new Point(0, 35), new Point(0, 36), new Point(0, 37), new Point(0, 38), new Point(1, 37), new Point(2, 37), new Point(3, 36), new Point(4, 36), new Point(5, 35), new Point(6, 35), new Point(7, 34), new Point(8, 33), new Point(9, 32), new Point(10, 32), new Point(11, 31), new Point(12, 30), new Point(13, 30), new Point(14, 29), new Point(15, 28), new Point(16, 28), new Point(17, 27), new Point(18, 26), new Point(19, 26), new Point(20, 25), new Point(21, 25), new Point(22, 25), new Point(23, 25), new Point(24, 36), new Point(24, 44), new Point(23, 45), new Point(22, 46), new Point(21, 47), new Point(20, 48), new Point(19, 49), new Point(18, 50), new Point(18, 51), new Point(21, 51), new Point(22, 50), new Point(23, 50), new Point(24, 50), new Point(25, 54) };
            Point[] myNave;
            GraphicsPath ObjGrafico = new GraphicsPath();
            if (Tipox == 1)
            {
                largoN = 77;
                anchoN = 60;
                myNave = new Point[myNave1.Length];
                for (int i = 0; i < myNave1.Length; i++)
                {
                    myNave[i].X = myNave1[i].X;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave1[i].Y;
                    else
                        myNave[i].Y = myNave1[i].Y;
                }
                ObjGrafico.AddPolygon(myNave);
            }
            else if (Tipox == 2)
            {
                largoN = 46 * 1;
                anchoN = 47 * 1;
                myNave = new Point[myNave2.Length];
                for (int i = 0; i < myNave2.Length; i++)
                {
                    myNave[i].X = myNave2[i].X * 1;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave2[i].Y * 1;
                    else
                        myNave[i].Y = myNave2[i].Y * 1;
                }
                ObjGrafico.AddLines(myNave);
            }
            else if (Tipox == 3)
            {
                largoN = 54;
                anchoN = 51;
                myNave = new Point[myNave3.Length];
                for (int i = 0; i < myNave3.Length; i++)
                {
                    myNave[i].X = myNave3[i].X;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave3[i].Y;
                    else
                        myNave[i].Y = myNave3[i].Y;
                }
                ObjGrafico.AddPolygon(myNave);
            }
            Avion.BackColor = Pintar;
            Avion.Size = new Size(anchoN, largoN);
            Avion.Region = new Region(ObjGrafico);
            Avion.Location = new Point(0, 0);
            contiene.Controls.Add(Avion);
            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            Point[] Colorea = { new Point(24, 2), new Point(27, 5), new Point(27, 18), new Point(31, 22), new Point(34, 22), new Point(37, 19), new Point(38, 19), new Point(39, 28), new Point(39, 30), new Point(45, 36), new Point(45, 39), new Point(41, 39), new Point(38, 42), new Point(35, 42), new Point(32, 39), new Point(38, 39), new Point(25, 44), new Point(21, 44), new Point(16, 39), new Point(14, 39), new Point(11, 42), new Point(8, 42), new Point(9, 19), new Point(12, 22), new Point(1, 39), new Point(1, 36), new Point(7, 30), new Point(7, 28), new Point(8, 19), new Point(9, 19), new Point(12, 22), new Point(15, 22), new Point(19, 18), new Point(19, 5), new Point(22, 2) };
            Point[] poix = new Point[myNave2.Length];
            for (int i = 0; i < myNave2.Length; i++)
            {
                poix[i].X = myNave2[i].X;
                poix[i].Y = myNave2[i].Y;
            }
            PintaImg.DrawPolygon(Pens.Black, ObjGrafico.PathData.Points);
            Avion.Image = Imagen;
            Avion.Tag = Vida;
            Avion.Visible = true;
        }

        //*********** EFECTOS DE LA NAVE PRINCIPAL ***********//
        public void NaveCorre(PictureBox Avion, int AngRotar, int velox)
        {
            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            Point[] puntoDer = { new Point(35, 28), new Point(35, 30), new Point(36, 30), new Point(37, 31), new Point(37, 37), new Point(38, 38), new Point(38, 40), new Point(39, 41), new Point(39, 44), new Point(40, 45), new Point(48, 46), new Point(42, 48), new Point(43, 48), new Point(44, 49), new Point(40, 64), new Point(43, 65), new Point(42, 65), new Point(41, 66), new Point(40, 66), new Point(38, 68), new Point(36, 68), new Point(36, 69), new Point(36, 63), new Point(35, 62), new Point(35, 28) };
            Point[] puntoIzq = { new Point(23, 28), new Point(23, 30), new Point(22, 30), new Point(21, 31), new Point(21, 37), new Point(20, 38), new Point(20, 40), new Point(19, 41), new Point(19, 44), new Point(18, 45), new Point(18, 46), new Point(16, 48), new Point(15, 48), new Point(14, 49), new Point(14, 64), new Point(15, 65), new Point(16, 65), new Point(17, 66), new Point(18, 66), new Point(20, 68), new Point(22, 68), new Point(22, 69), new Point(22, 63), new Point(23, 62), new Point(23, 28) };
            Point[] puntoAtr = { new Point(29, 21), new Point(31, 19), new Point(32, 19), new Point(33, 20), new Point(33, 25), new Point(32, 26), new Point(32, 63), new Point(34, 65), new Point(34, 68), new Point(33, 69), new Point(33, 71), new Point(32, 73), new Point(31, 73), new Point(29, 71), new Point(27, 73), new Point(26, 73), new Point(25, 74), new Point(25, 69), new Point(24, 68), new Point(24, 65), new Point(26, 63), new Point(26, 26), new Point(25, 25), new Point(25, 20), new Point(26, 19), new Point(27, 19), new Point(29, 21) };
            PintaImg.FillPolygon(Brushes.DarkGreen, puntoDer);
            PintaImg.FillPolygon(Brushes.DarkGreen, puntoIzq);
            PintaImg.FillPolygon(Brushes.DarkGreen, puntoAtr);
            PintaImg.FillRectangle(Brushes.Silver, 25, 35, 1, 15);
            PintaImg.FillRectangle(Brushes.Silver, 32, 35, 1, 15);
            PintaImg.FillRectangle(Brushes.Silver, 29, 58, 1, 13);
            if (velox == 1)
            {
                PintaImg.FillRectangle(Brushes.DarkOrange, 35, 68, 6, 11);
                PintaImg.FillRectangle(Brushes.Orange, 36, 69, 4, 1);
                PintaImg.FillRectangle(Brushes.Yellow, 37, 70, 2, 1);
                PintaImg.FillRectangle(Brushes.DarkOrange, 17, 68, 6, 1);
                PintaImg.FillRectangle(Brushes.Orange, 18, 69, 4, 1);
                PintaImg.FillRectangle(Brushes.Yellow, 19, 70, 2, 1);
                RotateImage(naveX.Image, angulo);
            }
            else if (velox == 2)
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 15, 30, 1, 8);
                PintaImg.FillRectangle(Brushes.DarkRed, 25, 28, 1, 16);
                PintaImg.FillRectangle(Brushes.DarkRed, 35, 30, 1, 8);
            }
            else if (velox == 3)
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 15, 30, 1, 8);
                PintaImg.FillRectangle(Brushes.DarkRed, 25, 28, 1, 16);
                PintaImg.FillRectangle(Brushes.DarkRed, 35, 30, 1, 8);
            }
            Avion.Image = RotateImage(Imagen, AngRotar);
        }

        //*************** CREAR ANGULO DE ROTACION ***************//
        public static Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            return bmp;
        }

        //*********** MOVIMIENTO DEL TECLADO DEL USUARIO ***********//
        public void ActividadTecla(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 37: // Flecha izquierda
                    {
                        if (contiene.Left < naveX.Left)
                            naveX.Left -= 18;
                        angulo = -15;
                        NaveCorre(naveX, 1, 0);
                        break;
                    }
                case 38: // Flecha arriba
                    {
                        if (contiene.Top < naveX.Top)
                            naveX.Top -= 10;
                        NaveCorre(naveX, 0, 1);
                        break;
                    }
                case 39: // Flecha derecha
                    {
                        if (contiene.Right > naveX.Right)
                            naveX.Left += 10;
                        angulo = 15;
                        NaveCorre(naveX, 1, 0);
                        break;
                    }
                case 40: // Flecha abajo
                    {
                        if (contiene.Bottom > naveX.Bottom)
                            naveX.Top += 10;
                        NaveCorre(naveX, 0, 1);
                        break;
                    }
                case 13: // Tecla Enter (Disparo) - Object Pooling ESTRICTO
                    {
                        tiempo.Start();

                        // 1) Contar cuántos misiles del pool están visibles ahora
                        int activos = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (poolMisiles[i] != null && poolMisiles[i].Visible)
                                activos++;
                        }

                        // 2) Si ya hay 3 activos → NO DISPARA (espera a que desaparezcan)
                        if (activos >= 3)
                            return;

                        // 3) Cooldown lento entre disparos (600 ms)
                        if ((DateTime.Now - ultimoDisparo).TotalMilliseconds < 600)
                            return;

                        // 4) Activar SOLO UN misil libre
                        for (int i = 0; i < 3; i++)
                        {
                            if (poolMisiles[i] != null && !poolMisiles[i].Visible)
                            {
                                int x = naveX.Location.X + (naveX.Width / 2) - 4;
                                int y = naveX.Location.Y;
                                poolMisiles[i].Location = new Point(x, y);
                                poolMisiles[i].Visible = true;
                                ultimoDisparo = DateTime.Now;
                                break;   // solo uno por tecla
                            }
                        }
                        break;
                    }
            }
        }


        //************ ACTIVAR ACCIONES DE INICIALIZACION ************//
        public void Iniciar()
        {
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.Width = 400;
            this.Height = 600;
            this.Text = "JUEGO DE AVIONES BASICO";
            toolStripStatusLabel1.Text = "Mi Rival";
            toolStripStatusLabel2.Text = "Mi Avion";
            this.KeyDown += new KeyEventHandler(ActividadTecla);

            contiene.Location = new Point(0, 0);
            contiene.BackColor = Color.AliceBlue;
            contiene.Size = new Size(600, 600);
            contiene.Dock = DockStyle.Fill;
            Controls.Add(contiene);
            contiene.Visible = true;

            Random r = new Random();
            int aleaty = r.Next(250, 310);
            int aleatx = r.Next(50, 250);
            CrearNave(naveX, 0, 1, Color.SeaGreen, 20);

            Random sal = new Random();
            int sale = sal.Next(1, 3);
            CrearNave(naveRival, 180, sale, Color.DarkBlue, 50);
            naveX.Location = new Point(aleatx, aleaty);

            tiempo = new System.Windows.Forms.Timer();
            tiempo.Interval = 25;          // más lento
            tiempo.Enabled = true;
            tiempo.Tick += new EventHandler(ImpactarTick);

            // *** Object Pooling: crear EXACTAMENTE 3 misiles ***
            for (int i = 0; i < 3; i++)
            {
                CrearMisil(0, Color.DarkMagenta, "Misil", -800, -800);
            }

            // Guardar referencias de los 3 misiles del pool
            int idx = 0;
            foreach (Control c in contiene.Controls)
            {
                if (c is PictureBox && c.Tag != null && c.Tag.ToString() == "Misil")
                {
                    poolMisiles[idx] = (PictureBox)c;
                    poolMisiles[idx].Visible = false;
                    poolMisiles[idx].Location = new Point(-200, -200);
                    idx++;
                    if (idx >= 3) break;
                }
            }

            this.KeyPreview = true;
        }

        public Form1()
        {
            InitializeComponent();
            Iniciar();
        }
    }
}
