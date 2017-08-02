using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Change_Icon
{
    public partial class Technical_Details : Form
    {
        public Technical_Details(string appName, string version, string buildDate)
        {
            InitializeComponent();
            Text = $@"{appName} Technical Details";
            build_label.Text = buildDate;
            App_label.Text = appName;
            AppVersion_label.Text = version;
        }

        private void build_label_Click(object sender, EventArgs e)
        {

        }
    }
}
