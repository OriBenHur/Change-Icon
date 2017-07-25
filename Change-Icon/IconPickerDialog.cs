using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace Change_Icon
{
    public class IconPickerDialog : CommonDialog
    {
        private static class NativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "#62", CharSet = CharSet.Unicode, SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            public static extern bool SHPickIconDialog(
                IntPtr hWnd, StringBuilder pszFilename, int cchFilenameMax, out int pnIconIndex);
        }

        private const int MaxPath = 260;

        [DefaultValue(default(string))]
        public string FileName
        {
            get;
            set;
        }

        [DefaultValue(0)]
        public int IconIndex
        {
            get;
            set;
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            var buf = new StringBuilder(FileName, MaxPath);
            int index;

            var ok = NativeMethods.SHPickIconDialog(hwndOwner, buf, MaxPath, out index);
            if (!ok) return false;
            FileName = Environment.ExpandEnvironmentVariables(buf.ToString());
            IconIndex = index;
            return true;
        }

        public override void Reset()
        {
            FileName = null;
            IconIndex = 0;
        }
    }
}
