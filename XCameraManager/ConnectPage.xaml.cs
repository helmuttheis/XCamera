using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace XCameraManager
{
    /// <summary>
    /// Interaktionslogik für ConnectPage.xaml
    /// </summary>
    public partial class ConnectPage : Page
    {
        WebServer webServer;
        public ConnectPage()
        {
            InitializeComponent();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            string szIP = LocalIPAddress().ToString();
            tbIP.Text =szIP ;
            webServer = new WebServer(SendResponse, "http://localhost:" + tbPort.Text.Trim() + "/test/");
            webServer.Run();
        }
        public static string SendResponse(HttpListenerRequest request)
        {
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            webServer.Stop();
            MainWindow main = Application.Current.MainWindow as MainWindow;
            if (main != null)
            {
                main.FrameWithinGrid.Source = null;
            }
        }
        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
