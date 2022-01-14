using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyHttpServer.Models;

namespace MyHttpServer
{
    /// <summary>
    /// The static website
    /// </summary>
    public class StaticWebsiteHandler: IHttpHandler
    {

        /// <summary>
        /// The static website relative path 
        /// </summary>
        private readonly DirectoryInfo path;

        /// <summary>
        /// Creates an instance of static website
        /// </summary>
        /// <param name="pathValue">The root path</param>
        public StaticWebsiteHandler(string pathValue)
        {
            this.path = new DirectoryInfo(pathValue);
        }

        /// <summary>
        /// Process http request and return corresponding http response
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        /// <returns>The http response</returns>
        public async Task<HttpResponse> Handle(HttpRequest httpRequest)
        {
            // safety check
            if (httpRequest == null || httpRequest.Path == null)
            {
                return null;
            }

            var localPaths = httpRequest.Path.Split('/', '\\', StringSplitOptions.RemoveEmptyEntries);

            var fragments = new List<string>();
            fragments.Add(this.path.FullName);
            fragments.AddRange(localPaths);

            // create the absolute path
            var absolutePath = Path.Combine(fragments.ToArray());

            // file doesn't exists
            if (!File.Exists(absolutePath))
            {
                return null;
            }

            // the http response
            var httpResponse = new HttpResponse();

            // read message body from the file
            
            var messageBody = await File.ReadAllBytesAsync(absolutePath);

            // process message body if exists
            if (messageBody.Length > 0)
            {
                httpResponse.Body = messageBody;

                // add content length
                httpResponse.Headers.Add("Content-Length", new List<string> { messageBody.Length.ToString() });
            }

            // add version and status code
            httpResponse.Version = httpRequest.Version;
            httpResponse.StatusCode = "200 OK";

            // add content-type in headers
            var contentType = MimeType.GetMimeType(absolutePath.Split('.').Last());
            httpResponse.Headers.Add("Content-Type", new List<string> { contentType });

            return httpResponse;
        }
    }
}
