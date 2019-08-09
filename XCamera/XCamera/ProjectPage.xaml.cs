using System;
using System.Collections.Generic;
using System.ComponentModel;
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
      //  public FlagToBoolean flagToBoolean = new FlagToBoolean();


        private Boolean bIsRemote = false;
        private List<string> projects;
        private List<string> remoteProjects;

        private const string ID_IP = "ID_IP";
        private const string ID_URL = "ID_URL";
        private const string ID_NAME = "ID_NAME";
        
        private Boolean bFirstRun = true;
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

           // this.BindingContext = this;
		}
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (bFirstRun)
            {
                List<string> deletedFiles = ProjectUtil.GetDeletedProjectList();
                if(deletedFiles.Count > 0)
                {
                    Overlay overlay = new Overlay(grdOverlay);
                    overlay.ShowQuestion("Es gibt " + deletedFiles.Count + " Projekte, die zum Löschen vorgemerkt sind." + Environment.NewLine +
                        " Sollen die jetzt gelöscht werden?", () =>
                    {
                        foreach(var deleted in deletedFiles)
                        {
                            DeleteProject(true,deleted.Split('.').First());
                        }
                    });
                }
            }
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
                FlagToBoolean.bIsConnected = bIsRemote;
                FlagToBoolean.bIsConnectedToSend = bIsRemote;
                
                UpdateBtnConnect();
                projects = ProjectUtil.GetProjectList();
                lstProjects.StyleId = bIsRemote.ToString();

                lstProjects.ItemsSource = null;
                lstProjects.ItemsSource = projects;
                lstProjects.SelectedItem = projects.Find(proj => { return proj.Equals(XCamera.Util.Config.current.szCurProject); });

                return;
            }
            Overlay overlay = new Overlay(grdOverlay);
            overlay.Reset();
            Entry entryServer = overlay.AddInput("IP Adresse", "Url", Config.current.szIP, ID_IP);
            Entry entryPort = overlay.AddInput("Port", "Port",Config.current.szPort, ID_URL);
            var uploadButton = overlay.AddButton("senden");
            var downloadButton = overlay.AddButton("empfangen");
            overlay.AddRowDefinitions();
            overlay.AddCancelX();

            uploadButton.Clicked += async (senderx, e2) =>
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
                    FlagToBoolean.bIsConnected = bIsRemote;
                    FlagToBoolean.bIsConnectedToSend = bIsRemote;

                    UpdateBtnConnect();

                    lstProjects.ItemsSource = null;
                    lstProjects.ItemsSource = projects;
                    // close the overlay
                    overlay.Close();
                }
            };
            downloadButton.Clicked += async (senderx, e2) =>
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
                    FlagToBoolean.bIsConnected = bIsRemote;
                    FlagToBoolean.bIsConnectedToSend = !bIsRemote;

                    lstProjects.StyleId = bIsRemote.ToString();
                    UpdateBtnConnect();
                    remoteProjects = new List<string>();
                    lblStatus.Text = "Verbindung mit " + ProjectUtil.szServer + " wird aufgebaut ...";

                    Overlay overlay2 = new Overlay(grdOverlay2);
                    overlay2.ShowRunMessage("Versuche eine Verbindung mit dem Server " + szIp + " aufzubauen...");

                    try
                    {
                        remoteProjects = await ProjectUtil.GetRemoteProjectListAsync();
                    }
                    catch (Exception ex)
                    {
                        // could not connect 
                        lblStatus.Text = "Keine Verbindung mit " + ProjectUtil.szServer;
                    }
                    finally
                    {
                        overlay2.Close();
                        if (remoteProjects.Count > 0)
                        {
                            lblStatus.Text = "Verbindung mit " + ProjectUtil.szServer;
                            Config.current.szIP = szIp;
                            Config.current.szPort = szPort;
                            lstProjects.ItemsSource = null;
                            lstProjects.ItemsSource = remoteProjects;

                            // close the overlay
                            overlay.Close();
                        }
                        else
                        {
                            // revert the state
                            bIsRemote = false;
                            FlagToBoolean.bIsConnected = bIsRemote;
                            FlagToBoolean.bIsConnectedToSend = bIsRemote;

                            lstProjects.StyleId = bIsRemote.ToString();
                            UpdateBtnConnect();

                            // here we could add an error text to overlay
                        }
                    }
                }
            };
            overlay.Show();

        }

        private void BtnNew_Clicked(object sender, EventArgs e)
        {
            Overlay overlay = new Overlay(grdOverlay);
            overlay.Reset();
            Entry entryProject = overlay.AddInput("Projekt", "Name", "",ID_NAME);
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
                SetStatusLine("");
                ProjectUtil.GetRemoteMetaData(szProjectName,(msg)=>
                {
                    AddStatusLine(msg);
                });
                projects.Add(szProjectName);
                lstProjects.ItemsSource = null;
                lstProjects.ItemsSource = projects;
                XCamera.Util.Config.current.szCurProject = szProjectName;
                new ProjectSql(XCamera.Util.Config.current.szCurProject);

                // change the state to edit instead of send/download.
                BtnConnect_Clicked(null, null);
            }
        }
        // private void DownloadProject
        private async Task SendProjectAsync(string szProjectName, Action<Boolean, Boolean, string> cb)
        {
            if (projects.Any(proj => { return proj.Equals(szProjectName); }))
            {
                // ToDo: open the overlay with the server settings
                // here we expect the current settings to be correct.
                cb(false, false, "sende Projekt " + szProjectName);
                ProjectSql tmpProject = new ProjectSql(szProjectName);
                List<Bild> bilder =  tmpProject.GetBilderChanged();
                Boolean bError = false;
                // send all changed images
                cb(false, false, "sende " + bilder.Count.ToString() + " Bilder");
                foreach (var bild in bilder)
                {
                    cb(false, false, "sende " + bild.Name);
                    bError = ! await ProjectUtil.SendFileAsync(szProjectName, Path.GetFileName(bild.Name));
                    if( bError)
                    {
                        break;
                    }
                }
                if (!bError)
                {
                    // send all changed data
                    foreach (var bild in bilder)
                    {
                        cb(false, false, "sende geänderte Daten für " + bild.Name);

                        BildInfo bi = tmpProject.GetBildInfo(bild.Name, DateTime.Now);

                        string szJson = Newtonsoft.Json.JsonConvert.SerializeObject(bi);
                        if (await ProjectUtil.SendJsonAsync(szProjectName, szJson))
                        {
                            tmpProject.ClearStatus(bild.ID, STATUS.CHANGED);
                        }

                    }
                }
                string szMessage = "";
                if( bError)
                {
                    szMessage = "Es gab einen internen Fehler.";
                }
                cb(true, bError, szMessage);

            }
        }

        private void LstProjects_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (lstProjects.SelectedItem != null)
            {
                string szProjectName = lstProjects.SelectedItem.ToString();
                if (bIsRemote && !FlagToBoolean.bIsConnectedToSend)
                {
                    if (  ProjectSql.DbExists(szProjectName) )
                    {
                        ProjectSql tmpProject = new ProjectSql(szProjectName);
                        if(tmpProject.IsDirty())
                        {
                            Overlay overlay = new Overlay(grdOverlay);
                            overlay.ShowMessage("Das Projekt enthält geänderte Daten, die noch nicht gesichert wurden.");
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
                else if( bIsRemote && FlagToBoolean.bIsConnectedToSend)
                {
                    Overlay overlay = new Overlay(grdOverlay);
                    overlay.ShowRunMessage("Versuche das Projekt " + szProjectName + " an den Server " + Config.current.szIP + " zu senden ...");

                    Task.Run(async () =>
                    {
                        SetStatusLine("");
                        await SendProjectAsync(szProjectName, (bFinished, bError, szStr) =>
                        {
                            /* bFinished == true the sending is completed
                             * bError == true an error occured
                             * szStr   the message or error text
                             */
                            if (bFinished)
                            {
                                // the overlay does not have the cancel X, so we have to close it here
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    overlay.Close();
                                });
                            }
                            AddStatusLine(szStr);
                        });
                    });

                }
                else if( !bIsRemote )
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
                if( FlagToBoolean.bIsConnectedToSend)
                {
                    lblCaption.Text = "Projektliste zum Senden";
                }
                else
                {
                    lblCaption.Text = "Projektliste zum Empfangen";
                }
            }
            else
            {
                btnConnect.Text = "verbinden";
                btnNew.IsEnabled = true;
                lblCaption.Text = "Projektliste zum Bearbeiten";
            }
        }
