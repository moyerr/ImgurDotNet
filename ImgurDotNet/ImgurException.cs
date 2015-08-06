using System;
using System.Collections.Generic;

namespace ImgurDotNet
{
    /// <summary>
    /// Represents errors that occur during the use of the Imgur API.
    /// </summary>
    public class ImgurException : Exception
    {
        public string Request { get; private set; }
        public RequestMethod Method { get; private set; }

        public ImgurException(string message, string request, string method)
            : base(message)
        {
            Request = request;
            Method = ConvertRequestMethod(method);
        }

        public ImgurException(IDictionary<string, object> data) : base((string)data["error"])
        {
            Request = (string) data["request"];
            Method = ConvertRequestMethod((string) data["method"]);
        }

        private static RequestMethod ConvertRequestMethod(string mthd)
        {
            switch (mthd)
            {
                case "GET":
                    return RequestMethod.GET;
                case "POST":
                    return RequestMethod.POST;
                case "PUT":
                    return RequestMethod.PUT;
                case "DELETE":
                    return RequestMethod.DELETE;
                default:
                    return RequestMethod.GET;
            }
        }
    }
}
