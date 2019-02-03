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
        // public string szFullImageName { get; set; }
        public string szImageName { get; set; }


        public ProjectSql curProjectSql { get; set; }
        public MainPage()
        {
            InitializeComponent();
            this.Resources.Add(StyleSheet.FromAssemblyResource(
            IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly,
            "XCamera.Resources.styles.css"));

            
            curProjectSql = new ProjectSql(XCamera.Util.Config.current.szCurProject);

            btnTakePhoto.Clicked += btnTakePhoto_Clicked;
            btnPickPhoto.Clicked += btnPickPhoto_Clicked;
            btnSaveComment.Clicked += BtnSaveComment_Clicked;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if( !string.IsNullOrWhiteSpace(szImageName))
            {
                string szFullImageName = curProjectSql.GetImageFullName(szImageName);
                var memoryStream = new MemoryStream();

                using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                PhotoImage.Source = ImageSource.FromStream(()=> memoryStream);
                string szComment = curProjectSql.GetKommentar(szImageName);
                entryComment.Text = szComment;
            }
        }
        private void BtnSaveComment_Clicked(object sender, EventArgs e)
        {
            if ( !string.IsNullOrWhiteSpace(szImageName) )
            {
                curProjectSql.SetComment(szImageName, entryComment.Text);
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
                        szImageName = Path.GetFileName(curPhoto.Path);
                        curProjectSql.szTempProjectPath = Path.GetDirectoryName(curPhoto.Path);
                        string szFullImageName = Path.Combine(curProjectSql.szProjectPath, Path.GetFileName(curPhoto.Path));

                        File.Copy(curPhoto.Path, szFullImageName);

                        var memoryStream = new MemoryStream();

                        using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(memoryStream);
                        }
                        memoryStream.Position = 0;
                        PhotoImage.Source = ImageSource.FromStream(() => memoryStream);
                        ShowMetadata();
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
        BildInfo lastBi;
        private void ShowMetadata()
        {
            Overlay overlay = new Overlay(grdOverlay);
            // if (lstPicker == null)
            {
                BildInfo bi = curProjectSql.GetBildInfo(szImageName);
                if (lastBi != null)
                {
                    if (!bi.bBildIdFound)
                    {
                        bi.GebaeudeId = lastBi.GebaeudeId;
                        bi.EtageId = lastBi.EtageId;
                        bi.WohnungId = lastBi.WohnungId;
                        bi.ZimmerId = lastBi.ZimmerId;
                    }
                }
                List<Gebaeude> gebaeudeListe = curProjectSql.GetGebaeudeListe();
                overlay.Reset();
                grdOverlay.Children.Clear();
                Picker pGebaeude = overlay.AddPicker(overlay.iRow.ToString(), "Gebäude", true, (picker, szEntry) => {
                    Gebaeude newGebaeude = curProjectSql.EnsureGebaeude(szEntry);
                    if (!gebaeudeListe.Any(g => g.ID == newGebaeude.ID))
                    {
                        gebaeudeListe.Add(newGebaeude);
                        picker.ItemsSource = null;
                        picker.ItemsSource = gebaeudeListe.OrderBy(o => o.Bezeichnung).ToList();
                        picker.SelectedItem = newGebaeude;
                    }
                });
                pGebaeude.ItemsSource = gebaeudeListe.OrderBy(o => o.Bezeichnung).ToList();
                pGebaeude.ItemDisplayBinding = new Binding("Bezeichnung");
                pGebaeude.SelectedItem = gebaeudeListe.Find(x => x.ID == bi.GebaeudeId);

                List<Etage> etageListe = curProjectSql.GetEtagenListe();
                Picker pEtage = overlay.AddPicker(overlay.iRow.ToString(), "Etage", true, (picker, szEntry) => {
                    Etage newEtage = curProjectSql.EnsureEtage(szEntry);
                    if (!etageListe.Any(g => g.ID == newEtage.ID))
                    {
                        etageListe.Add(newEtage);
                        picker.ItemsSource = null;
                        picker.ItemsSource = etageListe.OrderBy(o => o.Bezeichnung).ToList();
                        picker.SelectedItem = newEtage;
                    }
                });
                pEtage.ItemsSource = etageListe.OrderBy(o => o.Bezeichnung).ToList();
                pEtage.ItemDisplayBinding = new Binding("Bezeichnung");
                pEtage.SelectedItem = etageListe.Find(x => x.ID == bi.EtageId);

                List<Wohnung> wohnungListe = curProjectSql.GetWohnungListe();
                Picker pWohnung = overlay.AddPicker(overlay.iRow.ToString(), "Wohnung", true, (picker, szEntry) => {
                    Wohnung newWohnung = curProjectSql.EnsureWohnung(szEntry);
                    if (!wohnungListe.Any(g => g.ID == newWohnung.ID))
                    {
                        wohnungListe.Add(newWohnung);
                        picker.ItemsSource = null;
                        picker.ItemsSource = wohnungListe.OrderBy(o => o.Bezeichnung).ToList(); ;
                        picker.SelectedItem = newWohnung;
                    }
                });
                pWohnung.ItemsSource = wohnungListe.OrderBy(o => o.Bezeichnung).ToList(); ;
                pWohnung.ItemDisplayBinding = new Binding("Bezeichnung");
                pWohnung.SelectedItem = wohnungListe.Find(x => x.ID == bi.WohnungId);

                List<Zimmer> zimmerListe = curProjectSql.GetZimmerListe();
                Picker pZimmer = overlay.AddPicker(overlay.iRow.ToString(), "Zimmer", true, (picker, szEntry) => {
                    Zimmer newZimmer = curProjectSql.EnsureZimmer(szEntry);
                    if (!zimmerListe.Any(g => g.ID == newZimmer.ID))
                    {
                        zimmerListe.Add(newZimmer);
                        picker.ItemsSource = null;
                        picker.ItemsSource = zimmerListe.OrderBy(o => o.Bezeichnung).ToList(); ;
                        picker.SelectedItem = newZimmer;
                    }
                });
                pZimmer.ItemsSource = zimmerListe.OrderBy(o => o.Bezeichnung).ToList(); ;
                pZimmer.ItemDisplayBinding = new Binding("Bezeichnung");
                pZimmer.SelectedItem = zimmerListe.Find(x => x.ID == bi.ZimmerId);

                kommentarEntry = overlay.AddInput("", "", bi.KommentarBezeichnung);
                var submitButton = overlay.AddButton("speichern");
                submitButton.Clicked += (senderx, e2) =>
                {
                    var selGebaeude = pGebaeude.SelectedItem as Gebaeude;
                    var selEtage = pEtage.SelectedItem as Etage;
                    var selWohnung = pWohnung.SelectedItem as Wohnung;
                    var selZimmer = pZimmer.SelectedItem as Zimmer;

                    curProjectSql.SetGebaeude(bi.BildId, selGebaeude != null ? selGebaeude.ID : -1);
                    curProjectSql.SetEtage(bi.BildId, selEtage != null ? selEtage.ID : -1);
                    curProjectSql.SetWohnung(bi.BildId, selWohnung != null ? selWohnung.ID : -1);
                    curProjectSql.SetZimmer(bi.BildId, selZimmer != null ? selZimmer.ID : -1);

                    curProjectSql.SetComment(szImageName, kommentarEntry.Text);

                    lastBi = curProjectSql.GetBildInfo(szImageName);
                    overlay.Close();
                };

                overlay.AddCancelX();
            }
            
            overlay.Show();
        }
        private void EntryComment_Focused(object sender, FocusEventArgs e)
        {
            ShowMetadata();
        }

        private void NewPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
