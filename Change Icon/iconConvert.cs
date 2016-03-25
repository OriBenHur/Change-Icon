using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
/// <summary>
/// Provides helper methods for imaging
/// </summary>
namespace Change_Icon
{
    public static class iconConvert
    {
        /// <summary>
        /// Converts a PNG image to a icon (ico)
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="output">The output stream</param>
        /// <param name="size">The size (16x16 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool ConvertToIcon(Stream input, Stream output, int size = 16, bool preserveAspectRatio = false)
        {
            Bitmap inputBitmap = (Bitmap)Image.FromStream(input);
            if (inputBitmap != null)
            {
                int width, height;
                if (preserveAspectRatio)
                {
                    width = size;
                    height = inputBitmap.Height / inputBitmap.Width * size;
                }
                else
                {
                    width = height = size;
                }
                Bitmap newBitmap = new Bitmap(inputBitmap, new Size(width, height));
                if (newBitmap != null)
                {
                    // save the resized png into a memory stream for future use
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        newBitmap.Save(memoryStream, ImageFormat.Png);

                        BinaryWriter iconWriter = new BinaryWriter(output);
                        if (output != null && iconWriter != null)
                        {
                            // 0-1 reserved, 0
                            iconWriter.Write((byte)0);
                            iconWriter.Write((byte)0);

                            // 2-3 image type, 1 = icon, 2 = cursor
                            iconWriter.Write((short)1);

                            // 4-5 number of images
                            iconWriter.Write((short)1);

                            // image entry 1
                            // 0 image width
                            iconWriter.Write((byte)width);
                            // 1 image height
                            iconWriter.Write((byte)height);

                            // 2 number of colors
                            iconWriter.Write((byte)0);

                            // 3 reserved
                            iconWriter.Write((byte)0);

                            // 4-5 color planes
                            iconWriter.Write((short)0);

                            // 6-7 bits per pixel
                            iconWriter.Write((short)32);

                            // 8-11 size of image data
                            iconWriter.Write((int)memoryStream.Length);

                            // 12-15 offset of image data
                            iconWriter.Write((int)(6 + 16));

                            // write image data
                            // png data must contain the whole png data file
                            iconWriter.Write(memoryStream.ToArray());

                            iconWriter.Flush();
                            //File.Delete(Path.GetTempPath() + "*.png");
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Converts a PNG image to a icon (ico)
        /// </summary>
        /// <param name="inputPath">The input path</param>
        /// <param name="outputPath">The output path</param>
        /// <param name="size">The size (16x16 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool ConvertToIcon(string inputPath, string outputPath, int size = 16, bool preserveAspectRatio = false)
        {

            string In = Path.GetTempPath() + Path.GetFileNameWithoutExtension(outputPath) + ".png";
            string png1 = Path.GetTempPath() + "Helper1.png";
            string png2 = Path.GetTempPath() + "Helper2.png";
            string outpng = Path.GetTempPath() + "out.png";
            //Image img1 = null;
            //Image img2 = null;
            //Image main = null;
            //Bitmap img3 = null;
            //Graphics g = null;
            //Graphics q = null;
            //Graphics o = null;
            //FileStream inStream = null;

            int width, height;
            //File.Copy(In, outpng , true);

            File.Copy(inputPath, In, true);
            Bitmap bmp = new Bitmap(256, 256);
            using (FileStream inStream = new FileStream(In, FileMode.Open))
            {
                using (Bitmap inputBitmap = (Bitmap)Image.FromStream(inStream))
                {


                    if (inputBitmap.Width > inputBitmap.Height)
                    {
                        width = inputBitmap.Width;
                        height = inputBitmap.Height;
                        int cHeight = (width - height) / 2;
                        if (cHeight < 1) cHeight = 1;
                        bmp = new Bitmap(width, cHeight);
                    }

                    else if (inputBitmap.Width < inputBitmap.Height)
                    {
                        height = inputBitmap.Height;
                        width = inputBitmap.Width;
                        int cWidth = (height - width) / 2;
                        if (cWidth < 1) cWidth = 1;
                        bmp = new Bitmap(cWidth, height);

                    }
                    else
                    {
                        bmp = new Bitmap(1, 1);
                    }

                    Graphics g = Graphics.FromImage(bmp);
                    g.Clear(Color.Transparent);
                    g.Flush();
                    bmp.Save(png1, ImageFormat.Png);
                    bmp.Save(png2, ImageFormat.Png);
                    bmp.Dispose();

                    //Graphics g = Graphics.FromImage(bmp);
                    g.FillRectangle(Brushes.Transparent, 100, 100, 100, 100);
                }
            }
            Image img1 = Image.FromFile(png1);
            Image img2 = Image.FromFile(png2);
            Image main = Image.FromFile(In);
            int _width = 0;
            int _height = 0;
            if (main.Width > main.Height)
            {
                _width = Math.Max(Math.Max(img1.Width, main.Width), Math.Max(img2.Width, main.Width));
                _height = main.Height + img1.Height + img2.Height;
                Bitmap img3 = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(img3);
                g.DrawImage(img1, new Rectangle(0, 0, img1.Width, img1.Height));
                g.DrawImage(main, new Rectangle(0, img1.Height, main.Width, main.Height));
                g.DrawImage(img2, new Rectangle(0, main.Height, img2.Width, img2.Height));
                img3.Save(outpng, ImageFormat.Png);
                img3.Dispose();
                g.Dispose();
                img1.Dispose();
                img2.Dispose();
                main.Dispose();


            }
            else if (main.Width < main.Height)
            {
                _width = img1.Width + img2.Width + main.Width;
                _height = Math.Max(Math.Max(img1.Height, main.Height), Math.Max(img2.Height, main.Height));
                Bitmap img3 = new Bitmap(_width, _height);
                Graphics g = Graphics.FromImage(img3);
                g.DrawImage(img1, new Rectangle(0, 0, img1.Width, img1.Height));
                g.DrawImage(main, new Rectangle(img1.Width, 0, main.Width, main.Height));
                g.DrawImage(img2, new Rectangle(main.Width, 0, img2.Width, img2.Height));
                img3.Save(outpng, ImageFormat.Png);
                img3.Dispose();
                g.Dispose();
                img1.Dispose();
                img2.Dispose();
                main.Dispose();

            }

            else
            {
                _height = main.Height;
                _width = main.Width;
                Bitmap img3 = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(img3);
                g.DrawImage(main, new Rectangle(0, 0, main.Width, main.Height));
                img3.Save(outpng, ImageFormat.Png);
                img3.Dispose();
                g.Dispose();
                img1.Dispose();
                img2.Dispose();
                main.Dispose();

            }
            File.Copy(outpng, In, true);
            File.Delete(png1);
            File.Delete(png2);
            File.Delete(outpng);
            using (FileStream inputStream = new FileStream(In, FileMode.Open))
            using (FileStream outputStream = new FileStream(outputPath, FileMode.OpenOrCreate))
            {
                return ConvertToIcon(inputStream, outputStream, size, preserveAspectRatio);
            }
        }
    }
}