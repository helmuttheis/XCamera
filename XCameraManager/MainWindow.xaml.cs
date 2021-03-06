﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static XCamera.Util.ProjectSql;

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
            dpStart.SelectedDate = null;
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
                    if (!string.IsNullOrWhiteSpace(cmbGebaeude.Text.ToString()))
                    {
                        if (gebaeude == null || !gebaeude.Bezeichnung.Equals(cmbGebaeude.Text.Trim()))
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
                }
                return;
            };
            cmbEtage.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Etage etage = cmbEtage.SelectedItem as Etage;
                    if (!string.IsNullOrWhiteSpace(cmbEtage.Text.ToString())) {
                        if (etage == null || !etage.Bezeichnung.Equals(cmbEtage.Text.Trim()))
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
                }
                return;
            };
            cmbWohnung.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Wohnung wohnung = cmbWohnung.SelectedItem as Wohnung;
                    if (!string.IsNullOrWhiteSpace(cmbWohnung.Text.ToString()))
                    {
                        if (wohnung == null || !wohnung.Bezeichnung.Equals(cmbWohnung.Text.Trim()))
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
                }
                return;
            };
            cmbZimmer.PreviewKeyDown += (se, ev) =>
            {
                if (ev.Key == Key.Enter || ev.Key == Key.Return || ev.Key == Key.Tab)
                {
                    Zimmer zimmer = cmbZimmer.SelectedItem as Zimmer;
                    if (!string.IsNullOrWhiteSpace(cmbZimmer.Text.ToString()))
                    {
                        if (zimmer == null || !zimmer.Bezeichnung.Equals(cmbZimmer.Text.Trim()))
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
            string szSuffix = Config.current.szDbSuffix;
            if( !string.IsNullOrWhiteSpace(szProjectName))
            {
                string szFullProjectPath = System.IO.Path.Combine(Config.current.szBasedir, szProjectName, szSuffix);
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
                    OpenProject(szProjectName, szSuffix);
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
        private void OpenProject(string szProjectName, string szSuffix="")
        {
            SetTitle(szProjectName);

            projectSql = new ProjectSql(szProjectName, szSuffix);

            cmbGebaeude.ItemsSource = null;
            cmbGebaeude.ItemsSource = projectSql.sqlGebaeude.GetListe();

            cmbEtage.ItemsSource = null;
            cmbEtage.ItemsSource = projectSql.sqlEtage.GetListe();

            cmbWohnung.ItemsSource = null;
            cmbWohnung.ItemsSource = projectSql.sqlWohnung.GetListe();

            cmbZimmer.ItemsSource = null;
            cmbZimmer.ItemsSource = projectSql.sqlZimmer.GetListe();

            tbKommentar.Text = null;

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


            string szSearchKommentar = tbKommentar.Text.Trim().ToLower();
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
                if (szKommentar != null)
                {
                    if (string.IsNullOrWhiteSpace(szSearchKommentar) || szKommentar.ToLower().Contains(szSearchKommentar))
                        bmk.Add(new BildMitKommentar
                        {
                            BildName = System.IO.Path.GetFileName(bild.Name),
                            BildInfo = projectSql.GetBildInfo(bild.Name, DateTime.Now),
                            Kommentar = szKommentar,
                            BildPath = projectSql.GetImageFullName(bild.Name, Config.current.szPicSuffix),
                            ToBeLaoded = LoadVisibility,
                            CaptureDate = bild.CaptureDate.ToString()
                        });
                }
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

        private void BtnEditTag_Click(object sender, RoutedEventArgs e)
        {
            var gebaeude = ((Button)sender).Tag as Gebaeude;
            if (gebaeude != null)
            {
                int selectedIndex = cmbGebaeude.SelectedIndex;
                string szNewBezeichnung = new InputWindow("Gebäude-Tag bearbeiten", gebaeude.Bezeichnung)
                {
                    IsValidHandler = (szWert) => {
                        var existingGebaeude = projectSql.sqlGebaeude.Get(szWert) as Gebaeude;
                        if (existingGebaeude == null)
                        {
                            return true;
                        }
                        return false;
                    }
                }.ShowDialog();
                if (!string.IsNullOrWhiteSpace(szNewBezeichnung))
                {
                    projectSql.sqlGebaeude.Edit(gebaeude, szNewBezeichnung);
                }
                cmbGebaeude.ItemsSource = null;
                cmbGebaeude.ItemsSource = projectSql.sqlGebaeude.GetListe();
                cmbGebaeude.SelectedIndex = selectedIndex;
            }

            var etage = ((Button)sender).Tag as Etage;
            if (etage != null)
            {
                int selectedIndex = cmbEtage.SelectedIndex;
                string szNewBezeichnung = new InputWindow("Etagen-Tag bearbeiten", etage.Bezeichnung)
                {
                    IsValidHandler = (szWert) => {
                        var existingEtage = projectSql.sqlEtage.Get(szWert) as Etage;
                        if (existingEtage == null)
                        {
                            return true;
                        }
                        return false;
                    }
                }.ShowDialog();
                if(!string.IsNullOrWhiteSpace(szNewBezeichnung))
                {
                    projectSql.sqlEtage.Edit(etage, szNewBezeichnung);
                }
                cmbEtage.ItemsSource = null;
                cmbEtage.ItemsSource = projectSql.sqlEtage.GetListe();
                cmbEtage.SelectedIndex = selectedIndex;
            }

            var wohnung = ((Button)sender).Tag as Wohnung;
            if (wohnung != null)
            {
                int selectedIndex = cmbWohnung.SelectedIndex;
                string szNewBezeichnung = new InputWindow("Wohnungs-Tag bearbeiten", wohnung.Bezeichnung) 
                { 
                    IsValidHandler = (szWert) => {
                        var existingWohnung = projectSql.sqlWohnung.Get(szWert) as Wohnung;
                        if (existingWohnung == null)
                        {
                            return true;
                        }
                        return false;
                    } 
                }.ShowDialog();
                if (!string.IsNullOrWhiteSpace(szNewBezeichnung))
                {
                    projectSql.sqlWohnung.Edit(wohnung, szNewBezeichnung);
                }
                cmbWohnung.ItemsSource = null;
                cmbWohnung.ItemsSource = projectSql.sqlWohnung.GetListe();
                cmbWohnung.SelectedIndex = selectedIndex;
            }

            var zimmer = ((Button)sender).Tag as Zimmer;
            if (zimmer != null)
            {
                int selectedIndex = cmbZimmer.SelectedIndex;
                string szNewBezeichnung = new InputWindow("Gebäude Tag bearbeiten", zimmer.Bezeichnung)
                {
                    IsValidHandler = (szWert) => {
                        var existingZimmer = projectSql.sqlZimmer.Get(szWert) as Zimmer;
                        if (existingZimmer == null)
                        {
                            return true;
                        }
                        return false;
                    }
                }.ShowDialog();
                if (!string.IsNullOrWhiteSpace(szNewBezeichnung))
                {
                    projectSql.sqlZimmer.Edit(zimmer, szNewBezeichnung);
                }
                cmbZimmer.ItemsSource = null;
                cmbZimmer.ItemsSource = projectSql.sqlZimmer.GetListe();
                cmbZimmer.SelectedIndex = selectedIndex;
            }
        }

        private void BtnEditImage_Click(object sender, RoutedEventArgs e)
        {
            var bmk = ((Button)sender).Tag as BildMitKommentar;
            var newbmk = new ImageEditWindow(projectSql, bmk) { Owner = this }.ShowDialog();

            if (newbmk != null)
            {
                if (newbmk.BildInfo.GebaeudeId != 0) projectSql.sqlGebaeude.Set(newbmk.BildInfo.BildId, newbmk.BildInfo.GebaeudeId);
                if (newbmk.BildInfo.EtageId != 0) projectSql.sqlEtage.Set(newbmk.BildInfo.BildId, newbmk.BildInfo.EtageId);
                if (newbmk.BildInfo.WohnungId != 0) projectSql.sqlWohnung.Set(newbmk.BildInfo.BildId, newbmk.BildInfo.WohnungId);
                if (newbmk.BildInfo.ZimmerId != 0) projectSql.sqlZimmer.Set(newbmk.BildInfo.BildId, newbmk.BildInfo.ZimmerId);
                if (newbmk.BildInfo.KommentarId != 0) projectSql.SetComment(newbmk.BildInfo.BildId, newbmk.BildInfo.KommentarBezeichnung);
                if (newbmk.BildInfo.CaptureDate != DateTime.MinValue) projectSql.SetCaptureDate(newbmk.BildInfo.BildId, newbmk.BildInfo.CaptureDate);
                BtnSearch_Click(null, null);
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
                string fullname = projectSql.GetImageFullName(bmk.BildName, Config.current.szPicSuffix);
                if (File.Exists(fullname))
                {
                    imgBild.Source = new BitmapImage(new Uri(fullname));
                }
                else
                {
                    Logging.AddError("Image not found: " + fullname);
                }
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

            string templatePath = new PublishWindow().ShowDialog();

            if (string.IsNullOrWhiteSpace(templatePath)) return;

            Config.current.szWordTemplate = templatePath;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".docx";
            dlg.AddExtension = true;
            dlg.InitialDirectory = Config.current.szBasedir;
            dlg.Filter = "Word (*.docx)|*.docx|Alle (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                var bilist = lvBilder.ItemsSource as List<BildMitKommentar>;
                Dictionary<string, BildMitKommentar> dictBilder = new Dictionary<string, BildMitKommentar>();
                foreach (var bild in bilist.FindAll(bi => bi.IsSelected == true))
                {
                    BildMitKommentar bmk = bild as BildMitKommentar;
                    if (bmk != null)
                    {
                        string szFullName = projectSql.GetImageFullName(bmk.BildName, Config.current.szPicSuffix);
                        if (!dictBilder.ContainsKey(szFullName))
                        {
                            dictBilder.Add(szFullName, bmk);
                        }
                    }
                }
                Dictionary<string, string> varDict = new Dictionary<string, string>();
                varDict.Add("Project", projectSql.szProjectName);
                varDict.Add("SearchBuilding", cmbGebaeude.Text);
                varDict.Add("SearchFloor", cmbEtage.Text);
                varDict.Add("SearchFlat", cmbWohnung.Text);
                varDict.Add("SearchRoom", cmbZimmer.Text);
                varDict.Add("SearchComment", tbKommentar.Text);

                if (dpStart.SelectedDate != null)
                {
                    varDict.Add("SearchStartDate", dpStart.SelectedDate.Value.ToShortDateString());
                }
                else
                {
                    varDict.Add("SearchStartDate", Config.current.szWordEmptySearch);
                }
                if (dpEnd.SelectedDate != null)
                {
                    varDict.Add("SearchEndDate", dpEnd.SelectedDate.Value.ToShortDateString());
                }
                else
                {
                    varDict.Add("SearchEndDate", Config.current.szWordEmptySearch);
                }

                varDict.Add("ImageCount", lvBilder.Items.Count.ToString());

                Dictionary<string, string> tmpVarDict = new Dictionary<string, string>(varDict);
                foreach (var kv in tmpVarDict)
                {
                    if(kv.Value == "")
                    {
                        varDict[kv.Key] = Config.current.szWordEmptySearch;
                    }
                }

                if (Docx.FillTable(dlg.FileName, varDict, dictBilder))
                {
                    Mouse.OverrideCursor = null;
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
                    Mouse.OverrideCursor = null;
                    MessageBox.Show("Fehler beim Erstellen der Word-Datei: " + Docx.szError);
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
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
            string szSuffix = Config.current.szDbSuffix;
            dpEnd.SelectedDate = null;
            dpStart.SelectedDate = null;
            lvBilder.ItemsSource = null;
            if ( !string.IsNullOrWhiteSpace(szProjectName) )
            {
                OpenProject(szProjectName, szSuffix);
                cmbProjects.AllowDrop = true;
            }
            else
            {
                cmbProjects.AllowDrop = true;

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
            dpEnd.BlackoutDates.Clear();
            dpEnd.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, dpStart.SelectedDate ?? DateTime.Now));
        }

        private void dpEnd_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            dpStart.BlackoutDates.Clear();
            dpStart.BlackoutDates.Add(new CalendarDateRange(dpEnd.SelectedDate ?? DateTime.Now, DateTime.MaxValue));
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            cmbGebaeude.SelectedItem = null;
            cmbEtage.SelectedItem = null;
            cmbWohnung.SelectedItem = null;
            cmbZimmer.SelectedItem = null;

            tbKommentar.Text = null;

            dpEnd.SelectedDate = null;
            dpStart.SelectedDate = null;

            lvBilder.ItemsSource = null;
        }

        internal void ImportImage()
        {
            System.Windows.Forms.OpenFileDialog fileDlg = new System.Windows.Forms.OpenFileDialog();
            fileDlg.Multiselect = true;
            fileDlg.Filter = "Alle Bilder|*.JPG;*.PNG;*.JPEG;*.GIF;*.TIF;*.TIFF;*.BMP|JPEG-Dateien (*.JPG; *.JPEG)|*.JPG;*.JPEG|PNG-Dateien (*.PNG) |*.PNG|Alle (*.*)|*.*";
            System.Windows.Forms.DialogResult result = fileDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string[] filePaths = fileDlg.FileNames;
                int cnt = 0;
                BildMitKommentar lastbmk = new BildMitKommentar();
                foreach (string filePath in filePaths)
                {
                    cnt++;
                    if (!ImportFile(filePath, string.Format("{0} von {1}", cnt, filePaths.Length), lastbmk))
                    {
                        // ToDo: should we skip all of the following images as well?
                        // break;
                    }
                }

            }
        }
        private Boolean ImportFile(string filePath, string title="", BildMitKommentar lastbmk = null)
        {
            BildMitKommentar newbmk = new BildMitKommentar();
            newbmk.BildInfo = new BildInfo();

            if(lastbmk != null && lastbmk.BildInfo != null)
            {
                newbmk.BildInfo.GebaeudeId = lastbmk.BildInfo.GebaeudeId;
                newbmk.BildInfo.EtageId = lastbmk.BildInfo.EtageId;
                newbmk.BildInfo.WohnungId = lastbmk.BildInfo.WohnungId;
                newbmk.BildInfo.ZimmerId = lastbmk.BildInfo.ZimmerId;
            }

            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string fileExt = System.IO.Path.GetExtension(filePath);
            string baseDir = System.IO.Path.Combine(projectSql.szProjectPath, Config.current.szPicSuffix);
            string newFilePath = System.IO.Path.Combine(baseDir, fileName + fileExt);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(newFilePath))
                    break;

                newFilePath = System.IO.Path.Combine(baseDir, fileName + " (" + i + ")" + fileExt);
            }

            File.Copy(filePath, newFilePath);

            newbmk.BildName = System.IO.Path.GetFileName(newFilePath);
            newbmk.BildPath = newFilePath;

            using (FileStream fs = new FileStream(newFilePath, FileMode.Open, FileAccess.Read))
            {
                using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
                {
                    Regex r = new Regex(":");
                    try
                    {
                        PropertyItem propItem = myImage.GetPropertyItem(36867);
                        string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                        newbmk.BildInfo.CaptureDate = DateTime.Parse(dateTaken);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            var iew = new ImageEditWindow(projectSql, newbmk, true) { Owner = this};
            if (!string.IsNullOrWhiteSpace(title))
            {
                iew.Title += " " + title;
            }
            newbmk = iew.ShowDialog();

            if (newbmk == null)
            {
                File.Delete(newFilePath);
                return false;
            }

            if (lastbmk != null)
            {
                if (lastbmk.BildInfo == null) lastbmk.BildInfo = new BildInfo();

                lastbmk.BildInfo.GebaeudeId = newbmk.BildInfo.GebaeudeId;
                lastbmk.BildInfo.EtageId = newbmk.BildInfo.EtageId;
                lastbmk.BildInfo.WohnungId = newbmk.BildInfo.WohnungId;
                lastbmk.BildInfo.ZimmerId = newbmk.BildInfo.ZimmerId;
            }

            projectSql.AddBild(newbmk.BildName);
            GetBildIdResult bildID = projectSql.GetBildId(newbmk.BildName, newbmk.BildInfo.CaptureDate);

            if (newbmk.BildInfo.GebaeudeBezeichnung != null) projectSql.sqlGebaeude.Set(bildID.BildId, newbmk.BildInfo.GebaeudeId);
            if (newbmk.BildInfo.EtageBezeichnung != null) projectSql.sqlEtage.Set(bildID.BildId, newbmk.BildInfo.EtageId);
            if (newbmk.BildInfo.WohnungBezeichnung != null) projectSql.sqlWohnung.Set(bildID.BildId, newbmk.BildInfo.WohnungId);
            if (newbmk.BildInfo.ZimmerBezeichnung != null) projectSql.sqlZimmer.Set(bildID.BildId, newbmk.BildInfo.ZimmerId);
            if (newbmk.BildInfo.KommentarBezeichnung != null) projectSql.SetComment(bildID.BildId, newbmk.BildInfo.KommentarBezeichnung);

            projectSql.SetCaptureDate(bildID.BildId, newbmk.BildInfo.CaptureDate);
            return true;
        }
        private void imgBild_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                int cnt = 0;
                BildMitKommentar lastbmk = new BildMitKommentar();
                foreach( var file in files)
                {
                    cnt++;
                    if( !ImportFile(file,string.Format("{0} von {1}",cnt,files.Length), lastbmk))
                    {
                        // ToDo: should we skip all of the following images as well?
                        // break;
                    }
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                if(lvBilder.SelectedItems != null)
                {
                    System.Collections.IList imgs = lvBilder.SelectedItems;
                    StringCollection paths = new StringCollection();
                    foreach (BildMitKommentar img in imgs)
                    {
                        paths.Add(img.BildPath);
                    }
                    Clipboard.SetFileDropList(paths);
                }
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
        public bool IsSelected { get; set; } = true;
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
            return false;
                //Application.Current != null && Application.Current.MainWindow != null
                //&& ((MainWindow)Application.Current.MainWindow).spProject.IsEnabled;
        }

        public void Execute(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).Manage();
        }
    }
    public class ApplicationImportImageCommand : ICommand
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
            ((MainWindow)Application.Current.MainWindow).ImportImage();
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
                ProjectSql tmpProject = new ProjectSql(projekt, Config.current.szDbSuffix);
                tmpProject.Patch();
            }
            ((MainWindow)Application.Current.MainWindow).ToLog("Patch done");
            Mouse.OverrideCursor = null;
        }
    }

    public class ApplicationLokalCommand : ICommand
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

            string szCommand = "CheckNetIsolation.exe LoopbackExempt -a -n=13b20aa7-cdd8-4f1c-addb-9b2e991d4a31_tejrjx29myjjt";
            try
            {
                Process q = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.UseShellExecute = true;
                info.Arguments = "/c " + szCommand;
                q.StartInfo = info;
                q.Start();
            }
            catch (Exception ex)
            {
                Process q = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.UseShellExecute = true;
                info.Arguments = "/c " + szCommand;
                info.Verb = "runas";
                q.StartInfo = info;
                q.Start();
            }
        }
    }
    public class ApplicationDocCommand : ICommand
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
            Process.Start(new ProcessStartInfo("https://github.com/helmuttheis/XCamera/wiki"));
        }
    }

    public class ApplicationInfoCommand : ICommand
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
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog();
        }
    }

    public class ApplicationMoveDataCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            // You may not need a body here at all...
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Application.Current != null && Application.Current.MainWindow != null && ((MainWindow)Application.Current.MainWindow).cmbProjects.SelectedItem == null;
        }

        public void Execute(object parameter)
        {
            MoveDataWindow moveDataWindow = new MoveDataWindow();
            moveDataWindow.ShowDialog();
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

        private static readonly ICommand appImportImageCmd = new ApplicationImportImageCommand();
        public static ICommand ApplicationImportImageCommand
        {
            get { return appImportImageCmd; }
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
        private static readonly ICommand appLokalCmd = new ApplicationLokalCommand();
        public static ICommand ApplicationLokalCommand
        {
            get { return appLokalCmd; }
        }
        private static readonly ICommand appDocCmd = new ApplicationDocCommand();
        public static ICommand ApplicationDocCommand
        {
            get { return appDocCmd; }
        }
        private static readonly ICommand appInfoCmd = new ApplicationInfoCommand();
        public static ICommand ApplicationInfoCommand
        {
            get { return appInfoCmd; }
        }
        private static readonly ICommand appMoveDataCmd = new ApplicationMoveDataCommand();
        public static ICommand ApplicationMoveDataCommand
        {
            get { return appMoveDataCmd; }
        }
    }
}
