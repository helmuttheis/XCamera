using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XCamera.Util;

namespace XCameraManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProjectSql projectSql;   
        public MainWindow()
        {
            InitializeComponent();
            XCamera.Util.Config.szConfigFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            XCamera.Util.Config.szConfigFile = XCamera.Util.Config.szConfigFile.Replace(@"\bin\Debug", "");
            XCamera.Util.Config.szConfigFile = System.IO.Path.Combine(XCamera.Util.Config.szConfigFile, "XcameraManmager.xml");

        }
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string szProjectName = new InputBox("text").ShowDialog();
            if( !string.IsNullOrWhiteSpace(szProjectName))
            {
                string szFullProjectPath = System.IO.Path.Combine(Config.current.szBasedir, szProjectName);
                // create the directory
                if ( !Directory.Exists(szFullProjectPath))
                {
                    Directory.CreateDirectory(szFullProjectPath);
                    if( projectSql != null )
                    {
                        // project.Close();
                    }
                    ProjectUtil.szBasePath = Config.current.szBasedir;
                    ProjectSql.szProjectName = szProjectName;
                    // create the SQLite database
                    projectSql = new ProjectSql();
                    GenerateSampleData();
                }
                else
                {
                    MessageBox.Show("Das Propjekt " + szProjectName + " existiert schon");
                }
            }
            txtEditor.Text = "";
        }
        private void GenerateSampleData()
        {
            string[] gebaeude = { "Haus A","Haus B","Haus C"};
            string[] etagen = { "Keller","Parterre","Etage 1","Etage 2"};
            string[] wohnungen = { "Wohnung Links", "Wohnung Mitte", "Wohnung rechts" };
            string[] zimmer = { "Bad", "Küche", "Flut", "Eltern", "Kind ", "Kind 2", "Wohnzimmer" };
            using (Font font1 = new Font("Arial", 48, System.Drawing.FontStyle.Bold, GraphicsUnit.Point))
            {
                foreach (var haus in gebaeude)
                {
                    int hausID = projectSql.AddGebaeude(haus);
                    foreach (var etage in etagen)
                    {
                        int etageID = projectSql.AddEtage(etage);
                        foreach (var wohnung in wohnungen)
                        {
                            int wohnungID = projectSql.AddWohnung(wohnung);
                            foreach (var raum in zimmer)
                            {
                                ToLog(haus + " " + etage + " " + wohnung + " " + raum);
                                int raumID = projectSql.AddZimmer(raum);
                                string szImgName = (haus + "_" + etage + "_" + wohnung + "_" + raum).Replace(" ", "_");
                                string szFullImgName = System.IO.Path.Combine(projectSql.szProjectPath, szImgName + ".jpg");
                                Bitmap a = new Bitmap(1024, 1024);

                                using (Graphics g = Graphics.FromImage(a))
                                {
                                    g.FillRegion(System.Drawing.Brushes.LightGray, new Region(new RectangleF(3, 3, 1020, 1020)));
                                    RectangleF rectF1 = new RectangleF(30, 30, 1000, 1000);
                                    g.DrawString(szImgName, font1, System.Drawing.Brushes.Blue, rectF1); // requires font, brush etc
                                }
                                a.Save(szFullImgName, ImageFormat.Jpeg);
                                int bildId = projectSql.AddBild(szImgName + ".jpg");
                                projectSql.SetGebaeude(bildId, hausID);
                                projectSql.SetEtage(bildId, etageID);
                                projectSql.SetWohnung(bildId, wohnungID);
                                projectSql.SetZimmer(bildId, raumID);
                                projectSql.SetComment(bildId, szImgName);
                            }
                        }
                    }
                }
            }
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Config.current.szBasedir;
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".db";
            dlg.Filter = "SQLite Files (*.db)|*.db";
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                txtEditor.Text = filename;

                ProjectUtil.szBasePath = Config.current.szBasedir;
                ProjectSql.szProjectName = System.IO.Path.GetFileNameWithoutExtension(filename);
                projectSql = new ProjectSql();
                List<Gebaeude> gebaeudeListe = projectSql.GetGebaeude();
                List<Etage> etageListe = projectSql.GetEtagen();
                List<Wohnung> wohnungiste = projectSql.GetWohnung();

                List<Zimmer> zimmerListe = projectSql.GetZimmer();


                List<Bild> bildListe = projectSql.GetBilder(gebaeudeListe[0].ID);
                txtEditor.Text = "";
                foreach (var bild in bildListe)
                {
                    txtEditor.Text += bild.Name + Environment.NewLine;
                }

            }

        }
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtEditor.Text = "";
        }

        public void ToLog(string szMsg)
        {
            Application.Current.Dispatcher.BeginInvoke(
              new Action(() =>
              {
                  lblLog.Content = szMsg;
              }), DispatcherPriority.Background);
            System.Windows.Forms.Application.DoEvents();
        }
    }

    public class ApplicationCloseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            // You may not need a body here at all...
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Application.Current != null && Application.Current.MainWindow != null;
        }

        public void Execute(object parameter)
        {
            Application.Current.MainWindow.Close();
        }
    }
    public class ApplicationConnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            // You may not need a body here at all...
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Application.Current != null && Application.Current.MainWindow != null;
        }

        public void Execute(object parameter)
        {
            // 
            ConnectWindow connectWindow = new ConnectWindow();
            connectWindow.ShowDialog();
        }
    }
    public static class MyCommands
    {
        private static readonly ICommand appCloseCmd = new ApplicationCloseCommand();
        public static ICommand ApplicationCloseCommand
        {
            get { return appCloseCmd; }
        }

        private static readonly ICommand appConnectCmd = new ApplicationConnectCommand();
        public static ICommand ApplicationConnectCommand
        {
            get { return appConnectCmd; }
        }
    }
}
