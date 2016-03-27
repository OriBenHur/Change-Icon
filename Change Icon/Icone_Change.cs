using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;


namespace Change_Icon
{
    public partial class Icon_Change : Form
    {
        public Icon_Change()
        {
            InitializeComponent();
        }

        //Global List that will hold all the chosen images (in the the current operation)
        private static List<string> pics = new List<string>();

        private void Folder_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            FolderSelectDialog folder = new FolderSelectDialog();

            bool result = folder.ShowDialog();
            if (result)
            {

                Folder_textBox.Text = folder.FileName;
            }
        }

        public List<string> getMyList
        {
            get { return pics; }
            set { pics = value; }
        }


        private void Icon_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();

            if (Folder_textBox.Text == "")
            {
                Folder_Error.SetIconPadding(Icone_textBox, 100);
                Folder_Error.SetError(Icone_textBox, "You must selcet folder first");
            }
            else
            {
                OpenFileDialog icon = new OpenFileDialog();
                //icon.InitialDirectory = Icone_textBox.Text;
                icon.Filter = "Icon files (*.ico)| *.ico|Common image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp";
                icon.FilterIndex = 1;
                icon.RestoreDirectory = false;
                DialogResult result = icon.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string icofile = Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(icon.FileName), "png");
                    Icone_textBox.Text = icon.FileName;
                    string destfile = Path.ChangeExtension(icofile, Path.GetExtension(icon.FileName));
                    File.Copy(icon.FileName, destfile, true);
                    string ext = Path.GetExtension(icon.FileName);
                    if (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".jpeg") || ext.ToLower().Equals(".png") || ext.ToLower().Equals(".bmp"))
                    {
                        iconConvert.ConvertToIcon(destfile, Path.ChangeExtension(destfile, "ico"), 256, false);
                    }

                    if (File.Exists(destfile))
                    {
                        pics.Add(destfile);
                    }
                    else destfile = icon.FileName;
                    using (FileStream fs = new FileStream(destfile, FileMode.Open))
                    {
                        pictureBox1.Image = Image.FromStream(fs);
                    }
                }
            }
        }


        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        static extern uint SHGetSetFolderCustomSettings(ref LPSHFOLDERCUSTOMSETTINGS pfcs, string pszPath, uint dwReadWrite);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct LPSHFOLDERCUSTOMSETTINGS
        {
            public uint dwSize;
            public uint dwMask;
            public IntPtr pvid;
            public string pszWebViewTemplate;
            public uint cchWebViewTemplate;
            public string pszWebViewTemplateVersion;
            public string pszInfoTip;
            public uint cchInfoTip;
            public IntPtr pclsid;
            public uint dwFlags;
            public string pszIconFile;
            public uint cchIconFile;
            public int iIconIndex;
            public string pszLogo;
            public uint cchLogo;
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
                string path = Folder_textBox.Text + "\\desktop.ini";
                string iconS = "";
                string sourceFile = "";
                if (Folder_textBox.Text.Length > 0)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    string ti = Folder_textBox.Text + "\\" + Path.ChangeExtension(Path.GetFileName(Icone_textBox.Text), "ico");
                    if (File.Exists(ti))
                    {
                        iconS = ti;
                    }

                    else
                    {
                        string tmpSource = Path.GetTempPath() + Path.ChangeExtension(Path.GetFileName(Icone_textBox.Text), "ico");
                        if (File.Exists(tmpSource)) sourceFile = tmpSource;
                        else sourceFile = Icone_textBox.Text;
                        iconS = Folder_textBox.Text + "\\" + Path.GetFileName(sourceFile);
                        File.Copy(sourceFile, iconS, true);
                    }

                    if (File.Exists(iconS))
                    {
                        FolderIcon myFolderIcon = new FolderIcon(Folder_textBox.Text);
                        myFolderIcon.CreateFolderIcon(iconS);
                        myFolderIcon = null;
                        File.SetAttributes(iconS, File.GetAttributes(iconS) | FileAttributes.Hidden);
                        File.SetAttributes(iconS, File.GetAttributes(iconS) | FileAttributes.System);
                        uint FCSM_ICONFILE = 0x00000010;
                        uint FCS_READ = 0x00000001;
                        uint FCS_FORCEWRITE = 0x00000002;
                        uint FCS_WRITE = FCS_READ | FCS_FORCEWRITE;
                        LPSHFOLDERCUSTOMSETTINGS FolderSettings = new LPSHFOLDERCUSTOMSETTINGS();
                        FolderSettings.dwSize = (uint)iconS.Length;
                        FolderSettings.dwMask = FCSM_ICONFILE;
                        FolderSettings.pszIconFile = Path.GetFileName(iconS);
                        FolderSettings.cchIconFile = 0;
                        FolderSettings.iIconIndex = 0;

                        uint HRESULT = SHGetSetFolderCustomSettings(ref FolderSettings, Folder_textBox.Text, FCS_FORCEWRITE);
                        MessageBox.Show("Done", "OK");
                        Application.Exit();
                    }
                }
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
                string[] filePaths = Directory.GetFiles(Folder_textBox.Text);
                bool find = false;
                foreach (string filePath in filePaths)
                {

                    if (Path.GetExtension(filePath).Equals(".ini") || Path.GetExtension(filePath).Equals(".ico"))
                    {
                        find = true;
                        File.Delete(filePath);
                    }
                }
                if (find)
                {
                    MessageBox.Show("Folder reset to default", "OK");
                    find = false;
                }
                else MessageBox.Show("Folder is already at default", "GeneralMessage");
            }
        }

        private void Icon_Change_FormClosed(object sender, FormClosedEventArgs e)
        {
            //general cleanup
            GC.SuppressFinalize(this);
            foreach (string pic in pics)
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
