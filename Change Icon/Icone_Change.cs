using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
                    var icon = new OpenFileDialog
                    {
                        Filter =
                            @"Icon files (*.ico)| *.ico|Common image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp",
                        FilterIndex = 1,
                        RestoreDirectory = false
                    };
                    var result = icon.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        var icofile = Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(icon.FileName), "png");
                        Icone_textBox.Text = icon.FileName;
                        var destfile = Path.ChangeExtension(icofile, Path.GetExtension(icon.FileName));
                        var ext = Path.GetExtension(icon.FileName);

                        if (ext != null && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".jpeg") || ext.ToLower().Equals(".png") || ext.ToLower().Equals(".bmp")))
                        {
                            File.Copy(icon.FileName, destfile, true);
                            if(!IconConvert.ConvertToIcon(destfile, Path.ChangeExtension(destfile, "ico"))) Dispose();
                        }

                        if (File.Exists(destfile))
                        {
                            _pics.Add(destfile);
                            using (var temp = new Bitmap(destfile))
                                pictureBox1.Image = new Bitmap(temp);
                        }
                        else
                        {
                            destfile = icon.FileName;
                            var img = Image.FromFile(destfile);
                            pictureBox1.Image = img;
                        }
                    }
                    break;
            }
        }


        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        static extern uint SHGetSetFolderCustomSettings(ref Lpshfoldercustomsettings pfcs, string pszPath, uint dwReadWrite);
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
                        var extension = Path.GetExtension(filePath);
                        if (extension != null && (extension.Equals(".ini") || extension.Equals(".ico")))
                        {
                            File.Delete(filePath);
                        }
                    }
                }

                var ti =
                    $@"{Folder_textBox.Text}\{Path.ChangeExtension(Path.GetFileName(Icone_textBox.Text), "ico")}";
                string iconS;
                if (File.Exists(ti))
                {
                    iconS = ti;
                }

                else
                {
                    var tmpSource = Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(Icone_textBox.Text), "ico");
                    var sourceFile = File.Exists(tmpSource) ? tmpSource : Icone_textBox.Text;
                    iconS = $@"{Folder_textBox.Text}\{Path.GetFileName(sourceFile)}";
                    File.Copy(sourceFile, iconS, true);
                }

                if (!File.Exists(iconS)) return;
                var myFolderIcon = new FolderIcon(Folder_textBox.Text);
                myFolderIcon.CreateFolderIcon(iconS);
                File.SetAttributes(iconS, File.GetAttributes(iconS) | FileAttributes.Hidden);
                File.SetAttributes(iconS, File.GetAttributes(iconS) | FileAttributes.System);
                const uint fcsmIconfile = 0x00000010;
                var folderSettings = new Lpshfoldercustomsettings
                {
                    dwSize = (uint) iconS.Length,
                    dwMask = fcsmIconfile,
                    pszIconFile = Path.GetFileName(iconS),
                    cchIconFile = 0,
                    iIconIndex = 0
                };

                SHGetSetFolderCustomSettings(ref folderSettings, Folder_textBox.Text, fcsForcewrite);
                MessageBox.Show(@"Done", @"OK");
                Application.Exit();
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
                    var extension = Path.GetExtension(filePath);
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
            GC.SuppressFinalize(this);
            foreach (var pic in _pics)
            {
                if (File.Exists(Path.ChangeExtension(pic, "png"))) File.Delete(Path.ChangeExtension(pic, "png"));
                if (File.Exists(Path.ChangeExtension(pic, "bmp"))) File.Delete(Path.ChangeExtension(pic, "bmp"));
                if (File.Exists(Path.ChangeExtension(pic, "jpg"))) File.Delete(Path.ChangeExtension(pic, "jpg"));
                if (File.Exists(Path.ChangeExtension(pic, "jpeg"))) File.Delete(Path.ChangeExtension(pic, "jpeg"));
                if (File.Exists(Path.ChangeExtension(pic, "ico"))) File.Delete(Path.ChangeExtension(pic, "ico"));
            }
        }
    }
}
