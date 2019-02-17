using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            dpStart.Value = dpStart.MinDate;
            dpEnd.Value = dpEnd.MaxDate;

            XCamera.Util.Config.szConfigFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            XCamera.Util.Config.szConfigFile = XCamera.Util.Config.szConfigFile.Replace(@"\bin\Debug", "");
            XCamera.Util.Config.szConfigFile = System.IO.Path.Combine(XCamera.Util.Config.szConfigFile, "XCameraManager.xml");

            ProjectUtil.szBasePath = Config.current.szBasedir;
            tbBasedir.Text = Config.current.szBasedir;
            LoadProjects();
            
            cmbGebaeude.SelectionChanged += (se, ev) =>
            {
                Gebaeude gebaeude = cmbGebaeude.SelectedItem as Gebaeude;

                if (gebaeude == null)
                {
                }
            };
            cmbGebaeude.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Gebaeude gebaeude = cmbGebaeude.SelectedItem as Gebaeude;

                    if (gebaeude == null)
                    {
                        // add the new value
                        projectSql.AddGebaeude(cmbGebaeude.Text.ToString());
                        gebaeude = projectSql.GetGebaeude(cmbGebaeude.Text.ToString());
                        cmbGebaeude.Items.Add(gebaeude);
                        cmbGebaeude.SelectedItem = gebaeude;
                    }
                }
                return;
            };
            cmbEtage.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Etage etage = cmbEtage.SelectedItem as Etage;

                    if (etage == null)
                    {
                        // add the new value
                        projectSql.AddEtage(cmbEtage.Text.ToString());
                        etage = projectSql.GetEtage(cmbEtage.Text.ToString());
                        cmbEtage.Items.Add(etage);
                        cmbEtage.SelectedItem = etage;
                    }
                }
                return;
            };
            cmbWohnung.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Wohnung wohnung = cmbWohnung.SelectedItem as Wohnung;

                    if (wohnung == null)
                    {
                        // add the new value
                        projectSql.AddWohnung(cmbWohnung.Text.ToString());
                        wohnung = projectSql.GetWohnung(cmbWohnung.Text.ToString());
                        cmbWohnung.Items.Add(wohnung);
                        cmbWohnung.SelectedItem = wohnung;
                    }
                }
                return;
            };
            cmbZimmer.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Zimmer zimmer = cmbZimmer.SelectedItem as Zimmer;

                    if (zimmer == null)
                    {
                        // add the new value
                        projectSql.AddZimmer(cmbZimmer.Text.ToString());
                        zimmer = projectSql.GetZimmer(cmbZimmer.Text.ToString());
                        cmbZimmer.Items.Add(zimmer);
                        //cmbZimmer.SelectedItem = zimmer;
                    }
                }
                return;
            };


        }
        private void LoadProjects()
        {
              var projekte = ProjectUtil.GetProjectList();
              cmbProjects.Items.Clear();
              foreach (var projekt in projekte)
              {
                  cmbProjects.Items.Add(projekt);
              }
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
                XCamera.Util.Config.current.szBasedir = folderDlg.SelectedPath;
                ProjectUtil.szBasePath = Config.current.szBasedir;
                LoadProjects();
            }
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
                    //ProjectSql.szProjectName = szProjectName;
                    // create the SQLite database
                    SetTitle(szProjectName);

                    projectSql = new ProjectSql(szProjectName);
                    if (MessageBox.Show("Sollen Demodaten erzeugt werden?", this.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        GenerateSampleData();
                    }
                }
                else
                {
                    MessageBox.Show("Das Propjekt " + szProjectName + " existiert schon");
                }
            }
            
        }
        private void GenerateSampleData()
        {
            Dictionary<string,int> gebaeude = new Dictionary<string, int> { {"Haus A", -1 },{ "Haus B", -1 },{ "Haus C", -1 } };
            Dictionary<string, int> etagen = new Dictionary<string, int> { { "Keller", -1 }, { "Parterre", -1 }, { "Etage 1", -1 }, { "Etage 2", -1 } };
            Dictionary<string, int> wohnungen = new Dictionary<string, int> { { "Wohnung Links", -1 }, { "Wohnung Mitte", -1 },{ "Wohnung rechts", - 1 } };
            Dictionary<string, int> zimmer = new Dictionary<string, int> { { "Bad", -1 }, { "Küche", -1 }, { "Flut", -1 }, { "Eltern", -1 }, { "Kind ", -1 }, { "Kind 2", -1 }, { "Wohnzimmer", -1 } };

            List<string> keys = new List<string>(gebaeude.Keys);
            foreach (string key in keys)
            {
                gebaeude[key] = projectSql.AddGebaeude(key);
            }
            keys = new List<string>(etagen.Keys);
            foreach (string key in keys)
            {
                etagen[key] = projectSql.AddEtage(key);
            }
            keys = new List<string>(wohnungen.Keys);
            foreach (string key in keys)
            {
                wohnungen[key] = projectSql.AddWohnung(key);
            }
            keys = new List<string>(zimmer.Keys);
            foreach (string key in keys)
            {
                zimmer[key] = projectSql.AddZimmer(key);
            }
            
            using (Font font1 = new Font("Arial", 48, System.Drawing.FontStyle.Bold, GraphicsUnit.Point))
            {
                foreach (var haus in gebaeude.Keys)
                {
                    foreach (var etage in etagen.Keys)
                    {
                        foreach (var wohnung in wohnungen.Keys)
                        {
                            foreach (var raum in zimmer.Keys)
                            {
                                ToLog(haus + " " + etage + " " + wohnung + " " + raum);
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
                                projectSql.SetGebaeude(bildId, gebaeude[haus]);
                                projectSql.SetEtage(bildId, etagen[etage]);
                                projectSql.SetWohnung(bildId, wohnungen[wohnung]);
                                projectSql.SetZimmer(bildId, zimmer[raum]);
                                projectSql.SetComment(bildId, szImgName);
                                projectSql.SetStatus(bildId, STATUS.NONE);
                            }
                        }
                    }
                }
            }
        }
        private void SetTitle(string szProjectName)
        {
            string szTitel = this.Title;
            szTitel = szTitel.Split(' ')[0] + " " + szProjectName;

            this.Title = szTitel;
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
                
                ProjectUtil.szBasePath = Config.current.szBasedir;
                string szProjectName = System.IO.Path.GetFileNameWithoutExtension(filename);
                OpenProject(szProjectName);
                
            }

        }
        private void OpenProject(string szProjectName)
        {
            SetTitle(szProjectName);

            projectSql = new ProjectSql(szProjectName);
            cmbGebaeude.Items.Clear();
            List<Gebaeude> gebaeudeListe = projectSql.GetGebaeudeListe();
            foreach (var gebaeude in gebaeudeListe)
            {
                cmbGebaeude.Items.Add(gebaeude);
            }
            cmbEtage.Items.Clear();
            List<Etage> etageListe = projectSql.GetEtagenListe();
            foreach (var etage in etageListe)
            {
                cmbEtage.Items.Add(etage);
            }
            cmbWohnung.Items.Clear();
            List<Wohnung> wohnungListe = projectSql.GetWohnungListe();
            foreach (var wohnung in wohnungListe)
            {
                cmbWohnung.Items.Add(wohnung);
            }

            cmbZimmer.Items.Clear();
            List<Zimmer> zimmerListe = projectSql.GetZimmerListe();
            foreach (var zimmer in zimmerListe)
            {
                cmbZimmer.Items.Add(zimmer);
            }
            spProject.IsEnabled = true;
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Gebaeude gebaude = cmbGebaeude.SelectedItem as Gebaeude;
            Etage etage = cmbEtage.SelectedItem as Etage;
            Wohnung wohnung = cmbWohnung.SelectedItem as Wohnung;
            Zimmer zimmer = cmbZimmer.SelectedItem as Zimmer;

            int gebaeudeId = gebaude !=null? gebaude .ID :  - 1;
            int etageId = etage != null ? etage.ID : -1;
            int wohnungId = wohnung != null ? wohnung.ID : -1;
            int zimmerId = zimmer != null ? zimmer.ID : -1;


            string szSerarchKommentar = tbKommentar.Text.Trim().ToLower();
            List<BildMitKommentar> bmk = new List<BildMitKommentar>();
            List<Bild> bildListe = projectSql.GetBilder(dpStart.Value, dpEnd.Value,gebaeudeId, etageId, wohnungId, zimmerId);
            foreach (var bild in bildListe)
            {
                string szKommentar = projectSql.GetKommentar(bild.ID);
                string LoadVisibility = "Collapsed";
                if (chkLoadImages.IsChecked == true)
                {
                    LoadVisibility = "Visible";
                }
                if ( string.IsNullOrWhiteSpace(szSerarchKommentar) || szKommentar.ToLower().Contains(szSerarchKommentar))
                bmk.Add(new BildMitKommentar {
                    BildName = System.IO.Path.GetFileName(bild.Name),
                    BildInfo = projectSql.GetBildInfo(bild.Name,DateTime.Now),
                    Kommentar = szKommentar,
                    BildPath = projectSql.GetImageFullName(bild.Name),
                    ToBeLaoded = LoadVisibility,
                    CaptureDate = bild.CaptureDate.ToString()
                });
            }
            lvBilder.ItemsSource = bmk;

            imgBild.Source = null;
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
            // clear the cmb
            spProject.IsEnabled = false;
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

        private void LvBilder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             BildMitKommentar bmk = lvBilder.SelectedItem as BildMitKommentar;
             if(bmk != null  )
             {
                imgBild.Source = new BitmapImage(new Uri(projectSql.GetImageFullName(bmk.BildName)));
                BildInfo bi = projectSql.GetBildInfo(bmk.BildName, DateTime.Now);
                lblGebaeude.Content = bi.GebaeudeBezeichnung;
                lblEtage.Content = bi.EtageBezeichnung;
                lblWohnung.Content = bi.WohnungBezeichnung;
                lblZimmer.Content = bi.ZimmerBezeichnung;
                lblKommentar.Content = bi.KommentarBezeichnung;
             }
        }

        private void CmbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             string szProjectName = cmbProjects.SelectedItem as string;
             if( !string.IsNullOrWhiteSpace(szProjectName) )
             {
                 OpenProject(szProjectName);
             }

        }

        private void BtnOpenProjectdir_Click(object sender, RoutedEventArgs e)
        {
            string szProjectName = cmbProjects.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(szProjectName))
            {
                Process.Start(projectSql.szProjectPath);
            }
        }

    }
    public class BildMitKommentar
    {
        public string BildName { get; set; }
        public BildInfo BildInfo { get; set; }
        public string Kommentar { get; set; }
        public string BildPath { get; set; }
        public string ToBeLaoded{ get; set; }
        public string CaptureDate { get; set; }
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
