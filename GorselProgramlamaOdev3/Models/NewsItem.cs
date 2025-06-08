using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GorselProgramlamaOdev3.Models
{
    public class NewsItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("pubDate")]
        public string PubDate { get; set; }

        [JsonPropertyName("enclosure")]
        public Enclosure Enclosure { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }

        public string Category { get; set; }

        public DateTime ParsedDate
        {
            get
            {
                if (DateTime.TryParse(PubDate, out DateTime result))
                    return result;
                return DateTime.Now;
            }
        }

        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return "";

                var cleanDescription = System.Text.RegularExpressions.Regex.Replace(Description, "<.*?>", "");
                return cleanDescription.Length > 150
                    ? cleanDescription.Substring(0, 150) + "..."
                    : cleanDescription;
            }
        }

        public string CleanDescription
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return "";

                var cleanText = System.Text.RegularExpressions.Regex.Replace(Description, "<.*?>", "");

                cleanText = cleanText.Replace("&quot;", "\"")
                                   .Replace("&amp;", "&")
                                   .Replace("&lt;", "<")
                                   .Replace("&gt;", ">")
                                   .Replace("&nbsp;", " ")
                                   .Replace("&#39;", "'");

                return cleanText.Trim();
            }
        }
    }

    public class Enclosure
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class RssResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("feed")]
        public Feed Feed { get; set; }

        [JsonPropertyName("items")]
        public List<NewsItem> Items { get; set; }
    }

    public class Feed
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}