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

            List<Bild> bilder = mainPage.curProjectSql.GetBilder();

            foreach (var bild in bilder)
            {
                string szImageName = bild.Name;
                string szFullImageName = mainPage.curProjectSql.GetImageFullName(szImageName);
                var memoryStream = new MemoryStream();

                using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;

                images.Add(new ImageViewModel {
                    Comment = mainPage.curProjectSql.GetKommentar(szImageName),
                    ImageSource = ImageSource.FromStream(() => memoryStream),
                    ImageName = szImageName
                });
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
                
                mainPage.curProjectSql.Delete(selectedLocation.ImageName);
            }
        }
    }

    public class ImageViewModel
    {
        public string Comment { get; set; }
        public string ImageName { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}