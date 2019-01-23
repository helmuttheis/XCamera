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
using System.Windows.Shapes;

namespace XCameraManager
{
    /// <summary>
    /// Interaktionslogik für ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        WebServer webServer;
        public ConnectWindow()
        {
            InitializeComponent();

            IPAddress[] addrList = LocalIPAddress();
            int sel = 0;
            foreach (var addr in addrList)
            {
                if(addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    string szIP = addr.ToString();
                    if( szIP.Equals(Config.current.szIP))
                    {
                        sel = cmbIP.Items.Count;
                    }
                    cmbIP.Items.Add(szIP);
                }
                
            }
            cmbIP.SelectedIndex = sel;
            tbPort.Text = Config.current.szPort;
            tbBasedir.Text = Config.current.szBasedir;
            btnDisconnect.IsEnabled = false;
        }
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {

            string szIP = cmbIP.SelectedValue.ToString();
            
            webServer = new WebServer(SendResponse, "http://" + szIP + ":" + tbPort.Text.Trim() + "/test/");
            webServer.Run();
            Config.current.szIP = szIP;
            Config.current.szPort = tbPort.Text.Trim();
            btnDisconnect.IsEnabled = true;
            btnConnect.IsEnabled = false;
        }
        public static string SendResponse(HttpListenerRequest request)
        {
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            webServer.Stop();
            this.Close();
        }
        private IPAddress[] LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList;
               // .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        private void BtnSelectBasedir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tbBasedir.Text = folderDlg.SelectedPath;
                Config.current.szBasedir = folderDlg.SelectedPath;
            }
        }
    }
}
