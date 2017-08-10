using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for SavedTemplates.xaml
    /// </summary>
    public partial class SavedTemplates : Window
    {
        public int SelectedId = -1;
        public List<int> DeletedItemIds= new List<int>();
        public SavedTemplates()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ListViewItem) TemplateListView.Items[TemplateListView.SelectedIndex];
            var id = int.Parse(selectedItem.Name.Substring(1));
            TemplateListView.Sil(TemplateListView.SelectedItem);
            new TemplateProperties().DeleteTemplateFromDatabase(id);
            DeletedItemIds.Add(id);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TemplateProperties.GetAllSavedTemplates(out var imagePaths, out var names, out var staffIds, out var iDs);
            for (int i = 0; i < imagePaths.Length; i++)
            {
                TemplateListView.Ekle(imagePaths[i], names[i], staffIds[i], iDs[i]);
                
            }
        }

        private void TemplateListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TemplateListView.SelectedIndex!=-1)
            {
                SelectedId = TemplateListView.GetSelectedId();
                Close();
            }
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            var update= new UpdateSavedTemplates();
            update.ShowDialog();
        }
    }
}
