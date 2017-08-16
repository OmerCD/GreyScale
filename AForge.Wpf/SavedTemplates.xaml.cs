using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ListViewItem = System.Windows.Controls.ListViewItem;
using Size = System.Drawing.Size;

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (SelectionItem) TemplateListView.Items[TemplateListView.SelectedIndex];
            var id = int.Parse(selectedItem.Id);
            TemplateListView.Sil(TemplateListView.SelectedItem);
            new TemplateProperties().DeleteTemplateFromDatabase(id);
            DeletedItemIds.Add(id);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshListView();
        }

        private void RefreshListView()
        {
            TemplateListView.Items.Clear();
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
                SelectedId = TemplateListView.GetSelectedId;
                Close();
            }
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            var update= new UpdateSavedTemplates(TemplateListView.GetSelectedId);
            update.ShowDialog();
            RefreshListView();
        }
    }
}
