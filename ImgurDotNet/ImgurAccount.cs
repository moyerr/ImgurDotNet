using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImgurDotNet
{
    public class ImgurAccount
    {
        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Bio { get; private set; }
        public double Reputation { get; private set; }
        public DateTime Created { get; private set; }
        public bool ProAccount { get; private set; }
        public DateTime ProExpiration { get; private set; }

        private const string urlFormat = "http://{0}.imgur.com/";

        public Uri Url
        {
            get { return new Uri(String.Format(urlFormat, Username)); }
        }

        public override string ToString()
        {
            return Username;
        }

        public static ImgurAccount Create(IDictionary<string, object> data)
        {
            var timeAddedRaw = Convert.ToInt64(data["created"]);
            bool isProAcct;
            long proAcct = 0;
            try
            {
                isProAcct = (bool) data["pro_expiration"];
            }
            catch (Exception ex)
            {
                isProAcct = true;
                proAcct = Convert.ToInt64(data["pro_expiration"]);
            }

            return new ImgurAccount
            {
                ID = Convert.ToInt32(data["id"]),
                Username = (string) data["url"],
                Bio = (string) data["bio"],
                Reputation = Convert.ToDouble(data["reputation"]),
                Created = ConvertDate(timeAddedRaw),
                ProAccount = isProAcct,
                ProExpiration = isProAcct ? ConvertDate(proAcct) : new DateTime()
            };
        }

        private static DateTime ConvertDate(long epochTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(epochTime);
        }
    }
}
