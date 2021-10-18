using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Search_App
{
    public class BookSearchInfo
    {
        public string key { get; set; }
        public IList<string> text { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int edition_count { get; set; }
        public IList<string> edition_key { get; set; }
        public int first_publish_year { get; set; }
        public string cover_edition_key { get; set; }
        public IList<string> language { get; set; } //Three letter code for language
        public IList<string> author_key { get; set; }
        public IList<string> author_name { get; set; }
        public IList<string> id_amazon { get; set; }
        public IList<string> id_canadian_national_library_archive { get; set; }
        public IList<string> id_goodreads { get; set; }
        public IList<string> id_wikidata { get; set; }
        public IList<string> person { get; set; }
        public IList<string> place { get; set; }
        public IList<string> subject { get; set; }
        public IList<string> time { get; set; }

        public override int GetHashCode()
        {
            return key.GetHashCode() + title.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (object.ReferenceEquals(this, obj)) return true;
            BookSearchInfo other = (BookSearchInfo)obj;

            if (this.GetType() != other.GetType()) {
                return false;
            }

            return this.key.Equals(other.key) && this.title.Equals(other.title);
        }
    }
}
