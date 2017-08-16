using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace AForge.Wpf
{
    class CustomListView:ListView
    {
        public void Guncelle()
        {
            
        }

        public void Sil(object item)
        {
            Items.Remove(item);
        }

        public int GetSelectedId => Convert.ToInt32(((SelectionItem) Items[SelectedIndex]).Id);

     
        public void Ekle(string resimYolu, string text1, string text2,string keyId)
        {
            Items.Add(new SelectionItem {ImagePath = resimYolu, Name = text1, StuffId = text2,Id=keyId});           
        }
    }
    public struct SelectionItem
    {
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public string StuffId { get; set; }
        public string Id { get; set; }
    }
}
