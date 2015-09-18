﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Script.Serialization;

namespace ImgurDotNet
{
    public enum RequestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    };

    public class ImgurUtils
    {
        private const string API_URL = "https://api.imgur.com/3/";
        private const string UPLOAD_URL = API_URL + "upload";
        private const string CREATE_ALBUM_URL = API_URL + "album/";
        private const string ALBUM_URL = API_URL + "album/{0}";
        private const string IMAGE_URL = API_URL + "image/{0}";
        private const string ACCOUNT_URL = API_URL + "account/{0}";
        private const string COMMENT_URL = API_URL + "comment/{0}";

        private string ClientID;

        /// <summary>Total credits/requests allowed per day for this aplication/client ID.</summary>
        /// <remarks>Value only available after executing any API request. More info: https://api.imgur.com/#limits</remarks>
        public UInt32? ClientLimit { get; private set; }

        /// <summary>Total credits available/remaining today for this aplication/client ID.</summary>
        /// <remarks>Value only available after executing any API request. More info: https://api.imgur.com/#limits</remarks>
        public UInt32? ClientLimitRemaining { get; private set; }

        /// <summary>Total credits/requests allowed per day for this IP address.</summary>
        /// <remarks>Value only available after executing any API request. More info: https://api.imgur.com/#limits</remarks>
        public UInt32? UserLimit { get; private set; }

        /// <summary>Total credits available/remaining today for this IP address.</summary>
        /// <remarks>Value only available after executing any API request. More info: https://api.imgur.com/#limits</remarks>
        public UInt32? UserLimitRemaining { get; private set; }

        /// <summary>Timestamp for when the credits will be reset for this IP address</summary>
        /// <remarks>Value only available after executing any API request. More info: https://api.imgur.com/#limits</remarks>
        public DateTime? UserLimitReset { get; private set; }

        private static readonly Dictionary<RequestMethod, string> MethodsDict = new Dictionary<RequestMethod, string>
        {
            { RequestMethod.GET, "GET"},
            { RequestMethod.POST, "POST"},
            { RequestMethod.PUT, "PUT"},
            { RequestMethod.DELETE, "DELETE"}
        };

        /// <summary>
        /// Creates an instance of the Imgur object, which provides various methods for using the 
        /// Imgur API. Register your application with Imgur to recieve a Client ID and a Client 
        /// Secret: https://api.imgur.com/oauth2/addclient
        /// </summary>
        /// <param name="clientId">The Client ID that was provided when you registered your application with Imgur.</param>
        public ImgurUtils(string clientId)
        {
            ClientID = clientId;
        }

        #region Album Actions
        public ImgurAlbum GetAlbum(string albumId, string deleteHash = "")
        {
            var response = GetParsedJsonResponse(String.Format(ALBUM_URL, albumId));
            var responseData = (IDictionary<string, object>)response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw new ImgurException(responseData);

            if (first.Key != "id") throw new Exception("Couldn't parse response: " + first.Key);

            if (!responseData.ContainsKey("deletehash")) responseData.Add("deletehash", deleteHash);
            return new ImgurAlbum(responseData);
        }

        public ImgurAlbum CreateAlbum(string title = "",
            string description = "",
            string privacy = "",
            string layout = "")
        {
            var listOfArgs = new List<string>();
            if (title != "") listOfArgs.Add("title=" + title);
            if (description != "") listOfArgs.Add("description=" + description);
            if (privacy != "") listOfArgs.Add("privacy=" + privacy);
            if (layout != "") listOfArgs.Add("layout=" + layout);

            for (int i = 0; i < listOfArgs.Count; i++)
            {
                if (i > 0) listOfArgs[i] = listOfArgs[i].Insert(0, "&");
            }

            var data = listOfArgs.Aggregate("", (current, arg) => current + arg);

            var response = GetParsedJsonResponse(CREATE_ALBUM_URL, data);
            var responseData = (IDictionary<string, object>)response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw new ImgurException(responseData);

            if (first.Key == "id")
                return GetAlbum((string)responseData["id"], (string)responseData["deletehash"]);

            throw new Exception("Couldn't parse response: " + first.Key);
        }

        public void DeleteAlbum(string deleteHash)
        {
            var response = GetParsedJsonResponse(String.Format(ALBUM_URL, deleteHash), RequestMethod.DELETE);
            var success = (bool)response["success"];

            if (success) return;

            var responseData = (IDictionary<string, object>)response["data"];

            if (responseData.First().Key == "error")
                throw new ImgurException(responseData);
        }

        public void DeleteAlbum(ImgurAlbum album)
        {
            DeleteAlbum(album.DeleteHash);
        }
        #endregion

        #region Image Actions
        public ImgurImage GetImage(string imageId)
        {
            var response = GetParsedJsonResponse(String.Format(IMAGE_URL, imageId));
            var responseData = (IDictionary<string, object>)response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw new ImgurException(responseData);

            if (first.Key == "id")
                return new ImgurImage(responseData);

            throw new Exception("Couldn't parse response: " + first.Key);
        }

        private ImgurImage UploadImage(string data)
        {
            var response = GetParsedJsonResponse(UPLOAD_URL, data);
            var responseData = (IDictionary<string, object>)response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw new ImgurException(responseData);

            if (first.Key == "id")
                return new ImgurImage(responseData);

            throw new Exception("Couldn't parse response: " + first.Key);
        }

        public ImgurImage UploadImageFromWeb(string imageURL,
            string title = "",
            string description = "",
            string albumID = "")
        {
            var data = String.Format("image={0}{1}{2}{3}",
                imageURL,
                title == "" ? "" : "&title=" + title,
                description == "" ? "" : "&description=" + description,
                albumID == "" ? "" : "&album=" + albumID
                );

            return UploadImage(data);
        }

        public ImgurImage UploadImageFromFile(string imageFilePath,
            string title = "",
            string description = "",
            string albumID = "")
        {
            byte[] bytes = File.ReadAllBytes(imageFilePath);

            var data = String.Format("image={0}{1}{2}{3}",
                EscapeBase64(Convert.ToBase64String(bytes)),
                title == "" ? "" : "&title=" + title,
                description == "" ? "" : "&description=" + description,
                albumID == "" ? "" : "&album=" + albumID
                );

            return UploadImage(data);
        }

        public ImgurImage UploadImage(byte[] imgData,
            string title = "",
            string description = "",
            string albumID = "")
        {
            var data = String.Format("image={0}{1}{2}{3}",
                EscapeBase64(Convert.ToBase64String(imgData)),
                title == "" ? "" : "&title=" + title,
                description == "" ? "" : "&description=" + description,
                albumID == "" ? "" : "&album=" + albumID
                );

            return UploadImage(data);
        }

        public ImgurImage UploadImage(Image image, ImageFormat format,
            string title = "",
            string description = "",
            string albumID = "")
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            return UploadImage(stream.ToArray(), title, description, albumID);
        }

        public void DeleteImage(string deleteHash)
        {
            var response = GetParsedJsonResponse(String.Format(IMAGE_URL, deleteHash), RequestMethod.DELETE);
            var success = (bool)response["success"];

            if (success) return;

            var responseData = (IDictionary<string, object>)response["data"];

            if (responseData.First().Key == "error")
                throw new ImgurException(responseData);
        }

        public void DeleteImage(ImgurImage img)
        {
            DeleteImage(img.DeleteHash);
        }
        #endregion

        #region Account Actions
        public ImgurAccount GetAccount(string username)
        {
            var response = GetParsedJsonResponse(String.Format(ACCOUNT_URL, username));
            var responseData = (IDictionary<string, object>)response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw new ImgurException(responseData);

            if (first.Key == "id")
                return new ImgurAccount(responseData);

            throw new Exception("Couldn't parse response: " + first.Key);
        }
        #endregion

        #region Comment Actions
        public ImgurComment GetComment(string commentId, bool getReplies)
        {
            var response = GetParsedJsonResponse(String.Format(getReplies ? COMMENT_URL + "/replies" : COMMENT_URL, commentId));
            var responseData = (IDictionary<string, object>)response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw new ImgurException(responseData);

            if (first.Key == "id")
                return new ImgurComment(responseData);

            throw new Exception("Couldn't parse response: " + first.Key);
        }
        #endregion

        #region Internal Utility Methods
        private static string EscapeBase64(string str)
        {
            string escaped = "";

            if (str.Length > 32766)
            {
                escaped += EscapeBase64(str.Substring(0, str.Length / 2));
                escaped += EscapeBase64(str.Substring(str.Length / 2));
            }
            else
            {
                escaped = Uri.EscapeDataString(str);
            }

            return escaped;
        }

        private IDictionary<string, object> GetParsedJsonResponse(WebRequest req)
        {
            Stream resp;
            WebResponse webResponse;
            try
            {
                webResponse = req.GetResponse();
                resp = webResponse.GetResponseStream();
                try
                {
                    this.ClientLimit = UInt32.Parse(webResponse.Headers["X-RateLimit-ClientLimit"]);
                    this.ClientLimitRemaining = UInt32.Parse(webResponse.Headers["X-RateLimit-ClientRemaining"]);
                    this.UserLimit = UInt32.Parse(webResponse.Headers["X-RateLimit-UserLimit"]);
                    this.UserLimitRemaining = UInt32.Parse(webResponse.Headers["X-RateLimit-UserRemaining"]);

                    string unixTimestamp = webResponse.Headers["X-RateLimit-UserReset"];
                    long unixTimeStamp = long.Parse(unixTimestamp);
                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                    this.UserLimitReset = dtDateTime;
                }
                catch (FormatException)
                {
                    //unable to retrieve user and client limits
                }
            }
            catch (WebException e)
            {
                resp = e.Response.GetResponseStream();
            }
            var reader = new StreamReader(resp);
            var deserializer = new JavaScriptSerializer();
            var responseData = (IDictionary<string, object>)deserializer.DeserializeObject(reader.ReadToEnd());
            reader.Close();
            if (resp != null) resp.Close();
            return responseData;
        }

        private IDictionary<string, object> GetParsedJsonResponse(string url)
        {
            var request = WebRequest.Create(url);
            request.Headers.Add("Authorization", "Client-ID " + ClientID);

            return GetParsedJsonResponse(request);
        }

        private IDictionary<string, object> GetParsedJsonResponse(string url, RequestMethod method)
        {
            var request = WebRequest.Create(url);
            request.Method = MethodsDict[method];
            request.Headers.Add("Authorization", "Client-ID " + ClientID);

            return GetParsedJsonResponse(request);
        }

        private IDictionary<string, object> GetParsedJsonResponse(string url, string postData)
        {
            var request = WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = MethodsDict[RequestMethod.POST];
            request.Headers.Add("Authorization", "Client-ID " + ClientID);

            var data = UTF8Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;

            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();

            var res = GetParsedJsonResponse(request);
            return res;
        }
        #endregion
    }
}
