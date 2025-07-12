namespace TheStoryVault.Services.Contracts;

public interface IBookService
{
    Task<BookData> GetBookByIdAsync(int id, CancellationToken cancellationToken);

    Task<BookRecord> GetBookInfoByIdAsync(int id, CancellationToken cancellationToken);

    Task<decimal> GetPrice(IEnumerable<BasketItem> books, CancellationToken cancellationToken);

    Task<BookReviews> GetReviews(int id, string? userId, CancellationToken cancellationToken);

    Task<IList<TextReview>> GetTextReviews(int id, CancellationToken cancellationToken);

    Task SetReview(string userId, int id, int value, CancellationToken cancellationToken);

    Task SetTextReview(string userId, int id, string title, string text, CancellationToken cancellationToken);
}

public enum BookType
{
    Paperback,
    Ebook,
    Both
}

public record BookData
    (
    int Id,
    string Title,
    string Author,
    int AuthorId,
    string Description,
    string CoverImageUrl,
    DateTime PublishedDate,
    CategoryInfo[] Categories,
    int PageCount,
    string Language,
    string ISBN,
    string Publisher,
     BookType BookType,
     decimal Price
);

public record BookRecord
    (
    int Id,
    string Title,
    string Author,
    double Price,
    int Rating,
    string CoverImageUrl,
    BookType BookType
);

public abstract class BookRecordQuery : QueryObject<Data.Book>
{

}

public class BookReviews
{
    public int NumberOf1Stars { get; init; }
    public int NumberOf2Stars { get; init; }
    public int NumberOf3Stars { get; init; }
    public int NumberOf4Stars { get; init; }
    public int NumberOf5Stars { get; init; }

    public int? CurrentUserReview { get; init; }


    public int ReviewsCount
    {
        get => this.NumberOf1Stars + this.NumberOf2Stars + this.NumberOf3Stars + this.NumberOf4Stars + this.NumberOf5Stars;
    }

    public double AvgValue
    {
        get => Math.Round((this.NumberOf1Stars * 1.0 + this.NumberOf2Stars * 2.0 + this.NumberOf3Stars * 3.0 + this.NumberOf4Stars * 4.0 + this.NumberOf5Stars * 5.0)
            / (this.NumberOf1Stars + this.NumberOf2Stars + this.NumberOf3Stars + this.NumberOf4Stars + this.NumberOf5Stars),
            2);
    }

    public BookReviews()
    {

    }

    public BookReviews(IDictionary<int, int> dictionary, int? currentUserStars)
    {
        int value;

        value = 0;
        dictionary.TryGetValue(1, out value);
        this.NumberOf1Stars = value;

        value = 0;
        dictionary.TryGetValue(2, out value);
        this.NumberOf2Stars = value;

        value = 0;
        dictionary.TryGetValue(3, out value);
        this.NumberOf3Stars = value;

        value = 0;
        dictionary.TryGetValue(4, out value);
        this.NumberOf4Stars = value;

        value = 0;
        dictionary.TryGetValue(5, out value);
        this.NumberOf5Stars = value;

        this.CurrentUserReview = currentUserStars;
    }
}

public class TextReview
{
    public string UserId { get; init; }
    public string Title { get; init; }
    public string Text { get; init; }
    public DateTimeOffset ReviewAt { get; init; }

    public int Stars { get; init; }

    public TextReview()
    {
        this.UserId = string.Empty;
        this.Title = string.Empty;
        this.Text = string.Empty;
    }
}