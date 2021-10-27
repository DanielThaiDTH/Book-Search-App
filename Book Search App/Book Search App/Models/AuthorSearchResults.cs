using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Search_App
{
    public class AuthorSearchResults : SearchResults
    {
        public IList<Author> docs { get; set; }
    }
}
