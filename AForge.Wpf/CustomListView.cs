using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var textbox1=new TextBox();
            var textbox2=new TextBox();
            var item = new ListViewItem {Name = "_" + keyId};
            var imgUri = new Uri(resimYolu, UriKind.Relative);
            var bitmapImage = new BitmapImage(imgUri);
            img.Source = bitmapImage;
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
            img.Width = 200;
            img.Margin=new Thickness(0,0,15,0);

            textbox1.Text = text1;
            textbox1.Margin = new Thickness(0, 0, 15, 0);
            textbox1.Style = Application.Current.FindResource("TextBox") as Style;

            textbox2.Text = text2;
            textbox2.Margin = new Thickness(0, 0, 15, 0);
            textbox2.Style = Application.Current.FindResource("TextBox") as Style;

            dock.Children.Add(img);
            dock.Children.Add(textbox1);
            dock.Children.Add(textbox2);

            item.Content = dock;

            Items.Add(item);
        }
    }
}
