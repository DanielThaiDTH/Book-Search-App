using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Search_App
{
    public class AuthorWorkSearchInfo
    {
        public string static_cover_url { get; set; }
        public string key { get; set; }
        public IList<int> covers { get; set; }
        public string title { get; set; }

        public AuthorWorkSearchInfo()
        {
            static_cover_url = "";
        }

        public string GetImgURL(int idx = 0)
        {
            if (covers == null || covers.Count > idx)
                return "";

            return "https://covers.openlibrary.org/b/id/" + covers[idx] + "-M.jpg";
        }
    }
}
