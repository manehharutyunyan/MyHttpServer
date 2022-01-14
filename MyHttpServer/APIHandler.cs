using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyHttpServer.Models;

namespace MyHttpServer
{
    /// <summary>
    /// The website API class
    /// </summary>
    public class APIHandler : IHttpHandler
    {
        /// <summary>
        /// The http request method type
        /// </summary>
        private readonly string method;

        /// <summary>
        /// The path of a http request
        /// </summary>
        private readonly string pathRegex;

        private readonly Func<HttpRequest, HttpResponse> handlerFunction;

        /// <summary>
        /// Generates the instance of website API
        /// </summary>
        /// <param name="method">The http method type</param>
        /// <param name="pathRegexValue">The path of a http request</param>
        /// <param name="handlerFunction">The handler function</param>
        public APIHandler(string method, string pathRegexValue, Func<HttpRequest, HttpResponse> handlerFunction)
        {
            this.method = method;
            this.pathRegex = pathRegexValue;
            this.handlerFunction = handlerFunction;
        }

        /// <summary>
        /// Handle http requests
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        /// <returns></returns>
        public async Task<HttpResponse> Handle(HttpRequest httpRequest)
        {
            // safety check
            if (httpRequest == null)
            {
                return null;
            }

            // check method
            if (httpRequest.Method != this.method)
            {
                return null;
            }

            // the regular expression to match specified path
            var regex = new Regex(this.pathRegex);

            // check path
            if (!regex.Match(httpRequest.Path).Success)
            {
                return null;
            }

            return this.handlerFunction.Invoke(httpRequest);
        }
    }
}
