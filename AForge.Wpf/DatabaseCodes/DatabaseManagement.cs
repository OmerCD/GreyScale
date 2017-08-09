using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AForge.Wpf
{
    class DatabaseManagement
    {
        private string DatabaseFilePath => Application.StartupPath + "\\greyScaleDb.sqlite";
        private bool CheckDatabaseFile=> File.Exists(DatabaseFilePath);
        public static string ConnectionString;

        public void CreateDatabase()
        {
            ConnectionString = $"Data Source = {DatabaseFilePath};";
            var dbConnection = new SQLiteConnection(ConnectionString);
            var tables = new [,]
            {
                {"Templates","CREATE TABLE `Templates` (`Name`TEXT NOT NULL,`StuffId`TEXT NOT NULL UNIQUE,`Id`INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT)" },
                {"SoftwareOptions","CREATE TABLE SoftwareOptions (Language TEXT NOT NULL); Insert Into SoftwareOptions(Language) VALUES ('en-GB')" }
            };
            if (!CheckDatabaseFile)
            {
                CreateDatabaseFile();
            }
            dbConnection.Open();
            for (int i = 0; i < tables.GetLength(0); i++)
            {
                using (var cmd = new SQLiteCommand($"Select Count(*) From sqlite_master Where type='table' And name = '{tables[i,0]}'",dbConnection))
                {
                    if (Convert.ToBoolean( cmd.ExecuteScalar())) continue;
                    cmd.CommandText = tables[i, 1];
                    cmd.ExecuteNonQuery();
                }
            }
            dbConnection.Close();
        }

        private void CreateDatabaseFile()
        {
            SQLiteConnection.CreateFile(DatabaseFilePath);
        }
    }
}
