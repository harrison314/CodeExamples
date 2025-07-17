namespace TheStoryVault.Services.Data;

public class BookReview
{
    public int Id
    {
        get;
        set;
    }

    public int BookId
    {
        get;
        set;
    }

    public string UserId
    {
        get;
        set;
    }

    public Book Book
    {
        get;
        set;
    } = default!;

    public int Stars
    {
        get;
        set;
    }

    public DateTimeOffset? AddStars
    {
        get;
        set;
    }

    public string? Title
    {
        get;
        set;
    }

    public string? Text
    {
        get;
        set;
    }

    public DateTimeOffset? AddComment
    {
        get;
        set;
    }

    public BookReview()
    {
        this.UserId = string.Empty;
    }
}