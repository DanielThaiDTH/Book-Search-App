using System;
using System.Threading;
using System.Threading.Tasks;
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

        static DatabaseManager database;
        public static DatabaseManager Database
        {
            get
            {
                if (database == null) {
                    database = new DatabaseManager();
                }
                return database;
            }
        }

        InfoManager infoManager;

        public App()
        {
            InitializeComponent();
            infoManager = new InfoManager();
            InitializeDB(infoManager);

            //Seems to fix the issue with ListView appearing, could be 
            //caused by sync issues (App modifies infoManager, while ListView in main
            //accesses it to use as an item source.)
            try {
                Thread.Sleep(500);
            }
            catch (ThreadInterruptedException e) {
                Console.WriteLine(e.Message);
            }

            MainPage = new NavigationPage(new MainPage(infoManager));
            ToolbarItem savedPageNav = new ToolbarItem { Text = "Saved Books/Authors" };
            MainPage.ToolbarItems.Add(savedPageNav);
            savedPageNav.Clicked += SavedPageNav_Clicked;
        }

        private void SavedPageNav_Clicked(object sender, EventArgs e)
        {
            MainPage.Navigation.PushAsync(new SavedPage(infoManager));
        }

        private async void InitializeDB(InfoManager im)
        {
            im.Authors = await Database.InitAuthorsTable();
            im.Saved_Works = await Database.InitWorksTable();

            
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
