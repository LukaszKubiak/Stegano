using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Steganografia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap bmp;
        Bitmap bmp2;
        string enc = "";
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bmp = new Bitmap(Bitmap.FromFile(openFileDialog1.FileName));
                bmp = new Bitmap(bmp, pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = bmp;
            }
        }
        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (bmp != null)
            {
                bmp2 = bmp;
                pictureBox2.Image = bmp2;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bmp2 = new Bitmap(Bitmap.FromFile(openFileDialog1.FileName));
                bmp2 = new Bitmap(bmp2, pictureBox2.Width, pictureBox2.Height);
                pictureBox2.Image = bmp2;
            }
        }
        void encode(int[] to_encode)
        {
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width - 2; j++)
                {
                    if (j + i * Width >= to_encode.Length)
                    {
                        Color pixel1 = bmp.GetPixel(j, i);
                        Color pixel3 = Color.FromArgb(pixel1.R - (pixel1.R % 8), pixel1.G - (pixel1.G % 8), pixel1.B - (pixel1.B % 8));
                        bmp.SetPixel(j, i, pixel3);
                        pixel1 = bmp.GetPixel(j + 1, i);
                        pixel3 = Color.FromArgb(pixel1.R - (pixel1.R % 8), pixel1.G - (pixel1.G % 8), pixel1.B - (pixel1.B % 4));
                        bmp.SetPixel(j + 1, i, pixel3);
                        break;
                    }
                    int B = to_encode[j + i * Width] % 4;
                    int G = ((to_encode[j + i * Width] % 32) - B) >> 2;
                    int R = (to_encode[j + i * Width] - B - G) >> 5;
                    Color pixel = bmp.GetPixel(j, i);
                    Color pixel2 = Color.FromArgb(pixel.R - (pixel.R % 8) + R, pixel.G - (pixel.G % 8) + G, pixel.B - (pixel.B % 4) + B);
                    int t = R * 32 + G * 4 + B;
                    bmp.SetPixel(j, i, pixel2);

                }

            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (bmp != null)
            {
                if (enc == "")
                {
                    int[] to_encode = Encoding.ASCII.GetBytes(textBox1.Text).Select(x => (int)x).ToArray();
                    encode(to_encode);
                    pictureBox1.Refresh();
                }
                else
                {
                    int[] to_encode = Encoding.ASCII.GetBytes(enc).Select(x => (int)x).ToArray();
                    encode(to_encode);
                    pictureBox1.Refresh();
                }
                

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (bmp2 != null)
            {
                string Encoded = "";

                for (int i = 0; i < bmp2.Height; i++)
                {
                    for (int j = 0; j < bmp2.Width - 2; j++)
                    {
                        Color pixel = bmp2.GetPixel(j, i);
                        int RGB = (pixel.R%8) * 32 + (pixel.G%8) * 4 + (pixel.B%4);
                        if (RGB == 0)
                        {
                            Color pixel2 = bmp2.GetPixel(j+1, i);
                            int RGB2 = (pixel2.R%8) * 32 + (pixel2.G%8) * 4 + (pixel2.B%4);
                            if (RGB2 == 0)
                            {
                                i = bmp2.Height;
                                break;
                            }

                        }
                        Encoded += (char)RGB;
                    }

                }
                if (Encoded.Length <= textBox2.MaxLength)
                    textBox2.Text = Encoded;
                else
                {
                    saveFileDialog1.DefaultExt = ".txt";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                StreamWriter sr = new StreamWriter(saveFileDialog1.FileName);
                sr.Write(Encoded);

            }

                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = ".jpg";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bmp.Save(saveFileDialog1.FileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
             openFileDialog1.Filter = "Text (*.txt) | *.txt";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                if (bmp != null)
                {
                   
                    foreach(var line in File.ReadAllLines(openFileDialog1.FileName))
                    {
                        enc +=line;
                    }
                   

                }

            }
        }
    }
}
