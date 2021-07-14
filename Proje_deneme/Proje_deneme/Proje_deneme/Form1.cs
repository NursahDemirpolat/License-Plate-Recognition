using AForge.Imaging;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tesseract;

namespace Proje_deneme
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "resimler |*.bmp|All Files|*.*";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            pictureBox1.ImageLocation = sfd.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox1.Image);
            Bitmap gray = grayLevel(image);
            pictureBox2.Image = gray;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox2.Image);
            Bitmap edge = Makebinary(image);
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
            Bitmap İmage = (Bitmap)pictureBox5.Image;
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 10;
            blobCounter.MinWidth = 10;
            blobCounter.MaxWidth = 350;
            blobCounter.MaxHeight = 350;
            blobCounter.ProcessImage(İmage);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Graphics g = Graphics.FromImage(İmage);
            if (rects.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("No rectangle found in image ");
            }
            else if (rects.Length > 1)
            {
                Console.WriteLine("Using largest rectangle found in image ");
                var r2 = rects.OrderByDescending(r => r.Height * r.Width).ToList();
                Rectangle objectRect = r2[1];
                using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5))
                {
                    g.DrawRectangle(pen, objectRect);
                }
                g.Dispose();
            }
            else
            {
                Console.WriteLine("Huh? on image ");
            }

            pictureBox6.Image = İmage;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap images = new Bitmap(pictureBox4.Image);
            Bitmap İmage = sobalEdgeDetection(images);
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 10;
            blobCounter.MinWidth = 10;
            blobCounter.MaxWidth = 350;
            blobCounter.MaxHeight = 350;
            blobCounter.ProcessImage(İmage);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            if (rects.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("No rectangle found in image ");
            }
            else if (rects.Length > 1)
            {
                // get largets rect
                Console.WriteLine("Using largest rectangle found in image ");
                var r2 = rects.OrderByDescending(r => r.Height * r.Width).ToList();
                İmage = İmage.Clone(r2[1], İmage.PixelFormat);
            }
            else
            {
                Console.WriteLine("Huh? on image ");
            }
            pictureBox7.Image = İmage;

        }
        private void button8_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox2.Image);
            Bitmap scr = ResmiEsiklemeYap(image);
            Bitmap sc = Select(scr);
            pictureBox8.Image = sc;
            var ocr = new TesseractEngine("./tessdata", "eng");
            var page = ocr.Process(sc);
            richTextBox1.Text = page.GetText();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox2.Image);
            Bitmap sc = ResmiEsiklemeYap(image);
            pictureBox9.Image = sc;
        }

        private Bitmap Select(Bitmap İmage)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.ProcessImage(İmage);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            if (rects.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("No rectangle found in image ");
            }
            else if (rects.Length > 1)
            {
                // get largets rect
                Console.WriteLine("Using largest rectangle found in image ");
                var r2 = rects.OrderByDescending(r => r.Height * r.Width).ToList();
                İmage = İmage.Clone(r2[3], İmage.PixelFormat);
            }
            else
            {
                Console.WriteLine("Huh? on image ");
            }
            return İmage;
        }

        public Bitmap ResmiEsiklemeYap(Bitmap GirisResmi)
        {
            Color OkunanRenk, DonusenRenk;
            int R = 0, G = 0, B = 0;
            int ResimGenisligi = GirisResmi.Width; //GirisResmi global tanımlandı.
            int ResimYuksekligi = GirisResmi.Height;
            Bitmap CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi); //Cikis resmini oluşturuyor. Boyutları giriş resmi ile aynı olur.
            int i = 0, j = 0; //Çıkış resminin x ve y si olacak.
            for (int x = 0; x < ResimGenisligi; x++)
            {
                j = 0;
                for (int y = 0; y < ResimYuksekligi; y++)
                {
                    OkunanRenk = GirisResmi.GetPixel(x, y);
                    if (OkunanRenk.R >= 128)
                        R = 255;
                    else
                        R = 0;
                    if (OkunanRenk.G >= 128)
                        G = 255;
                    else
                        G = 0;
                    if (OkunanRenk.B >= 128)
                        B = 255;
                    else
                        B = 0;
                    DonusenRenk = Color.FromArgb(R, G, B);
                    CikisResmi.SetPixel(i, j, DonusenRenk);
                    j++;
                }
                i++;
            }
            return CikisResmi;
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

    }
}

