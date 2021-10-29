using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Book_Search_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorWorksPage : ContentPage
    {
        InfoManager infoManager;
        public AuthorWorksPage(string key, InfoManager im, string name = "")
        {
            InitializeComponent();
            infoManager = im;
            AuthorWorksList.ItemsSource = infoManager.By_Author_Search_Results;
            PageHeader.Text += name;
            GetInfo(key);
        }

        public AuthorWorksPage(InfoManager im, string name="")
        {
            InitializeComponent();
            infoManager = im;
            AuthorWorksList.ItemsSource = infoManager.By_Author_Search_Results;
            PageHeader.Text += name;
        }

        public async void GetInfo(string key)
        {
            AuthorWorksList.IsRefreshing = true;
            var results = await App.NetManager.searchWorksBy(key);
            AuthorWorksList.IsRefreshing = false;
            infoManager.PreviousAuthorWorkSearchKey = key;

            if (results != null) {
                infoManager.SetAuthorWorkSearchResults(results);
            }
        }

        private async void AuthorWorksList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            string key = (e.SelectedItem as AuthorWorkSearchInfo).key;
            WorkInfo wi = infoManager.GetCachedWork(key);
            WorkInfo savedWI = infoManager.GetSavedWork(key);

            if (wi != null) {
                await Navigation.PushAsync(new WorkInfoPage(wi, infoManager));
            } else if (savedWI != null) {
                await Navigation.PushAsync(new WorkInfoPage(savedWI, infoManager));
            } else {
                IList<IList<string>> infoLists = await App.NetManager.GetEditionsAndAuthors(key.Substring(7));
                if (infoLists != null)
                    await Navigation.PushAsync(new WorkInfoPage(key, infoLists[1], infoLists[0], infoManager));
                else
                    await Navigation.PushAsync(new WorkInfoPage(key, new List<string>(), new List<string>(), infoManager));
            }
        }
    }
}