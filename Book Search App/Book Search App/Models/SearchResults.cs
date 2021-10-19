using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Search_App
{
    public class SearchResults
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public bool numFoundExact { get; set; }
        public int num_found { get; set; }
        public IList<BookSearchInfo> docs { get; set; }

        public DateTime SearchTime { get; set; }

        public string SearchString { get; set; }

        SearchResults()
        {
            SearchTime = System.DateTime.Now;
            numFound = 0;
        }

        public void cleanResults()
        {
            for (int i = 0; i < docs.Count;) {
                if (docs[i].author_key is null) {
                    docs.RemoveAt(i);
                } else {
                    i++;
                }
            }
        }

        public override int GetHashCode()
        {
            return SearchTime.GetHashCode() + SearchString.GetHashCode() + numFound;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (object.ReferenceEquals(this, obj)) return true;
            SearchResults other = (SearchResults)obj;

            if (this.GetType() != other.GetType()) {
                return false;
            }

            return this.SearchTime.Equals(other.SearchTime) 
                && this.numFound == other.numFound
                && this.SearchString.Equals(other.SearchString);
        }
    }
}
