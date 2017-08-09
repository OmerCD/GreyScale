using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
                //MessageBox Uyarısı
                //*ToDo*
            }
            else
            {
                var tP = new TemplateProperties();
                tP.AddTemplate(_image,TxtName.Text,TxtStuffId.Text,_templates);
                Close();
            }
        }
    }
}
