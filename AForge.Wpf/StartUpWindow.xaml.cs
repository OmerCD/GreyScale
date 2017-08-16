using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AForge.Wpf.DatabaseCodes;
using AForge.Wpf.LanguageLocalization;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private int _trialCounter = 10;

        public StartUpWindow()
        {
            var db = new DatabaseManagement();
            db.CreateDatabase();
            string GetSavedLanguage()
            {
                var oP = new OptionsProperties();
                return oP.GetOption<string>("Language");
            }
            Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(GetSavedLanguage());
            InitializeComponent();

            _backgroundWorker.DoWork += SearchPorts; // TO DO // HASAN'ın Notu Buraya bir şey mi eklenecek? Lisans yok mu şimdilik?
            var mainWindow = new MainWindow();

            mainWindow.Show();
            Close();
        }

        private void SearchPorts(object sender, DoWorkEventArgs e)
        {
            var found = false;
            var ports = SerialPort.GetPortNames();
            var sPorts = ports.Select(portName => new SerialPort(portName, 9600)).ToList();
            while (_trialCounter > 0)
            {
                foreach (var port in sPorts)
                {
                    if (!port.IsOpen)
                        port.Open();
                    port.Write("1");
                    var message = "";
                    message = Timeout(port, message);
                    var check = "";
                    if (message.Length > 3)
                        check = message.Substring(0, message.Length - 1);
                    if (check == "123456")
                    {
                        Dispatcher.Invoke(delegate
                        {
                            var mainWindow = new MainWindow();

                            mainWindow.Show();
                            Close();

                        });
                        found = true;
                    }
                }
                if (found)
                {
                    break;
                }
                _trialCounter--;
            }
            if (_trialCounter == 0)
            {
                MessageBox.Show(ResLocalization.DeviceNotFound, ResLocalization.Error,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(Close);
            }
        }
        private static string Timeout(SerialPort port, string message)
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
            var task = Task.Run(() => message = port.ReadLine());
            return task.Wait(TimeSpan.FromSeconds(3)) ? task.Result : message;
        }

        private void StartupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.RunWorkerAsync();
        }
    }
}