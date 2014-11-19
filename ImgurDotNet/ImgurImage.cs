using System;
using System.Collections.Generic;
using System.IO;

namespace ImgurDotNet
{
    /// <summary>
    /// Represents an image on Imgur -- read about this Imgur API data model
    /// here: https://api.imgur.com/models/image
    /// </summary>
    public class ImgurImage
    {
        public enum ThumbnailSize
        {
            SmallSquare,
            BigSquare,
            SmallThumbnail,
            MediumThumbnail,
            LargeThumbnail,
            HugeThumbnail
        }

        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime TimeAdded { get; private set; }
        public string Type { get; private set; }
        public bool Animated { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Size { get; private set; }
        public int Views { get; private set; }
        public long Bandwith { get; private set; }
        public string DeleteHash { get; private set; }
        public string Name { get; private set; }
        public string Section { get; private set; }
        public Uri Link { get; private set; }
        public Uri Gifv { get; private set; }
        public Uri Mp4 { get; private set; }
        public Uri Webm { get; private set; }
        public bool Looping { get; private set; }
        public bool Favorite { get; private set; }
        public bool Nsfw { get; private set; }
        public string Vote { get; private set; }
        public string AccountID { get; private set; }

        public override string ToString()
        {
            return Link.ToString();
        }

        public static ImgurImage Create(IDictionary<string, object> data)
        {
            var imgName = data.ContainsKey("name") ? (string) data["name"] : null;
            var desc = data.ContainsKey("description") ? (string)data["description"] : null;
            var deleteHash = data.ContainsKey("deletehash") ? (string) data["deletehash"] : null;
            var gifvUri = data.ContainsKey("gifv") ? new Uri((string) data["gifv"]) : null;
            var mp4Uri = data.ContainsKey("mp4") ? new Uri((string) data["mp4"]) : null;
            var webmUri = data.ContainsKey("webm") ? new Uri((string) data["webm"]) : null;
            var isLooping = data.ContainsKey("looping") && (bool) data["looping"];

            var timeAddedRaw = Convert.ToInt64(data["datetime"]);

            return new ImgurImage
            {
                ID = (string) data["id"],
                Title = (string) data["title"],
                Description = desc,
                TimeAdded = ConvertDate(timeAddedRaw),
                Type = (string) data["type"],
                Animated = (bool) data["animated"],
                Height = Convert.ToInt32(data["height"]),
                Width = Convert.ToInt32(data["width"]),
                Size = Convert.ToInt32(data["size"]),
                Views = Convert.ToInt32(data["views"]),
                Bandwith = Convert.ToInt64(data["bandwidth"]),
                DeleteHash = deleteHash,
                Name = imgName,
                Section = data["section"] == null ? null : (string) data["section"],
                Link = new Uri((string) data["link"]),
                Gifv = gifvUri,
                Mp4 = mp4Uri,
                Webm = webmUri,
                Looping = isLooping,
                Favorite = (bool) data["favorite"],
                Nsfw = data["nsfw"] != null && (bool) data["nsfw"],
                Vote = data["vote"] == null ? null : (string) data["vote"],
                AccountID = data["account_url"] == null ? null : (string) data["account_url"]
            };
        }

        private static DateTime ConvertDate(long epochTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(epochTime);
        }

        public Uri GetThumbnail(ThumbnailSize size)
        {
            const string linkTemplate = "http://i.imgur.com/{0}{1}{2}";
            var extension = Path.GetExtension(Link.AbsolutePath);

            string thumb;
            switch (size)
            {
                case ThumbnailSize.SmallSquare:
                    thumb = "s";
                    break;
                case ThumbnailSize.BigSquare:
                    thumb = "b";
                    break;
                case ThumbnailSize.SmallThumbnail:
                    thumb = "t";
                    break;
                case ThumbnailSize.MediumThumbnail:
                    thumb = "m";
                    break;
                case ThumbnailSize.LargeThumbnail:
                    thumb = "l";
                    break;
                case ThumbnailSize.HugeThumbnail:
                    thumb = "h";
                    break;
                default:
                    thumb = "";
                    break;
            }

            return new Uri(String.Format(linkTemplate, ID, thumb, extension));
        }
    }
}
