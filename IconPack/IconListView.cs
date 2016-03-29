using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TAFactory.IconPack
{
    public partial class IconListView : ListView
    {
        private const int minWidth = 64;
        private const int textHeight = 18;
        private const int verticalSpacing = 5;
        private static readonly Padding TilePadding = new Padding(5, 1, 5, 1);
        
        public IconListView()
        {
            InitializeComponent();
            View = View.Tile;
            TileSize = base.TileSize;
            OwnerDraw = true;
            DrawItem += IconListView_DrawItem;
        }

        private Size _tileSize;
        public new Size TileSize
        {
            get { return _tileSize; }
            set 
            {
                _tileSize = value;
                BeginUpdate();
                base.TileSize = new Size(Math.Max(minWidth, value.Width) + TilePadding.Horizontal, value.Height + verticalSpacing + textHeight + TilePadding.Vertical);
                if (Items.Count != 0)
                {
                    var list = new List<IconListViewItem>(Items.Count);
                    foreach (IconListViewItem item in Items)
                    {
                        list.Add(item);
                    }
                    Items.Clear();
                    foreach (var item in list)
                    {
                        Items.Add(item);
                    }
                    //base.RedrawItems(0, base.Items.Count - 1, false);
                }

                EndUpdate();
            }
        }

        private void IconListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var item = e.Item as IconListViewItem;
            if (item == null)
            {
                e.DrawDefault = true;
                return;
            }

            // Draw item
            e.DrawBackground();
            var border = SystemPens.ControlLight;
            if (e.Item.Selected)
            {
                if (Focused)
                    border = SystemPens.Highlight;
                else
                    border = SystemPens.ButtonFace;
            }
            var centerSpacing = (e.Bounds.Width - TileSize.Width - TilePadding.Horizontal) / 2 + TilePadding.Left;
            var newBounds = new Rectangle(e.Bounds.X + centerSpacing, e.Bounds.Y + TilePadding.Top, TileSize.Width, TileSize.Height);
            e.Graphics.DrawRectangle(border, newBounds);

            //e.Graphics.DrawString("Whatever", this.Font, e., 0, 0);
            var x = e.Bounds.X + (newBounds.Width - item.Icon.Width) / 2 + centerSpacing + 1;
            var y = e.Bounds.Y + (newBounds.Height - item.Icon.Height) / 2 + TilePadding.Top + 1;
            var rect = new Rectangle(x, y, item.Icon.Width, item.Icon.Height);
            var clipReg = new Region(newBounds);
            e.Graphics.Clip = clipReg;
            e.Graphics.DrawIcon(item.Icon, rect);

            var text = string.Format("{0} x {1}", item.Icon.Width, item.Icon.Height);
            var stringSize = e.Graphics.MeasureString(text, Font);
            var stringWidth = (int) Math.Round(stringSize.Width);
            var stringHeight = (int) Math.Round(stringSize.Height);
            x = e.Bounds.X + (e.Bounds.Width - stringWidth - TilePadding.Horizontal) / 2 + TilePadding.Left;
            y = e.Bounds.Y + TileSize.Height + verticalSpacing + TilePadding.Top;
            clipReg = new Region(e.Bounds);
            e.Graphics.Clip = clipReg;
            if (e.Item.Selected)
            {
                if (Focused)
                {
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, x - 1, y - 1, stringWidth + 2, stringSize.Height + 2);
                    e.Graphics.DrawString(text, Font, SystemBrushes.HighlightText, x, y);
                }
                else
                {
                    e.Graphics.FillRectangle(SystemBrushes.ButtonFace, x - 1, y - 1, stringWidth + 2, stringSize.Height + 2);
                    e.Graphics.DrawString(text, Font, SystemBrushes.ControlText, x, y);
                }
            }
            else
                e.Graphics.DrawString(text, Font, SystemBrushes.ControlText, x, y);
        }
    }
    public class IconListViewItem : ListViewItem
    {
        private Icon _icon;
        public Icon Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
    }
}
