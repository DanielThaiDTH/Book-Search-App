using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Book_Search_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage(new NetworkingManager(), 
                                                        new BookInfoManager()));
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
