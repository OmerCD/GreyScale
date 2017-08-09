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
using AForge.Wpf.DatabaseCodes;
using AForge.Wpf.LanguageLocalization;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for Languages.xaml
    /// </summary>
    public partial class Languages : Window
    {
        public Languages()
        {
            InitializeComponent();
        }

        void SaveClick(object sender, EventArgs e)
        {
            if (LanguageListView.SelectedIndex != -1)
            {
                var lang =
                    ((ListViewItem)LanguageListView.Items[LanguageListView.SelectedIndex]).Name.Replace('_',
                        '-');
                if (MessageBox.Show(ResLocalization.AskForRestart, ResLocalization.Warning, MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var oP = new OptionsProperties();
                    oP.SetOption("Language",lang);
                    Application.Current.Shutdown();
                    System.Windows.Forms.Application.Restart();
                }
            }
        }
    }
}
