using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XCamera.Util;

namespace XCamera
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectPage : ContentPage
	{
        private List<string> projects;

        public ProjectPage ()
		{
			InitializeComponent ();
            projects = Project.GetList();
            
            lstProjects.ItemsSource = projects;
            
		}
        async protected override void OnAppearing()
        {
            base.OnAppearing();
            if( !string.IsNullOrWhiteSpace(Project.szProjectName))
            {
                var curProject = new Project(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                if( curProject.HasDeleted())
                {
                    Boolean bClear = await DisplayAlert("Projekt " + Project.szProjectName, "Die gelöschten Bilder endgültig entfernen?", "Ja", "Nein");
                    if (bClear )
                    {
                        curProject.ClearDeleted();
                        await DisplayAlert("Projekt ", "Die gelöschten Bilder wurden endgültig entfernen.", "Weiter");
                    }
                }
                if( curProject.IsDirty())
                {
                    await DisplayAlert("Projekt ", "IsDirty", "Weiter");
                }
                if (!string.IsNullOrWhiteSpace(curProject.GetTempDir()))
                {
                    string[] imgFiles = System.IO.Directory.GetFiles(curProject.GetTempDir(), "*.jpg");
                    foreach (var imgFile in imgFiles)
                    {
                        try
                        {
                            System.IO.File.Delete(imgFile);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
        private void BtnNew_Clicked(object sender, EventArgs e)
        {
            grdOverlay.Children.Clear();

            int iRow = 0;
            Entry entryProject = AddInput(grdOverlay, iRow++, "Projekt", "Name", "");

            var submitButton = new Button { Text = "anlegen" };
            submitButton.Clicked += async (senderx, e2) =>
            {
                string szNewProject = entryProject.Text.Trim();
                if( string.IsNullOrWhiteSpace(szNewProject))
                {
                    await DisplayAlert("", "Den Projektname darf nicht leer sein.", "Weiter");
                }
                else if (!projects.Any(s => s.Equals(szNewProject, StringComparison.OrdinalIgnoreCase)))
                {
                    projects.Add(szNewProject);
                    lstProjects.ItemsSource = null;
                    lstProjects.ItemsSource = projects;
                    Project.szProjectName = szNewProject;
                    await Navigation.PushAsync(new MainPage());
                    // close the overlay
                    overlay.IsVisible = false;
                }
                else
                {
                    await DisplayAlert("", "Den Projektname gibt es schon.", "Weiter");
                }
            };
            grdOverlay.Children.Add(submitButton, 0, iRow);

            var cancelButton = new Button { Text = "abbrechen" };
            cancelButton.Clicked += (senderx, e2) =>
            {
                overlay.IsVisible = false;
            };
            grdOverlay.Children.Add(cancelButton, 1, iRow);


            overlay.IsVisible = true;
            
        }
        private void ShowWait(string szMessage)
        {
            grdOverlay.Children.Clear();

            int iRow = 0;
            AddLabel(grdOverlay, iRow++, szMessage);

            overlay.IsVisible = true;
        }
        
        private Entry AddInput(Grid sl, int iRow, string szLabel, string szPlaceholder, string szDefaultValue)
        {
            var label = new Label { Text = szLabel };
            var entry = new Entry { Placeholder = szPlaceholder };
            if (!string.IsNullOrWhiteSpace(szDefaultValue))
            {
                entry.Text = szDefaultValue;
            }

            sl.Children.Add(label, 0, iRow);
            sl.Children.Add(entry, 1, iRow);

            return entry;
        }
        private void AddLabel(Grid sl, int iRow, string szLabel)
        {
            var label = new Label { Text = szLabel };

            sl.Children.Add(label, 0, iRow);

        }
        void OnCancelButtonClicked(object sender, EventArgs args)
        {
            overlay.IsVisible = false;
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