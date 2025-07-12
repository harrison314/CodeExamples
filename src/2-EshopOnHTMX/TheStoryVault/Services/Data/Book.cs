
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Services.Data;

public class Book : ICreated
{
    public int Id
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    } = string.Empty;

    public string Description
    {
        get;
        set;
    } = string.Empty;

    public string Isbn
    {
        get;
        set;
    } = string.Empty;
    public int PublishYear
    {
        get;
        set;
    }

    public int AuthorId
    {
        get;
        set;
    }

    public Author Author
    {
        get;
        set;
    } = default!;

    public ICollection<BookCategory> Categories
    {
        get;
        set;
    } = new List<BookCategory>();

    public string Publisher
    {
        get;
        set;
    } = string.Empty;
    public string CoverImageUrl
    {
        get;
        set;
    } = string.Empty;

    public decimal Price
    {
        get;
        set;
    }

    public BookType BookType
    {
        get;
        set;
    }

    public DateTimeOffset CreatedAt
    {
        get;
        set;
    }

    public ICollection<BookReview> Reviews
    {
        get;
        set;
    } = new List<BookReview>();

    public string SearchText
    {
        get;
        set;
    } = string.Empty;

    public ICollection<BookInteraction> Interactions
    {
        get;
        set;
    } = new List<BookInteraction>();

    public Book()
    {
        
    }
}
