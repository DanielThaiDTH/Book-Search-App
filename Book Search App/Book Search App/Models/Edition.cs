using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Book_Search_App
{
    public class Edition
    {
        [Unique]
        public string key { get; set; }
        public string title { get; set; }
        public int number_of_pages { get; set; }
        public string by_statement { get; set; }
        public string publish_date { get; set; }


        [TextBlob("authorlistBlob")]
        public IList<string> author_list { get; set; }
        public string authorlistBlob { get; set; }


        //Keys: librarything, openlibrary, goodreads, amazon, isbn_10, isbn_13
        [Ignore]
        public IDictionary<string, IList<string>> identifiers { get; set; }
        public string librarything { get; set; }
        public string openlibrary { get; set; }
        public string goodreads { get; set; }
        public string amazon { get; set; }
        public string isbn10 { get; set; }
        public string isbn13 { get; set; }

        public DateTime addTime { get; set; }


        //Call this after deserialization to flatten attributes
        public virtual void FlattenAttributes()
        {
            IList<string> temp;

            if (identifiers == null)
                return;

            if (identifiers.TryGetValue("librarything", out temp))
                librarything = temp[0];
            if (identifiers.TryGetValue("openlibrary", out temp))
                openlibrary = temp[0];
            if (identifiers.TryGetValue("goodreads", out temp))
                goodreads = temp[0];
            if (identifiers.TryGetValue("amazon", out temp))
                amazon = temp[0];
            if (identifiers.TryGetValue("isbn_10", out temp))
                isbn10 = temp[0];
            if (identifiers.TryGetValue("isbn_13", out temp))
                isbn13 = temp[0];
        }


        public string GetGoodreadsURL()
        {
            if (goodreads == null)
                return "";

            return "https://www.goodreads.com/book/show/" + goodreads;
        }
        public string GetLibrarythingURL()
        {
            if (librarything == null)
                return "";

            return "https://www.librarything.com/work/" + librarything;
        }

        public string GetAmazonURL()
        {
            if (amazon == null)
                return "";

            return "https://www.amazon.com/dp/" + amazon;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            Edition other = (Edition)obj;

            if (GetType() != other.GetType()) {
                return false;
            }

            return key.Equals(other.key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

    }
}
