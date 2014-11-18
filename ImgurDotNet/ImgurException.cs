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
        public string Method { get; private set; }

        public ImgurException(string message, string request, string method)
            : base(message)
        {
            Request = request;
            Method = method;
        }

        public static ImgurException Create(IDictionary<string, object> data)
        {
            return new ImgurException(
                (string)data["error"],
                (string)data["request"],
                (string)data["method"]
                );
        }
    }
}
