using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for Recognition.xaml
    /// </summary>
    public partial class Recognition : Window
    {
        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public FilterInfo CurrentDevice
        {
            get => _currentDevice;
            set { _currentDevice = value; OnPropertyChanged("CurrentDevice"); }
        }
        private FilterInfo _currentDevice;
        private IVideoSource _videoSource;

        public Recognition()
        {
            InitializeComponent();
            DataContext = this;
            GetVideoDevices();
        }

        private void StartCamera()
        {
            if (CurrentDevice == null) return;
            _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
            _videoSource.NewFrame += Video_NewFrame;
            _videoSource.Start();
        }

        private void StopCamera()
        {
            if (_videoSource == null || !_videoSource.IsRunning) return;
            _videoSource.SignalToStop();
            _videoSource.NewFrame -= Video_NewFrame;
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            BitmapImage bi;
            using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                bi = bitmap.ToBitmapImage();
            }
            bi.Freeze(); // avoid cross thread operations and prevents leaks
            Dispatcher.BeginInvoke(new ThreadStart(delegate { VideoPlayer.Source = bi; }));
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
    }
}
