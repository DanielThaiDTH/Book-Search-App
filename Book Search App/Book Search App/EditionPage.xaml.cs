using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Book_Search_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditionPage : ContentPage
    {
        readonly InfoManager infoManager;
        EditionInfo edition;
        public EditionInfo EditionInformation
        {
            get
            {
                return edition;
            }
        }

        public EditionPage(EditionInfo ed, InfoManager bim)
        {
            InitializeComponent();
            edition = ed;
            infoManager = bim;
        }

        public EditionPage(string key, InfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            getInfo(key);
        }

        private async void getInfo(string key)
        {
            edition = await App.NetManager.queryEdition(key);
            TitleText.Text = edition.title + ": " + edition.subtitle??"";
            PublisherText.Text = edition.GetPublishersCS();
            LocationText.Text = edition.GetPublishLocationsCS();

            if (edition.number_of_pages > 0)
                NoOfPagesText.Text = edition.number_of_pages.ToString();

            AuthorsText.Text = edition.GetAuthorsCS();
            GenresText.Text = edition.GetGenresCS();


            ISBNText.Text = edition.GetISBNs();
            LanguageText.Text = edition.Language;
            GoodreadsLink.Text = edition.GetGoodreadsURL();
            AmazonLink.Text = edition.GetAmazonURL();
            LibrarythingLink.Text = edition.GetLibrarythingURL();

            if (edition.getImgURL().Length > 0)
                BookCover.HeightRequest = 275;

            BookCover.Source = edition.getImgURL();
        }

        private void Link_Clicked(object sender, EventArgs e)
        {
            string url = (sender as Button).Text;
            OpenBrowser(url);
        }

        private async void OpenBrowser(string url)
        {
            try {
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            } catch (Exception e) {
                await DisplayAlert("Unable to open browser", e.Message, "Ok");
            }
        }
    }
}