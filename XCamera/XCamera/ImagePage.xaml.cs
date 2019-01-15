using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                images.Add(new ImageViewModel {
                    Comment = mainPage.curProject.GetComment(szFullImageName),
                    ImageSource = ImageSource.FromFile(szFullImageName),
                    ImageName = szFullImageName
                });
            }
            lstView.ItemsSource = images;
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
    }

    public class ImageViewModel
    {
        public string Comment { get; set; }
        public string ImageName { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}