using System.Collections.Generic;

namespace MyHttpServer.Models
{
    /// <summary>
    /// The Http Request
    /// </summary>
    public class HttpRequest
    {
        // The http method
        public string Method { get; set; }

        // The path
        public string Path { get; set; }

        // The version of http request
        public string Version { get; set; }

        // The key value pair of headers
        public Dictionary<string, List<string>> Headers { get; set; }

        // The body
        public byte[] Body { get; set; }

        /// <summary>
        /// Creates an instance of Http Request
        /// </summary>
        public HttpRequest()
        {
            this.Headers = new Dictionary<string, List<string>>();
        }
    }
}
