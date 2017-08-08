using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using ContourAnalysisNS;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;
using Size = System.Drawing.Size;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Public properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; OnPropertyChanged("CurrentDevice"); }
        }

        private FilterInfo _currentDevice;

        #endregion


        #region Private fields

        private bool Recognition
        {

            set
            {
                RecognitionAnimation(value);
                _recognition = value;
                if (!_recognition)
                {
                    _dispatcherTimer.Stop();

                }
                else
                {
                    _dispatcherTimer.Start();

                }
                SetButtonsActivity(!value);
            }
        }

        private double _strokeThickness;

        private bool _recognition;
        private VideoCaptureDevice _videoSource;

        private Image<Bgr, Byte> _frame;
        private readonly ImageProcessor _processor;

        private CroppedBitmap _croppedImage;
        private BitmapImage _videoImage;
        private Templates _designedSamples;

        DispatcherTimer _dispatcherTimer = new DispatcherTimer();


        #endregion

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture =
             new System.Globalization.CultureInfo("en-GB");
            InitializeComponent();
            DataContext = this;
            GetVideoDevices();
            Closing += MainWindow_Closing;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _processor = new ImageProcessor();
            _designedSamples= new Templates();
            ApplySettings();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
        
                GetFoundTemplates();
            
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public BitmapSource ToBitmapSource(IImage image)
        {
            using (Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
        public Bitmap ToBitmap(BitmapSource bitmapsource)
        {
            using (var outStream = new MemoryStream())
            {
                // from System.Media.BitmapImage to System.Drawing.Bitmap
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                return new Bitmap(outStream);
            }
        }


        private void ProcessFrame(BitmapSource bSource,bool showSelectedAreaContourCount)
        {
            if (bSource == null || bSource.Height <= 1 || bSource.Width <= 1) return;
            _frame = new Image<Bgr, byte>(ToBitmap(bSource));
            _processor.ProcessImage(_frame);
            AlanSayisi.Text = "Kayıtlı Alan Sayısı :" + _processor.templates.Count;
            if(showSelectedAreaContourCount)
            ResimdekiAlanSayisi.Text = "Seçilen Kısımda Alan Sayısı :" + _processor.samples.Count;
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            StopCamera();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
            StartCamera();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            BitmapImage bi;
            using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                bi = bitmap.ToBitmapImage();
            }
            bi.Freeze(); // avoid cross thread operations and prevents leaks
            Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));
            _videoImage = bi;


        }

        private void GetFoundTemplates()
        {
            
            ProcessFrame(_videoImage,false);
            TxtMatches.Text = "Bulma Oranı : "+_processor.templates.Count + "/" + _processor.foundTemplates.Count;

        }

        private void SetButtonsActivity(bool state)
        {
            YeniButon.IsEnabled = state;
            BtnSave.IsEnabled = state;
            BtnAlanEkle.IsEnabled = state;
            SecimValue.IsEnabled = state;
            DogrulukValue.IsEnabled = state;
        }
        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }

        void Paint()
        {

            PaintCanvas.Children.Clear();
            if (_processor.contours==null)
            {
                return;
            }
            foreach (var contour in _processor.contours)
            {
                if (contour.Total > 1)
                {
                    var contourArray = contour.ToArray();
                    foreach (var point in contourArray)
                    {

                        Line line = new Line
                        {
                            StrokeThickness = _strokeThickness,
                            Stroke = Brushes.Red,
                            X1 = point.X - 0.1,
                            X2 = point.X + 0.1,
                            Y1 = point.Y - 0.1,
                            Y2 = point.Y + 0.1
                        };
                        PaintCanvas.Children.Add(line);
                    }
                }
            }
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
        private void BtnCrop_OnClick(object sender, RoutedEventArgs e)
        {
            //Crop();
            //AddCropToElement(videoPlayer);
            //RefreshCropImage();
        }

        private void Crop()
        {
            if (selectionRectangle != null)
            {
                if (selectionRectangle.Width == 0 || selectionRectangle.Height == 0)
                {
                    return;
                }
                var relativePoint = selectionRectangle.PointToScreen(new System.Windows.Point(0, 0));
                var imagePoint = videoPlayer.PointToScreen(new System.Windows.Point(0, 0));

                //var relativePoint = selectionRectangle.TransformToVisual(videoPlayer)
                //    .Transform(new System.Windows.Point(0, 0));
                if (relativePoint.X < imagePoint.X)
                {
                    var diff = imagePoint.X - relativePoint.X;
                    if (selectionRectangle.Width - diff < 1)
                    {
                        return;
                    }
                    relativePoint.X = imagePoint.X;
                    selectionRectangle.Width -= diff;
                }
                if (relativePoint.X + selectionRectangle.ActualWidth > imagePoint.X + videoPlayer.ActualWidth)
                {
                    var calc = videoPlayer.ActualWidth - (relativePoint.X - imagePoint.X);
                    if (calc < 1)
                    {
                        return;
                    }
                    selectionRectangle.Width = calc;
                }
                if (relativePoint.Y < imagePoint.Y)
                {
                    var diff = imagePoint.Y - relativePoint.Y;
                    if (selectionRectangle.Height - diff < 1)
                    {
                        return;
                    }
                    relativePoint.Y = imagePoint.Y;
                    selectionRectangle.Height -= diff;
                }
                if (relativePoint.Y + selectionRectangle.ActualHeight > imagePoint.Y + videoPlayer.ActualHeight)
                {
                    var calc = videoPlayer.ActualHeight - (relativePoint.Y - imagePoint.Y);
                    if (calc < 1)
                    {
                        return;
                    }
                    selectionRectangle.Height = calc;
                }
                relativePoint = new System.Windows.Point(relativePoint.X - imagePoint.X, relativePoint.Y - imagePoint.Y);

                Int32Rect rect = new Int32Rect((int)(relativePoint.X), (int)relativePoint.Y, (int)selectionRectangle.Width, (int)selectionRectangle.Height);
                try
                {
                    _croppedImage = new CroppedBitmap((BitmapSource)videoPlayer.Source, rect);
                }
                catch { }
                if (rect.Height<1 || rect.Width<1)
                {
                    return;
                }
                ImageBrush ib = new ImageBrush(_croppedImage);
                PaintCanvas.Background = ib;
                PaintCanvas.Height = rect.Height;
                PaintCanvas.Width = rect.Width;
                ProcessFrame(_croppedImage,true);
                Paint();
            }
        }

        private void SayiKontrol(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("^[0-9]([,][0-9])");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool _mouseDown; // Set to 'true' when mouse is held down.
        private System.Windows.Point _mouseDownPos; // The point where the mouse button was clicked down.
        private double _correctnessPercentage;

        private void VideoCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Capture and track the mouse.
            _mouseDown = true;
            _mouseDownPos = e.GetPosition(VideoCanvas);
            VideoCanvas.CaptureMouse();

            // Initial placement of the drag selection box.         
            Canvas.SetLeft(selectionRectangle, _mouseDownPos.X);
            Canvas.SetTop(selectionRectangle, _mouseDownPos.Y);
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
            _croppedImage = null;
            // Make the drag selection box visible.
            selectionRectangle.Visibility = Visibility.Visible;
        }

        private void VideoCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                // When the mouse is held down, reposition the drag selection box.

                System.Windows.Point mousePos = e.GetPosition(VideoCanvas);

                if (_mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(selectionRectangle, _mouseDownPos.X);
                    selectionRectangle.Width = mousePos.X - _mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(selectionRectangle, mousePos.X);
                    selectionRectangle.Width = _mouseDownPos.X - mousePos.X;
                }

                if (_mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(selectionRectangle, _mouseDownPos.Y);
                    selectionRectangle.Height = mousePos.Y - _mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(selectionRectangle, mousePos.Y);
                    selectionRectangle.Height = _mouseDownPos.Y - mousePos.Y;
                }
            }
        }

        private void ValueChange(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void CropWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            selectionRectangle.Visibility = Visibility.Collapsed;
        }

        private void VideoCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Release the mouse capture and stop tracking it.
            _mouseDown = false;
            VideoCanvas.ReleaseMouseCapture();
            Crop();

            // Hide the drag selection box.
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        private void SaveTemplates(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    new BinaryFormatter().Serialize(fs, _designedSamples);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
                //foreach (var sample in _processor.templates)
                //{
                //    sample.name = "test";
                //}
                ////ProcessFrame(_croppedImage);
                //SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Template|*bin",DefaultExt = "bin"};
                //if (saveFileDialog.ShowDialog() == true)
                //{
                //    SaveTemplates(saveFileDialog.FileName);
                //}
            var save = new Save();
            save.Show();

        }
        private void LoadTemplates(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    _processor.templates = (Templates)new BinaryFormatter().Deserialize(fs);
                AlanSayisi.Text = "Kayıtlı Alan Sayısı :" + _processor.templates.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            bool wasActiveBefore = false;
            //SampleSelection ss = new SampleSelection(_croppedImage,processor.contours);
            if (_recognition)
            {
                wasActiveBefore = true;
                Recognition = false;
            }
            ProcessFrame(_croppedImage,true);
            SampleSelection ss = new SampleSelection(_croppedImage, _processor.samples, _processor.contours,_strokeThickness);

            ss.ShowDialog();
            _designedSamples.Clear();
            _processor.contours = ss.Contours;
            _processor.samples = ss.Samples;
            _designedSamples.AddRange(ss.Samples);
            ResimdekiAlanSayisi.Text = "Seçilen Kısımda Alan Sayısı :" + _designedSamples.Count;
            Paint();
            if (wasActiveBefore)
            {
                Recognition = true;
            }
        }

        //private void BtnYükle_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var oPF = new OpenFileDialog { Filter = "Template|*bin" };
        //    if (oPF.ShowDialog() == true)
        //    {
        //        LoadTemplates(oPF.FileName);
        //        Paint();
        //    }
            
        //}

        void RecognitionAnimation(bool active)
        {
            if (active)
            {                
                var animation = new ColorAnimation
                {
                    From = Colors.Gray,
                    To = Colors.Red,
                    RepeatBehavior = RepeatBehavior.Forever,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };
                BtnRecognition.Background = new SolidColorBrush(Colors.Red);
                BtnRecognition.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                BtnRecognitinionImage.Source = new BitmapImage(new Uri(@"\Images\pause.png", UriKind.Relative));
            }
            else
            {
                BtnRecognition.Background = new SolidColorBrush(Colors.LightSkyBlue);
                BtnRecognition.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { BeginTime = null });
                BtnRecognition.Background = Brushes.LightSkyBlue;
                BtnRecognitinionImage.Source = new BitmapImage(new Uri(@"\Images\play.png",UriKind.Relative));
            }
        }
        private void BtnRecognition_OnClick(object sender, RoutedEventArgs e)
        {
            //Recognition frm = new Recognition();
            //frm.ShowDialog();
            
            
            Recognition = !_recognition;
            _processor.onlyFindContours = !_recognition;
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
                MinIcf.Text= "0.85";
                Blur.IsChecked = true;
                NoiseFilter.IsChecked = false;
                CannyThreshold.Text="50";
                AdaptiveThresholdBlockSize.Text="19";
                AdaptiveNoiseFilter.IsChecked =true; 
                ApplySettings();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void YeniButon_Click(object sender, RoutedEventArgs e)
        {
            _processor.templates=new Templates();
            _processor.foundTemplates= new List<FoundTemplateDesc>();
            _designedSamples = new Templates();
            AlanSayisi.Text = "Kayıtlı Alan Sayısı :0";
        }

        private void BtnAlanEkle_Click(object sender, RoutedEventArgs e)
        {
            Templates toSaveSamples;
            if (_designedSamples.Count > 0)
            {
                toSaveSamples = _designedSamples;
            }
            else if (_processor.samples.Count > 0)
            {
                toSaveSamples = _processor.samples;
            }
            else return;
            _processor.templates.AddRange(toSaveSamples);
            AlanSayisi.Text = "Kayıtlı Alan Sayısı :" + _processor.templates.Count;
        }

        private void SecimValue_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _strokeThickness = SecimValue.Value;
        }

        private void DogrulukValue_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _correctnessPercentage = DogrulukValue.Value;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var savedTemplates = new SavedTemplates();
            savedTemplates.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var languages = new Languages();
            languages.Show();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var videoSettings =new VideoSettings();
            videoSettings.Show();
        }
    }
}
