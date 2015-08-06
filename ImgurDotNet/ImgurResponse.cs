using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ImgurDotNet
{
    public class ImgurResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
