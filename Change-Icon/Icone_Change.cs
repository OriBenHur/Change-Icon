using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using TMDbLib.Client;
using static System.IO.File;
using static System.IO.Path;
// ReSharper disable AssignNullToNotNullAttribute

namespace Change_Icon
{
    public partial class IconChange : Form
    {
        public IconChange()
        {
            InitializeComponent();

        }

        #region Old Check For Update Method
        //private void IconChange_Load(object sender, EventArgs e)
        // {
        //     NewVersion();
        // }

        //private void NewVersion(bool isLoad)
        //{
        //    var downloadUrl = @"";
        //    Version newVersion = null;
        //    XElement change = null;
        //    var xmlUrl = @"https://onedrive.live.com/download?cid=D9DE3B3ACC374428&resid=D9DE3B3ACC374428%217999&authkey=ADJwQu1VOTfAOVg";
        //    Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        //    var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        //    try
        //    {
        //        var doc = XDocument.Load(xmlUrl);
        //        foreach (var dm in doc.Descendants(appName))
        //        {
        //            var versionElement = dm.Element(@"version");
        //            if (versionElement == null) continue;
        //            var urlelEment = dm.Element(@"url");
        //            if (urlelEment == null) continue;
        //            newVersion = new Version(versionElement.Value);
        //            downloadUrl = urlelEment.Value;
        //            change = dm.Element(@"change_log");

        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }

        //    if (appVersion.CompareTo(newVersion) < 0)
        //    {
        //        //Debug.Assert(change != null, "change != null");
        //        if (change == null) return;
        //        change.Value = change.Value;
        //        var result = MessageBox.Show(
        //            $@"{appName.Replace('_', ' ')} v.{newVersion} is out!{Environment.NewLine}{change.Value}",
        //            @"New Version is avlibale", MessageBoxButtons.YesNo);
        //        if (result == DialogResult.Yes)
        //            System.Diagnostics.ProcessItem.Start(downloadUrl);
        //    }
        //    else
        //    {
        //        if (!isLoad)
        //            MessageBox.Show(@"You Are Running The Last Version.", @"No New Updates");
        //    }
        //}
        #endregion

        //Global List that will hold all the chosen images (in the the current operation)
        private static readonly List<string> Pics;

        static IconChange()
        {
            Pics = new List<string>();
        }

        private void Folder_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            Info.Clear();
            pictureBox1.Image = null;
            Icone_textBox.Text = "";
            Size = new Size(378, 272);
            pictureBox1.Location = new Point(50, 103);
            Reset_Folder.Location = new Point(193, 208);
            Set.Location = new Point(275, 208);
            Movie_radioButton.CheckedChanged -= Movie_radioButton_CheckedChanged;
            Movie_radioButton.Checked = false;
            Movie_radioButton.CheckedChanged += Movie_radioButton_CheckedChanged;
            TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
            TV_radioButton.Checked = false;
            TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;
            Movie_radioButton.Visible = false;
            TV_radioButton.Visible = false;
            pictureBox1.Visible = true;
            var folder = new FolderSelectDialog();

