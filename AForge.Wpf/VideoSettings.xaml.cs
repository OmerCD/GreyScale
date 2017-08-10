using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using AForge.Wpf.DatabaseCodes;
using ContourAnalysisNS;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for VideoSettings.xaml
    /// </summary>
    public partial class VideoSettings : Window
    {
        public ImageProcessor Processor;
        ContourOptions _contourOptions = new ContourOptions();

        public VideoSettings(ImageProcessor processor)
        {
            InitializeComponent();
            Processor = processor;
        }

        private void LoadSettingsFromDatabase()
        {
            _contourOptions.LoadSavedOptions();
            EqualizeHist.IsChecked = _contourOptions.EqualizeHist;
            MaxRotateAngle.IsChecked = _contourOptions.MaxRotateAngle;
            MinContourArea.Text = _contourOptions.MinContourArea.ToString();
            MinContourLength.Text = _contourOptions.MinContourLength.ToString();
            MaxAcfDescriptorDeviation.Text = _contourOptions.MaxAcfDescriptorDeviation.ToString();
            MinAcf.Text = _contourOptions.MinAcf.ToString();
            MinIcf.Text = _contourOptions.MinIcf.ToString();
            Blur.IsChecked = _contourOptions.Blur;
            NoiseFilter.IsChecked = _contourOptions.NoiseFilter;
            CannyThreshold.Text = _contourOptions.CannyThreshold.ToString();
            AdaptiveThresholdBlockSize.Text = _contourOptions.AdaptiveThresholdBlockSize.ToString();
            AdaptiveNoiseFilter.IsChecked = _contourOptions.AdaptiveNoiseFilter;
        }
        private void ApplySettings()
        {
            if (Processor == null)
            {
                return;

            }
            try
            {
                var rotateAngle = (bool) MaxRotateAngle.IsChecked ? System.Math.PI : System.Math.PI / 4;
                var noiseFilter = (bool)AdaptiveNoiseFilter.IsChecked ? 1.5 : 0.5;
                Processor.equalizeHist = (bool)EqualizeHist.IsChecked;
                Processor.finder.maxRotateAngle = rotateAngle;
                Processor.minContourArea = int.Parse(MinContourArea.Text);
                Processor.minContourLength = int.Parse(MinContourLength.Text);
                Processor.finder.maxACFDescriptorDeviation = int.Parse(MaxAcfDescriptorDeviation.Text);
                Processor.finder.minACF = Convert.ToDouble(MinAcf.Text.Replace('.',','));
                Processor.finder.minICF = Convert.ToDouble(MinIcf.Text.Replace('.',','));
                Processor.blur = (bool)Blur.IsChecked;
                Processor.noiseFilter = (bool)NoiseFilter.IsChecked;
                Processor.cannyThreshold = int.Parse(CannyThreshold.Text);
                Processor.adaptiveThresholdBlockSize = int.Parse(AdaptiveThresholdBlockSize.Text);
                Processor.adaptiveThresholdParameter = noiseFilter;

                _contourOptions.SaveOptions(Processor.equalizeHist,rotateAngle==System.Math.PI,Processor.minContourArea,Processor.minContourLength,Processor.finder.maxACFDescriptorDeviation,Processor.finder.minACF,Processor.finder.minICF,Processor.blur,Processor.noiseFilter,Processor.cannyThreshold,Processor.adaptiveThresholdBlockSize,noiseFilter==1.5);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnFactoryDefaults_Click(object sender, RoutedEventArgs e)
        {
            if (Processor == null)
            {
                return;
            }
            try
            {
                EqualizeHist.IsChecked = false;
                MaxRotateAngle.IsChecked = true;
                MinContourArea.Text = "70";
                MinContourLength.Text = "70";
                MaxAcfDescriptorDeviation.Text = "4";
                MinAcf.Text = "0,96";
                MinIcf.Text = "0,85";
                Blur.IsChecked = true;
                NoiseFilter.IsChecked = false;
                CannyThreshold.Text = "50";
                AdaptiveThresholdBlockSize.Text = "19";
                AdaptiveNoiseFilter.IsChecked = true;
                ApplySettings();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SayiKontrol(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("^[0-9]([,][0-9])");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettingsFromDatabase();
        }
    }
}
