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

        public int GetSelectedId()
        {
            return Convert.ToInt32(((ListViewItem) Items[SelectedIndex]).Name.Substring(1));
        }
        public void Ekle(string resimYolu, string text1, string text2,string keyId)
        {
            var dock = new DockPanel();
            var img =new Image();
            var textBlock1=new TextBlock();
            var textBlock2 = new TextBlock();
            var item = new ListViewItem {Name = "_" + keyId};
            var imgUri = new Uri(resimYolu, UriKind.Absolute);
            var bitmapImage = new BitmapImage(imgUri);
            img.Source = bitmapImage;
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
            img.Width = 200;
            img.Margin=new Thickness(0,0,15,0);

            textBlock1.Text = text1;
            textBlock1.Margin = new Thickness(0, 0, 15, 0);
            textBlock1.Style = Application.Current.FindResource("TextBlock") as Style;
            textBlock1.FontSize = 24;
            textBlock1.Background = Brushes.LightBlue;
            textBlock1.Width = 120;
            textBlock1.Foreground = Brushes.White;

            textBlock2.Text = text2;
            textBlock2.Margin = new Thickness(0, 0, 15, 0);
            textBlock2.Style = Application.Current.FindResource("TextBlock") as Style;
            textBlock2.FontSize = 24;
            textBlock2.Background = Brushes.LightCoral;
            textBlock2.Width = 120;
            textBlock2.Foreground = Brushes.White;


            dock.Children.Add(img);
            dock.Children.Add(textBlock1);
            dock.Children.Add(textBlock2);

            item.Content = dock;

            Items.Add(item);
        }
    }
}
