using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Book_Search_App
{
    public partial class MainPage : ContentPage
    {
        string query;
        NetworkingManager openLibraryManager;
        BookInfoManager infoManager;
        public MainPage(NetworkingManager nm, BookInfoManager bim)
        {
            InitializeComponent();
            openLibraryManager = nm;
            infoManager = bim;
            SearchResultsList.ItemsSource = infoManager.Search_Results;
        }

        private void BookQueryEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            query = e.NewTextValue;
        }

        async private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            if (query.Length < 3 || 
                query.Equals("the", StringComparison.InvariantCultureIgnoreCase))
                return;

            SearchResults results = await openLibraryManager.searchBooks(query, "eng");
            if (results == null){
                await DisplayAlert("API Access Error", "Could not connect to OpenLibrary", "OK");
            } else if (results.numFound != 0) {
                infoManager.SetSearchResults(results);
                await DisplayAlert("Good", 
                    "Data found, " + results.numFound.ToString() + " books returned.",
                    "OK");
            } else {
                await DisplayAlert("Error", "Nothing found", "OK");
            }
        }


        private void SearchResultsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            WorkInfo wi = infoManager.GetCachedWork((e.Item as BookSearchInfo).key);
            if (wi != null) {
                Navigation.PushAsync(new WorkInfoPage(openLibraryManager, wi,
                                                        infoManager.CacheWorkInfo));
            } else {
                Navigation.PushAsync(new WorkInfoPage(openLibraryManager,
                                                      (e.Item as BookSearchInfo).key,
                                                      (e.Item as BookSearchInfo).author_name,
                                                      infoManager.CacheWorkInfo
                    ));
            }
        }
    }
}
