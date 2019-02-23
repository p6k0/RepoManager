using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.Common;
using MySql.Data.MySqlClient;

namespace RepoManager
{
    static class Program
    {
        public const string
            FilesPath = @"C:\web\data\repos\",
            IconsPath = @"C:\web\webui\repos\icons\";
        public static MySqlConnection conn;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            conn = DBMySQLUtils.GetDBConnection("127.0.0.1", 3306, "repos", "root", "kandibober");
            conn.Open();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RepoList());
        }
    }
}
