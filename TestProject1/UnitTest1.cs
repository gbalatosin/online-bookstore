using Xunit;

public class BookRepositoryTests
{
    [Fact]
    public async Task GetBooksAsync_ReturnsBooks()
    {
        var repository = new BookRepository();
        var books = await repository.GetBooksAsync();
        Assert.NotNull(books);
    }
}