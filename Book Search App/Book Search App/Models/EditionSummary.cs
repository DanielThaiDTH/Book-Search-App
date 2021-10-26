using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Book_Search_App
{
    public class EditionSummary : Edition
    {
        [PrimaryKey, AutoIncrement]
        public int UID { get; set; }
        
        //Keys: url, name
        [Ignore]
        public IList<IDictionary<string, string>> authors { get; set; }


        //Keys: name
        [Ignore]
        public IList<IDictionary<string,string>> publishers { get; set; }
        [TextBlob("publisherslistBlob")]
        public IList<string> publishers_list { get; set; }
        public string publisherslistBlob { get; set; }

        //Keys: name
        [Ignore]
        public IList<IDictionary<string,string>> publish_places { get; set; }
        [TextBlob("publishplaceslistBlob")]
        public IList<string> publish_places_list { get; set; }
        public string publishplaceslistBlob { get; set; }


        public override void FlattenAttributes()
        {
            base.FlattenAttributes();

            for (int i = 0; i < (publishers?.Count?? 0); i++) {
                publishers_list = new List<string>();
                publishers_list.Add(publishers[i]["name"]);
            }

            for (int i = 0; i < (publish_places?.Count ?? 0); i++) {
                publish_places_list = new List<string>();
                publish_places_list.Add(publish_places[i]["name"]);
            }

            for (int i = 0; i < (authors?.Count ?? 0); i++) {
                author_list = new List<string>();
                author_list.Add(authors[i]["name"]);
            }
        }
    }
}
