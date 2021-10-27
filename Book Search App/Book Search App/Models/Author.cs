using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SQLite;
using SQLiteNetExtensions;

namespace Book_Search_App
{
    [Table("Author")]
    public class Author
    {
        [PrimaryKey, AutoIncrement]
        public int UID { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string biography { get; set; }
        public string birth_date { get; set; }
        public string death_date { get; set; }

        /*Json schema is not consistent, some tokens have to be parsed individually to determine the 
         structure in order to obtain the data.*/
        public void readUncleanValues(string json_string)
        {
            IDictionary<string, JToken> data = JObject.Parse(json_string);

            foreach (KeyValuePair<string, JToken> ele in data) {
                if (ele.Value is JArray) {

                } else if (ele.Value is JObject) {
                    if (ele.Key.Equals("bio")) {
                        biography = (string)(ele.Value as JObject).GetValue("value");
                    }
                } else {
                    if (ele.Key.Equals("bio")) {
                        biography = (string)ele.Value;
                    }
                }
            }
        }
    }
}
