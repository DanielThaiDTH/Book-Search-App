using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Book_Search_App
{
    public delegate void WorkInfoDel(WorkInfo info);
    public class WorkInfo
    {
        public string Desc { get; set; }
        public List<string> Author_List { get; private set; }
        public string title { get; set; }
        public string key { get; set; }
        public IList<int> covers { get; set; }
        public IList<string> subject_places { get; set; }
        public string first_publish_date { get; set; }
        public IList<string> subjects { get; set; }

        WorkInfo()
        {
            Author_List = new List<string>();
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


        public string getImgURL()
        {
            if (covers != null && covers.Count >= 1)
                return "http://covers.openlibrary.org/b/id/" + covers[0] + ".jpg";
            else
                return "";
        }
    }
}
