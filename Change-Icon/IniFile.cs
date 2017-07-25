using System.Runtime.InteropServices;
using System.Text;

namespace Change_Icon
{
    internal class IniFile
    {
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
            FilePath = filePath;
        }

        public string Read(string section, string key)
        {
            var sb = new StringBuilder(255);
            var unused = GetPrivateProfileString(section, key, "", sb, 255, FilePath);
            return sb.ToString();
        }

        private string FilePath { get; set; }
    }
}