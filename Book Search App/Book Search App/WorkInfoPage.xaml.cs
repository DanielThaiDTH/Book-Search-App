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
    public partial class WorkInfoPage : ContentPage
    {
        NetworkingManager openLibraryManager;
        WorkInfo info;
        private IList<string> author_list = new List<string>();
        WorkInfoDel cacheWork;
        public WorkInfo Info
        {
            get
            {
                return info;
            }
        }
        public WorkInfoPage(NetworkingManager nm, string key, IList<string> authors, WorkInfoDel infoCacheFunc)
        {
            InitializeComponent();
            cacheWork = infoCacheFunc;
            openLibraryManager = nm;
            author_list = authors;
            getInfo(key);
        }

        public WorkInfoPage(NetworkingManager nm, WorkInfo old_info, WorkInfoDel infoCacheFunc)
        {
            InitializeComponent();
            cacheWork = infoCacheFunc;
            openLibraryManager = nm;
            info = old_info;
            cacheWork(info);
            setDisplay();
        }

        private async void getInfo(string key)
        {
            info = await openLibraryManager.queryWork(key);

            foreach(string author in author_list) {
                info.addAuthor(author);
            }

            setDisplay();
            cacheWork(info);
        }


        private void setDisplay()
        {
            WorkInfoDisplay.BindingContext = Info;
            Title_Text.Text = Info.title;
            try {
                Desc.Text = Info.Desc;
            }
            catch (NullReferenceException e) {
                Desc.Text = "";
            }
            Author_Text.Text = Info.getAuthorStr();
            BookCover.Source = Info.getImgURL();

            if (Info.getImgURL().Length > 0)
                BookCover.HeightRequest = 300;
        }

        private void saveClicked(object sender, EventArgs e)
        {
            //cacheWork(info);
        }
    }//class
}