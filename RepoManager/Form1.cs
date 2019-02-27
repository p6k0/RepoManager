using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RepoManager
{
    public partial class Form1 : Form
    {
        int RepoId;
        string FilePath = string.Empty;
        public Form1(int RepoId = -1, string file = null)
        {
            this.RepoId = RepoId;

            InitializeComponent();
            if (RepoId != -1)
            {
                button2.Text = "Сохранить";
                using (MySqlCommand cmd = Program.conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT  `Name`, `FileName`, `Size`, `Version` FROM `data` WHERE Id=" + RepoId;
                    MySqlDataReader r = cmd.ExecuteReader();
                    if (r.HasRows)
                    {
                        r.Read();
                        RepoNameTbx.Text = r.GetString(0);
                        FileNameLbl.Text = r.GetString(1);
                        FileSizeLbl.Text = r.GetInt32(2).ToString();
                        RepoVerTbx.Text = r.GetString(3);
                        using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Program.IconsPath + RepoId + ".png")))
                            pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                
            }
                if (file != null)
                {
                    FilePath = file;
                    updateFileInfo();
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog f = new OpenFileDialog())
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                FilePath = f.FileName;
                updateFileInfo();
            }
        }

        private void updateFileInfo()
        {
            FileNameLbl.Text = Path.GetFileName(FilePath);
            FileVersionInfo fv = FileVersionInfo.GetVersionInfo(FilePath);
            RepoNameTbx.Text = fv.ProductName;
            RepoVerTbx.Text = fv.ProductVersion;
            FileSizeLbl.Text = new System.IO.FileInfo(FilePath).Length.ToString();
            using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(FilePath))
                pictureBox1.Image = ico.ToBitmap();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (MySqlCommand cmd = Program.conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = RepoId == -1 ?
                    "INSERT INTO `data`(`Name`,FileName, `Size`, `Version`) VALUES (@Name,@FileName,@Size,@Version);" :
                    "UPDATE `data` SET `Name`=@Name,`FileName`=@FileName,`Size`=@Size,`Version`=@Version,`Added`=NOW() WHERE `Id`=@Id";

                cmd.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.Text) { Value = RepoNameTbx.Text });
                cmd.Parameters.Add(new MySqlParameter("@FileName", MySqlDbType.Text) { Value = FileNameLbl.Text });
                cmd.Parameters.Add(new MySqlParameter("@Size", MySqlDbType.Int32) { Value = Convert.ToInt32(FileSizeLbl.Text) });
                cmd.Parameters.Add(new MySqlParameter("@Version", MySqlDbType.Text) { Value = RepoVerTbx.Text });
                cmd.Parameters.Add(new MySqlParameter("@Id", MySqlDbType.Int32) { Value = RepoId });

                cmd.ExecuteNonQuery();

                if (RepoId == -1)
                {
                    cmd.CommandText = "Select Id from data where name = @Name";
                    RepoId = (int)cmd.ExecuteScalar();
                }
                if (FilePath != string.Empty)
                    File.Copy(FilePath, Program.FilesPath + RepoId, true);
                pictureBox1.Image.Save(Program.IconsPath + RepoId + ".png", System.Drawing.Imaging.ImageFormat.Png);
                Close();

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog f = new OpenFileDialog())
            {
                f.Filter = "Изображения| *.BMP; *.JPG; *.JPEG; *.PNG; *.GIF";
                if (f.ShowDialog() == DialogResult.OK)
                    pictureBox1.Image = new Bitmap(  Image.FromFile(f.FileName),32,32);
            }
        }
    }
}
