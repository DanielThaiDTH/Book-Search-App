using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Book_Search_App
{
    public class EditionInfo
    {
        public string key { get; set; }
        public string title { get; set; }

        [TextBlob("authorlistBlob")]
        public IList<string> author_list { get; set; }
        public string authorlistBlob { get; set; }

        public string subtitle { get; set; }
        public int number_of_pages { get; set; }

        //Keys: librarything, openlibrary, goodreads, amazon, isbn_10, isbn_13
        [Ignore]
        public IDictionary<string, IList<string>> identifiers { get; set; }
        public string librarything { get; set; }
        public string openlibrary { get; set; }
        public string goodreads { get; set; }
        public string amazon { get; set; }
        //public string isbn_10 { get; set; }
        //public string isbn_13 { get; set; }


        [TextBlob("publishersBlob")]
        public IList<string> publishers { get; set; }
        public string publishersBlob { get; set; }

        
        [TextBlob("publishersplacesBlob")]
        public IList<string> publish_places { get; set; }
        public string publishplacesBlob { get; set; }

        public string publish_date { get; set; }


        [TextBlob("coversBlob")]
        public IList<int> covers { get; set; }
        public string coversBlob { get; set; }


        //Call this after deserialization to flatten attributes
        public void FlattenAttributes()
        {
            IList<string> temp;
            
            if (identifiers.TryGetValue("librarything", out temp))
                librarything = temp[0];
            if (identifiers.TryGetValue("openlibrary", out temp))
                openlibrary = temp[0];
            if (identifiers.TryGetValue("goodreads", out temp))
                goodreads = temp[0];
            if (identifiers.TryGetValue("amazon", out temp))
                amazon = temp[0];
        }
    }
}
