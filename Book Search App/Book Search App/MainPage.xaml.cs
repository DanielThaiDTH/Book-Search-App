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
        NetworkingManager openLibraryManager = new NetworkingManager();
        public MainPage()
        {
            InitializeComponent();
        }

        private void BookQueryEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            query = e.NewTextValue;
        }

        async private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            if (query.Length < 4)
                return;

            SearchResults results = await openLibraryManager.searchBooks(query);
            if (results == null){
                await DisplayAlert("API Access Error", "Could not connect to OpenLibrary", "OK");
            } else if (results.numFound != 0) {
                await DisplayAlert("Good", 
                    "Data found, " + results.numFound.ToString() + " books returned.",
                    "OK");
            } else {
                await DisplayAlert("Error", "Nothing found", "OK");
            }
        }
    }
}
