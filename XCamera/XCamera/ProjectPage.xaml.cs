using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XCamera
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectPage : ContentPage
	{
		public ProjectPage ()
		{
			InitializeComponent ();
            var projects = Project.GetList();
            
            lstProjects.ItemsSource = projects;
            
		}

        private void BtnContinue_Clicked(object sender, EventArgs e)
        {
            if (lstProjects.SelectedItem != null)
            {
                Project.szProjectName = lstProjects.SelectedItem.ToString();
                Navigation.PushAsync(new MainPage());
            }
        }

        private void LstProjects_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (lstProjects.SelectedItem != null)
            {
                Project.szProjectName = lstProjects.SelectedItem.ToString();
                Navigation.PushAsync(new MainPage());
            }
        }
    }
}