using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.StyleSheets;
using Xamarin.Forms.Xaml;
using XCamera.Util;

namespace XCamera
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectPage : ContentPage
	{
        private Boolean bIsRemote = false;
        private List<string> projects;
        private List<string> remoteProjects;

        public ProjectPage ()
		{
			InitializeComponent();
            this.Resources.Add(StyleSheet.FromAssemblyResource(
            IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly,
            "XCamera.Resources.styles.css"));

            string szTemp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Logging.szLogFile = System.IO.Path.Combine(szTemp, "XCamera.log");
            XCamera.Util.Config.szConfigFile = System.IO.Path.Combine(szTemp,  "XCamera.xml");
            ProjectUtil.szBasePath = szTemp;

            ProjectUtil.szServer = "http://" + Config.current.szIP + ":" + Config.current.szPort + "/xcamera";

            lstProjects.StyleId = bIsRemote.ToString();
            projects = ProjectUtil.GetProjectList();
            
            lstProjects.ItemsSource = projects;

            lstProjects.SelectedItem = projects.Find(proj => { return proj.Equals(XCamera.Util.Config.current.szCurProject); });

		}
        async protected override void OnAppearing()
        {
            base.OnAppearing();
#if false
            if curProject != null )//u( !string.IsNullOrWhiteSpace(ProjectSql.szProjectName))
            {
               // ProjectUtil.szBasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
               // var curProject = new ProjectSql();
                if( curProject.HasDeleted())
                {
                    Boolean bClear = await DisplayAlert("Projekt " + ProjectSql.szProjectName, "Die gelöschten Bilder endgültig entfernen?", "Ja", "Nein");
                    if (bClear )
                    {
                        curProject.ClearDeleted();
                        await DisplayAlert("Projekt ", "Die gelöschten Bilder wurden endgültig entfernen.", "Weiter");
                    }
                }
                if( curProject.IsDirty())
                {
                   // await DisplayAlert("Projekt ", "IsDirty", "Weiter");
                }
                if (!string.IsNullOrWhiteSpace(curProject.GetTempDir()))
                {
                    string[] imgFiles = System.IO.Directory.GetFiles(curProject.GetTempDir(), "*.jpg");
                    foreach (var imgFile in imgFiles)
                    {
                        try
                        {
                            System.IO.File.Delete(imgFile);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
#endif
        }
        private void BtnConnect_Clicked(object sender, EventArgs e)
        {
            
            if ( bIsRemote )
            {
                bIsRemote = false;
                FlagToBoolean.bVisible = !bIsRemote;
                UpdateBtnConnect();
                projects = ProjectUtil.GetProjectList();
                lstProjects.StyleId = bIsRemote.ToString();

                lstProjects.ItemsSource = projects;
                lstProjects.SelectedItem = projects.Find(proj => { return proj.Equals(XCamera.Util.Config.current.szCurProject); });

                return;
            }
            Overlay overlay = new Overlay(grdOverlay);
            overlay.Reset();
            Entry entryServer = overlay.AddInput("IP Adresse", "Url", Config.current.szIP);
            Entry entryPort = overlay.AddInput("Port", "Port",Config.current.szPort);
            var submitButton = overlay.AddButton("verbinden");
            overlay.AddRowDefinitions();
            overlay.AddCancelX();

            submitButton.Clicked += async (senderx, e2) =>
            {
                string szIp = "";
                string szPort = "";
                if (entryServer.Text != null)
                {
                    szIp = entryServer.Text.Trim();
                }
                if (entryPort.Text != null)
                {
                    szPort = entryPort.Text.Trim();
                }
                if (string.IsNullOrWhiteSpace(szIp))
                {
                    await DisplayAlert("", "Die IP Adresse darf nicht leer sein.", "Weiter");
                }
                else 
                {
                    ProjectUtil.szServer = "http://" + szIp + ":" + szPort + "/xcamera";
                    bIsRemote = true;
                    FlagToBoolean.bVisible = !bIsRemote;

                    lstProjects.StyleId = bIsRemote.ToString();
                    UpdateBtnConnect();
                    remoteProjects = new List<string>();
                    try
                    {
                        remoteProjects = ProjectUtil.GetRemoteProjectList();
                    }
                    catch (Exception ex)
                    {
                        // could not connect 
                        lblStatus.Text = "Keine Verbidung mit " + ProjectUtil.szServer;
                    }
                    if( remoteProjects.Count > 0)
                    {
                        lblStatus.Text = "Verbidung mit " + ProjectUtil.szServer;
                        Config.current.szIP = szIp;
                        Config.current.szPort = szPort;
                    }

                    lstProjects.ItemsSource = remoteProjects;

                    // close the overlay
                    overlay.Close();
                }
            };

            overlay.Show();

        }

        private void BtnNew_Clicked(object sender, EventArgs e)
        {
            Overlay overlay = new Overlay(grdOverlay);
            overlay.Reset();
            Entry entryProject = overlay.AddInput("Projekt", "Name", "");
            var submitButton = overlay.AddButton( "anlegen");
            overlay.AddRowDefinitions();
            overlay.AddCancelX();

            submitButton.Clicked += async (senderx, e2) =>
            {
                string szNewProject = "";
                if (entryProject.Text != null)
                {
                    szNewProject = entryProject.Text.Trim();
                }
                if( string.IsNullOrWhiteSpace(szNewProject))
                {
                    await DisplayAlert("", "Den Projektname darf nicht leer sein.", "Weiter");
                }
                else if ( !ProjectUtil.IsValidName(szNewProject))
                {
                    await DisplayAlert("", "Den Projektname darf nicht mit __ beginnen.", "Weiter");
                }
                else if (!projects.Any(s => s.Equals(szNewProject, StringComparison.OrdinalIgnoreCase)))
                {
                    projects.Add(szNewProject);
                    lstProjects.ItemsSource = null;
                    lstProjects.ItemsSource = projects;
                    XCamera.Util.Config.current.szCurProject = szNewProject;
                    await Navigation.PushAsync(new MainPage());
                    // close the overlay
                    overlay.Close();
                }
                else
                {
                    await DisplayAlert("", "Den Projektname gibt es schon.", "Weiter");
                }
            };

            overlay.Show();
            
        }
        private void DownloadProject(string szProjectName)
        {
            Boolean bLoad = true;
            if (projects.Any(proj => { return proj.Equals(szProjectName); }))
            {
            
            }
            if( bLoad )
            {
                ProjectUtil.GetRemoteMetaData(szProjectName);
                projects.Add(szProjectName);
                lstProjects.ItemsSource = null;
                lstProjects.ItemsSource = projects;
                XCamera.Util.Config.current.szCurProject = szProjectName;
                new ProjectSql(XCamera.Util.Config.current.szCurProject);

                BtnConnect_Clicked(null, null);
            }
        }
        // private void DownloadProject
        private void SendProject(string szProjectName, Action<string> cb)
        {
            if (projects.Any(proj => { return proj.Equals(szProjectName); }))
            {
                // ToDo: open the overlay with the server settings
                // here we expect the current settings to be correct.

                ProjectSql tmpProject = new ProjectSql(szProjectName);
                List<Bild> bilder =  tmpProject.GetBilderChanged();

                // send all changed images
                foreach (var bild in bilder)
                {
                    cb("send " + bild.Name);
                    ProjectUtil.SendFile(szProjectName, Path.GetFileName(bild.Name));

                }
                // send all changed data
                foreach (var bild in bilder)
                {
                    cb("send changed data for " + bild.Name);

                    BildInfo bi = tmpProject.GetBildInfo(bild.Name);

                    string szJson = Newtonsoft.Json.JsonConvert.SerializeObject(bi);
                    ProjectUtil.SendJson(szProjectName, szJson);
                    tmpProject.SetStatus(bild.ID, STATUS.NONE);
                }
                cb("");

            }
        }

        private void LstProjects_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (lstProjects.SelectedItem != null)
            {
                if (bIsRemote)
                {
                    string szProjectName = lstProjects.SelectedItem.ToString();
                    if (  ProjectSql.DbExists(szProjectName) )
                    {
                        ProjectSql tmpProject = new ProjectSql(szProjectName);
                        if(tmpProject.IsDirty())
                        {
                            Overlay overlay = new Overlay(grdOverlay);
                            overlay.ShowMessage("Das Projekt enthält noch geänderte Daten, die noch nicht gesichert wurden.");
                        }
                        else
                        {
                            DownloadProject(szProjectName);
                        }
                    }
                    else
                    {
                        DownloadProject(szProjectName);
                    }
                }
                else
                {
                    XCamera.Util.Config.current.szCurProject = lstProjects.SelectedItem.ToString();
                    Navigation.PushAsync(new MainPage());
                }
            }
        }
        private void UpdateBtnConnect()
        {
            if( bIsRemote)
            {
                btnConnect.Text = "trennen";
                btnNew.IsEnabled = false;
            }
            else
            {
                btnConnect.Text = "verbinden";
                btnNew.IsEnabled = true;
            }
        }

        private void BtnSend_Clicked(object sender, EventArgs e)
        {
            if (bIsRemote)
            {

            }
            else
            {
                string szProject = (sender as Button).CommandParameter.ToString();

                SendProject(szProject, SetStatusLine);
            }
        }
        private void SetStatusLine(string szMessage)
        {
            Device.BeginInvokeOnMainThread(() => {
                lblStatus.Text = szMessage;
            });
        }
        private void DeleteProject(string szProjectName)
        {
            string szDelRet = ProjectSql.Delete(szProjectName);
            if (string.IsNullOrWhiteSpace(szDelRet))
            {
                lblStatus.Text = "";
                projects.Remove(szProjectName);
                lstProjects.ItemsSource = null;
                lstProjects.ItemsSource = projects;
            }
            else
            {
                lblStatus.Text = szDelRet;
            }
        }
        private void BtnDelete_Clicked(object sender, EventArgs e)
        {
            if (bIsRemote)
            {

            }
            else
            {
                string szProjectName = (sender as Button).CommandParameter.ToString();

                Overlay overlay = new Overlay(grdOverlay);
                overlay.ShowQuestion("Soll das Projekt " + szProjectName + " gelöscht werden?", () => {
                    ProjectSql tmpProject = new ProjectSql(szProjectName);
                    if(tmpProject.GetBilderChanged().Count > 0 )
                    {
                        overlay.ShowQuestion("Das Projekt " + szProjectName + " wurde nicht nicht gesichert." + Environment.NewLine +"Soll es trotzdem gelöscht werden?", () =>
                        {
                            DeleteProject(szProjectName);
                        });
                    }
                    else
                    {
                        DeleteProject(szProjectName);
                    }
                });
            }
        }
    }
    class FlagToBoolean : IValueConverter
    {
        public static Boolean bVisible = true;
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return bVisible ;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return bVisible;
        }
    }
}