using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyHttpServer.Models;

namespace MyHttpServer
{
    // The Http Server
    public class MyHttpServer
    {
        // the path of a root folder
        public List<IHttpHandler> Handlers { get; }

        /// <summary>
        /// Creates an instance of MyHttpServer
        /// </summary>
        public MyHttpServer()
        {
            this.Handlers = new List<IHttpHandler>();
        }

            /// <summary>
        /// Run the Http server
        /// </summary>
        /// <param name="port">The port</param>
        public void Run(int port)
        {
            Console.WriteLine("http server...");

            var listener = new TcpListener(IPAddress.Any, port);

            listener.Start();

            // process tcp clients
            while (true)
            {
                // accept the tcp client
                var client = listener.AcceptTcpClient();

                // process clients asynchronous
                Task.Run(async () => await this.HandleConnection(client));
            }
        }

        /// <summary>
        /// Handle http request asynchronously
        /// </summary>
        /// <param name="incomingClient">The incoming tcp client</param>
        private async Task HandleConnection(TcpClient incomingClient)
        {
            // use client and close after this method
            using var client = incomingClient;

            // get stream out of client and close when method is done
            await using var stream = incomingClient.GetStream();

            // wrap stream into the reader and close when method is done
            using var reader = new StreamReader(stream);

            // read http request message from reader
            var httpRequest = await ReadHttpRequest(reader);

            // process http request and produce http response
            var httpResponse = await this.ProcessHttpRequest(httpRequest);

            // write back to the client the http response
            this.WriteHttpResponse(httpResponse, stream);
        }

        /// <summary>
        /// Read and create new http request message from reader
        /// </summary>
        /// <param name="reader">The stream text reader</param>
        private static async Task<HttpRequest> ReadHttpRequest(TextReader reader)
        {
            var isFirstLine = true;
            var httpRequest = new HttpRequest();
            string currentLine;

            // read stream value and store
            while ((currentLine = await reader.ReadLineAsync()) != null && !currentLine.Equals(""))
            {
                Console.WriteLine(currentLine);

                // get method, path and version from first line and store
                if (isFirstLine)
                {
                    isFirstLine = false;

                    var args = currentLine.Split(" ");
                    httpRequest.Method = args[0];
                    httpRequest.Path = args[1];
                    httpRequest.Version = args[2];

                    continue;
                }

                // get header key-value pair
                var header = currentLine.Split(":", 2, StringSplitOptions.RemoveEmptyEntries);
                var headerKey = header[0];
                var headerValue = header[1];

                // store header elements
                if (httpRequest.Headers.ContainsKey(headerKey))
                {
                    httpRequest.Headers[headerKey].Add(headerValue);
                }
                else
                {
                    httpRequest.Headers.TryAdd(headerKey, new List<string> { headerValue });
                }
            }

            // if request hasn't body
            if (currentLine is not "" || !httpRequest.Headers.ContainsKey("Content-Length"))
            {
                return httpRequest;
            }

            // get the length of a body
            var contentLength = Int32.Parse(httpRequest.Headers["Content-Length"].FirstOrDefault() ?? string.Empty);

            // read
            var charArray = new char[contentLength];
            await reader.ReadAsync(charArray, 0, contentLength);

            // store
            httpRequest.Body = new byte[contentLength];
            httpRequest.Body = charArray.Select(c => (byte)c).ToArray();

            return httpRequest;
        }

        /// <summary>
        /// Process http request and return corresponding http response
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        /// <returns>Http response</returns>
        private async Task<HttpResponse> ProcessHttpRequest(HttpRequest httpRequest)
        {
            // process handlers
            foreach (var handler in Handlers)
            {
                // get corresponding http response
                var httpResponse = await handler.Handle(httpRequest);

                // return http request
                if (httpResponse != null)
                {
                    return httpResponse;
                }
            }

            // return 404 error
            return NotFoundHttpResponse();
        }

        /// <summary>
        /// Write back to the client the http response
        /// </summary>
        /// <param name="httpResponse">The http response</param>
        /// <param name="stream">The network stream</param>
        private void WriteHttpResponse(HttpResponse httpResponse, NetworkStream stream)
        {
            Console.WriteLine("");

            StringBuilder builder = new StringBuilder();

            // add status line
            builder.AppendLine($"{httpResponse.Version} {httpResponse.StatusCode}");

            // add headers
            if (httpResponse.Headers != null)
            {
                foreach (var (key, valuesList) in httpResponse.Headers)
                {
                    foreach (var value in valuesList)
                    {
                        builder.AppendLine($" {key}: {value}");
                    }
                }
            }

            // add message body
            if (httpResponse.Body is { Length: > 0 })
            {
                builder.AppendLine(@"");
            }

            var sendBytes = Encoding.UTF8.GetBytes(builder.ToString());
            stream.Write(sendBytes, 0, sendBytes.Length);

            if (httpResponse.Headers.ContainsKey("Content-Length"))
            {
                stream.Write(httpResponse.Body, 0, httpResponse.Body.Length);
            }
        }

        /// <summary>
        /// Returns http 404 not found response
        /// </summary>
        /// <returns>The 404 http response</returns>
        public static HttpResponse NotFoundHttpResponse()
        {
            return new HttpResponse
            {
                StatusCode = "404 Not Found",
                Version = "HTTP/1.1",
            };
        }
    }
}
