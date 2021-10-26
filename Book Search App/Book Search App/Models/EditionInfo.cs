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
        public IList<string> genres { get; set; }

        public override void FlattenAttributes()
        {
            base.FlattenAttributes();
            isbn10 = isbn_10?[0];
            isbn13 = isbn_13?[0];
        }
    }
}
