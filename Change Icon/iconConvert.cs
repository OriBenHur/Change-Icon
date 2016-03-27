
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        /// <param name="size">The size (256x256 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool ConvertToIcon(System.IO.Stream input, Stream output, int size = 256, bool preserveAspectRatio = false)
        {
            Bitmap inputBitmap = (Bitmap)Bitmap.FromStream(input);
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
        /// <param name="size">The size (256x256 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool ConvertToIcon(string inputPath, string outputPath, int size = 256, bool preserveAspectRatio = false)
        {
            //General Helper Filse and Paths
            string In = Path.GetTempPath() + Path.GetFileNameWithoutExtension(outputPath) + ".png";
            using (Image img = Image.FromFile(inputPath))
            {
                using (Bitmap bmp = new Bitmap(img))
                {
                    if(!Path.GetExtension(inputPath).Equals(".png"))
                    img.Save(In, ImageFormat.Png);
                }
            }
            string png1 = Path.GetTempPath() + "Helper1.png";
            string png2 = Path.GetTempPath() + "Helper2.png";
            string outpng = Path.GetTempPath() + "out.png";
            using (Image inPic = Image.FromFile(In))
            {
                Bitmap bmp;
                int height = inPic.Height;
                int width = inPic.Width;
                if (width > height)
                {
                    height = (width - height) / 2;
                    if (height < 1) height = 1;
                }

                else if (width < height)
                {
                    width = (height - width) / 2;
                    if (width < 1) width = 1;
                }
                bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(Brushes.Transparent, 0, 0, width, height);
                g.Dispose();
                bmp.Save(png1, ImageFormat.Png);
                bmp.Save(png2, ImageFormat.Png);
                bmp.Dispose();
            }

            using (Image img1 = Image.FromFile(png1))
            {
                using (Image img2 = Image.FromFile(png2))
                {
                    using (Image main = Image.FromFile(In))
                    {
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

                        }
                    }
                }
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