            var result = folder.ShowDialog();
            if (result)
            {
                Folder_textBox.Text = folder.FileName;
            }
        }


        private static readonly Random Rnd = new Random();
        private readonly int _mRnd = Rnd.Next(1, 1000);

        private void Icon_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            Info.Clear();
            switch (Folder_textBox.Text)
            {
                case "":
                    Folder_Error.SetIconPadding(Icone_textBox, 100);
                    Folder_Error.SetError(Icone_textBox, "You must selcet folder first");
                    break;
                default:
                    Icone_textBox.Text = "";
                    Size = new Size(378, 272);
                    pictureBox1.Location = new Point(50, 103);
                    Reset_Folder.Location = new Point(193, 208);
                    Set.Location = new Point(275, 208);
                    Movie_radioButton.CheckedChanged -= Movie_radioButton_CheckedChanged;
                    Movie_radioButton.Checked = false;
                    Movie_radioButton.CheckedChanged += Movie_radioButton_CheckedChanged;
                    TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
                    TV_radioButton.Checked = false;
                    TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;
                    Movie_radioButton.Visible = false;
                    TV_radioButton.Visible = false;
                    pictureBox1.Visible = true;

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
                        pictureBox1.Image = Properties.Resources.loading;
                        Local_backgroundWorker.RunWorkerAsync(myIcon.FileName);
                    }
                    break;
            }
        }
        private static readonly ApiKeys ApiKeys = new ApiKeys();
        private static readonly string TmdBapikey = ApiKeys.TmdBapikey;
        private static readonly TMDbClient Tmdb = new TMDbClient(TmdBapikey);
        private const string BaseUrl = "https://image.tmdb.org/t/p/w342";
        private int _year;
        private void imdb_button_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            Info.Clear();
            switch (Folder_textBox.Text)
            {
                case "":
                    Folder_Error.SetIconPadding(Icone_textBox, 100);
                    Folder_Error.SetError(Icone_textBox, "You must selcet folder first");
                    break;
                default:
                    if (Icone_textBox.Text == "") Icone_textBox.Text = GetVideoName();

                    else
                    {
                        GetYear(Icone_textBox.Text);
                    }

                    if (Icone_textBox.Text == "")
                    {
                        Info.SetIconPadding(Icone_textBox, 100);
                        Info.SetError(Icone_textBox, $@"Couldn't identify Series \ Movie name.{Environment.NewLine}Please input name manually{Environment.NewLine}For Movies please provide a year.{Environment.NewLine}{Environment.NewLine}Exp:{Environment.NewLine}MovieName.1970{Environment.NewLine}MovieName 1970");
                    }

                    else
                    {
                        Size = new Size(388, 301);
                        pictureBox1.Location = new Point(50, 116);
                        Reset_Folder.Location = new Point(193, 221);
                        Set.Location = new Point(275, 221);
                        Movie_radioButton.CheckedChanged -= Movie_radioButton_CheckedChanged;
                        Movie_radioButton.Checked = false;
                        Movie_radioButton.CheckedChanged += Movie_radioButton_CheckedChanged;
                        TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
                        TV_radioButton.Checked = false;
                        TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;
                        Movie_radioButton.Visible = true;
                        TV_radioButton.Visible = true;
                        pictureBox1.Image = null;
                    }

                    break;
            }
        }

        private void GetYear(string video)
        {
            var match = Regex.Match(video, @"(19|20)[0-9][0-9]");
            _year = match.Success ? int.Parse(match.Value) : 0;

        }

        private static string ExtractvideoName(string videoName)
        {
            var tmpName = "";
            var yearMatche = Regex.Match(videoName.ToLower(), ".(19|20)[0-9][0-9]");
            if (yearMatche.Success) return videoName.Substring(0, videoName.IndexOf(yearMatche.Value, StringComparison.Ordinal));
            var sEmatch = Regex.Match(videoName, ".[sS][0-9]{2}[eE][0-9]{2}");
            if (sEmatch.Success) return videoName.Substring(0, videoName.IndexOf(sEmatch.Value, StringComparison.Ordinal));
            var format = SercheMatch(tmpName, Matches.FormatRegex);
            tmpName = tmpName.Substring(0, tmpName.IndexOf(format, StringComparison.Ordinal));
            return tmpName.EndsWith(".") ? tmpName.Substring(0, tmpName.Length - 1) : tmpName;
        }

        private string GetVideoName()
        {
            var di = new DirectoryInfo(Folder_textBox.Text);

            string video = di.GetFiles("*", SearchOption.AllDirectories).Select(fi => fi.Name).FirstOrDefault(name => GetExtension(name) == ".mp4" || GetExtension(name) == ".mkv" || GetExtension(name) == ".avi" || GetExtension(name) == ".srt");
            if (video == null) return "";
            GetYear(video);
            var videoName = ExtractvideoName(GetFileNameWithoutExtension(video));
            return videoName;
        }

        private static string SercheMatch(string input, string pattern)
        {
            var tmp = @"";
            var index = new List<int>();
            foreach (Match match in Regex.Matches(input.ToLower(), pattern.ToLower(), RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline))
            {
                tmp = match.Value;
                index.Add(input.ToLower().IndexOf(tmp, StringComparison.Ordinal));
            }
            if (index.Count == 0) return "";
            var size = index[index.Count - 1] + tmp.Length + 1 - index[0];
            if (size - input.Length == 1 || size - input.Length == 0) return input;
            return index.Count > 1 ? input.Substring(index[0], size) : tmp;
        }


        private static string CleanString(string str, bool clean = true)
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

        private void Set_Click(object sender, EventArgs e)
        {
            Folder_Error.Clear();
            Info.Clear();
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
                    var filePaths = Directory.GetFiles(Folder_textBox.Text);
                    var iconfile = "";
                    var ini = $@"{Folder_textBox.Text}\desktop.ini";
                    const uint fcsForcewrite = 0x00000002;
                    if (Folder_textBox.Text.Length <= 0) return;
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
                            if (!GetFileName(iconfile).Equals(GetFileName(filePath))) continue;
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

                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
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
                    Process.Start("ie4uinit.exe", "-show");


                    //ShellNotification.refreshThumbnail(Folder_textBox.Text);
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
            Info.Clear();
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
                        if (!GetFileName(iconfile).Equals(GetFileName(filePath))) continue;
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
                    MessageBox.Show(@"Folder reset to default", @"Done");
                }
                else MessageBox.Show(@"Folder is already at default", @"Nothing To Do");
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CeckUpdate.RunWorkerAsync(true);
        }


        private void IconChange_Shown(object sender, EventArgs e)
        {
            CeckUpdate.RunWorkerAsync(false);
        }


        private void TV_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            var found = false;
            pictureBox1.Visible = true;
            pictureBox1.Image = Properties.Resources.loading;
            var original = Icone_textBox.Text.Replace(_year.ToString(), string.Empty).Trim('.').Trim();
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
                if (!FullMatch(video.Name.ToLower(), original.ToLower())) continue;
                IMDB_backgroundWorker.RunWorkerAsync(video.PosterPath);
                found = true;
                break;
            }

            if (found) return;
            MessageBox.Show(@"Couldn't Find The Requested Artwork", @"Sorry");
            pictureBox1.Image = null;
            TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
            TV_radioButton.Checked = false;
            TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;
        }

        private void Movie_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            var found = false;
            pictureBox1.Visible = true;
            pictureBox1.Image = Properties.Resources.loading;
            var original = Icone_textBox.Text.Replace(_year.ToString(), string.Empty).Trim('.').Trim();
            var iconName = CleanString(original.Trim());
            var results = Tmdb.SearchMovieAsync(iconName, 0, false, _year).Result;
            if (results.TotalResults < 1)
            {
                MessageBox.Show(@"Couldn't Find The Requested Artwork", @"Sorry");
                pictureBox1.Image = null;
                Movie_radioButton.CheckedChanged -= Movie_radioButton_CheckedChanged;
                Movie_radioButton.Checked = false;
                Movie_radioButton.CheckedChanged += Movie_radioButton_CheckedChanged;
                return;

            }
            foreach (var video in results.Results)
            {
                if (!FullMatch(video.Title.ToLower(), original.ToLower())) continue;
                IMDB_backgroundWorker.RunWorkerAsync(video.PosterPath);
                found = true;
                break;
            }

            if (found) return;
            MessageBox.Show(@"Couldn't Find The Requested Artwork", @"Sorry");
            pictureBox1.Image = null;
            TV_radioButton.CheckedChanged -= TV_radioButton_CheckedChanged;
            TV_radioButton.Checked = false;
            TV_radioButton.CheckedChanged += TV_radioButton_CheckedChanged;
        }

        private static bool FullMatch(string videoTitle, string original)
        {
            if (videoTitle.Length > original.Length) return false;
            var strArray1 = Regex.Split(videoTitle, "[^a-zA-Z0-9]+");
            var strArray2 = Regex.Split(original, "[^a-zA-Z0-9]+");
            var num1 = Math.Max(strArray1.Length, strArray2.Length);
            var num2 = 0.0;
            foreach (var t in strArray1)
            {
                if (strArray2.Contains(t))
                    ++num2;
            }
            return num2 / num1 * 100.0 > 75.0;
        }

        private void ProcessItem(string iconPath)
        {
            var icofile = GetTempPath() + ChangeExtension(GetFileName(iconPath), "png");
            var extension = GetExtension(iconPath);
            var destfile = ChangeExtension(icofile, GetExtension(icofile));
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
                Pics.Add(ChangeExtension(destfile, "ico"));
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

        private void IMDB_backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var original = Icone_textBox.Text.Replace(_year.ToString(), string.Empty).Trim('.').Trim();
            var iconName = CleanString(original.Trim());
            var iconPath = $@"{Folder_textBox.Text}\{CleanString(iconName, false)}.jpg";
            if (Exists(iconPath)) Delete(iconPath);
            using (var client = new WebClient())
            {
                client.DownloadFile(BaseUrl + e.Argument, iconPath);
            }
            SetAttributes(iconPath, GetAttributes(iconPath) | FileAttributes.Hidden);
            if (!Pics.Contains(iconPath)) Pics.Add(iconPath);
            ProcessItem(iconPath);
            if (Icone_textBox.InvokeRequired)
            {
                Icone_textBox.Invoke(new MethodInvoker(delegate { Icone_textBox.Text = iconPath; }));
            }
        }

        private void Local_backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var filename = e.Argument.ToString();
            var icofile = GetTempPath() + ChangeExtension(GetFileName(filename), "png");
            Icone_textBox.Invoke(new MethodInvoker(delegate { Icone_textBox.Text = filename; }));
            var file = GetFileNameWithoutExtension(e.Argument.ToString());
            var dir = GetDirectoryName(filename);
            var extension = GetExtension(filename);
            {
                var name = $@"{dir}\{file + _mRnd}{extension}";
                var viewer = new Viewer(name);
                var destfile = ChangeExtension(icofile, GetExtension(filename));
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
                        Copy(filename, destfile, true);
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
                    var iconPick = new IconPickerDialog { FileName = filename };
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
                    if (!Pics.Contains(destfile)) Pics.Add(destfile);
                    if (!Pics.Contains(ChangeExtension(destfile, "ico"))) Pics.Add(ChangeExtension(destfile, "ico"));
                    if (!Pics.Contains(ChangeExtension(destfile, "png"))) Pics.Add(ChangeExtension(destfile, "png"));
                    using (var temp = new Bitmap(destfile))
                        pictureBox1.Image = new Bitmap(temp);
                }

                else
                {
                    if (!extension.Equals(".dll"))
                    {
                        destfile = filename;
                        var img = Image.FromFile(destfile);
                        pictureBox1.Image = img;
                    }
                    else Icone_textBox.Text = "";
                }
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
                    if (Exists(pic)) Delete(pic);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void CeckUpdate_DoWork(object sender, DoWorkEventArgs e)
        {

            {
                var downloadUrl = @"";
                Version newVersion = null;
                XElement change = null;
                const string xmlUrl = @"https://oribenhur.github.io/update.xml";
                var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
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
                        Process.Start(downloadUrl);
                }
                else
                {
                    if ((bool)e.Argument)
                        MessageBox.Show(@"You Are Running The Last Version.", @"No New Updates");
                }
            }
        }
    }
}
