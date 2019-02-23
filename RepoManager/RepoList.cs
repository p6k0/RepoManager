using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Forms;

namespace RepoManager
{
    public partial class RepoList : Form
    {
        public RepoList()
        {
            InitializeComponent();
            listBox1.DisplayMember = "Name";
            listBox1.ValueMember = "Id";
            UpdateList();
        }

        private void DeleteItem(int Id)
        {
            using (MySqlCommand cmd = Program.conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM `data` WHERE `Id`="+Id;
                cmd.ExecuteNonQuery();
            }
            File.Delete(Program.FilesPath + Id);
            File.Delete(Program.IconsPath + Id + ".png");
            UpdateList();
        }

        private void UpdateList()
        {
            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            using (MySqlCommand cmd = Program.conn.CreateCommand())
            {
                cmd.CommandText = "SELECT `Id`, `Name` FROM `data` WHERE 1";
                MySqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    Repo rep;
                    while (r.Read())
                    {
                        rep = new Repo(r.GetInt32(0), r.GetString(1));
                        listBox1.Items.Add(rep);
                    }
                }

            }

            listBox1.EndUpdate();
        }
        private class Repo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Repo(int Id, string Name)
            {
                this.Id = Id;
                this.Name = Name;
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            using (Form1 f = new Form1())
            {
                f.ShowDialog();
            }
            UpdateList();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
           if( MessageBox.Show(
                this,
                "Вы действительно желаете  удалить репозиторий \r\n\""+((Repo)listBox1.SelectedItem).Name+"\"?",
                "Удаление репозитория",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            )== DialogResult.Yes)
            {
                DeleteItem(((Repo)listBox1.SelectedItem).Id);
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            using (Form1 f = new Form1(((Repo)listBox1.SelectedItem).Id))
            {
                f.ShowDialog();
            }
            UpdateList();
        }

        private void RepoList_DragDrop(object sender, DragEventArgs e)
        {
            string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            using (Form1 f = new Form1(-1, file))
                f.ShowDialog();
            UpdateList();
        }

        private void RepoList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
           if(listBox1.SelectedIndex != -1)
            {
                button1.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void listBox1_DoubleClick(object sender, System.EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
                button3_Click(null, null);
        }
    }
}
