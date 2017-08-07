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

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for SavedTemplates.xaml
    /// </summary>
    public partial class SavedTemplates : Window
    {
        public SavedTemplates()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var list = new CustomListView(ListView);
            list.Ekle("Images/settings.png","abc","abs");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var list = new CustomListView(ListView);
            list.Sil(ListView.SelectedItem);
        }
    }
}
