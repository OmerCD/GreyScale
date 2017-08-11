using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Wpf.LanguageLocalization;
using ContourAnalysisNS;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for Save.xaml
    /// </summary>
    public partial class Save
    {
        private readonly CroppedBitmap _image;
        private readonly Templates _templates;
        public Save(CroppedBitmap image, Templates templates)
        {
            InitializeComponent();
            _image = image;
            _templates = templates;
        }

        private void Kayıt_Click(object sender, RoutedEventArgs e)
        {
            if (TxtName.Text.Length<2 || TxtStuffId.Text.Length<1)
            {
                MessageBox.Show(ResLocalization.WrongEnter, ResLocalization.Warning, MessageBoxButton.OK,MessageBoxImage.Warning);
                //*ToDo*
            }
            else
            {
                var tP = new TemplateProperties();
                tP.AddTemplate(_image,TxtName.Text,TxtStuffId.Text,_templates);
                DialogResult = true;
                Close();
            }
        }
    }
}
