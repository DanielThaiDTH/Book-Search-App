using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Book_Search_App
{
    public partial class App : Application
    {
        static NetworkingManager netManager;

        public static NetworkingManager NetManager
        {
            get
            {
                if (netManager == null) {
                    netManager = new NetworkingManager();
                }
                return netManager;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage(new BookInfoManager()));
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
