using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetActu();
        }

        public void SetActu()
        {
            string url = "https://strandedinspace.000webhostapp.com/Actus/actus.txt";
            using (WebClient client = new WebClient())
            {
                string actus = client.DownloadString(url);
                if (actus !=String.Empty)
                {
                    actu.Text = actus;
                }
            }
            string url1 = "https://strandedinspace.000webhostapp.com/Actus/prevactus.txt";
            using (WebClient client = new WebClient())
            {
                string actus = client.DownloadString(url1);
                if (actus != String.Empty)
                {
                    prevactu.Text = actus;
                }
            }

        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("SIS-2.0.exe");
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://strandedinspace.000webhostapp.com/pages/inscription.php");
        }
    }
}
