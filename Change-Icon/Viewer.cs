using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;


namespace Change_Icon
{
    public partial class Viewer : Form
    {
        private readonly string _name;
        public Viewer(string name)
        {
            InitializeComponent();
            _name = name;
        }
        public bool Exit { get; private set; }

        public class IconListViewItem : ListViewItem
        {
            public Bitmap Bitmap { get; set; }
            public new string ToolTipText { get; set; }
        }
        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var item = e.Item as IconListViewItem;

            // Draw item

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.Clip = new Region(e.Bounds);

            e.Graphics.FillRectangle(e.Item.Selected ? SystemBrushes.MenuHighlight : SystemBrushes.Window, e.Bounds);

            if (item != null)
            {
                var w = Math.Min(128, item.Bitmap.Width);
                var h = Math.Min(128, item.Bitmap.Height);

                var x = e.Bounds.X + (e.Bounds.Width - w) / 2;
                var y = e.Bounds.Y + (e.Bounds.Height - h) / 2;
                var dstRect = new Rectangle(x, y, w, h);
                var srcRect = new Rectangle(Point.Empty, item.Bitmap.Size);


                e.Graphics.DrawImage(item.Bitmap, dstRect, srcRect, GraphicsUnit.Pixel);
            }

            var textRect = new Rectangle(
                e.Bounds.Left, e.Bounds.Bottom - Font.Height - 4,
                e.Bounds.Width, Font.Height + 2);
            if (item != null) TextRenderer.DrawText(e.Graphics, item.ToolTipText, Font, textRect, ForeColor);

            e.Graphics.Clip = new Region();
            e.Graphics.DrawRectangle(SystemPens.ControlLight, e.Bounds);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var path = Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(_name), @".png");
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    MessageBox.Show(@"Faild to delete: " + path);
                }
            }

            if (listView1.SelectedIndices.Count <= 0)
            {
                //exit = true;
                return;
            }
            var intselectedindex = listView1.SelectedIndices[0];
            if (intselectedindex < 0) return;
            //var tt = listView1.FocusedItem.Index;                
            IconListViewItem item = listView1.SelectedItems[0] as IconListViewItem;
            if (item != null)
            {
                var bit = item.Bitmap;
                bit.Save(path, ImageFormat.Png);
                //exit = false;
            }
            Dispose();
        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit = listView1.SelectedIndices.Count <= 0;
        }
    }
}
