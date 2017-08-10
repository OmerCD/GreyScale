using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AForge.Wpf.DatabaseCodes;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        private BackgroundWorker _backgroundWorker= new BackgroundWorker();
        private int _trialCounter=10;

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
            _backgroundWorker.DoWork += SearchPorts;
        }

        private void SearchPorts(object sender, DoWorkEventArgs e)
        {
            bool found = false;
            var ports = SerialPort.GetPortNames();
            while (_trialCounter > 0)
            {
                foreach (var portName in ports)
                {
                    var port = new SerialPort(portName, 9600);
                    port.Open();
                    port.Write("1");
                    string message = "";
                    message = Timeout(port, message);
                    string check = "";
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
                if (MessageBox.Show("Uyarı", "Bağlı bir kart bulunumadı. Tekrar denemek ister misiniz?", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    SearchPorts(null, null);
                }
                else
                {
                    Close();
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