#if false
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            if (bIsRemote)
            {
                string szProject = (sender as Button).CommandParameter.ToString();

                Overlay overlay = new Overlay(grdOverlay);
                overlay.ShowRunMessage("Versuche das Projekt " + szProject + " an den Server " + Config.current.szIP + " zu senden ...");

                await SendProjectAsync(szProject,(bFinished, bError, szStr) => {
                    if (bFinished)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            overlay.Close();
                        });
                    }
                    SetStatusLine(szStr);
                });
            }
        }
#endif
        private void SetStatusLine(string szMessage)
        {
            Device.BeginInvokeOnMainThread(() => {
                lblStatus.Text = szMessage;
            });
        }
        private void AddStatusLine(string szMessage)
        {
            Device.BeginInvokeOnMainThread(() => {
                lblStatus.Text += Environment.NewLine + szMessage;
            });
        }
        private void DeleteProject(Boolean bForce,string szProjectName)
        {
                string szDelRet = ProjectSql.Delete(bForce, szProjectName);
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
                            DeleteProject(false,szProjectName);
                        });
                    }
                    else
                    {
                        DeleteProject(false,szProjectName);
                    }
                });
            }
        }
    }
    public class FlagToBoolean : IValueConverter, INotifyPropertyChanged
    {
        public static Boolean bIsConnected { get; set; } = false;
        public static Boolean bIsConnectedToSend { get; set; } = false;

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter is string)
            {
                if (parameter.Equals("send"))
                {
                    return bIsConnectedToSend;
                }
                return !bIsConnected;
            }
            return bIsConnected;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter is string)
            {
                if (parameter.Equals("send"))
                {
                    return bIsConnectedToSend;
                }
                return !bIsConnected;
            }
            return bIsConnected;
        }
        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}