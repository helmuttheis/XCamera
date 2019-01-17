using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XCamera.Util;

namespace XCamera
{
    public partial class MainPage : ContentPage
    {
        public string szFullImageName { get; set; }

        public Project curProject { get; set; }

        public MainPage()
        {
            //this.exif = exif;
            //this.manager = manager;
            InitializeComponent();

            curProject = new Project(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

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
                string szComment = curProject.GetComment(szFullImageName);
                entryComment.Text = szComment;
            }
        }
        private void BtnSaveComment_Clicked(object sender, EventArgs e)
        {
            if ( !string.IsNullOrWhiteSpace(szFullImageName) )
            {
                curProject.SetComment(szFullImageName, entryComment.Text);
                curProject.Save();
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
                            Directory = "Sample"
                        });

                    if (curPhoto != null)
                    {
                        curProject.szTempProjectPath = Path.GetDirectoryName(curPhoto.Path);
                        szFullImageName = Path.Combine(curProject.szProjectPath, Path.GetFileName(curPhoto.Path));

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
    }
}
