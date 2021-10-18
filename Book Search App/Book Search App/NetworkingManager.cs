using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Book_Search_App
{
    public enum SearchType
    {
        REGULAR,
        TITLE,
        AUTHOR,
        ISBN
    }
    /*At least one result will be returned.*/
    public class NetworkingManager
    {
        private string url = "https://openlibrary.org/search.json?";
        private string limit = "&limit=";
        private int _returnLimit;
        private static readonly int MAXLIMIT = 100;
        public int ReturnLimit {
            set {
                if (value <= 0)
                    return;
                else if (value > MAXLIMIT)
                    _returnLimit = MAXLIMIT;
                else
                    _returnLimit = value;
            }
            get { return _returnLimit; } 
        }

        public SearchType Option { get; set; }
        private readonly Dictionary<SearchType, string> queryOption;

        HttpClient client = new HttpClient();

        public NetworkingManager() 
        {
            ReturnLimit = 10;
            Option = SearchType.REGULAR;
            queryOption = new Dictionary<SearchType, string>();
            queryOption.Add(SearchType.REGULAR, "q=");
            queryOption.Add(SearchType.TITLE, "title=");
            queryOption.Add(SearchType.AUTHOR, "author=");
            queryOption.Add(SearchType.ISBN, "isbn=");
        }

        public async Task<SearchResults> searchBooks(string query)
        {
            string queryURL = url + queryOption[Option];
            queryURL += query + limit + ReturnLimit.ToString();
            var response = await client.GetAsync(queryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                SearchResults results = JsonConvert.DeserializeObject<SearchResults>(jsonString);
                results.SearchTime = DateTime.Now;
                return results;
            } else {
                return null;
            }
        }
    }
}
