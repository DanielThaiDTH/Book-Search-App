using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;

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
    public class NetworkingManager :INotifyPropertyChanged
    {
        /*Search API building blocks*/
        private readonly string url = "https://openlibrary.org/search.json?";
        private readonly string limit = "&limit=";
        private readonly string languageOpt = "&language=";
        private static readonly int MAXLIMIT = 100;

        /*Author search API building blocks*/
        private readonly string authorURL = "https://openlibrary.org/search/authors.json?";

        public event PropertyChangedEventHandler PropertyChanged;

        private int _returnLimit;
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
        private readonly string key_type = ",OLID:";
        private readonly string query_data_format = "&format=json&jscmd=data";
        public const int QUERYLIMIT = 220;


        /*Specific Search API building blocks*/
        private readonly string query_base = "https://openlibrary.org";
        private readonly string query_end = ".json";

        private SearchType _option;
        public SearchType Option 
        {
            get { return _option; } 
            set
            {
                if (value == _option)
                    return;

                _option = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Option)));
            }
        }
        private readonly Dictionary<SearchType, string> queryOption;

        public bool _authorSearchOption = false;
        public bool AuthorSearchOption
        {
            get { return _authorSearchOption; }
            set 
            {
                if (value == _authorSearchOption)
                    return;

                _authorSearchOption = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(AuthorSearchOption)));
            }
        }

        HttpClient client = new HttpClient();

        public NetworkingManager() 
        {
            ReturnLimit = 100;
            Option = SearchType.REGULAR;
            queryOption = new Dictionary<SearchType, string>();
            queryOption.Add(SearchType.REGULAR, "q=");
            queryOption.Add(SearchType.TITLE, "title=");
            queryOption.Add(SearchType.AUTHOR, "author=");
            queryOption.Add(SearchType.ISBN, "isbn=");
        }

        public async Task<BookSearchResults> searchBooks(string query, string lang = "eng")
        {
            string queryURL = url + queryOption[Option];
            queryURL += query + limit + ReturnLimit.ToString() + languageOpt + lang;
            var response = await client.GetAsync(queryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                BookSearchResults results = JsonConvert.DeserializeObject<BookSearchResults>(jsonString);
                results.SearchTime = DateTime.Now;
                results.SearchString = query;
                results.SearchOption = Option;
                results.cleanResults(); //Remove books with no authors
                return results;
            } else {
                return null;
            }
        }


        public async Task<AuthorSearchResults> searchAuthors(string query)
        {
            string queryURL = authorURL + "q=" + query;

            var response = await client.GetAsync(queryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                AuthorSearchResults results = JsonConvert.DeserializeObject<AuthorSearchResults>(jsonString);
                results.SearchTime = DateTime.Now;
                results.SearchString = query;
                
                return results;
            } else {
                return null;
            }
        }


        public async Task<AuthorWorkSearchResults> searchWorksBy(string key)
        {
            string queryURL = query_base + key + "/works.json?" + limit + ReturnLimit.ToString();

            var response = await client.GetAsync(queryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<AuthorWorkSearchResults>(jsonString);

                return results;
            } else {
                return null;
            }
        }


        /// <summary> 
        /// Keys in returned dictionary are in the format of 'OLID:key'. Limited to 220 editions, 
        /// otherwise the request is bad.
        /// </summary>
        /// <param name="edition_keys"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, EditionSummary>> searchEditions(IList<string> edition_keys)
        {
            int count = 0;
            string editonQueryURL = edition_query;
            bool initial = true;


            foreach (string key in edition_keys) {
                if (count >= QUERYLIMIT)
                    break;
                if (initial) {
                    editonQueryURL += "OLID:" + key;
                    initial = false;
                } else {
                    editonQueryURL += key_type + key;
                }
                count++;
            }

            editonQueryURL += query_data_format;
            var response = await client.GetAsync(editonQueryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject
                                            <Dictionary<string, EditionSummary>>(jsonString);
                foreach (var ed in results) {
                    ed.Value.FlattenAttributes();
                }
                return results;
            } else {
                return null;
            }
        }

        /// <summary>
        ///Keys should have the relative path attached to it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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


        /// <summary> 
        ///Keys should have the relative path attached to it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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


        public async Task<Author> queryAuthor(string key)
        {
            string queryURL = query_base + "/authors/" + key + query_end;
            var response = await client.GetAsync(queryURL);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonString = await response.Content.ReadAsStringAsync();
                Author result = JsonConvert.DeserializeObject<Author>(jsonString);
                result.readUncleanValues(jsonString);
                return result;
            } else {
                return null;
            }
        }


        /// <summary>
        /// Obtains two lists, a list of edition keys and a list of author names. 
        /// Needed because author work search does not return compelte information.
        /// </summary>
        /// <param name="work_key">OpenLibrary work key</param>
        /// <returns>Two lists, the first being the edition keys, the second the author names</returns>
        public async Task<IList<IList<string>>> GetEditionsAndAuthors(string work_key)
        {
            var result = await searchBooks(work_key);
            IList <IList<string>> returnLists = new List<IList<string>>();
            if (result != null) {
                if (result.docs == null || result.docs.Count == 0)
                    return null;
                
                var info = result.docs[0];
                returnLists.Add(info.edition_key);
                returnLists.Add(info.author_name);

                return returnLists;
            }

            return null;
        }


        public bool IsQueryGood(string query)
        {
            return query != null && query.Length >= 3 &&
                !query.Equals("the", StringComparison.InvariantCultureIgnoreCase) &&
                !query.Equals("for", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
