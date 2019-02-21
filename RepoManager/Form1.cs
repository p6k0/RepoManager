using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RepoManager
{
    public partial class Form1 : Form
    {
        int RepoId;
        string FilePath = string.Empty;
        public Form1(int RepoId=-1)
        {
            this.RepoId = RepoId;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog f = new OpenFileDialog())
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                FilePath = f.FileName;
                FileNameLbl.Text = Path.GetFileName(FilePath);
                FileVersionInfo fv = FileVersionInfo.GetVersionInfo(f.FileName);
                RepoNameTbx.Text = fv.ProductName;
                RepoVerTbx.Text = fv.ProductVersion;
                FileSizeLbl.Text = new System.IO.FileInfo(f.FileName).Length.ToString();
                using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(f.FileName))
                    pictureBox1.Image = ico.ToBitmap();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (MySqlCommand cmd = Program.conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = RepoId == -1 ?
                    "INSERT INTO `data`(`Name`, `FileName`, `Description`, `Size`, `Version`) VALUES (@Name,@FileName,@Description,@Size,@Version);" :
                    "UPDATE `data` SET `Name`=@Name,`FileName`=@FileName,`Description`=@Description,`Size`=@Size,`Version`=@Version,`Added`=NOW() WHERE `Id`=@Id";

                cmd.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.Text) { Value=RepoNameTbx.Text });
                cmd.Parameters.Add(new MySqlParameter("@FileName", MySqlDbType.Text) { Value = FileNameLbl.Text });
                cmd.Parameters.Add(new MySqlParameter("@Description", MySqlDbType.Text) { Value = RepoDescrTbx.Text });
                cmd.Parameters.Add(new MySqlParameter("@Size", MySqlDbType.Int32) { Value = new FileInfo(FilePath).Length });
                cmd.Parameters.Add(new MySqlParameter("@Version", MySqlDbType.Text) { Value = RepoVerTbx.Text });
                cmd.Parameters.Add(new MySqlParameter("@Id", MySqlDbType.Int32) { Value = RepoId });

                cmd.ExecuteNonQuery();

                if (RepoId == -1) {
                    cmd.CommandText = "Select Id from data where name = @Name";
                    RepoId = (int)cmd.ExecuteScalar();
                }
                File.Copy(FilePath, @"C:\web\data\repos\" + RepoId, true);
                pictureBox1.Image.Save(@"C:\web\webui\repos\icons\" + RepoId + ".png", System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show(RepoId.ToString());

            }
        }
    }
}
