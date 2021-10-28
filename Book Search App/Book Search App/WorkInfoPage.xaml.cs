using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FFImageLoading;
using System.ComponentModel;
using System.Windows.Input;

namespace Book_Search_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkInfoPage : ContentPage, INotifyPropertyChanged
    {
        readonly BookInfoManager infoManager;
        private IList<string> author_list = new List<string>();

        WorkInfo info;
        public WorkInfo Info
        {
            get { return info; }
        }

        new public event PropertyChangedEventHandler PropertyChanged;
        bool _isSaved = false;
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

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSaved)));
            }
        }

        
        public WorkInfoPage(string key, 
                            IList<string> authors,
                            IList<string> edition_keys,
                            BookInfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            author_list = authors;

            getInfo(key, edition_keys);
        }

        public WorkInfoPage(WorkInfo old_info, BookInfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            info = old_info;
            infoManager.CacheWorkInfo(info);
            if (infoManager.Saved_Works.Contains(info))
                IsSaved = true;
            else
                IsSaved = false;

            setDisplay();
        }

        private async void getInfo(string key, IList<string> edition_keys)
        {
            info = await App.NetManager.queryWork(key);
            if (info.IsRedirect())
                info = await App.NetManager.queryWork(info.GetRedirect());

            foreach(string author in author_list) {
                info.addAuthor(author);
            }

            foreach(string ed_key in edition_keys) {
                info.Editions_Keys.Add(ed_key);
            }

            setDisplay();

            if (infoManager.Saved_Works.Contains(info))
                IsSaved = true;
            else
                IsSaved = false;
            
            infoManager.CacheWorkInfo(info);
        }


        private void setDisplay()
        {
            WorkInfoDisplay.BindingContext = Info;
            Title_Text.Text = Info.title??"";
            Desc.Text = Info.Desc ?? "";
            Author_Text.Text = Info.getAuthorStr();
            Subject_Text.Text = Info.getSubjectStr();
            Subject_Places_Text.Text = Info.getSubjectPlaces();
            PublishDate.Text = Info.first_publish_date ?? "";


            if (Info.getImgURL().Length > 0)
                BookCover.HeightRequest = 275;

            BookCover.Source = Info.getImgURL();
        }

        public void saveClicked(object sender, EventArgs e)
        {
            if (infoManager.AddWork(info)) { 
                DisplayAlert(info.title + " Saved", "This book has been successfully saved.", "OK");
                IsSaved = true;
            } else {
                IsSaved = true;
            }
        }

        public void deleteClicked(object sender, EventArgs e)
        {
            if(infoManager.RemoveWork(info)) { 
                DisplayAlert(info.title + " Deleted", "This book has been removed from your saved books list.", "OK");
                IsSaved = false;
            }
        }

        private void ViewEditionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditionListPage(info.Editions_Keys, infoManager));
        }
    }//class
}