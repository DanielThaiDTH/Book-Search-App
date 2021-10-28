using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Book_Search_App
{
    public delegate void WorkInfoDel(WorkInfo info);
    [Table("Work_Info")]
    public class WorkInfo
    {
        [PrimaryKey, AutoIncrement]
        public int UID { get; set; }

        [Unique]
        public string key { get; set; }
        public string Desc { get; set; }
       
        //key: key
        [Ignore]
        public IDictionary<string, string> type { get; set; }
        //Redirection location
        public string location { get; set; }

        [TextBlob("authorlistBlob")]
        public List<string> Author_List { get; set; }
        public string authorlistBlob { get; set; }


        public string title { get; set; }

        //Stores the cover ids
        [TextBlob("coversBlob")]
        public IList<int> covers { get; set; }
        public string coversBlob { get; set; }


        [TextBlob("subjectplacesBlob")]
        public IList<string> subject_places { get; set; }
        public string subjectplacesBlob { get; set; }

        public string first_publish_date { get; set; }

        [TextBlob("subjectsBlob")]
        public IList<string> subjects { get; set; }
        public string subjectsBlob { get; set; }

        [TextBlob("editionkeysBlob")]
        public IList<string> Editions_Keys { get; set; }
        public string editionkeysBlob { get; set; }

        public DateTime AddTime { get; set; }


        public WorkInfo()
        {
            Author_List = new List<string>();
            Editions_Keys = new List<string>();
            UID = -1;
            key = "";
        }


        public void addAuthor(string author)
        {
            Author_List.Add(author);
        }


        public string getAuthorStr()
        {
            string authors = "";

            foreach (string author in Author_List) {
                authors += author + ", ";
            }

            if (authors.Length > 2)
                return authors.Substring(0, authors.Length - 2);
            else
                return "";
        }


        public string getSubjectStr()
        {
            string subject_string = "";
            int subjects_count = subjects?.Count ?? 0;

            for (int i = 0; i < subjects_count; i++) {
                if (i > 0)
                    subject_string += ", ";

                subject_string += subjects[i];
            }

            return subject_string;
        }


        public string getSubjectPlaces()
        {
            string place_string = "";
            int count = subject_places?.Count ?? 0;

            for (int i = 0; i < count; i++) {
                if (i > 0)
                    place_string += ", ";

                place_string += subject_places[i];
            }

            return place_string;
        }


        public bool IsRedirect()
        {
            if (type == null || !type["key"].Equals("/type/redirect"))
                return false;
            else if (type["key"].Equals("/type/redirect"))
                return true;
            else
                return false;
        }


        public string GetRedirect()
        {
            return location ?? key;
        }


        /*Json schema is not consistent, some tokens have to be parsed individually to determine the 
         structure in order to obtain the data.*/
        public void readUncleanValues(string json_string)
        {
            IDictionary<string, JToken> data = JObject.Parse(json_string);

            foreach (KeyValuePair<string, JToken> ele in data) {
                if (ele.Value is JArray) {
                    
                } else if (ele.Value is JObject) {
                    if (ele.Key.Equals("description")) {
                        Desc = (string)(ele.Value as JObject).GetValue("value");
                    }
                } else {
                    if (ele.Key.Equals("description")) {
                        Desc = (string)ele.Value;
                    }
                }
            }
        }


        public string getImgURL(int idx = 0)
        {
            if (covers != null && covers.Count >= 1)
                return "https://covers.openlibrary.org/b/id/" + covers[idx] + "-L.jpg";
            else
                return "";
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            WorkInfo other = (WorkInfo)obj;

            if (this.GetType() != other.GetType()) {
                return false;
            }

            return this.key.Equals(other.key) && this.UID == other.UID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + this.UID;
        }
    }
}
