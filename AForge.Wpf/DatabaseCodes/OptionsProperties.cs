using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForge.Wpf.DatabaseCodes
{
    class OptionsProperties
    {
        public T GetOption<T>(string columnName) 
        {
            var con = new SQLiteConnection(DatabaseManagement.ConnectionString);
            con.Open();
            object returnObject;
            using (var cmd = new SQLiteCommand($"Select {columnName} From SoftwareOptions",con))
            {
                returnObject = cmd.ExecuteScalar();
            }
            con.Close();
            if (returnObject==null)
            {
                return (T)Convert.ChangeType(new object(), typeof(T));
            }
            return (T) Convert.ChangeType(returnObject, typeof(T));
        }

        public void SetOption<T>(string columnName, T value)
        {
            var con = new SQLiteConnection(DatabaseManagement.ConnectionString);
            con.Open();
            using (var cmd = new SQLiteCommand($"Update SoftwareOptions Set {columnName} = @V",con))
            {
                cmd.Parameters.AddWithValue("@V", value);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }
}
