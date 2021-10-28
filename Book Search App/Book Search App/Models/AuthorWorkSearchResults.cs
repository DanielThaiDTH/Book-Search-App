using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Search_App
{
    public class AuthorWorkSearchResults
    {
        public int size { get; set; }
        public IList<AuthorWorkSearchInfo> entries { get; set; }
    }
}
