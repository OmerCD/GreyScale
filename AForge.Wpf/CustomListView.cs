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

        struct SelectionItem
        {
            public string ImagePath { get; set; }
            public string Name { get; set; }
            public string StuffId { get; set; }
            public string Id { get; set; }
        }
        public void Ekle(string resimYolu, string text1, string text2,string keyId)
        {
            Items.Add(new SelectionItem {ImagePath = resimYolu, Name = text1, StuffId = text2,Id=keyId});
            //var dock = new DockPanel();
            //var img =new Image();
            //var textBlock1=new TextBlock();
            //var textBlock2 = new TextBlock();
            //var item = new ListViewItem {Name = "_" + keyId};
            //var imgUri = new Uri(resimYolu, UriKind.Absolute);
            //var bitmapImage = new BitmapImage(imgUri);
            //img.Source = bitmapImage;
            //RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
            //img.Width = 200;
            //img.Margin=new Thickness(0,0,15,0);

            //textBlock1.Text = text1;
            //textBlock1.Margin = new Thickness(0, 0, 15, 0);
            //textBlock1.FontSize = 24;
            //textBlock1.FontFamily= Application.Current.FindResource("SemiBold") as FontFamily;
            //textBlock1.Background = Brushes.LightBlue;
            //textBlock1.Width = 150;
            //textBlock1.Height = 100;
            //textBlock1.Foreground = Brushes.White;
            //textBlock1.TextAlignment=TextAlignment.Center;

            //textBlock2.Text = text2;
            //textBlock2.Margin = new Thickness(0, 0, 15, 0);
            //textBlock2.FontFamily = Application.Current.FindResource("SemiBold") as FontFamily;
            //textBlock2.FontSize = 24;
            //textBlock2.Background = Brushes.CornflowerBlue;
            //textBlock2.Width = 150;
            //textBlock2.Height = 100;
            //textBlock2.Foreground = Brushes.White;
            //textBlock2.TextAlignment = TextAlignment.Center;


            //dock.Children.Add(img);
            //dock.Children.Add(textBlock1);
            //dock.Children.Add(textBlock2);

            //item.Content = dock;

            //Items.Add(item);
        }
    }
}
