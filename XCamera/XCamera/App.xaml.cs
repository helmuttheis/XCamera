using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XCamera
{
    public partial class App : Application
    {
        public App(IResizeImage resizeImage)
        {
            Global.resizeImage = resizeImage;
            InitializeComponent();
            MainPage = new NavigationPage( new ProjectPage());
        }
        

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
