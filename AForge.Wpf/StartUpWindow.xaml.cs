using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
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
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        private int _i;
        private BackgroundWorker _backgroundWorker= new BackgroundWorker();
        public StartUpWindow()
        {
            InitializeComponent();
            _backgroundWorker.DoWork += SearchPorts;
        }

        private void SearchPorts(object sender, DoWorkEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            foreach (var portName in ports)
            {
                var port = new SerialPort(portName, 9600);
                port.Open();
                port.Write("1");
                string message = "";
                message = Timeout(port, message);
                string check="";
                if(message.Length>3)
                check = message.Substring(0, message.Length - 1);
                if (check == "123456")
                {
                    Dispatcher.Invoke(delegate
                    {
                        var mainWindow = new MainWindow();

                        mainWindow.Show();
                        Close();
                    });
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            
        }

        private static string Timeout(SerialPort port, string message)
        {
            var task = Task.Run(() => message = port.ReadLine());
            if (task.Wait(TimeSpan.FromSeconds(3)))
            {
                return task.Result;
            }

            return message;
        }

        private void StartupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.RunWorkerAsync();
        }
    }
}
