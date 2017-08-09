using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ContourAnalysisNS;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for VideoSettings.xaml
    /// </summary>
    public partial class VideoSettings : Window
    {
        private ImageProcessor _processor;

        public VideoSettings()
        {
            InitializeComponent();
        }
        private void ApplySettings()
        {
            if (_processor == null)
            {
                return;

            }
            try
            {
                _processor.equalizeHist = (bool)EqualizeHist.IsChecked;
                _processor.finder.maxRotateAngle = (bool)MaxRotateAngle.IsChecked ? System.Math.PI : System.Math.PI / 4; //Checkbox
                _processor.minContourArea = int.Parse(MinContourArea.Text);
                _processor.minContourLength = int.Parse(MinContourLength.Text);
                _processor.finder.maxACFDescriptorDeviation = int.Parse(MaxAcfDescriptorDeviation.Text);
                _processor.finder.minACF = Convert.ToDouble(MinAcf.Text);
                _processor.finder.minICF = Convert.ToDouble(MinIcf.Text);
                _processor.blur = (bool)Blur.IsChecked;
                _processor.noiseFilter = (bool)NoiseFilter.IsChecked;
                _processor.cannyThreshold = int.Parse(CannyThreshold.Text);
                _processor.adaptiveThresholdBlockSize = int.Parse(AdaptiveThresholdBlockSize.Text);
                _processor.adaptiveThresholdParameter = (bool)AdaptiveNoiseFilter.IsChecked ? 1.5 : 0.5; //Checkbox
                //cam resolution
                //int camWidth = 1920;
                //int camHeight = 1080;
                //if (this.camHeight != camHeight || this.camWidth != camWidth)
                //{
                //    this.camWidth = camWidth;
                //    this.camHeight = camHeight;
                //    ApplyCamSettings();
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnFactoryDefaults_Click(object sender, RoutedEventArgs e)
        {
            if (_processor == null)
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
                MinAcf.Text = "0.96";
                MinIcf.Text = "0.85";
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
    }
}
