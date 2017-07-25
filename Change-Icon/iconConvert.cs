using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Change_Icon
{
    public static class IconConvert
    {
        /// <summary>
        /// Converts a PNG image to a icon (ico)
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="output">The output stream</param>
        /// <param name="size">The size (256x256 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        private static bool ConvertToIcon(Stream input, Stream output, int size = 256, bool preserveAspectRatio = false)
        {
            try
            {
                //int width, height;
                using (var inputBitmap = (Bitmap)Image.FromStream(input))
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

                    using (var newBitmap = new Bitmap(inputBitmap, new Size(width, height)))
                    {
                        // save the resized png into a memory stream for future use
                        using (var memoryStream = new MemoryStream())
                        {
                            newBitmap.Save(memoryStream, ImageFormat.Png);
                            var iconWriter = new BinaryWriter(output);
                            if (GetIconWriter(iconWriter) != null)
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
                                iconWriter.Write(6 + 16);

                                // write image data
                                // png data must contain the whole png data file
                                iconWriter.Write(memoryStream.ToArray());

                                iconWriter.Flush();

                                return true;
                            }

                        }

                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Error in ConvertToIcon Helper" + Environment.NewLine + @"The Program will terminate");
                return false;
            }
            return false;
        }

        private static BinaryWriter GetIconWriter(BinaryWriter iconWriter)
        {
            return iconWriter;
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
            var In = Path.GetTempPath() + Path.GetFileNameWithoutExtension(outputPath) + ".png";
            var png1 = Path.GetTempPath() + "Helper1.png";
            var png2 = Path.GetTempPath() + "Helper2.png";
            var outpng = Path.GetTempPath() + "out.png";
            CreateTmpFile(In, inputPath);
            HelperFiles(In, png1, png2);
            MargeFiles(In, png1, png2, outpng);
            try
            {
                File.Copy(outpng, In, true);
                File.Delete(png1);
                File.Delete(png2);
                File.Delete(outpng);
            }
            catch
            {

                MessageBox.Show(@"Failed to replase Original File With TMP File" + Environment.NewLine + @"The Program will terminate");
                return false;
            }

            try
            {


                using (var inputStream = new FileStream(In, FileMode.Open))
                {
                    using (var outputStream = new FileStream(outputPath, FileMode.OpenOrCreate))
                    {
                        return ConvertToIcon(inputStream, outputStream, size, preserveAspectRatio);
                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Failed to To Send To ConvertToIcon Helper" + Environment.NewLine + @"The Program will terminate");
            }
            return false;
        }

        private static void CreateTmpFile(string In, string inputPath)
        {
            try
            {
                using (var img = Image.FromFile(inputPath))
                {
                    using (new Bitmap(img))
                    {
                        var extension = Path.GetExtension(inputPath);
                        if (!extension.Equals(".png"))
                            img.Save(In, ImageFormat.Png);
                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Failed to Create TMP File" + Environment.NewLine + @"The Program will terminate");
            }
        }

        private static void HelperFiles(string In, string png1, string png2)
        {
            try
            {
                using (var inPic = Image.FromFile(In))
                {
                    var height = inPic.Height;
                    var width = inPic.Width;
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
                    using (var bmp = new Bitmap(width, height))
                    {
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.FillRectangle(Brushes.Transparent, 0, 0, width, height);
                            bmp.Save(png1, ImageFormat.Png);
                            bmp.Save(png2, ImageFormat.Png);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Failed To Create Helper Files" + Environment.NewLine + @"The Program will terminate");
            }
        }

        private static void MargeFiles(string In, string png1, string png2, string outpng)
        {
            try
            {
                using (var img1 = Image.FromFile(png1))
                {
                    using (var img2 = Image.FromFile(png2))
                    {
                        using (var fs = new FileStream(In, FileMode.Open, FileAccess.Read))
                        using (var main = Image.FromStream(fs))
                        {
                            int width;
                            int height;
                            if (main.Width > main.Height)
                            {
                                width = Math.Max(Math.Max(img1.Width, main.Width), Math.Max(img2.Width, main.Width));
                                height = main.Height + img1.Height + img2.Height;
                                var img3 = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                                var g = Graphics.FromImage(img3);
                                g.DrawImage(img1, new Rectangle(0, 0, img1.Width, img1.Height));
                                g.DrawImage(main, new Rectangle(0, img1.Height, main.Width, main.Height));
                                g.DrawImage(img2, new Rectangle(0, main.Height, img2.Width, img2.Height));
                                img3.Save(outpng, ImageFormat.Png);
                                img3.Dispose();
                                g.Dispose();
                            }
                            else if (main.Width < main.Height)
                            {
                                width = img1.Width + img2.Width + main.Width;
                                height = Math.Max(Math.Max(img1.Height, main.Height), Math.Max(img2.Height, main.Height));
                                var img3 = new Bitmap(width, height);
                                var g = Graphics.FromImage(img3);
                                g.DrawImage(img1, new Rectangle(0, 0, img1.Width, img1.Height));
                                g.DrawImage(main, new Rectangle(img1.Width, 0, main.Width, main.Height));
                                g.DrawImage(img2, new Rectangle(main.Width, 0, img2.Width, img2.Height));
                                img3.Save(outpng, ImageFormat.Png);
                                img3.Dispose();
                                g.Dispose();
                            }
                            else
                            {
                                height = main.Height;
                                width = main.Width;
                                var img3 = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                                var g = Graphics.FromImage(img3);
                                g.DrawImage(main, new Rectangle(0, 0, main.Width, main.Height));
                                img3.Save(outpng, ImageFormat.Png);
                                img3.Dispose();
                                g.Dispose();

                            }
                        }
                        //fs.Close();
                    }
                }
            }
            catch
            {
                MessageBox.Show(@"Failed to Marage TMP File With Helper Files" + Environment.NewLine + @"The Program will terminate");
            }
        }
    }
}