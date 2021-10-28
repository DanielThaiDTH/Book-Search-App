using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Book_Search_App
{
    public class EditionInfo : Edition
    {
        [PrimaryKey, AutoIncrement]
        public int UID { get; set; }
        public string subtitle { get; set; }


        [TextBlob("publishersBlob")]
        public IList<string> publishers { get; set; }
        public string publishersBlob { get; set; }

        
        [TextBlob("publishersplacesBlob")]
        public IList<string> publish_places { get; set; }
        public string publishplacesBlob { get; set; }


        [TextBlob("coversBlob")]
        public IList<int> covers { get; set; }
        public string coversBlob { get; set; }

        [TextBlob("subjectplaceBlob")]
        public IList<string> subject_place { get; set; }
        public string subjectplaceBlob { get; set; }

        public IList<string> isbn_10 { get; set; }
        public IList<string> isbn_13 { get; set; }
        [TextBlob("genresBlob")]
        public IList<string> genres { get; set; }
        public string genresBlob { get; set; }

        //Key: key
        [Ignore]
        public IList<IDictionary<string,string>> languages { get; set; }
        public string Language { get; set; }

        public override void FlattenAttributes()
        {
            base.FlattenAttributes();
            isbn10 = isbn_10?[0];
            isbn13 = isbn_13?[0];

            string temp;
            if (languages != null && languages.Count > 0 && languages[0].TryGetValue("key", out temp))
                Language = temp.Substring(11); //Removes relative path '/languages/'
        }

        public string getImgURL(int idx = 0)
        {
            if (covers != null && covers.Count >= 1)
                return "https://covers.openlibrary.org/b/id/" + covers[idx] + "-L.jpg";
            else
                return "";
        }

        private string generateCommaSeperated(IList<string> string_list)
        {
            if (string_list == null || string_list.Count == 0)
                return "";

            string commaStr = "";

            foreach (string str in string_list) {
                commaStr += str + ", ";
            }

            if (commaStr.Length > 2)
                return commaStr.Substring(0, commaStr.Length - 2);
            else
                return "";
        }

        /// <summary>
        /// Returns comma separated values of publishers
        /// </summary>
        public string GetPublishersCS()
        {
            return generateCommaSeperated(publishers);
        }

        /// <summary>
        /// Gets comma seperated string of publish locations
        /// </summary>
        /// <returns></returns>
        public string GetPublishLocationsCS()
        {
            return generateCommaSeperated(publish_places);
        }

        /// <summary>
        /// </summary>
        /// <returns>Comma separated list of authors</returns>
        public string GetAuthorsCS()
        {
            return generateCommaSeperated(author_list);
        }

        /// <summary
        /// </summary>
        /// <returns>Comman separated list of genres</returns>
        public string GetGenresCS()
        {
            return generateCommaSeperated(genres);
        }

        public string GetISBNs()
        {
            string text = "";

            if (isbn10 != null)
                text += isbn10;
            if (isbn13 != null)
                text += ", " + isbn13;

            return text;
        }
    }
}
