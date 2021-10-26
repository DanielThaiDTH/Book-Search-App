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
        /*Search API building blocks*/
        private string url = "https://openlibrary.org/search.json?";
        private string limit = "&limit=";
        private string languageOpt = "&language=";
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

        /*Edition Search API building blocks*/
        private string edition_query = "https://openlibrary.org/api/books?bibkeys=";
        private string key_type = ",OLID:";
        private string query_data_format = "&format=json&jscmd=data";

        /*Specific Search API building blocks*/
        private string query_base = "https://openlibrary.org";
        private string query_end = ".json";

        public SearchType Option { get; set; }
        private readonly Dictionary<SearchType, string> queryOption;

        HttpClient client = new HttpClient();

        public NetworkingManager() 
        {
            ReturnLimit = 25;
            Option = SearchType.REGULAR;
            queryOption = new Dictionary<SearchType, string>();
            queryOption.Add(SearchType.REGULAR, "q=");
            queryOption.Add(SearchType.TITLE, "title=");
            queryOption.Add(SearchType.AUTHOR, "author=");
            queryOption.Add(SearchType.ISBN, "isbn=");
        }

        public async Task<SearchResults> searchBooks(string query, string lang = "eng")
        {
            string queryURL = url + queryOption[Option];
            queryURL += query + limit + ReturnLimit.ToString() + languageOpt + lang;
            var response = await client.GetAsync(queryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                SearchResults results = JsonConvert.DeserializeObject<SearchResults>(jsonString);
                results.SearchTime = DateTime.Now;
                results.SearchString = query;
                results.cleanResults(); //Remove books with no authors
                return results;
            } else {
                return null;
            }
        }

        //Keys in returned dictionary are in the format of 'OLID:key'.
        public async Task<IDictionary<string, EditionSummary>> searchEditions(IList<string> edition_keys)
        {
            string editonQueryURL = edition_query;
            bool initial = true;

            foreach (string key in edition_keys) {
                if (initial) {
                    editonQueryURL += "OLID:" + key;
                    initial = false;
                } else {
                    editonQueryURL += key_type + key;
                }
            }

            editonQueryURL += query_data_format;
            var response = await client.GetAsync(editonQueryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                IDictionary<string, EditionSummary> results = JsonConvert
                                                                .DeserializeObject<Dictionary
                                                                                  <string, EditionSummary>>
                                                                                  (jsonString);
                foreach (KeyValuePair<string,EditionSummary> ed in results) {
                    ed.Value.FlattenAttributes();
                }
                return results;
            } else {
                return null;
            }
        }


        //Keys should have the relative path attached to it
        public async Task<EditionInfo> queryEdition(string key)
        {
            string editionQueryURL = query_base + key + query_end;
            var response = await client.GetAsync(editionQueryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                EditionInfo result = JsonConvert.DeserializeObject<EditionInfo>(jsonString);
                result.FlattenAttributes();
                return result;
            } else {
                return null;
            }
        }


        //Keys should have the relative path attached to it
        public async Task<WorkInfo> queryWork(string key)
        {
            string workQueryURL = query_base + key + query_end;
            var response = await client.GetAsync(workQueryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                WorkInfo results = JsonConvert.DeserializeObject<WorkInfo>(jsonString);
                results.readUncleanValues(jsonString);
                return results;
            } else {
                return null;
            }
        }
    }
}
