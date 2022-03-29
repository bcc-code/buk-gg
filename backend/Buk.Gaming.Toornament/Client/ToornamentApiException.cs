using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Buk.Gaming.Toornament
{
    public class ToornamentApiException : Exception
    {
        public ToornamentApiException()
        {
        }

        public ToornamentApiException(string message) : base(message)
        {
        }

        public ToornamentApiException(string message, Exception innerException) : base(message, innerException)
        {
        }


        public HttpStatusCode StatusCode { get; set; }

        public string Response { get; set; }

        public string RequestUri { get; set; }
    }
}
