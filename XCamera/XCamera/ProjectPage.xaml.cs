using System;
using System.Collections.Generic;
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

            XCamera.Util.Config.szConfigFile = System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            XCamera.Util.Config.szConfigFile = System.IO.Path.Combine(XCamera.Util.Config.szConfigFile, "LocalState", "XCamera.xml");
            Logging.szLogFile = System.IO.Path.Combine(XCamera.Util.Config.szConfigFile, "LocalState", "XCamera.log");

            ProjectUtil.szServer = "http://" + Config.current.szIP + ":" + Config.current.szPort + "/xcamera";

            projects = ProjectUtil.GetList();
            
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
            if( bIsRemote )
            {
                bIsRemote = false;
                UpdateBtnConnect();
                projects = ProjectUtil.GetList();

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
                    UpdateBtnConnect();

                    remoteProjects = ProjectUtil.GetRemoteList();
                    if( remoteProjects.Count > 0)
                    {
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
                    // ProjectSql.szProjectName = szNewProject;
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
            // crete the directory
            string szDestDir = "";
            // get the SQLite file
            string szDbFile = "";

            if (projects.Any(proj => { return proj.Equals(szProjectName); }))
            {

            }
            else
            {
            }
        }
        //void OnCancelButtonClicked(object sender, EventArgs args)
        //{
        //    overlay.IsVisible = false;
        //}
        private void LstProjects_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (lstProjects.SelectedItem != null)
            {
                if (bIsRemote)
                {
                    DownloadProject(lstProjects.SelectedItem.ToString());
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
    }
}