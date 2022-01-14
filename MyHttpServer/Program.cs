namespace MyHttpServer
{
    class Program
    {
        static void Main()
        {
            // create http server
            var httpServer = new MyHttpServer();

            // add API handlers
            httpServer.Handlers.Add(new APIHandler("GET", "^/api/books$", BooksController.GetAll));
            httpServer.Handlers.Add(new APIHandler("GET", "^/api/book/get/[0-9]$", BooksController.Get));
            httpServer.Handlers.Add(new APIHandler("POST", "^/api/book/post$", BooksController.Post));
            httpServer.Handlers.Add(new APIHandler("PUT", "^/api/book/put$", BooksController.Put));
            httpServer.Handlers.Add(new APIHandler("DELETE", "^/api/book/delete/[0-9]$", BooksController.Delete));
            
            // add website handler
            httpServer.Handlers.Add(new StaticWebsiteHandler("C:\\Users\\Maneh.Harutyunyan\\MyProjects\\Z-ReactProject\\hello-world-app\\build"));

            // run http server on 8080 port
            httpServer.Run(8080);
        }
    }
}

