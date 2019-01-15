using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XCamera
{
    public partial class MainPage : ContentPage
    {
        
        public string szFullImageName { get; set; }
        // private Plugin.Media.Abstractions.MediaFile curPhoto;

        public Project curProject { get; set; }

        public MainPage()
        {
            //this.exif = exif;
            //this.manager = manager;
            InitializeComponent();

            curProject = new Project();

            btnTakePhoto.Clicked += btnTakePhoto_Clicked;
            btnPickPhoto.Clicked += btnPickPhoto_Clicked;
            btnSaveComment.Clicked += BtnSaveComment_Clicked;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if( !string.IsNullOrWhiteSpace(szFullImageName))
            {
                PhotoImage.Source = ImageSource.FromFile(szFullImageName);
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
                        //  entryComment.Text = await exif.GetComment(curPhoto.GetStream());
                        szFullImageName = curPhoto.Path;
                        PhotoImage.Source = ImageSource.FromStream(() => { return curPhoto.GetStream(); });
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
