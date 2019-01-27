using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.StyleSheets;
using XCamera.Util;

namespace XCamera
{
    public partial class MainPage : ContentPage
    {
        public string szFullImageName { get; set; }

        // public Project curProject { get; set; }
        public ProjectSql curProjectSql { get; set; }
        public MainPage()
        {
            //this.exif = exif;
            //this.manager = manager;
            InitializeComponent();
            this.Resources.Add(StyleSheet.FromAssemblyResource(
            IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly,
            "XCamera.Resources.styles.css"));

            XCamera.Util.Config.current.szCurProject = ProjectSql.szProjectName;

            //curProject = new Project(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            ProjectUtil.szBasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            curProjectSql = new ProjectSql();

            btnTakePhoto.Clicked += btnTakePhoto_Clicked;
            btnPickPhoto.Clicked += btnPickPhoto_Clicked;
            btnSaveComment.Clicked += BtnSaveComment_Clicked;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if( !string.IsNullOrWhiteSpace(szFullImageName))
            {
                var memoryStream = new MemoryStream();

                using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                PhotoImage.Source = ImageSource.FromStream(()=> memoryStream);
                string szComment = curProjectSql.GetComment(szFullImageName);
                entryComment.Text = szComment;
            }
        }
        private void BtnSaveComment_Clicked(object sender, EventArgs e)
        {
            if ( !string.IsNullOrWhiteSpace(szFullImageName) )
            {
                curProjectSql.SetComment(szFullImageName, entryComment.Text);
                // curProject.SetComment(szFullImageName, entryComment.Text);
                // curProject.Save();
            }
        }
        
        private async void btnTakePhoto_Clicked(object sender, EventArgs e)
        {
            await Plugin.Media.CrossMedia.Current.Initialize();

            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                if (Plugin.Media.CrossMedia.Current.IsCameraAvailable)
                {
                    Plugin.Media.Abstractions.MediaFile  curPhoto = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(
                        new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                        {
                            AllowCropping = false,
                            Directory = "__temp__"
                        });

                    if (curPhoto != null)
                    {
                        curProjectSql.szTempProjectPath = Path.GetDirectoryName(curPhoto.Path);
                        szFullImageName = Path.Combine(curProjectSql.szProjectPath, Path.GetFileName(curPhoto.Path));

                        File.Copy(curPhoto.Path, szFullImageName);

                        szFullImageName = szFullImageName;
                        var memoryStream = new MemoryStream();

                        using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(memoryStream);
                        }
                        memoryStream.Position = 0;
                        PhotoImage.Source = ImageSource.FromStream(() => memoryStream);

                    }
                }
                else
                {
                    await DisplayAlert("No camera available", "Unable to take photos.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }

            
        }
        private async void btnPickPhoto_Clicked(object sender, EventArgs e)
        {

            ImagePage imagePage = new ImagePage(this);
            await Navigation.PushModalAsync(imagePage);
            // await DisplayAlert("Image selected", imagePage.szImageName, "OK");

         // if (curPhoto != null)
         // {
         //     var stream = curPhoto.GetStream();
         //     PhotoImage.Source = ImageSource.FromStream(() => { return stream; });
         //     string szComment = curProject.GetComment(curPhoto.Path);
         //     entryComment.Text = szComment;
         // }
        }
        List< Picker> lstPicker;
        Entry kommentarEntry;
        private void EntryComment_Focused(object sender, FocusEventArgs e)
        {
            Overlay overlay = new Overlay(grdOverlay);
            if (lstPicker == null)
            {
                overlay.Reset();
                grdOverlay.Children.Clear();
                lstPicker = new List<Picker>();
                
                var lstLevel = curProjectSql.GetLevelList();
                foreach (var szLevel in lstLevel)
                {
                    Picker newPicker = overlay.AddPicker(overlay.iRow.ToString(), szLevel);

                    newPicker.Items.Add("---     ---");
                    newPicker.Items.Add("--- neu ---");
                    var levelValuesList = curProjectSql.GetLevelValuesList(overlay.iRow);
                    foreach(var levelValue in levelValuesList)
                    {
                        newPicker.Items.Add(levelValue);
                    }

                    newPicker.SelectedIndexChanged += (curSender, e1) => {
                        Picker curPicker = (Picker)curSender;
                        string szId = curPicker.StyleId;
                        int r;
                        if( int.TryParse(szId, out r) )
                        {
                            for(int i=r+1;i<lstLevel.Count;i++)
                            {
                                lstPicker[i].IsVisible = false;
                            }
                            string szValue = curPicker.SelectedItem.ToString();
                            if( !szValue.StartsWith("-"))
                            {
                                if (r < lstLevel.Count - 1)
                                {
                                    lstPicker[r + 1].IsVisible = true;
                                }
                            }
                        }

                    };
                    lstPicker.Add(newPicker);
                }
                kommentarEntry = overlay.AddInput("", "", curProjectSql.GetComment(szFullImageName));
                var submitButton = overlay.AddButton("anlegen" );
                submitButton.Clicked += (senderx, e2) =>
                {
                    curProjectSql.SetComment(szFullImageName, kommentarEntry.Text);
                    overlay.Close();
                };                

                // var cancelButton = overlay.AddButton("abbrechen");
                // cancelButton.Clicked += (senderx, e2) =>
                // {
                //     overlay.Close();
                // };
                overlay.AddCancelX();
            }
            else
            {
                // set the selected items in the pickers

                // set the comment
                kommentarEntry.Text = curProjectSql.GetComment(szFullImageName);
            }
            overlay.Show();
        }

        private void NewPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
