using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XDeco.Integration.nytimes
{
   public class Source
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Article
    {
        [JsonProperty("source")]
        public Source Source { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("urlToImage")]
        public string UrlToImage { get; set; }
        [JsonProperty("publishedAt")]
        public DateTime PublishedAt { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }
        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }
    }


}