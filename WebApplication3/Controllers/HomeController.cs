﻿using System;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace WebApplication3.Controllers {
    public class HomeController : Controller {
        string connectionString = ConfigurationManager.ConnectionStrings["DaveTestConnection"].ConnectionString;

        public ActionResult Index() {
            // Search for queen and render images 
            var uri = "https://api.spotify.com/v1/search?q=queen&type=artist";

            string json = null;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json";

            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream())) {
                json = sr.ReadToEnd();
            }

            var searchResult = JsonConvert.DeserializeObject<SearchResult>(json);


            // Save name and popularity to db
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(null, connection)) {
                connection.Open();

                // loop through all results and save
                foreach (var artist in searchResult.artists.items) {
                    command.CommandText = String.Format("INSERT INTO Thing(Name, Popularity) VALUES (@Name, @Popularity)");
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Name", artist.name);
                    command.Parameters.AddWithValue("@Popularity", artist.popularity);
                    command.ExecuteNonQuery();
                }
            }
            return View(searchResult);

        }
    }


    public class SearchResult2 {
        public class ExternalUrls {
            public string spotify { get; set; }
        }

        public class Followers {
            public object href { get; set; }
            public int total { get; set; }
        }

        public class Item {
            public ExternalUrls external_urls { get; set; }
            public Followers followers { get; set; }
            public List<object> genres { get; set; }
            public string href { get; set; }
            public string id { get; set; }
            public List<object> images { get; set; }
            public string name { get; set; }
            public int popularity { get; set; }
            public string type { get; set; }
            public string uri { get; set; }
        }

        public class Artists {
            public string href { get; set; }
            public List<Item> items { get; set; }
            public int limit { get; set; }
            public string next { get; set; }
            public int offset { get; set; }
            public object previous { get; set; }
            public int total { get; set; }
        }

        public Artists artists { get; set; }
    }

    public class SearchResult {
        public class ExternalUrls {
            public string spotify { get; set; }
        }

        public class Followers {
            public object href { get; set; }
            public int total { get; set; }
        }

        public class Item {
            public ExternalUrls external_urls { get; set; }
            public Followers followers { get; set; }
            public List<object> genres { get; set; }
            public string href { get; set; }
            public string id { get; set; }
            public List<Image> images { get; set; }
            public string name { get; set; }
            public int popularity { get; set; }
            public string type { get; set; }
            public string uri { get; set; }
        }

        public class Image {
            public string height { get; set; }
            public string url { get; set; }
            public string width { get; set; }

        }

        public class Artists {
            public string href { get; set; }
            public List<Item> items { get; set; }
            public int limit { get; set; }
            public string next { get; set; }
            public int offset { get; set; }
            public object previous { get; set; }
            public int total { get; set; }
        }

        public Artists artists { get; set; }
    }
}