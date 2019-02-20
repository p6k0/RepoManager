using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace RepoManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog f = new OpenFileDialog())
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                FileNameLbl.Text = f.FileName;
                FileVersionInfo fv = FileVersionInfo.GetVersionInfo(f.FileName);
                RepoNameTbx.Text = fv.ProductName;
                RepoVerTbx.Text = fv.ProductVersion;
                using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(f.FileName))
                    pictureBox1.Image = ico.ToBitmap();
            }
        }
    }
}
