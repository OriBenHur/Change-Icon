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
        private string name;
        public Viewer(string name)
        {
            InitializeComponent();
            this.name = name;
        }
        public string _name { get; set; }

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

            if (e.Item.Selected)
                e.Graphics.FillRectangle(SystemBrushes.MenuHighlight, e.Bounds);
            else
                e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

            if (item != null)
            {
                int w = Math.Min(128, item.Bitmap.Width);
                int h = Math.Min(128, item.Bitmap.Height);

                int x = e.Bounds.X + (e.Bounds.Width - w) / 2;
                int y = e.Bounds.Y + (e.Bounds.Height - h) / 2;
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
            string _path = Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(name), @".png");
            if (File.Exists(_path))
            {
                try
                {
                    File.Delete(_path);
                }
                catch
                {
                    MessageBox.Show(@"Faild to delete: " + _path);
                }
            }

            if (listView1.SelectedIndices.Count <= 0)
            {
                return;
            }
            int intselectedindex = listView1.SelectedIndices[0];
            if (intselectedindex >= 0)
            {
                var tt = listView1.FocusedItem.Index;                
                IconListViewItem item = listView1.SelectedItems[0] as IconListViewItem;
                if (item != null)
                {
                    var bit = item.Bitmap;
                    bit.Save(_path, ImageFormat.Png);
                }
                Dispose();
            }
        }
    }
}
