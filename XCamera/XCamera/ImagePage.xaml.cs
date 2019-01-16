using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            mainPage.szFullImageName = "";
            InitializeComponent ();


            images = new ObservableCollection<ImageViewModel>();
            foreach (string szImageName in mainPage.curProject.GetImages())
            {
                string szFullImageName = mainPage.curProject.GetImageFullName(szImageName);
                var memoryStream = new MemoryStream();

                using (var fileStream = new FileStream(szFullImageName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;

                images.Add(new ImageViewModel {
                    Comment = mainPage.curProject.GetComment(szFullImageName),
                    ImageSource = ImageSource.FromStream(() => memoryStream),
                    ImageName = szFullImageName
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
        private void btnSelect_Clicked(object sender, EventArgs e)
        {
            if (lstView.SelectedItem != null)
            {
                mainPage.szFullImageName = ((ImageViewModel)lstView.SelectedItem).ImageName;
                Navigation.PopModalAsync();
            }
        }

        private void LstView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (lstView.SelectedItem != null)
            {
                mainPage.szFullImageName = ((ImageViewModel)lstView.SelectedItem).ImageName;
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
                
                mainPage.curProject.Delete(selectedLocation.ImageName);
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