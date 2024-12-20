using Microsoft.AspNetCore.Mvc;
using ONLINE_BOOKSTORE.Data;
using ONLINE_BOOKSTORE.Models;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookRepository _bookRepository;

    public BooksController()
    {
        _bookRepository = new BookRepository();
    }

    // GET: api/books
    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _bookRepository.GetBooksAsync();
        return Ok(books);
    }

    // POST: api/books
    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        await _bookRepository.AddBookAsync(book);
        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
    }

    // POST: api/books/checkout
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (request.TotalAmount <= 0 || string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            return BadRequest("Invalid checkout details.");
        }

        var orderId = await _bookRepository.CheckoutAsync(request.UserId, request.PaymentMethod, request.TotalAmount);
        return Ok(new { OrderId = orderId, Message = "Checkout successful!" });
    }

    // GET: api/books/purchase-history/{userId}
    [HttpGet("purchase-history/{userId}")]
    public async Task<IActionResult> GetPurchaseHistory(int userId)
    {
        var purchaseHistory = await _bookRepository.GetPurchaseHistoryAsync(userId);
        if (purchaseHistory == null || !purchaseHistory.Any())
        {
            return NotFound("No purchase history found.");
        }

        return Ok(purchaseHistory);
    }
}

// Model for Checkout Request
public class CheckoutRequest
{
    public int UserId { get; set; }
    public string PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
}
