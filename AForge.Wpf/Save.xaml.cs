using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Wpf.LanguageLocalization;
using ContourAnalysisNS;
using Emgu.CV;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for Save.xaml
    /// </summary>
    public partial class Save
    {
        private readonly CroppedBitmap _image;
        private readonly Templates _templates;
        private List<Contour<System.Drawing.Point>> _contours;
        private double _strokeThickness;

        public Save(CroppedBitmap image, Templates templates, List<Contour<System.Drawing.Point>> contours,double strokeThickness)
        {
            InitializeComponent();
            _image = image;
            _templates = templates;
            _contours = contours;
            _strokeThickness = strokeThickness;
        }

        private void Kayıt_Click(object sender, RoutedEventArgs e)
        {
            if (TxtName.Text.Length < 2 || TxtStuffId.Text.Length < 1)
            {
                MessageBox.Show(ResLocalization.WrongEnter, ResLocalization.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var testImage = (ImageSource) _image;
                SaveImageWindows frm = new SaveImageWindows(testImage,_contours,_strokeThickness,new System.Drawing.Size(_image.PixelWidth,_image.PixelHeight));
                frm.ShowDialog();
                var tP = new TemplateProperties();
                tP.AddTemplate(frm.Image, TxtName.Text, TxtStuffId.Text, _templates);
                DialogResult = true;
                Close();
            }
        }
    }
}
