using System.Collections.Generic;

namespace MyHttpServer.Models
{
    // The Http Response
    public class HttpResponse
    {
        // The status code
        public string StatusCode { get; set; }

        // The version
        public string Version { get; set; }

        // The key value pair of a header
        public Dictionary<string, List<string>> Headers { get; set; }

        // The body
        public byte[] Body { get; set; }

        /// <summary>
        /// Creates new object of Http Response
        /// </summary>
        public HttpResponse()
        {
            this.Headers = new Dictionary<string, List<string>>();
            this.Body = new byte[]{};
        }
    }
}
