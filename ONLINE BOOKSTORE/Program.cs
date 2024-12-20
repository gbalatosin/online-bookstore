using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace ONLINE_BOOKSTORE
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Transalliance Online Bookstore Backend");

            // Define the connection string for PostgreSQL
            var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=gbala123;Database=bookstore";

            // Establish a connection
            using var connection = new NpgsqlConnection(connectionString);

            // Initialize the database schema
            await InitializeDatabase(connection);

            // Execute sample operations for testing
            await RunSampleOperations(connection);
        }

        // Initializes the database schema (creates tables if they don't exist)
        private static async Task InitializeDatabase(IDbConnection connection)
        {
            var createBooksTable = @"CREATE TABLE IF NOT EXISTS Books (
                Id SERIAL PRIMARY KEY,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                Genre TEXT NOT NULL,
                ISBN TEXT NOT NULL UNIQUE,
                PublicationYear INT NOT NULL
            );";

            var createCartTable = @"CREATE TABLE IF NOT EXISTS ShoppingCart (
                Id SERIAL PRIMARY KEY,
                BookId INT NOT NULL,
                Quantity INT NOT NULL,
                FOREIGN KEY (BookId) REFERENCES Books(Id)
            );";

            await connection.ExecuteAsync(createBooksTable);
            await connection.ExecuteAsync(createCartTable);
        }

        // Runs a series of sample operations to test the system
        private static async Task RunSampleOperations(IDbConnection connection)
        {
            // Adding a sample book
            var newBook = new Book
            {
                Title = "Transalliance Book",
                Author = "Transalliance",
                Genre = "Business Plan",
                ISBN = "1234567890",
                PublicationYear = 2024
            };

            Console.WriteLine("Adding a new book...");
            await AddBook(connection, newBook);

            // Searching for books by author
            Console.WriteLine("Searching for books by author 'Jane Doe'...");
            var books = await SearchBooks(connection, "Jane Doe", null, null, null);
            foreach (var book in books)
            {
                Console.WriteLine($"{book.Title} by {book.Author}");
            }

            // Adding items to the cart
            Console.WriteLine("Adding book with ID 1 to the shopping cart...");
            await AddToCart(connection, 1, 2);

            // Viewing items in the cart
            Console.WriteLine("Viewing items in the shopping cart...");
            var cartItems = await ViewCart(connection);
            foreach (var item in cartItems)
            {
                Console.WriteLine($"Book ID: {item.BookId}, Quantity: {item.Quantity}");
            }
        }

        // Adds a book to the database
        public static async Task AddBook(IDbConnection connection, Book book)
        {
            var sql = @"INSERT INTO Books (Title, Author, Genre, ISBN, PublicationYear)
                        VALUES (@Title, @Author, @Genre, @ISBN, @PublicationYear);";
            await connection.ExecuteAsync(sql, book);
        }

        // Searches for books in the database based on various criteria
        public static async Task<IEnumerable<Book>> SearchBooks(IDbConnection connection, string? author, string? title, string? genre, int? year)
        {
            var sql = @"SELECT * FROM Books WHERE 
                        (@Author IS NULL OR Author ILIKE '%' || @Author || '%') AND
                        (@Title IS NULL OR Title ILIKE '%' || @Title || '%') AND
                        (@Genre IS NULL OR Genre ILIKE '%' || @Genre || '%') AND
                        (@Year IS NULL OR PublicationYear = @Year);";

            return await connection.QueryAsync<Book>(sql, new { Author = author, Title = title, Genre = genre, Year = year });
        }

        // Adds a book to the shopping cart
        public static async Task AddToCart(IDbConnection connection, int bookId, int quantity)
        {
            var sql = @"INSERT INTO ShoppingCart (BookId, Quantity) VALUES (@BookId, @Quantity);";
            await connection.ExecuteAsync(sql, new { BookId = bookId, Quantity = quantity });
        }

        // Retrieves all items in the shopping cart
        public static async Task<IEnumerable<CartItem>> ViewCart(IDbConnection connection)
        {
            var sql = @"SELECT * FROM ShoppingCart;";
            return await connection.QueryAsync<CartItem>(sql);
        }

        // Represents the Book entity
        public class Book
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string Genre { get; set; }
            public string ISBN { get; set; }
            public int PublicationYear { get; set; }
        }

        // Represents a shopping cart item
        public class CartItem
        {
            public int Id { get; set; }
            public int BookId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
