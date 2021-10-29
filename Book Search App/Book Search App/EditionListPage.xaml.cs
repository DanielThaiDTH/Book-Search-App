using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Book_Search_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditionListPage : ContentPage
    {
        readonly InfoManager infoManager;
        IDictionary<string, EditionSummary> editions;
        ObservableCollection<EditionSummary> edition_list;
        public ObservableCollection<EditionSummary> EditionList => edition_list;

        public EditionListPage(IList<string> keys, InfoManager bim)
        {
            InitializeComponent();
            infoManager = bim;
            getEditions(keys);
        }

        private async void getEditions(IList<string> keys)
        {
            EditionListView.IsRefreshing = true;
            editions = await App.NetManager.searchEditions(keys);
            EditionListView.IsRefreshing = false;
            edition_list = new ObservableCollection<EditionSummary>(editions.Values);
            EditionListView.ItemsSource = EditionList;
            Header.Text = "Editons for this Work";
        }

        private void EditionListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new EditionPage( (e.Item as EditionSummary).key, 
                                                    infoManager));
        }
    }
}