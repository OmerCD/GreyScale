using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ContourAnalysisNS;
using MessageBox = System.Windows.MessageBox;

namespace AForge.Wpf
{
    class TemplateProperties
    {
        public static void GetAllSavedTemplates(out string[] imagePaths,out string[] names,out string[] staffIds,out string[] iDs)
        {
            var iP = new List<string>();
            var n = new List<string>();
            var sI = new List<string>();
            var i = new List<string>();
            var dbConnection = new SQLiteConnection(DatabaseManagement.ConnectionString);
            dbConnection.Open();
            using (var cmd = new SQLiteCommand("Select * From Templates", dbConnection))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    
                    while (rdr.Read())
                    {
                        var path = Application.StartupPath + "\\SavedTemplates\\";
                        var id = rdr["Id"].ToString();
                        var name = rdr["Name"].ToString();
                        var stuffId = rdr["StuffId"].ToString();
                        path += id+"\\image.png";
                        iP.Add(path);
                        n.Add(name);
                        sI.Add(stuffId);
                        i.Add(id);
                    }
                }
            }
            dbConnection.Close();
            imagePaths = iP.ToArray();
            names = n.ToArray();
            staffIds = sI.ToArray();
            iDs = i.ToArray();
        }
        private void CreateFiles(int id, CroppedBitmap image,Templates templates)
        {
            bool CreateDirectory(string directoryPath) {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    return true;
                }
                return false;
            }
            var path = Application.StartupPath+"\\SavedTemplates";
            CreateDirectory(path);
            path += $"\\{id}\\";
            if (CreateDirectory(path))
            {
                SaveCroppedBitmap(image, path + "image.png");
                SaveTemplates(templates, path + "templates.bin");
            }
        }
        void SaveCroppedBitmap(CroppedBitmap image, string path)
        {
            FileStream mStream = new FileStream(path, FileMode.Create);
            JpegBitmapEncoder jEncoder = new JpegBitmapEncoder();
            jEncoder.Frames.Add(BitmapFrame.Create(image));
            jEncoder.Save(mStream);
        }
        private void SaveTemplates(Templates templates,string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    new BinaryFormatter().Serialize(fs, templates);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void AddTemplate(CroppedBitmap image, string name, string stuffId, Templates templates)
        {
            var dbConnection = new SQLiteConnection(DatabaseManagement.ConnectionString);
            dbConnection.Open();
            using (var cmd = new SQLiteCommand("Insert Into Templates(Name,StuffId) VALUES (@N,@SI);select last_insert_rowid();", dbConnection))
            {
                cmd.Parameters.AddWithValue("@N", name);
                cmd.Parameters.AddWithValue("@SI", stuffId);
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                CreateFiles(id,image,templates);
            }
            dbConnection.Close();
        }
        public void UpdateTemplate(string name, string stuffId, int id)
        {
            var dbConnection = new SQLiteConnection(DatabaseManagement.ConnectionString);
            dbConnection.Open();
            using (var cmd = new SQLiteCommand("Update Templates Set ImagePath = @IP,Name = @N ,StuffId=@SI,TemplatePath = @TP Where Id = @Id", dbConnection))
            {
                cmd.Parameters.AddWithValue("@N", name);
                cmd.Parameters.AddWithValue("@SI", stuffId);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }

        public void DeleteTemplateFromDatabase(int id)
        {
            var dbConnection = new SQLiteConnection(DatabaseManagement.ConnectionString);
            dbConnection.Open();
            using (var cmd = new SQLiteCommand("Delete from Templates Where Id = @Id", dbConnection))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            dbConnection.Close();
            
        }

        public void DeleteTemplateFromDirectory(int id)
        {
            var path = Application.StartupPath + "\\SavedTemplates\\" + id;
            new DirectoryInfo(path).Delete(true);
        }
        //public string ImagePath { get; set; }
        //public string Name { get; set; }
        //public string StuffId { get; set; }
        //public string TemplatePath { get; set; }
        //public int Id { get; set; }
    }
}
