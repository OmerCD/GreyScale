using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Emgu.CV;
using Brushes = System.Windows.Media.Brushes;
using Size = System.Drawing.Size;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for SaveImageWindows.xaml
    /// </summary>
    public partial class SaveImageWindows : Window
    {
        private ImageSource _imageSource;
        private List<Contour<System.Drawing.Point>> _contours;
        private double _strokeThickness;
        public ImageSource Image;
        private System.Drawing.Size _size;

        public SaveImageWindows(ImageSource imageSource, List<Contour<System.Drawing.Point>> contours, double strokeThickness,Size size)
        {
            InitializeComponent();
            _imageSource = imageSource;
            _contours = contours;
            _strokeThickness = strokeThickness;
            _size = size;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            SaveImage.Source = _imageSource;
            Paint();
            await Task.Delay(2000);
            Image = GetSelectedImage();
            Close();
        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
        private ImageSource GetSelectedImage()
        {
            var pointCanvas = SaveImage.PointFromScreen(new System.Windows.Point(0, 0));
            var bitmap = new Bitmap(_size.Width, _size.Height);
            
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(-1*(int)pointCanvas.X, -1*(int)pointCanvas.Y, 0, 0, bitmap.Size);
            }
            return ImageSourceForBitmap(bitmap);
        }
        void Paint()
        {
            if (_contours == null)
            {
                return;
            }
            foreach (var contour in _contours)
            {
                if (contour.Total > 1)
                {
                    var contourArray = contour.ToArray();
                    foreach (var point in contourArray)
                    {
                        var positionBuffer = _strokeThickness * 0.08;
                        Line line = new Line
                        {
                            StrokeThickness = _strokeThickness,
                            Stroke = Brushes.Red,
                            X1 = point.X - positionBuffer,
                            X2 = point.X + positionBuffer,
                            Y1 = point.Y - positionBuffer,
                            Y2 = point.Y + positionBuffer
                        };
                        ImageCanvas.Children.Add(line);
                    }
                }
            }
        }
    }
}
