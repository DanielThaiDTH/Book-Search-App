using System;
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

        BookInfoManager infoManager;

        public App()
        {
            InitializeComponent();
            infoManager = new BookInfoManager();
            InitializeDB(infoManager);

            MainPage = new NavigationPage(new MainPage(infoManager));
            ToolbarItem savedPageNav = new ToolbarItem { Text = "Saved Books/Authors" };
            MainPage.ToolbarItems.Add(savedPageNav);
            savedPageNav.Clicked += SavedPageNav_Clicked;

        }

        private void SavedPageNav_Clicked(object sender, EventArgs e)
        {
            MainPage.Navigation.PushAsync(new SavedPage(infoManager));
        }

        private async Task<int> InitializeDB(BookInfoManager im)
        {
            im.Authors = await Database.InitAuthorsTable();
            im.Saved_Works = await Database.InitWorksTable();
            return 0;
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
