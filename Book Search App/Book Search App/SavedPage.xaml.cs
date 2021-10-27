using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Book_Search_App
{
    public enum SavedTabOption
    {
        WORKS,
        AUTHOR
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SavedPage : ContentPage
    {
        BookInfoManager infoManager;
        SavedTabOption selected;
        public SavedTabOption SelectedTab
        {
            get { return selected; }
            set
            {
                if (value == SavedTabOption.AUTHOR) {
                    SavedWorksList.IsVisible = false;
                    SavedAuthorsList.IsVisible = true;
                } else {
                    SavedWorksList.IsVisible = true;
                    SavedAuthorsList.IsVisible = false;
                }

                selected = value;
            }
        }

        public SavedPage(BookInfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            SavedAuthorsList.ItemsSource = infoManager.Authors;
            SavedWorksList.ItemsSource = infoManager.Saved_Works;
            SelectedTab = SavedTabOption.WORKS;
        }

        private void BooksButton_Clicked(object sender, EventArgs e)
        {
            SelectedTab = SavedTabOption.WORKS;
            BooksButton.Style = Resources["SelectedStyle"] as Style;
            AuthorsButton.Style = Resources["NormalStyle"] as Style;
        }

        private void AuthorsButton_Clicked(object sender, EventArgs e)
        {
            SelectedTab = SavedTabOption.AUTHOR;
            BooksButton.Style = Resources["NormalStyle"] as Style;
            AuthorsButton.Style = Resources["SelectedStyle"] as Style;
        }

        private void SavedAuthorsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushAsync(new AuthorInfoPage((Author)e.SelectedItem, infoManager));
        }

        private void SavedWorksList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushAsync(new WorkInfoPage((WorkInfo)e.SelectedItem, infoManager));
        }
    }
}