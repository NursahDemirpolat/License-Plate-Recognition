using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace Proje_deneme
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private Rectangle alan;
        Bitmap cropcloneBmp;
        List<Point> Points = new List<Point>();
        Bitmap newImage;
        private object scdLocal;
        List<IntPoint> cornerpoints;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "resimler |*.bmp|All Files|*.*";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            pictureBox1.ImageLocation = sfd.FileName;
        }

        // Gri yapma
        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap gray = grayLevel(image);
            pictureBox2.Image = gray;
        }


        // Binary
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap binary = new Bitmap(pictureBox2.Image);
            Bitmap edge = Makebinary(binary);
            pictureBox3.Image = edge;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox3.Image);
            Bitmap median = medianAlgorithm(image);
            pictureBox4.Image = median;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox4.Image);
            Bitmap sobel = sobalEdgeDetection(image);
            pictureBox5.Image = sobel;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap image = (Bitmap)pictureBox5.Image;
            // create instance of blob counter
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.MaxWidth = 350;
            blobCounter.MaxHeight = 350;
            //process the dilated image
            blobCounter.ProcessImage(image);
            // get info about detected objects
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // create a graphics object to draw on the image and a pen
            Graphics g = Graphics.FromImage(image);
            Pen Redpen = new Pen(Color.Red, 4);

            SimpleShapeChecker shapechecker = new SimpleShapeChecker();

            //check in image and draw around the object found as rectangle
            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgepoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                List<IntPoint> cornerpoints;
                Bitmap bmpCrop;
                List<Point> Points = new List<Point>();
                if (shapechecker.IsQuadrilateral(edgepoints, out cornerpoints))
                {
                    if (shapechecker.CheckPolygonSubType(cornerpoints) == PolygonSubType.Rectangle)
                    {
                        g.DrawPolygon(Redpen, cornerpoints.Select(p => new System.Drawing.Point(p.X, p.Y)).ToArray());
                    }
                }
            }

            Redpen.Dispose();
            g.Dispose();

            pictureBox6.Image = image;
        }

        private Point[] ToPointsArray(List<IntPoint> points)
        {
            return points.Select(p => new System.Drawing.Point(p.X, p.Y)).ToArray();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap image = (Bitmap)pictureBox6.Image;

            // create instance of blob counter

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.MaxWidth = 350;
            blobCounter.MaxHeight = 350;
            //process the dilated image
            blobCounter.ProcessImage(image);
            // get info about detected objects
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // create a graphics object to draw on the image and a pen
            Graphics g = Graphics.FromImage(image);
            Pen Redpen = new Pen(Color.Red, 4);

            SimpleShapeChecker shapechecker = new SimpleShapeChecker();

            //check in image and draw around the object found as rectangle
            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgepoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                

                List<Point> Points = new List<Point>();
                if (shapechecker.IsQuadrilateral(edgepoints, out cornerpoints))
                {
                    if (shapechecker.CheckPolygonSubType(cornerpoints) == PolygonSubType.Rectangle)
                    {
                        g.DrawPolygon(Redpen, cornerpoints.Select(p => new System.Drawing.Point(p.X, p.Y)).ToArray());
                    }
                }foreach (var point in cornerpoints)
            {

                Crop filter = new Crop(new Rectangle(point.X, point.Y, image.Width, image.Height));
                newImage = filter.Apply(image);
            }
                
            }
            

            Redpen.Dispose();
            g.Dispose();


            pictureBox7.Image = newImage;

        }

        // Gray Level 
        private Bitmap grayLevel(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Height - 1; i++)
            {
                for (int j = 0; j < bmp.Width - 1; j++)
                {
                    int value = (bmp.GetPixel(j, i).R + bmp.GetPixel(j, i).G + bmp.GetPixel(j, i).B) / 3;

                    Color renk;
                    renk = Color.FromArgb(value, value, value); //yeni oluşan renkler gri

                    bmp.SetPixel(j, i, renk); //oluşan gri rengini resme dönüştürdük
                }
            }
            return bmp;
        }


        //Binary
        private Bitmap Makebinary(Bitmap image)
        {
            try
            {
                grayLevel(image);

                int temp;
                int level = 127;
                Color color;

                for (int i = 0; i < image.Height - 1; i++)
                {
                    for (int j = 0; j < image.Width - 1; j++)
                    {
                        temp = image.GetPixel(j, i).B;
                        if (temp < level)
                        {
                            color = Color.FromArgb(0, 0, 0);
                            image.SetPixel(j, i, color);
                        }
                        else
                        {
                            color = Color.FromArgb(255, 255, 255);
                            image.SetPixel(j, i, color);
                        }
                    }

                }
                return image;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Image Operations");
                return null;
            }
        }


        // Median Algoritması

        private Bitmap medianAlgorithm(Bitmap image)
        {
            Bitmap buffer = new Bitmap(image.Width, image.Height);
            Color color;

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    if ((i == 0) || (i == image.Height - 1) || (j == 0) || (j == image.Width - 1))
                        continue;
                    else
                    {
                        int ortanca = medianFind(image, j, i);
                        color = Color.FromArgb(ortanca, ortanca, ortanca);
                        buffer.SetPixel(j, i, color);
                    }
                }
            }

            return buffer;
        }
        private int medianFind(Bitmap image, int j, int i)
        {
            int[] dizi = new int[9];
            Color color;

            int sagkomsu, sagustcaprazkomsu, ustkomsu, solustcapraz, solkomsu, solaltcapraz, altkomsu, sagaltcapraz;

            sagkomsu = image.GetPixel(j + 1, i).R;
            sagustcaprazkomsu = image.GetPixel(j + 1, i - 1).R;
            ustkomsu = image.GetPixel(j, i - 1).R;
            solustcapraz = image.GetPixel(j + 1, i - 1).R;
            solkomsu = image.GetPixel(j - 1, i).R;
            solaltcapraz = image.GetPixel(j - 1, i + 1).R;
            altkomsu = image.GetPixel(j, i + 1).R;
            sagaltcapraz = image.GetPixel(j + 1, i + 1).R;

            dizi[0] = image.GetPixel(j, i).R;
            dizi[1] = sagkomsu;
            dizi[2] = sagustcaprazkomsu;
            dizi[3] = ustkomsu;
            dizi[4] = solustcapraz;
            dizi[5] = solkomsu;
            dizi[6] = solaltcapraz;
            dizi[7] = altkomsu;
            dizi[8] = sagaltcapraz;

            for (int x = 0; x < 8; x++)
            {
                for (int y = x + 1; y < 9; y++)
                {
                    if (dizi[x] < dizi[y])
                        continue;
                    else
                    {
                        int temp = dizi[y];
                        dizi[y] = dizi[x];
                        dizi[x] = temp;
                    }
                }

            }


            return dizi[4];

        }

        // Edge Algorithm
        private Bitmap sobalEdgeDetection(Bitmap image)
        {
            Bitmap buffer = new Bitmap(image.Width, image.Height);

            Color renk;
            int valX, valY;
            int gradient;

            int[,] GX = new int[3, 3];
            int[,] GY = new int[3, 3];


            // Yatay yöndeki kenar için
            GX[0, 0] = -1; GX[0, 1] = 0; GX[0, 2] = 1;
            GX[1, 0] = -2; GX[1, 1] = 0; GX[1, 2] = 2;
            GX[2, 0] = -1; GX[2, 1] = 0; GX[2, 2] = 1;

            // Dikey yöndeki kenar için
            GY[0, 0] = -1; GY[0, 1] = -2; GY[0, 2] = -1;
            GY[1, 0] = 0; GY[1, 1] = 0; GY[1, 2] = 0;
            GY[2, 0] = 1; GY[2, 1] = 2; GY[2, 2] = 1;


            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    if (i == 0 || i == image.Height - 1 || j == 0 || j == image.Width - 1)
                    {
                        renk = Color.FromArgb(255, 255, 255);
                        buffer.SetPixel(j, i, renk);

                        valX = 0;
                        valY = 0;
                    }
                    else
                    {
                        valX = image.GetPixel(j - 1, i - 1).R * GX[0, 0] +
                            image.GetPixel(j, i - 1).R * GX[0, 1] +
                            image.GetPixel(j + 1, i - 1).R * GX[0, 2] +
                            image.GetPixel(j - 1, i).R * GX[1, 0] +
                            image.GetPixel(j, i).R * GX[1, 1] +
                            image.GetPixel(j + 1, i).R * GX[1, 2] +
                            image.GetPixel(j - 1, i + 1).R * GX[2, 0] +
                            image.GetPixel(j, i + 1).R * GX[2, 1] +
                            image.GetPixel(j + 1, i + 1).R * GX[2, 2];

                        valY = image.GetPixel(j - 1, i - 1).R * GY[0, 0] +
                           image.GetPixel(j, i - 1).R * GY[0, 1] +
                           image.GetPixel(j + 1, i - 1).R * GY[0, 2] +
                           image.GetPixel(j - 1, i).R * GY[1, 0] +
                           image.GetPixel(j, i).R * GY[1, 1] +
                           image.GetPixel(j + 1, i).R * GY[1, 2] +
                           image.GetPixel(j - 1, i + 1).R * GY[2, 0] +
                           image.GetPixel(j, i + 1).R * GY[2, 1] +
                           image.GetPixel(j + 1, i + 1).R * GY[2, 2];

                        gradient = (int)(Math.Abs(valX) + Math.Abs(valY));

                        if (gradient < 0)
                            gradient = 0;
                        if (gradient > 255)
                            gradient = 255;

                        renk = Color.FromArgb(gradient, gradient, gradient);
                        buffer.SetPixel(j, i, renk);

                    }
                }
            }

            return buffer;
        }


        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }



        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }
    }
}

