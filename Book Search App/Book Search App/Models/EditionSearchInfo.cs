using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Search_App
{
    public class EditionSearchInfo
    {
        public string key { get; set; }
        public string title { get; set; }
        //Keys: url, name
        public IList<IDictionary<string, string>> authors { get; set; }
        public string subtitle { get; set; }
        public int number_of_pages { get; set; }
        //Keys: librarything, openlibrary, goodreads, amazon, isbn_10, isbn_13
        public IDictionary<string, IList<int>> identifiers { get; set; }
        public IList<IDictionary<string, string>> publishers { get; set; }
        public IList<IDictionary<string, string>> publish_places { get; set; }
        public string publish_date { get; set; }
        public IDictionary<string,string> cover { get; set; }
    }
}
