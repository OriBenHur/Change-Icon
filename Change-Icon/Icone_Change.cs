using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using TMDbLib.Client;
using TMDbLib.Objects.Search;
using static System.IO.File;
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

        //private void IconChange_Load(object sender, EventArgs e)
        // {
        //     NewVersion();
        // }

        private void NewVersion(bool isLoad)
        {
            var downloadUrl = @"";
            Version newVersion = null;
            XElement change = null;
            var xmlUrl = @"https://onedrive.live.com/download?cid=D9DE3B3ACC374428&resid=D9DE3B3ACC374428%217999&authkey=ADJwQu1VOTfAOVg";
            Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            
            try
            {
                var doc = XDocument.Load(xmlUrl);
                foreach (var dm in doc.Descendants(appName))
                {
                    var versionElement = dm.Element(@"version");
                    if (versionElement == null) continue;
                    var urlelEment = dm.Element(@"url");
                    if (urlelEment == null) continue;
                    newVersion = new Version(versionElement.Value);
                    downloadUrl = urlelEment.Value;
                    change = dm.Element(@"change_log");

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (appVersion.CompareTo(newVersion) < 0)
            {
                //Debug.Assert(change != null, "change != null");
                if (change == null) return;
                change.Value = change.Value;
                var result = MessageBox.Show(
                    $@"{appName.Replace('_', ' ')} v.{newVersion} is out!{Environment.NewLine}{change.Value}",
                    @"New Version is avlibale", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    System.Diagnostics.Process.Start(downloadUrl);
            }
            else
            {
                if (!isLoad)
                    MessageBox.Show(@"You Are Running The Last Version.", @"No New Updates");
            }
        }
        //Global List that will hold all the chosen images (in the the current operation)
        private static readonly List<string> Pics;

        static IconChange()
        {
            Pics = new List<string>();
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

        /*
                public List<string> GetMyList
                {
                    get { return _pics; }
                    set { _pics = value; }
                }
        */

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
                    Size = new Size(388, 282);
                    pictureBox1.Location = new Point(50, 103);
                    Reset_Folder.Location = new Point(193, 208);
                    Set.Location = new Point(275, 2081);
                    Movie_radioButton.Checked = false;
                    TV_radioButton.Checked = false;
                    Movie_radioButton.Visible = false;
                    TV_radioButton.Visible = false;
                    pictureBox1.Visible = true;
                    pictureBox1.Image = null;

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
                        var dir = GetDirectoryName(myIcon.FileName);
                        var extension = GetExtension(myIcon.FileName);
                        if (extension != null)
                        {
                            var name = $@"{dir}\{file + _mRnd}{extension}";
                            var viewer = new Viewer(name);
                            var destfile = ChangeExtension(icofile, GetExtension(myIcon.FileName));
                            extension = extension.ToLower();
                            if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png") || extension.Equals(".bmp"))
                            {
                                try
                                {
                                    if (Exists(destfile))
                                    {
                                        SetAttributes(destfile, FileAttributes.Normal);
                                        Delete(destfile);
                                    }
                                    Copy(myIcon.FileName, destfile, true);
                                    SetAttributes(destfile, FileAttributes.Normal);

                                    if (!IconConvert.ConvertToIcon(destfile, ChangeExtension(destfile, "ico"))) Dispose();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else if (extension.Equals(".dll") || extension.Equals(".exe"))
                            {
                                var iconPick = new IconPickerDialog { FileName = myIcon.FileName };
                                var ico = iconPick.ShowDialog(this);
                                if (ico == DialogResult.OK)
                                {
                                    var fileName = iconPick.FileName;
                                    var index = iconPick.IconIndex;

                                    Icon icon;
                                    Icon[] splitIcons;
                                    try
                                    {
                                        var extractor = new IconExtractor(fileName);
                                        icon = extractor.GetIcon(index);
                                        splitIcons = IconUtil.Split(icon);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    Icone_textBox.Text = string.Format(fileName);

                                    // Update icons.

                                    icon.Dispose();

                                    var viewerList = viewer.list;
                                    viewerList.BeginUpdate();
                                    foreach (var i in splitIcons)
                                    {
                                        var item = new Viewer.IconListViewItem();
                                        var size = i.Size;
                                        var bits = IconUtil.GetBitCount(i);

                                        item.ToolTipText = $"{size.Width}x{size.Height}, {bits} bits";
                                        item.Bitmap = IconUtil.ToBitmap(i);
                                        i.Dispose();
                                        viewerList.Items.Add(item);
                                    }
                                    viewer.ShowDialog();
                                    var exit = viewer.Exit;
                                    viewerList.EndUpdate();
                                    if (!exit)
                                    {
                                        destfile = GetTempPath() + GetFileName(ChangeExtension(name, @"png"));
                                        if (!IconConvert.ConvertToIcon(destfile, ChangeExtension(destfile, "ico")))
                                            Dispose();
                                    }

                                    else
                                    {
                                        //Folder_textBox.Text = "";
                                        Icone_textBox.Text = "";
                                        if (pictureBox1.Image == null) return;
                                        pictureBox1.Image.Dispose();
                                        pictureBox1.Image = null;
                                    }

                                }
                            }

                            if (Exists(destfile))
                            {
                                Pics.Add(destfile);
                                using (var temp = new Bitmap(destfile))
                                    pictureBox1.Image = new Bitmap(temp);
                            }

                            else
                            {
                                if (!extension.Equals(".dll"))
                                {
                                    destfile = myIcon.FileName;
                                    var img = Image.FromFile(destfile);
                                    pictureBox1.Image = img;
                                }
                                else Icone_textBox.Text = "";
                            }
                        }
                    }
                    break;
            }
        }
        private static readonly ApiKeys ApiKeys = new ApiKeys();
        private static readonly string TmdBapikey = ApiKeys.TmdBapikey;
        private static readonly TMDbClient Tmdb = new TMDbClient(TmdBapikey);
        private const string BaseUrl = "https://image.tmdb.org/t/p/w342";

        private void imdb_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            switch (Folder_textBox.Text)
            {
                case "":
                    Folder_Error.SetIconPadding(Icone_textBox, 100);
                    Folder_Error.SetError(Icone_textBox, "You must selcet folder first");
                    break;
                default:
                    //if (Icone_textBox.Text == "")
                    //{
                    //    Folder_Error.SetIconPadding(Icone_textBox, 100);
                    //    Folder_Error.SetError(Icone_textBox, @"Input The Seriese \ Movie Name in the icon TextBox");
                    //}

                    //else
                    //{

                        Size = new Size(388, 301);
                        pictureBox1.Location = new Point(50, 116);
                        Reset_Folder.Location = new Point(193, 221);
                        Set.Location = new Point(275, 221);
                        Movie_radioButton.Visible = true;
                        TV_radioButton.Visible = true;
                        Icone_textBox.Text = GetVideoName();
                   // }

                    break;
            }
        }

        private string GetVideoName()
        {
            DirectoryInfo di = new DirectoryInfo(Folder_textBox.Text);

            string video = null;
            foreach (var fi in di.GetFiles("*", SearchOption.AllDirectories))
            {
                var name = fi.Name;
                if (GetExtension(name) == ".mp4" || GetExtension(name) == ".mkv" || GetExtension(name) == ".avi" || GetExtension(name) == ".srt")
                {
                    video = name;
                    break;
                }
            }
            var match = Regex.Match(video,
                @"^(?<series>.*)?((.(19|20)[0-9][0-9])|(.[sS][0-9]{2}[eE][0-9]{2}))(?<ext>.*)$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            var videoName = match.Groups["series"].Value;
            videoName = Regex.Replace(videoName, ".(19|20)[0-9][0-9]", String.Empty);
            return videoName;
        }


        public static string CleanString(string str, bool clean = true)
        {
            return clean ? str.Replace(".", " ") : str.Replace(" ", ".");
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


        private class IniFile
        {
            private string _filePath;

            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section,
                string key,
                string val,
                string filePath);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section,
                string key,
                string def,
                StringBuilder retVal,
                int size,
                string filePath);

            public IniFile(string filePath)
            {
                _filePath = filePath;
            }

            public string Read(string section, string key)
            {
                StringBuilder sb = new StringBuilder(255);
                int i = GetPrivateProfileString(section, key, "", sb, 255, _filePath);
                return sb.ToString();
            }

            public string FilePath
            {
                get { return _filePath; }
                set { _filePath = value; }
            }
        }
        private void Set_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            if (Icone_textBox.Text == "")
            {
                Folder_Error.SetIconPadding(Set, 0);
                Folder_Error.SetError(Set, "You must selcet icon first");
            }
            else
            {
                if (!Directory.Exists(Folder_textBox.Text)) MessageBox.Show(@"Invalid Folder Path", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!Exists(Icone_textBox.Text)) MessageBox.Show(@"Invalid Icon Path", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    var path = Folder_textBox.Text + @"\desktop.ini";
                    const uint fcsForcewrite = 0x00000002;
                    if (Folder_textBox.Text.Length <= 0) return;
                    if (Exists(path))
                    {
                        var filePaths = Directory.GetFiles(Folder_textBox.Text);
                        foreach (var filePath in filePaths)
                        {
                            var extension = GetExtension(filePath);
                            if (extension == null || !extension.Equals(".ini") && !extension.Equals(".ico")) continue;
                            try
                            {
                                Delete(filePath);
                            }
                            catch
                            {
                                Console.WriteLine("");
                            }
                        }
                    }
                    var changeExtension = ChangeExtension(GetFileName(Icone_textBox.Text), "ico");
                    var tmpIcon = $@"{Folder_textBox.Text}\{changeExtension}";
                    string iconS;
                    if (Exists(tmpIcon))
                    {
                        iconS = tmpIcon;
                    }

                    else
                    {
                        var extension = GetExtension(Icone_textBox.Text);
                        var file = GetFileNameWithoutExtension(Icone_textBox.Text);
                        var tempPath = GetTempPath();
                        var tmpSource = ChangeExtension($@"{tempPath}{file + _mRnd}{extension}", "ico");
                        var tmpSource2 = GetTempPath() + ChangeExtension(GetFileName(Icone_textBox.Text), "ico");
                        string sourceFile;
                        if (Exists(tmpSource)) sourceFile = tmpSource;
                        else if (Exists(tmpSource2)) sourceFile = tmpSource2;
                        else sourceFile = Icone_textBox.Text;
                        iconS = $@"{Folder_textBox.Text}\{GetFileName(sourceFile)}";
                        Copy(sourceFile, iconS, true);
                    }

                    if (!Exists(iconS)) return;
                    var myFolderIcon = new FolderIcon(Folder_textBox.Text);
                    myFolderIcon.CreateFolderIcon(iconS);
                    SetAttributes(iconS, GetAttributes(iconS) | FileAttributes.Hidden);
                    SetAttributes(iconS, GetAttributes(iconS) | FileAttributes.System);
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
                    Folder_textBox.Text = "";
                    Icone_textBox.Text = "";
                    Size = new Size(388, 282);
                    pictureBox1.Location = new Point(50, 103);
                    Reset_Folder.Location = new Point(193, 208);
                    Set.Location = new Point(275, 208);
                    TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
                    TV_radioButton.Checked = false;
                    TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;
                    Movie_radioButton.CheckedChanged -= Movie_radioButton_CheckedChanged;
                    Movie_radioButton.Checked = false;
                    Movie_radioButton.CheckedChanged += Movie_radioButton_CheckedChanged;
                    Movie_radioButton.Visible = false;
                    TV_radioButton.Visible = false;
                    if (pictureBox1.Image == null) return;
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
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
                var filePaths = Directory.GetFiles(Folder_textBox.Text);
                var find = false;
                var iconfile = "";
                var ini = $@"{Folder_textBox.Text}\desktop.ini";
                if (Exists(ini))
                {
                    var inif = new IniFile(ini);
                    iconfile = inif.Read(".ShellClassInfo", "IconResource");
                    iconfile = iconfile.Substring(0, iconfile.Length - 2);
                }
                foreach (var filePath in filePaths)
                {
                    var extension = GetExtension(filePath);
                    if (extension == null || !extension.Equals(".ini") && !extension.Equals(".ico")) continue;

                    if (extension.Equals(".ico"))
                    {
                        if (!iconfile.Equals(GetFileName(filePath))) continue;
                        try
                        {
                            Delete(filePath);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }

                    else if (extension.Equals(".ini"))
                    {
                        var fileName = GetFileName(filePath);
                        if (fileName == null || !fileName.Equals("desktop.ini")) continue;
                        try
                        {
                            Delete(filePath);
                            find = true;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }

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
                foreach (var pic in Pics)
                {
                    if (Exists(ChangeExtension(pic, "png"))) Delete(ChangeExtension(pic, "png"));
                    if (Exists(ChangeExtension(pic, "bmp"))) Delete(ChangeExtension(pic, "bmp"));
                    if (Exists(ChangeExtension(pic, "jpg"))) Delete(ChangeExtension(pic, "jpg"));
                    if (Exists(ChangeExtension(pic, "jpeg"))) Delete(ChangeExtension(pic, "jpeg"));
                    if (Exists(ChangeExtension(pic, "ico"))) Delete(ChangeExtension(pic, "ico"));
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewVersion(false);
        }


        private void IconChange_Shown(object sender, EventArgs e)
        {
            void Action()
            {
                NewVersion(true);
            }

            var thread = new Thread(Action) { IsBackground = true };
            thread.Start();
        }

        
        private void TV_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pictureBox1.Image = Properties.Resources.preloader;
            var original = Icone_textBox.Text;
            var iconName = CleanString(original.Trim());
            var results = Tmdb.SearchTvShowAsync(iconName).Result;
            if (results.TotalResults < 1)
            {
                MessageBox.Show(@"Couldn't Find The Requested Artwork", @"Sorry");
                pictureBox1.Image = null;
                TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
                TV_radioButton.Checked = false;
                TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;

            }
            foreach (var video in results.Results)
            {
                //if (video.Name.ToLower().Replace(".","").Equals(iconName.ToLower()))
                //{
                    backgroundWorker1.RunWorkerAsync(video.PosterPath);
                    break;

                //}
            }

        }

        private void Movie_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pictureBox1.Image = Properties.Resources.preloader;
            var original = Icone_textBox.Text;
            var iconName = CleanString(original.Trim());
            var results = Tmdb.SearchMovieAsync(iconName).Result;
            if (results.TotalResults < 1)
            {
                MessageBox.Show(@"Couldn't Find The Requested Artwork", @"Sorry");
                pictureBox1.Image = null;
                Movie_radioButton.CheckedChanged -= Movie_radioButton_CheckedChanged;
                Movie_radioButton.Checked = false;
                Movie_radioButton.CheckedChanged += Movie_radioButton_CheckedChanged;

            }
            foreach (var video in results.Results)
            {
                //if (!video.Title.ToLower().Equals(iconName.ToLower())) continue;
                backgroundWorker1.RunWorkerAsync(video.PosterPath);
                break;
            }
        }

        private void Process(string iconPath)
        {
            
            var icofile = GetTempPath() + ChangeExtension(GetFileName(iconPath), "png");
            
            var extension = GetExtension(iconPath);
            var destfile = ChangeExtension(iconPath, GetExtension(icofile));
            if (extension == null) return;
            extension = extension.ToLower();
            if (extension.Equals(".jpg"))
            {
                try
                {
                    if (Exists(destfile))
                    {
                        SetAttributes(destfile, FileAttributes.Normal);
                        Delete(destfile);
                    }
                    Copy(iconPath, destfile, true);
                    SetAttributes(destfile, FileAttributes.Normal);

                    if (!IconConvert.ConvertToIcon(destfile, ChangeExtension(destfile, "ico"))) Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (Exists(destfile))
            {
                Pics.Add(destfile);
                using (var temp = new Bitmap(destfile))
                    pictureBox1.Image = new Bitmap(temp);
            }

            else
            {
                if (!extension.Equals(".dll"))
                {
                    destfile = iconPath;
                    var img = Image.FromFile(destfile);
                    pictureBox1.Image = img;
                }
                else Icone_textBox.Text = "";
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var original = Icone_textBox.Text;
            var iconName = CleanString(original.Trim());
            var iconPath = GetTempPath() + CleanString(iconName, false) + ".jpg";
            using (var client = new WebClient())
            {
                client.DownloadFile(BaseUrl + e.Argument, iconPath);
            }

            Process(iconPath);
            if (Icone_textBox.InvokeRequired)
            {
                Icone_textBox.Invoke(new MethodInvoker(delegate {Icone_textBox.Text = iconPath; }));
            }

        }
    }
}
