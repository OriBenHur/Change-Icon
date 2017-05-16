using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.IO.Path;
// ReSharper disable AssignNullToNotNullAttribute

//using TsudaKageyu;

namespace Change_Icon
{
    public partial class IconChange : Form
    {
        public IconChange()
        {
            InitializeComponent();

        }


        //Global List that will hold all the chosen images (in the the current operation)
        private static List<string> _pics;

        static IconChange()
        {
            _pics = new List<string>();
        }

        private void Folder_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            var folder = new FolderSelectDialog();

            var result = folder.ShowDialog();
            if (result)
            {
                Folder_textBox.Text = folder.FileName;
            }
        }

        public List<string> GetMyList
        {
            get { return _pics; }
            set { _pics = value; }
        }

        private static readonly Random Rnd = new Random();
        private readonly int _mRnd = Rnd.Next(1, 1000);
        
        private void Icon_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();

            switch (Folder_textBox.Text)
            {
                case "":
                    Folder_Error.SetIconPadding(Icone_textBox, 100);
                    Folder_Error.SetError(Icone_textBox, "You must selcet folder first");
                    break;
                default:
                    var myIcon = new OpenFileDialog
                    {
                        Filter =
                            @"Icon files (*.ico)| *.ico|Common image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp|Resource Files (*.dll, *.exe)|*.dll; *.exe",
                        FilterIndex = 1,
                        RestoreDirectory = false



                    };
                    var result = myIcon.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        var icofile = GetTempPath() + ChangeExtension(GetFileName(myIcon.FileName), "png");
                        Icone_textBox.Text = myIcon.FileName;
                        var file = GetFileNameWithoutExtension(myIcon.FileName);
                        var fol = GetDirectoryName(myIcon.FileName);
                        var exte = GetExtension(myIcon.FileName);
                        string name = $@"{fol}\{file + _mRnd}{exte}";
                        var frm2 = new Viewer(name);
                        var destfile = ChangeExtension(icofile, GetExtension(myIcon.FileName));
                        var ext = GetExtension(myIcon.FileName);

                        if (ext != null && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".jpeg") || ext.ToLower().Equals(".png") || ext.ToLower().Equals(".bmp")))
                        {
                            try
                            {
                                if (File.Exists(destfile))
                                {
                                    File.SetAttributes(destfile, FileAttributes.Normal);
                                    File.Delete(destfile);  
                                }
                                File.Copy(myIcon.FileName, destfile, true);
                                File.SetAttributes(destfile, FileAttributes.Normal);

                                if (!IconConvert.ConvertToIcon(destfile, ChangeExtension(destfile, "ico"))) Dispose();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else if (ext != null && (ext.ToLower().Equals(".dll") || ext.ToLower().Equals(".exe")))
                        {
                            var iconPick = new IconPickerDialog {FileName = myIcon.FileName};
                            var ico = iconPick.ShowDialog(this);
                            if (ico == DialogResult.OK)
                            {
                                var fileName = iconPick.FileName;
                                var index = iconPick.IconIndex;

                                Icon icon;
                                Icon[] splitIcons;
                                try
                                {
                                    var extension = GetExtension(iconPick.FileName);
                                    if (extension != null && extension.ToLower() == ".ico")
                                    {
                                        icon = new Icon(iconPick.FileName);
                                    }
                                    else
                                    {
                                        var extractor = new IconExtractor(fileName);
                                        icon = extractor.GetIcon(index);
                                    }

                                    splitIcons = IconUtil.Split(icon);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                Icone_textBox.Text = string.Format(fileName);

                                // Update icons.

                                Icon = icon;
                                icon.Dispose();

                                var mylist = frm2.list;
                                mylist.BeginUpdate();
                                foreach (var i in splitIcons)
                                {
                                    var item = new Viewer.IconListViewItem();
                                    var size = i.Size;
                                    var bits = IconUtil.GetBitCount(i);
                                    
                                    item.ToolTipText = $"{size.Width}x{size.Height}, {bits} bits";
                                    item.Bitmap = IconUtil.ToBitmap(i);
                                    i.Dispose();
                                    mylist.Items.Add(item);
                                }
                                frm2.ShowDialog();
                                mylist.EndUpdate();
                                destfile = GetTempPath() + GetFileName(ChangeExtension(name, @"png"));
                                if (!IconConvert.ConvertToIcon(destfile, ChangeExtension(destfile, "ico"))) Dispose();

                            }
                        }

                        if (File.Exists(destfile))
                        {
                            _pics.Add(destfile);
                            using (var temp = new Bitmap(destfile))
                                pictureBox1.Image = new Bitmap(temp);
                        }

                        else
                        {
                            if (ext != null && !ext.Equals(".dll"))
                            {
                                destfile = myIcon.FileName;
                                var img = Image.FromFile(destfile);
                                pictureBox1.Image = img;
                            }
                            else Icone_textBox.Text = "";
                        }

                    }
                    break;
            }
        }


        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint SHGetSetFolderCustomSettings(ref Lpshfoldercustomsettings pfcs, string pszPath, uint dwReadWrite);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct Lpshfoldercustomsettings
        {
            public uint dwSize;
            public uint dwMask;
            private readonly IntPtr pvid;
            private readonly string pszWebViewTemplate;
            private readonly uint cchWebViewTemplate;
            private readonly string pszWebViewTemplateVersion;
            private readonly string pszInfoTip;
            private readonly uint cchInfoTip;
            private readonly IntPtr pclsid;
            private readonly uint dwFlags;
            public string pszIconFile;
            public uint cchIconFile;
            public int iIconIndex;
            private readonly string pszLogo;
            private readonly uint cchLogo;
        }

        private void Set_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            if (Icone_textBox.Text == "")
            {
                Folder_Error.SetIconPadding(Set, 0);
                Folder_Error.SetError(Set, "You must selcet icon first");
            }
            else {
                var path = Folder_textBox.Text + @"\desktop.ini";
                const uint fcsForcewrite = 0x00000002;
                if (Folder_textBox.Text.Length <= 0) return;
                if (File.Exists(path))
                {
                    var filePaths = Directory.GetFiles(Folder_textBox.Text);
                    foreach (var filePath in filePaths)
                    {
                        var extension = GetExtension(filePath);
                        if (extension != null && (extension.Equals(".ini") || extension.Equals(".ico")))
                        {
                            try
                            {
                                File.Delete(filePath);
                            }
                            catch
                            {
                                Console.WriteLine("");
                            }

                        }
                    }
                }

                var ti =
                    $@"{Folder_textBox.Text}\{ChangeExtension(GetFileName(Icone_textBox.Text), "ico")}";
                string iconS;
                if (File.Exists(ti))
                {
                    iconS = ti;
                }

                else
                {
                    var s = GetExtension(Icone_textBox.Text);
                    if (s != null && s.Equals("*.dll"))
                    {
                        iconS = Icone_textBox.Text;
                    }

                    else
                    {
                        var file = GetFileNameWithoutExtension(Icone_textBox.Text);
                        var fol = GetTempPath();
                        var exte = GetExtension(Icone_textBox.Text);
                        //var tt = $@"{fol}{file + _mRnd}{exte}";
                        //Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(Icone_textBox.Text) + _mRnd, "ico")
                        //? tmpSource : Icone_textBox.Text;
                        var tmpSource = ChangeExtension($@"{fol}{file + _mRnd}{exte}", "ico");
                        var tmpSource2 = GetTempPath() + ChangeExtension(GetFileName(Icone_textBox.Text), "ico");
                        string sourceFile;

                        if(File.Exists(tmpSource)) sourceFile = tmpSource;                  
                        else if (File.Exists(tmpSource2)) sourceFile = tmpSource2;
                        else sourceFile = Icone_textBox.Text;
     
                        iconS = $@"{Folder_textBox.Text}\{GetFileName(sourceFile)}";
                        File.Copy(sourceFile, iconS, true);
                    }
                }

                if (!File.Exists(iconS)) return;
                var myFolderIcon = new FolderIcon(Folder_textBox.Text);
                myFolderIcon.CreateFolderIcon(iconS);
                File.SetAttributes(iconS, File.GetAttributes(iconS) | FileAttributes.Hidden);
                File.SetAttributes(iconS, File.GetAttributes(iconS) | FileAttributes.System);
                const uint fcsmIconfile = 0x00000010;
                var folderSettings = new Lpshfoldercustomsettings
                {
                    dwSize = (uint)iconS.Length,
                    dwMask = fcsmIconfile,
                    pszIconFile = GetFileName(iconS),
                    cchIconFile = 0,

                    iIconIndex = 0
                };

                SHGetSetFolderCustomSettings(ref folderSettings, Folder_textBox.Text, fcsForcewrite);
                MessageBox.Show(@"Done", @"OK");
                //Application.Exit();
            }
        }

        private void Reset_Folder_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            if (Folder_textBox.Text == "")
            {
                Folder_Error.SetIconPadding(Reset_Folder, -90);
                Folder_Error.SetError(Reset_Folder, "You must selcet folder first");
            }
            else
            {
                var filePaths = Directory.GetFiles(Folder_textBox.Text);
                var find = false;
                foreach (var filePath in filePaths)
                {
                    var extension = GetExtension(filePath);
                    if (extension == null || (!extension.Equals(".ini") && !extension.Equals(".ico"))) continue;
                    find = true;
                    File.Delete(filePath);
                }
                if (find)
                {
                    MessageBox.Show(@"Folder reset to default", @"OK");
                }
                else MessageBox.Show(@"Folder is already at default", @"GeneralMessage");
            }
        }

        private void Icon_Change_FormClosed(object sender, FormClosedEventArgs e)
        {
            //general cleanup
            try
            {
                GC.SuppressFinalize(this);
                foreach (var pic in _pics)
                {
                    if (File.Exists(ChangeExtension(pic, "png"))) File.Delete(ChangeExtension(pic, "png"));
                    if (File.Exists(ChangeExtension(pic, "bmp"))) File.Delete(ChangeExtension(pic, "bmp"));
                    if (File.Exists(ChangeExtension(pic, "jpg"))) File.Delete(ChangeExtension(pic, "jpg"));
                    if (File.Exists(ChangeExtension(pic, "jpeg"))) File.Delete(ChangeExtension(pic, "jpeg"));
                    if (File.Exists(ChangeExtension(pic, "ico"))) File.Delete(ChangeExtension(pic, "ico"));
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
