using System;
using System.Data.SQLite;
using System.IO;
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
                {"Templates","CREATE TABLE `Templates` (`Name` TEXT NOT NULL,`StuffId` TEXT NOT NULL UNIQUE,`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT)" },
                {"SoftwareOptions","CREATE TABLE `SoftwareOptions` (`Language` TEXT NOT NULL, `ContourThickness` REAL NOT NULL, `SuccessRate` INTEGER NOT NULL);" +
                                   " Insert Into SoftwareOptions VALUES ('en-GB',2.5,80)" },
                {"ContourOptions","CREATE TABLE `ContourOptions` ( `EqualizeHist` INTEGER NOT NULL, `MaxRotateAngle` INTEGER NOT NULL, `MinContourArea` INTEGER NOT NULL, `MinContourLength` INTEGER NOT NULL, `MaxACFDescriptorDeviation` INTEGER NOT NULL, `MinACF` REAL NOT NULL, `MinICF` REAL NOT NULL, `Blur` INTEGER NOT NULL, `NoiseFilter` INTEGER NOT NULL, `CannyThreshold` INTEGER NOT NULL, `AdaptiveThresholdBlockSize` INTEGER NOT NULL, `AdaptiveThresholdParameter` INTEGER NOT NULL);" +
                                  "Insert Into ContourOptions VALUES(0,1,70,70,4,0.96,0.85,1,0,50,19,1)" }
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
