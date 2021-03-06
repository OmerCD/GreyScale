﻿using System;
using System.Data.Entity.Core.Objects;
using System.Data.SQLite;
using ContourAnalysisNS;

namespace AForge.Wpf.DatabaseCodes
{
    class ContourOptions
    {
        private bool _equalizeHist;
        private bool _maxRotateAngle;
        private int _minContourArea;
        private int _minContourLength;
        private int _maxAcfDescriptorDeviation;
        private double _minAcf;
        private double _minIcf;
        private bool _blur;
        private bool _noiseFilter;
        private int _cannyThreshold;
        private int _adaptiveThresholdBlockSize;
        private bool _adaptiveNoiseFilter;

        public static ImageProcessor Processor
        {
            get
            {
                var proc = new ImageProcessor();
                var cO = new ContourOptions();
                cO.LoadSavedOptions();

                var rotateAngle = cO.MaxRotateAngle ? System.Math.PI : System.Math.PI / 4;
                var noiseFilter = cO.AdaptiveNoiseFilter ? 1.5 : 0.5;

                proc.adaptiveThresholdBlockSize = cO.AdaptiveThresholdBlockSize;
                proc.adaptiveThresholdParameter = noiseFilter;
                proc.equalizeHist = cO.EqualizeHist;
                proc.finder.maxRotateAngle = rotateAngle;
                proc.minContourArea = cO.MinContourArea;
                proc.minContourLength = cO.MinContourLength;
                proc.finder.maxACFDescriptorDeviation = cO.MaxAcfDescriptorDeviation;
                proc.finder.minACF = cO.MinAcf;
                proc.finder.minICF = cO.MinIcf;
                proc.blur = cO.Blur;
                proc.cannyThreshold = cO.CannyThreshold;
                return proc;
            }
        }
        public static void SaveOption<T>(string columnName, T value)
        {

            var con = new SQLiteConnection(DatabaseManagement.ConnectionString);
            con.Open();
            using (var cmd = new SQLiteCommand($"Update ContourOptions Set {columnName} = @V", con))
            {
                cmd.Parameters.AddWithValue("@V", value);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static T GetOption<T>(string columnName)
        {
            var con = new SQLiteConnection(DatabaseManagement.ConnectionString);
            con.Open();
            object returnObject;
            using (var cmd = new SQLiteCommand($"Select {columnName} From ContourOptions", con))
            {
                returnObject = cmd.ExecuteScalar();
            }
            con.Close();
            if (returnObject == null)
            {
                return (T)Convert.ChangeType(new object(), typeof(T));
            }
            return (T)Convert.ChangeType(returnObject, typeof(T));
        }
        public void SaveOptions(bool equalizeHist, bool maxRotateAngle, int minContourArea, int minContourLength, int maxAcfDescriptorDeviation, double minAcf, double minIcf, bool blur, bool noiseFilter, int cannyThreshold, int adaptiveThresholdBlockSize, bool adaptiveNoiseFilter)
        {
            _equalizeHist = equalizeHist;
            _maxRotateAngle = maxRotateAngle;
            _minContourArea = minContourArea;
            _minContourLength = minContourLength;
            _maxAcfDescriptorDeviation = maxAcfDescriptorDeviation;
            _minAcf = minAcf;
            _minIcf = minIcf;
            _blur = blur;
            _noiseFilter = noiseFilter;
            _cannyThreshold = cannyThreshold;
            _adaptiveThresholdBlockSize = adaptiveThresholdBlockSize;
            _adaptiveNoiseFilter = adaptiveNoiseFilter;
            SaveContourOptions();
        }

        private void SaveContourOptions()
        {
            var con = new SQLiteConnection(DatabaseManagement.ConnectionString);
            con.Open();
            using (var cmd = new SQLiteCommand("UPDATE ContourOptions SET EqualizeHist=@equalizeHist, MaxRotateAngle=@maxRotateAngle, MinContourArea=@minContourArea, MinContourLength=@minContourLength, MaxACFDescriptorDeviation=@maxACFDescriptorDeviation, MinACF=@minACF, MinICF=@minICF, NoiseFilter=@noiseFilter, CannyThreshold=@cannyThreshold, AdaptiveThresholdBlockSize=@adaptiveThresholdBlockSize, AdaptiveThresholdParameter=@adaptiveThresholdParameter", con))
            {
                cmd.Parameters.AddWithValue("@equalizeHist", _equalizeHist);
                cmd.Parameters.AddWithValue("@maxRotateAngle", _maxRotateAngle);
                cmd.Parameters.AddWithValue("@minContourArea", _minContourArea);
                cmd.Parameters.AddWithValue("@minContourLength", _minContourLength);
                cmd.Parameters.AddWithValue("@maxACFDescriptorDeviation", _maxAcfDescriptorDeviation);
                cmd.Parameters.AddWithValue("@minACF", _minAcf);
                cmd.Parameters.AddWithValue("@minICF", _minIcf);
                cmd.Parameters.AddWithValue("@noiseFilter", _noiseFilter);
                cmd.Parameters.AddWithValue("@cannyThreshold", _cannyThreshold);
                cmd.Parameters.AddWithValue("@adaptiveThresholdBlockSize", _adaptiveThresholdBlockSize);
                cmd.Parameters.AddWithValue("@adaptiveThresholdParameter", _adaptiveNoiseFilter);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public bool EqualizeHist => _equalizeHist;
        public bool MaxRotateAngle => _maxRotateAngle;
        public int MinContourArea => _minContourArea;
        public int MinContourLength => _minContourLength;
        public int MaxAcfDescriptorDeviation => _maxAcfDescriptorDeviation;
        public double MinAcf => _minAcf;
        public double MinIcf => _minIcf;
        public bool Blur => _blur;
        public bool NoiseFilter => _noiseFilter;
        public int CannyThreshold => _cannyThreshold;
        public int AdaptiveThresholdBlockSize => _adaptiveThresholdBlockSize;
        public bool AdaptiveNoiseFilter => _adaptiveNoiseFilter;

        public void LoadSavedOptions()
        {
            var con = new SQLiteConnection(DatabaseManagement.ConnectionString);
            con.Open();
            using (var cmd = new SQLiteCommand("Select * From ContourOptions",con))
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        _equalizeHist = Convert.ToBoolean(rdr["EqualizeHist"]);
                        _maxRotateAngle = Convert.ToBoolean(rdr["MaxRotateAngle"]);
                        _minContourArea = Convert.ToInt32(rdr["MinContourArea"]);
                        _minContourLength = Convert.ToInt32(rdr["MinContourLength"]);
                        _maxAcfDescriptorDeviation= Convert.ToInt32(rdr["MaxACFDescriptorDeviation"]);
                        _cannyThreshold= Convert.ToInt32(rdr["CannyThreshold"]);
                        _adaptiveThresholdBlockSize= Convert.ToInt32(rdr["AdaptiveThresholdBlockSize"]);
                        _minAcf = Convert.ToDouble(rdr["MinACF"]);
                        _minIcf= Convert.ToDouble(rdr["MinICF"]);
                        _blur = Convert.ToBoolean(rdr["Blur"]);
                        _noiseFilter = Convert.ToBoolean(rdr["NoiseFilter"]);
                        _adaptiveNoiseFilter = Convert.ToBoolean(rdr["AdaptiveThresholdParameter"]);
                    }
                }
            }
            con.Close();
        }
    }
}
