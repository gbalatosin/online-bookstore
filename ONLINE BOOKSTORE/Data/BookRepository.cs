namespace ONLINE_BOOKSTORE.Data
{
    using Dapper;
    using Npgsql;
    using ONLINE_BOOKSTORE.Models;

    public class BookRepository
    {
        private string connectionString = "Host=localhost;Database=Bookstore;Username=yourusername;Password=yourpassword";

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Book>("SELECT * FROM Books");
            }
        }

        public async Task AddBookAsync(Book book)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var sql = "INSERT INTO Books (Title, Genre, ISBN, Author, PublicationYear) VALUES (@Title, @Genre, @ISBN, @Author, @PublicationYear)";
                await connection.ExecuteAsync(sql, book);
            }
        }

        public async Task<int> CheckoutAsync(int userId, string paymentMethod, decimal totalAmount)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var sql = "INSERT INTO Orders (UserId, PaymentMethod, TotalAmount) VALUES (@UserId, @PaymentMethod, @TotalAmount) RETURNING Id";
                return await connection.ExecuteScalarAsync<int>(sql, new
                {
                    UserId = userId,
                    PaymentMethod = paymentMethod,
                    TotalAmount = totalAmount
                });
            }
        }

        public async Task<IEnumerable<object>> GetPurchaseHistoryAsync(int userId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT Id AS OrderId, PaymentMethod, TotalAmount, OrderDate FROM Orders WHERE UserId = @UserId ORDER BY OrderDate DESC";
                return await connection.QueryAsync<object>(sql, new { UserId = userId });
            }
        }
    }
}
