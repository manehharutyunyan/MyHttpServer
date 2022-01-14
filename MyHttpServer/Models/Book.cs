namespace MyHttpServer.Models
{
    /// <summary>
    /// The book
    /// </summary>
    public class Book
    {
        /// <summary>
        /// The book Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The book title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The book title
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Creates an instance of Book
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="title">The title</param>
        /// <param name="author">The author</param>
        public Book(int id, string title, string author)
        {
            this.Id = id;
            this.Title = title;
            this.Author = author;
        }
    }
}
