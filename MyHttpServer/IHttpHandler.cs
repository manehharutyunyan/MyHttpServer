using System.Threading.Tasks;
using MyHttpServer.Models;

namespace MyHttpServer
{
    /// <summary>
    /// The Http server handler
    /// </summary>
    public interface IHttpHandler
    {
        /// <summary>
        /// Handle http request
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        /// <returns>The http response</returns>
        Task<HttpResponse> Handle(HttpRequest httpRequest);
    }
}
