using System;
using System.Collections.Generic;
using System.Linq;

namespace ImgurDotNet
{
    /// <summary>
    /// Represents an album of images on Imgur -- read about this Imgur API data model
    /// here: https://api.imgur.com/model/album
    /// </summary>
    public class ImgurAlbum
    {
        public enum LayoutType
        {
            Blog,
            Grid,
            Vertical,
            Horizontal
        }

        public enum PrivacyLevel
        {
            Public,
            Hidden,
            Secret
        }

        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime TimeAdded { get; private set; }
        public string Cover { get; private set; }
        public int CoverWidth { get; private set; }
        public int CoverHeight { get; private set; }
        public string AccountID { get; private set; }
        public PrivacyLevel Privacy { get; private set; }
        public LayoutType Layout { get; private set; }
        public int Views { get; private set; }
        public string Link { get; private set; }
        public bool Favorite { get; private set; }
        public bool Nsfw { get; private set; }
        public string Section { get; private set; }
        public int Order { get; private set; }
        public string DeleteHash { get; private set; }
        public int ImagesCount { get; private set; }
        public List<ImgurImage> Images { get; private set; }

        public static ImgurAlbum Create(IDictionary<string, object> data)
        {
            var layoutRaw = (string) data["layout"];
            var privacyRaw = (string) data["privacy"];
            var timeAddedRaw = Convert.ToInt64(data["datetime"]);

            var deleteHash = data.ContainsKey("deletehash") ? (string) data["deletehash"] : null;
            var order = data.ContainsKey("order") ? Convert.ToInt32(data["order"]) : 0;
            
            var imageArrayRaw = (IList<object>) data["images"];
            var imageArray = imageArrayRaw.Select(image => ImgurImage.Create((IDictionary<string, object>) image)).ToList();

            var album = new ImgurAlbum();

            album.ID = (string) data["id"];
            album.Title = (string) data["title"];
            album.Description = (string) data["description"];
            album.TimeAdded = ConvertDateTime(timeAddedRaw);
            album.Cover = (string) data["cover"];
            album.CoverWidth = Convert.ToInt32(data["cover_width"]);
            album.CoverHeight = Convert.ToInt32(data["cover_height"]);
            album.AccountID = data["account_url"] == null ? null : (string)data["account_url"];
            album.Privacy = ConvertPrivacy(privacyRaw);
            album.Layout = ConvertLayout(layoutRaw);
            album.Views = Convert.ToInt32(data["views"]);
            album.Link = (string) data["link"];
            album.Favorite = (bool) data["favorite"];
            album.Nsfw = data["nsfw"] != null && (bool) data["nsfw"];
            album.Section = data["section"] == null ? null : (string) data["section"];
            album.Order = order;
            album.DeleteHash = deleteHash;
            album.ImagesCount = Convert.ToInt32(data["images_count"]);
            album.Images = imageArray;

            return album;
        }

        #region Private Conversion Methods
        private static DateTime ConvertDateTime(long epochTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(epochTime);
        }

        private static LayoutType ConvertLayout(string rawLayout)
        {
            switch (rawLayout)
            {
                case "blog":
                    return LayoutType.Blog;
                case "grid":
                    return LayoutType.Grid;
                case "vertical":
                    return LayoutType.Vertical;
                case "horizontal":
                    return LayoutType.Horizontal;
                default:
                    throw new Exception("Couldn't parse album layout: " + rawLayout);
            }
        }

        private static PrivacyLevel ConvertPrivacy(string rawPrivacy)
        {
            switch (rawPrivacy)
            {
                case "public":
                    return PrivacyLevel.Public;
                case "hidden":
                    return PrivacyLevel.Hidden;
                case "secret":
                    return PrivacyLevel.Secret;
                default:
                    throw new Exception("Couldn't parse album privacy: " + rawPrivacy);
            }
        }
        #endregion
    }
}
