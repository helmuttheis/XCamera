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
            dpStart.SelectedDate =null;
            dpEnd.SelectedDate = null;

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

                    if (gebaeude == null && !string.IsNullOrWhiteSpace(cmbGebaeude.Text.ToString()) )
                    {
                        // add the new value
                        gebaeude = projectSql.sqlGebaeude.Get(cmbGebaeude.Text.ToString()) as Gebaeude;
                        if (gebaeude == null)
                        {
                            projectSql.sqlGebaeude.Add(cmbGebaeude.Text.ToString());
                            gebaeude = projectSql.sqlGebaeude.Get(cmbGebaeude.Text.ToString()) as Gebaeude;
                        }
                        cmbGebaeude.ItemsSource = null;
                        cmbGebaeude.ItemsSource = projectSql.sqlGebaeude.GetListe();
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

                    if (etage == null && !string.IsNullOrWhiteSpace(cmbEtage.Text.ToString()))
                    {
                        // add the new value
                        etage = projectSql.sqlEtage.Get(cmbEtage.Text.ToString()) as Etage;
                        if (etage == null)
                        {
                            projectSql.sqlEtage.Add(cmbEtage.Text.ToString());
                            etage = projectSql.sqlEtage.Get(cmbEtage.Text.ToString()) as Etage;
                        }
                        cmbEtage.ItemsSource = null;
                        cmbEtage.ItemsSource = projectSql.sqlEtage.GetListe();
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

                    if (wohnung == null && !string.IsNullOrWhiteSpace(cmbWohnung.Text.ToString()))
                    {
                        // add the new value
                        wohnung = projectSql.sqlWohnung.Get(cmbWohnung.Text.ToString()) as Wohnung;
                        if (wohnung == null)
                        {
                            projectSql.sqlWohnung.Add(cmbWohnung.Text.ToString());
                            wohnung = projectSql.sqlWohnung.Get(cmbWohnung.Text.ToString()) as Wohnung;
                        }
                        cmbWohnung.ItemsSource = null;
                        cmbWohnung.ItemsSource = projectSql.sqlWohnung.GetListe();
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

                    if (zimmer == null && !string.IsNullOrWhiteSpace(cmbZimmer.Text.ToString()))
                    {
                        // add the new value
                        zimmer = projectSql.sqlZimmer.Get(cmbZimmer.Text.ToString()) as Zimmer;
                        if (zimmer == null)
                        {
                            projectSql.sqlZimmer.Add(cmbZimmer.Text.ToString());
                            zimmer = projectSql.sqlZimmer.Get(cmbZimmer.Text.ToString()) as Zimmer;
                        }
                        cmbZimmer.ItemsSource = null;
                        cmbZimmer.ItemsSource = projectSql.sqlZimmer.GetListe();
                        cmbZimmer.SelectedItem = zimmer;
                    }
                }
                return;
            };
        }
        public void LoadProjects(string szProjectname = "")
        {
            var projekte = ProjectUtil.GetProjectList();
            cmbProjects.Items.Clear();
            int iSelectedProject = -1;
            foreach (var projekt in projekte)
            {
                if (projekt.Equals(szProjectname))
                {
                    iSelectedProject = cmbProjects.Items.Count;
                }
                cmbProjects.Items.Add(projekt);
            }
            if(iSelectedProject >= 0 )
            {
                cmbProjects.SelectedIndex = iSelectedProject;
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
            string szProjectName = new InputBox("Projektnamen eingeben:").ShowDialog();
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

                    SetTitle(szProjectName);
                    LoadProjects(szProjectName);
                    OpenProject(szProjectName);
                    // projectSql = new ProjectSql(szProjectName);


                   // if (MessageBox.Show("Sollen Demodaten erzeugt werden?", this.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                   // {
                   //     GenerateSampleData();
                   // }
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
                gebaeude[key] = projectSql.sqlGebaeude.Add(key);
            }
            keys = new List<string>(etagen.Keys);
            foreach (string key in keys)
            {
                etagen[key] = projectSql.sqlEtage.Add(key);
            }
            keys = new List<string>(wohnungen.Keys);
            foreach (string key in keys)
            {
                wohnungen[key] = projectSql.sqlWohnung.Add(key);
            }
            keys = new List<string>(zimmer.Keys);
            foreach (string key in keys)
            {
                zimmer[key] = projectSql.sqlZimmer.Add(key);
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
                                projectSql.sqlGebaeude.Set(bildId, gebaeude[haus]);
                                projectSql.sqlEtage.Set(bildId, etagen[etage]);
                                projectSql.sqlWohnung.Set(bildId, wohnungen[wohnung]);
                                projectSql.sqlZimmer.Set(bildId, zimmer[raum]);
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

            cmbGebaeude.ItemsSource = null;
            cmbGebaeude.ItemsSource = projectSql.sqlGebaeude.GetListe();

            cmbEtage.ItemsSource = null;
            cmbEtage.ItemsSource = projectSql.sqlEtage.GetListe();

            cmbWohnung.ItemsSource = null;
            cmbWohnung.ItemsSource = projectSql.sqlWohnung.GetListe();

            cmbZimmer.ItemsSource = null;
            cmbZimmer.ItemsSource = projectSql.sqlZimmer.GetListe();

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
            List<Bild> bildListe = projectSql.GetBilder(dpStart.SelectedDate, dpEnd.SelectedDate, gebaeudeId, etageId, wohnungId, zimmerId);
            foreach (var bild in bildListe)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
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
                    BildPath = projectSql.GetImageFullName(bild.Name,"Fotos"),
                    ToBeLaoded = LoadVisibility,
                    CaptureDate = bild.CaptureDate.ToString()
                });
            }
            lvBilder.ItemsSource = bmk;
            Mouse.OverrideCursor = null;

            imgBild.Source = null;
        }
        private void BtnDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            var gebaeude = ((Button)sender).Tag as Gebaeude;
            if( gebaeude != null )
            {
                if ( !projectSql.sqlGebaeude.Delete(gebaeude))
                {
                    MessageBox.Show("Das Gebäude kann nicht gelöscht werden, da es noch benutzt wird.");
                    cmbGebaeude.SelectedItem = gebaeude;
                }
                else
                {
                    cmbGebaeude.ItemsSource = null;
                    cmbGebaeude.Text = "";
                    cmbGebaeude.ItemsSource = projectSql.sqlGebaeude.GetListe();
                    cmbGebaeude.SelectedIndex = -1;
                }
                return;
            }
            var etage = ((Button)sender).Tag as Etage;
            if (etage != null)
            {
                if (!projectSql.sqlEtage.Delete(etage))
                {
                    MessageBox.Show("Die Etage kann nicht gelöscht werden, da sie noch benutzt wird.");
                    cmbEtage.SelectedItem = etage;
                }
                else
                {
                    cmbEtage.ItemsSource = null;
                    cmbEtage.Text = "";
                    cmbEtage.ItemsSource = projectSql.sqlEtage.GetListe();
                    cmbEtage.SelectedIndex = -1;
                }
                return;
            }
            var wohnung = ((Button)sender).Tag as Wohnung;
            if (wohnung != null)
            {
                if (!projectSql.sqlWohnung.Delete(wohnung))
                {
                    MessageBox.Show("Die Wohnung kann nicht gelöscht werden, da sie noch benutzt wird.");
                    cmbWohnung.SelectedItem = wohnung;
                }
                else
                {
                    // cmbWohnung.Items.Clear();
                    cmbWohnung.ItemsSource = null;
                    cmbWohnung.Text = "";

                    cmbWohnung.ItemsSource = projectSql.sqlWohnung.GetListe();
                    cmbWohnung.SelectedIndex = -1;
                }
                return;
            }
            var zimmer = ((Button)sender).Tag as Zimmer;
            if (zimmer != null)
            {
                if (!projectSql.sqlZimmer.Delete(zimmer))
                {
                    MessageBox.Show("Das Zimmer kann nicht gelöscht werden, da es noch benutzt wird.");
                    cmbZimmer.SelectedItem = zimmer;
                }
                else
                {
                    // cmbZimmer.Items.Clear();
                    cmbZimmer.ItemsSource = null;
                    cmbZimmer.Text = "";

                    cmbZimmer.ItemsSource = projectSql.sqlZimmer.GetListe();
                    cmbZimmer.SelectedIndex = - 1;

                }
                return;
            }
            var kommentar = ((Button)sender).Tag as Kommentar;
            if (kommentar != null)
            {
                if (!projectSql.DeleteKommentar(kommentar))
                {
                    MessageBox.Show("Die Kommentar kann nicht gelöscht werden, da er noch benutzt wird.");
                }
                else
                {
                   
                }
                return;
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
                imgBild.Source = new BitmapImage(new Uri(projectSql.GetImageFullName(bmk.BildName,"Fotos")));
                BildInfo bi = projectSql.GetBildInfo(bmk.BildName, DateTime.Now);
                lblGebaeude.Content = bi.GebaeudeBezeichnung;
                lblEtage.Content = bi.EtageBezeichnung;
                lblWohnung.Content = bi.WohnungBezeichnung;
                lblZimmer.Content = bi.ZimmerBezeichnung;
                lblKommentar.Content = bi.KommentarBezeichnung;

                
             }
        }
        public void Publish()
        {
            if(lvBilder.Items.Count == 0)
            {
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".docx";
            dlg.AddExtension = true;
            dlg.InitialDirectory = Config.current.szBasedir;
            dlg.Filter = "Word (*.docx)|*.docx|Alle (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                Dictionary<string, BildMitKommentar> dictBilder = new Dictionary<string, BildMitKommentar>();
                foreach (var bild in lvBilder.Items)
                {
                    BildMitKommentar bmk = bild as BildMitKommentar;
                    if (bmk != null)
                    {
                        string szFullName = projectSql.GetImageFullName(bmk.BildName,"Fotos");
                        if (!dictBilder.ContainsKey(szFullName))
                        {
                            dictBilder.Add(szFullName, bmk);
                        }
                    }
                }
                if (Docx.FillTable(dlg.FileName, projectSql.szProjectName, dictBilder))
                {
                    if (MessageBox.Show("Die Worddatei " + dlg.FileName + " wurde erzeugt." + Environment.NewLine +
                        "Soll sie in Word angezeigt werden?",
                        "XCameraManager",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Process.Start(dlg.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("Fehler beim Erstellen der Word-Datei: " + Docx.szError);
                }
            }
        }
        public void Manage()
        {
            ManageWindow manageWindow = new ManageWindow(projectSql);
            manageWindow.ShowDialog();
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
            // LoadProjects();
        }
        
        private void dpStart_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            dpEnd.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, dpStart.SelectedDate ?? DateTime.Now));
        }

        private void dpEnd_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            dpStart.BlackoutDates.Add(new CalendarDateRange(dpEnd.SelectedDate ?? DateTime.Now, DateTime.MaxValue));
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
            ((MainWindow)Application.Current.MainWindow).LoadProjects();
        }
    }
    public class ApplicationPublishCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            // You may not need a body here at all...
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Application.Current != null && Application.Current.MainWindow != null 
                && ((MainWindow)Application.Current.MainWindow).lvBilder.Items.Count > 0;
        }

        public void Execute(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).Publish();
        }
    }
    public class ApplicationManageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            // You may not need a body here at all...
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Application.Current != null && Application.Current.MainWindow != null
                && ((MainWindow)Application.Current.MainWindow).spProject.IsEnabled;
        }

        public void Execute(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).Manage();
        }
    }
    public class ApplicationPatchCommand : ICommand
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
            /// ((MainWindow)Application.Current.MainWindow).Cursor = Cursor.
            // loop through all project
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            var projekte = ProjectUtil.GetProjectList();
            foreach (var projekt in projekte)
            {
                ((MainWindow)Application.Current.MainWindow).ToLog("patching " + projekt);
                ProjectSql tmpProject = new ProjectSql(projekt);
                tmpProject.Patch();
            }
            ((MainWindow)Application.Current.MainWindow).ToLog("Patch done");
            Mouse.OverrideCursor = null;
        }
    }

    public static class MyCommands
    {
        private static readonly ICommand appPublishCmd = new ApplicationPublishCommand();
        public static ICommand ApplicationPublishCommand
        {
            get { return appPublishCmd; }
        }

        private static readonly ICommand appManageCmd = new ApplicationManageCommand();
        public static ICommand ApplicationManageCommand
        {
            get { return appManageCmd; }
        }

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
        private static readonly ICommand appPatchCmd = new ApplicationPatchCommand();
        public static ICommand ApplicationPatchCommand
        {
            get { return appPatchCmd; }
        }
    }
}
