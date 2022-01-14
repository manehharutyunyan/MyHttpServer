using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyHttpServer.Models;
using Newtonsoft.Json;

namespace MyHttpServer
{
    /// <summary>
    /// The books controller
    /// </summary>
    public static class BooksController
    {
        /// <summary>
        /// The list of books
        /// </summary>
        private static readonly List<Book> books = new List<Book>()
        {
            new Book(1, "Ulysses", "James Joyce"),
            new Book(2, "Don Quixote", " Miguel de Cervantes"),
            new Book(3, "One Hundred Years of Solitude ", " Gabriel Garcia Marquez"),
            new Book(4, "The Great Gatsby", "F. Scott Fitzgerald"),
            new Book(5, "War and Peace", "Leo Tolstoy"),
            new Book(6, "Hamlet", "William Shakespeare"),
            new Book(7, "To the Lighthouse", "Virginia Woolf"),
            new Book(8, "Nineteen Eighty Four", "George Orwell")
        };

        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        public static HttpResponse GetAll(HttpRequest httpRequest)
        {
            // create and return http response and put books in a body
            return CreateHttpResponse(httpRequest, Newtonsoft.Json.JsonConvert.SerializeObject(books));
        }

        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        public static HttpResponse Get(HttpRequest httpRequest)
        {
            var id = Int32.Parse(httpRequest.Path.Split('/').Last());

            // get book by id
            var book = books.FirstOrDefault(book => book.Id.Equals(id));

            // create and return http response and put book in a body
            return CreateHttpResponse(httpRequest, Newtonsoft.Json.JsonConvert.SerializeObject(book));
        }

        /// <summary>
        /// Add book to the list
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        public static HttpResponse Post(HttpRequest httpRequest)
        {
            // get book from body
            var book = JsonConvert.DeserializeObject<Book>(System.Text.Encoding.Default.GetString(httpRequest.Body));

            // book already exists
            if (books.Exists(b => b.Id.Equals(book.Id)))
            {
                return CreateHttpResponse(httpRequest, "Book already exists");
            }

            // store
            books.Add(book);

            // create and return http response and put new added book in a body
            return CreateHttpResponse(httpRequest, "Book has added successfully");
        }

        /// <summary>
        /// Edit book
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        public static HttpResponse Put(HttpRequest httpRequest)
        {
            // get book from body
            var book = JsonConvert.DeserializeObject<Book>(System.Text.Encoding.Default.GetString(httpRequest.Body));

            // book already exists
            if (!books.Exists(b => b.Id.Equals(book.Id)))
            {
                return CreateHttpResponse(httpRequest, "This book doesn't exists");
            }

            // get book
            var newBook = books.FirstOrDefault(b => b.Id.Equals(book.Id));

            // edit values
            newBook.Author = book.Author;
            newBook.Title = book.Title;

            // create and return http response
            return CreateHttpResponse(httpRequest, "Book has edited successfully");
        }

        /// <summary>
        /// Delete book from the list
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        public static HttpResponse Delete(HttpRequest httpRequest)
        {
            // get book id from a path
            var bookId = Int32.Parse(httpRequest.Path.Split('/').Last());

            // get book
            var book = books.FirstOrDefault(b => b.Id.Equals(bookId));

            // book doesn't exists
            if (book == null)
            {
                return CreateHttpResponse(httpRequest, "This book doesn't exists");
            }

            // remove book
            books.Remove(book);

            // create and return http response
            return CreateHttpResponse(httpRequest, "Book has deleted successfully");
        }

        /// <summary>
        /// Creates HttpResponse
        /// </summary>
        /// <param name="httpRequest">The http request</param>
        /// <param name="body">The message body</param>
        /// <returns></returns>
        private static HttpResponse CreateHttpResponse(HttpRequest httpRequest, string body)
        {
            // safety check
            if (httpRequest == null || body == null)
            {
                return null;
            }

            // the http response
            var httpResponse = new HttpResponse();

            // request body
            var messageBody = Encoding.ASCII.GetBytes(body);

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
            var contentType = MimeType.GetMimeType(httpRequest.Path.Split('.').Last());
            httpResponse.Headers.Add("Content-Type", new List<string> { contentType });

            return httpResponse;
        }
    }
}
