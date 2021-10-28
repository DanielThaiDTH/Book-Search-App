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
    public partial class AuthorInfoPage : ContentPage
    {
        BookInfoManager infoManager;
        Author author;
        public Author AuthorInfo => author;

        bool _isSaved;
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                DeleteButton.IsEnabled = value;
                SaveButton.IsEnabled = !value;

                if (value == _isSaved)
                    return;

                _isSaved = value;
            }
        }

        public AuthorInfoPage(string key, BookInfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            GetInfo(key);
        }

        public AuthorInfoPage(Author a, BookInfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            author = a;
            SetDisplay();

            if (infoManager.Authors.Contains(author))
                IsSaved = true;
            else
                IsSaved = false;
        }
        
        private async void GetInfo(string key)
        {
            author = await App.NetManager.queryAuthor(key);
            SetDisplay();

            if (infoManager.Authors.Contains(author))
                IsSaved = true;
            else
                IsSaved = false;
        }

        private void SetDisplay()
        {
            Name_Text.Text = AuthorInfo.name;
            DOB_Text.Text = AuthorInfo.birth_date ?? "";
            DOD_Text.Text = AuthorInfo.death_date ?? "";

            if (AuthorInfo.death_date == null)
                DODHeader.IsVisible = false;
            else
                DODHeader.IsVisible = true;

            Bio_Text.Text = AuthorInfo.biography;
            if (AuthorInfo.GetImgURL().Length > 0)
                AuthorImage.HeightRequest = 250;

            AuthorImage.Source = AuthorInfo.GetImgURL();
        }


        public void SaveClicked(object sender, EventArgs e)
        {
            if (!infoManager.AddAuthor(author)) { 
                IsSaved = true;
                DisplayAlert(author.name + " Saved", "This author has been successfully saved.", "OK");
            } else {
                IsSaved = true;
            }
        }

        public void DeleteClicked(object sender, EventArgs e)
        {
            if (infoManager.RemoveAuthor(author)) {
                DisplayAlert(author.name + " Deleted", "This author has been removed from yout list of saved authors.", "OK");
                IsSaved = false;
            }
        }
    }
}