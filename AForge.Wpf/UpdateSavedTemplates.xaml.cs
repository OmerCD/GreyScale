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
using AForge.Wpf.LanguageLocalization;
using Emgu.CV.ML.Structure;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for UpdateSavedTemplates.xaml
    /// </summary>
    public partial class UpdateSavedTemplates : Window
    {
        private string _name, _stuffId;
        private int _id;
        public UpdateSavedTemplates(int id)
        {
            InitializeComponent();
            var tP = new TemplateProperties();
            tP.GetTemplateNameAndId(id,out _name,out _stuffId);
            _id = id;
        }

        private void Iptal_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Kayıt_Click(object sender, RoutedEventArgs e)
        {
            if (TxtName.Text.Length < 2 || TxtStuffId.Text.Length < 1)
            {
                MessageBox.Show(ResLocalization.WrongEnter, ResLocalization.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
                //*ToDo*
            }
            else
            {
                var tP = new TemplateProperties();
                tP.UpdateTemplate(TxtName.Text.Trim(),TxtStuffId.Text.Trim(),_id);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtName.Text = _name;
            TxtStuffId.Text = _stuffId;
        }
    }
}
