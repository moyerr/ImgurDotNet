using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Script.Serialization;

namespace ImgurDotNet
{
    public class Imgur
    {
        private const string UPLOAD_URL = "https://api.imgur.com/3/upload";
        private const string ALBUM_URL = "https://api.imgur.com/3/album/{0}";
        private const string IMAGE_URL = "https://api.imgur.com/3/image/{0}";

        private static string ClientID;

        public enum RequestMethod
        {
            GET,
            POST,
            DELETE
        };

        private static Dictionary<RequestMethod, string> MethodsDict = new Dictionary<RequestMethod,string>
        {
            { RequestMethod.GET, "GET"},
            { RequestMethod.POST, "POST"},
            { RequestMethod.DELETE, "DELETE"}
        };

        /// <summary>
        /// Creates an instance of the Imgur object, which provides various methods for using the 
        /// Imgur API. Register your application with Imgur to recieve a Client ID and a Client 
        /// Secret: https://api.imgur.com/oauth2/addclient
        /// </summary>
        /// <param name="clientId">The Client ID that was provided when you registered your application with Imgur.</param>
        public Imgur(string clientId)
        {
            ClientID = clientId;
        }

        #region Album Actions
        public ImgurAlbum GetAlbum(string albumId)
        {
            var response = GetParsedJsonResponse(String.Format(ALBUM_URL, albumId));
            var responseData = (IDictionary<string, object>) response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw ImgurException.Create(responseData);
            
            if (first.Key == "id")
                return ImgurAlbum.Create(responseData);
            
            throw new Exception("Couldn't parse response: " + first.Key);
        }
        #endregion

        #region Image Actions
        public ImgurImage GetImage(string imageId)
        {
            var response = GetParsedJsonResponse(String.Format(IMAGE_URL, imageId));
            var responseData = (IDictionary<string, object>) response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw ImgurException.Create(responseData);

            if (first.Key == "id")
                return ImgurImage.Create(responseData);

            throw new Exception("Couldn't parse response: " + first.Key);
        }

        private ImgurImage UploadImage(string data)
        {
            var response = GetParsedJsonResponse(UPLOAD_URL, data);
            var responseData = (IDictionary<string, object>) response["data"];
            var first = responseData.First();

            if (first.Key == "error")
                throw ImgurException.Create(responseData);

            if (first.Key == "id")
                return ImgurImage.Create(responseData);

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
            var success = (bool) response["success"];

            if (success) return;
            
            var responseData = (IDictionary<string, object>) response["data"];
            
            if (responseData.First().Key == "error")
                throw ImgurException.Create(responseData);
        }

        public void DeleteImage(ImgurImage img)
        {
            DeleteImage(img.DeleteHash);
        }
        #endregion

        #region Internal Utility Methods
        private static string EscapeBase64(string str)
        {
            string escaped = "";

            if (str.Length > 32766)
            {
                escaped += EscapeBase64(str.Substring(0, str.Length/2));
                escaped += EscapeBase64(str.Substring(str.Length/2));
            }
            else
            {
                escaped = Uri.EscapeDataString(str);
            }

            return escaped;
        }

        private static IDictionary<string, object> GetParsedJsonResponse(string url)
        {
            var request = WebRequest.Create(url);
            request.Headers.Add("Authorization", "Client-ID " + ClientID);
            Stream resp = null;
            try
            {
                resp = request.GetResponse().GetResponseStream();
            }
            catch (WebException e)
            {
                resp = e.Response.GetResponseStream();
            }
            var reader = new StreamReader(resp);
            var deserializer = new JavaScriptSerializer();
            var response = (IDictionary<string, object>)deserializer.DeserializeObject(reader.ReadToEnd());
            reader.Close();
            resp.Close();
            return response;
        }

        private static IDictionary<string, object> GetParsedJsonResponse(string url, RequestMethod method)
        {
            var request = WebRequest.Create(url);
            request.Method = MethodsDict[method];
            request.Headers.Add("Authorization", "Client-ID " + ClientID);
            Stream resp = null;
            try
            {
                resp = request.GetResponse().GetResponseStream();
            }
            catch (WebException e)
            {
                resp = e.Response.GetResponseStream();
            }
            var reader = new StreamReader(resp);
            var deserializer = new JavaScriptSerializer();
            var response = (IDictionary<string, object>)deserializer.DeserializeObject(reader.ReadToEnd());
            reader.Close();
            resp.Close();
            return response;
        }

        private static IDictionary<string, object> GetParsedJsonResponse(string url, string postData)
        {
            var request = WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Headers.Add("Authorization", "Client-ID " + ClientID);
            var data = UTF8Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;

            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();

            Stream resp = null;
            try
            {
                resp = request.GetResponse().GetResponseStream();
            }
            catch (WebException e)
            {
                resp = e.Response.GetResponseStream();
            }
            var reader = new StreamReader(resp);
            var deserializer = new JavaScriptSerializer();
            var response = (IDictionary<string, object>)deserializer.DeserializeObject(reader.ReadToEnd());
            reader.Close();
            resp.Close();
            return response;
        }
        #endregion
    }
}
