using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImgurDotNet
{

    /// <summary>
    /// Represents a comment on Imgur -- read about this Imgur API data model
    /// here: https://api.imgur.com/models/comment
    /// </summary>
    public class ImgurComment
    {
        public int ID { get; private set; }
        public string ImageID { get; private set; }
        public string Comment { get; private set; }
        public string Author { get; private set; }
        public int AuthorID { get; private set; }
        public bool OnAlbum { get; private set; }
        public string AlbumCover { get; private set; }
        public int Ups { get; private set; }
        public int Downs { get; private set; }
        public int Points { get; private set; }
        public DateTime DateTime { get; private set; }
        public int ParentID { get; private set; }
        public bool Deleted { get; private set; }
        public string Vote { get; private set; }
        public List<ImgurComment> Children { get; private set; }

        public override string ToString()
        {
            return Comment;
        }

        public ImgurComment(IDictionary<string, object> data)
        {
            var commentArrayRaw = (IList<object>)data["children"];
            var commentArray = commentArrayRaw.Select(comment => new ImgurComment((IDictionary<string, object>)comment)).ToList();
            var dateTimeRaw = Convert.ToInt64(data["datetime"]);

            ID = Convert.ToInt32(data["id"]);
            ImageID = (string) data["image_id"];
            Comment = (string) data["comment"];
            Author = (string) data["author"];
            AuthorID = Convert.ToInt32(data["author_id"]);
            OnAlbum = (bool) data["on_album"];
            AlbumCover = (string) data["album_cover"];
            Ups = Convert.ToInt32(data["ups"]);
            Downs = Convert.ToInt32(data["downs"]);
            Points = Convert.ToInt32(data["points"]);
            DateTime = ConvertDate(dateTimeRaw);
            ParentID = Convert.ToInt32(data["parent_id"]);
            Deleted = (bool) data["deleted"];
            Vote = (string) data["vote"];
            Children = commentArray;
        }

        //public static ImgurComment Create(IDictionary<string, object> data)
        //{
        //    var commentArrayRaw = (IList<object>) data["children"];
        //    var commentArray = commentArrayRaw.Select(comment => Create((IDictionary<string, object>) comment)).ToList();
        //    var dateTimeRaw = Convert.ToInt64(data["datetime"]);
            
        //    return new ImgurComment
        //    {
        //        ID = Convert.ToInt32(data["id"]),
        //        ImageID = (string) data["image_id"],
        //        Comment = (string) data["comment"],
        //        Author = (string) data["author"],
        //        AuthorID = Convert.ToInt32(data["author_id"]),
        //        OnAlbum = (bool) data["on_album"],
        //        AlbumCover = (string) data["album_cover"],
        //        Ups = Convert.ToInt32(data["ups"]),
        //        Downs = Convert.ToInt32(data["downs"]),
        //        Points = Convert.ToInt32(data["points"]),
        //        DateTime = ConvertDate(dateTimeRaw),
        //        ParentID = Convert.ToInt32(data["parent_id"]),
        //        Deleted = (bool) data["deleted"],
        //        Vote = (string) data["vote"],
        //        Children = commentArray
        //    };
        //}

        private static DateTime ConvertDate(long epochTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(epochTime);
        }
    }
}
