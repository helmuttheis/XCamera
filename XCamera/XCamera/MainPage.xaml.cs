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
        IExif exif;
        IManager manager;
        public MainPage(IExif exif, IManager manager)
        {
            this.exif = exif;
            this.manager = manager;
            InitializeComponent();
            if (Device.RuntimePlatform.Equals(Device.WPF))
            {
                btnTakePhoto.Clicked += btnTakePhotoWpf_Clicked;
                btnPickPhoto.Clicked += btnPickPhotoWpf_Clicked;
            }
            else
            {
                btnTakePhoto.Clicked += btnTakePhoto_Clicked;
                btnPickPhoto.Clicked += btnPickPhoto_Clicked;
            }
        }

        private async void btnTakePhotoWpf_Clicked(object sender, EventArgs e)
        {
        }
        private async void btnPickPhotoWpf_Clicked(object sender, EventArgs e)
        {
            await manager.Open(); 
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
                    var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(
                        new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                        {
                            AllowCropping = false,
                            SaveToAlbum = true,
                            Directory = "Sample"
                        });

                    if (photo != null)
                    {
                        PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
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
            var photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions()
            {
            });

            if (photo != null)
            {
                entryComment.Text = await exif.GetComment(photo.Path);
                PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
            }
        }
    }
}
