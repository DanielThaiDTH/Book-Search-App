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
        bool isAuthorSearch = false;
        BookInfoManager infoManager;
        public MainPage(BookInfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            SearchResultsList.ItemsSource = infoManager.Search_Results;
            AuthorSearchList.ItemsSource = infoManager.Authors_Found;
        }

        private void BookQueryEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            query = e.NewTextValue;
        }

        async private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            //Ignore queries that are too common
            if (!App.NetManager.IsQueryGood(query))
                return;

            if (infoManager.HasBooksFound()) {
                SearchResultsList.ScrollTo(infoManager.Search_Results[0], 
                                            ScrollToPosition.Start, false);
            }

            if (infoManager.HasAuthorsFound()) {
                AuthorSearchList.ScrollTo(infoManager.Authors_Found[0],
                                            ScrollToPosition.Start, false);
            }

            if (!isAuthorSearch) {
                SearchResultsList.IsVisible = true;
                AuthorSearchList.IsVisible = false;
                
                SearchResultsList.IsRefreshing = true;
                if (!infoManager.IsPreviousQuery(query, App.NetManager.Option)) {

                    BookSearchResults results = await App.NetManager.searchBooks(query, "eng");
                    infoManager.RecordPreviousQuery(query, App.NetManager.Option);

                    if (results == null) {
                        await DisplayAlert("API Access Error", "Could not connect to OpenLibrary", "OK");
                    } else if (results.numFound != 0) {
                        infoManager.SetSearchResults(results);
                    } else {
                        await DisplayAlert("No Results", "No books found with that query.", "OK");
                    }
                }

                SearchResultsList.IsRefreshing = false;

            } else {
                SearchResultsList.IsVisible = false;
                AuthorSearchList.IsVisible = true;
                AuthorSearchList.IsRefreshing = true;
                AuthorSearchResults results = await App.NetManager.searchAuthors(query);
                AuthorSearchList.IsRefreshing = false;
                if (results == null) {
                    await DisplayAlert("API Access Error", "Could not connect to OpenLibrary", "OK");
                } else if (results.numFound != 0) {
                    infoManager.SetAuthorSearchResults(results);
                } else {
                    await DisplayAlert("No Results", "No authors found with that query.", "OK");
                }
            }
        }


        private void SearchResultsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            string key = (e.Item as BookSearchInfo).key;
            WorkInfo wi = infoManager.GetCachedWork(key);
            WorkInfo savedWI = infoManager.GetSavedWork(key);
            if (wi != null) {
                Navigation.PushAsync(new WorkInfoPage(wi, infoManager));
            } else if (savedWI != null) {
                Navigation.PushAsync(new WorkInfoPage(savedWI, infoManager));
            } else {
                Navigation.PushAsync(new WorkInfoPage((e.Item as BookSearchInfo).key,
                                                      (e.Item as BookSearchInfo).author_name,
                                                      (e.Item as BookSearchInfo).edition_key,
                                                      infoManager
                    ));
            }
        }

        private void AuthorSearchList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new AuthorInfoPage((e.Item as Author).key, infoManager));
        }


        private void SettingsButton_Clicked(object sender, EventArgs e)
        {
            SettingsFrame.IsVisible = !SettingsFrame.IsVisible;
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton checkedButton = (RadioButton)sender;

            if (!checkedButton.IsChecked)
                return;

            string value = checkedButton.Content.ToString();

            if (value == "Regular") {
                App.NetManager.Option = SearchType.REGULAR;
            } else if (value == "Title") {
                App.NetManager.Option = SearchType.TITLE;
            } else if (value == "Author") {
                App.NetManager.Option = SearchType.AUTHOR;
            } else if (value == "ISBN") {
                App.NetManager.Option = SearchType.ISBN;
            }
        }

        private void AuthorOptionCheck_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            isAuthorSearch = check.IsChecked;
        }

    }//Class
}
