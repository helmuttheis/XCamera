using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XCamera.Util;

namespace XCamera
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImagePage : ContentPage
	{
        
        public ObservableCollection<ImageViewModel> images { get; set; }

        private MainPage mainPage;
        public ImagePage (MainPage mainPage)
		{
            this.mainPage = mainPage;
            mainPage.szImageName = "";
            InitializeComponent ();


            images = new ObservableCollection<ImageViewModel>();

            List<Bild> bilder = mainPage.curProjectSql.GetBilder(null,null);

            foreach (var bild in bilder)
            {
                if (!string.IsNullOrEmpty(bild.Name))
                {
                    string szImageName = bild.Name;
                    string szFullImageName = mainPage.curProjectSql.GetImageFullName(szImageName);

                    if (File.Exists(szFullImageName))
                    {
                        var memoryStream = new MemoryStream();

                        using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(memoryStream);
                        }
                        memoryStream.Position = 0;
                        BildInfo bildInfo = mainPage.curProjectSql.GetBildInfo(szFullImageName, DateTime.Now);
                        images.Add(new ImageViewModel
                        {
                            BildInfo = mainPage.curProjectSql.GetBildInfo(szImageName, DateTime.Now),
                            Comment = mainPage.curProjectSql.GetKommentar(szImageName),
                            ImageSource = ImageSource.FromStream(() => memoryStream),
                            ImageName = szImageName
                        });
                    }
                }
            }
            lstView.ItemsSource = images;
        }
        protected override void OnDisappearing()
        {
            for (int i = images.Count - 1; i >= 0;i--)
            {
                images.RemoveAt(i);
            }
            BindingContext = null;
            Content = null;
            base.OnDisappearing();
            GC.Collect();
        }

        private void LstView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (lstView.SelectedItem != null)
            {
                mainPage.szImageName = ((ImageViewModel)lstView.SelectedItem).ImageName;
                mainPage.bShowMetadata = true;
                Navigation.PopModalAsync();
            }
        }

        private void BtnDelete_Clicked(object sender, EventArgs e)
        {
            var selectedLocation = images.First(item =>
                            item.ImageName.Equals((sender as Button).CommandParameter));
            if ( selectedLocation != null)
            {
                images.Remove(selectedLocation);
                
                mainPage.curProjectSql.DeleteImage(selectedLocation.ImageName);
            }
        }
    }

    public class ImageViewModel
    {
        public BildInfo BildInfo { get; set; }
        public string Comment { get; set; }
        public string ImageName { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}