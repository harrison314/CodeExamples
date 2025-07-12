namespace TheStoryVault.Services.Data;

public class BookInteraction
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

    public Book Book
    {
        get;
        set;
    } = default!;

    public string UserId
    {
        get;
        set;
    } = string.Empty;
    public DateTimeOffset CreatedAt
    {
        get;
        set;
    } = DateTimeOffset.UtcNow;

    public float Weight
    {
        get;
        set;
    }

    public BookInteraction()
    {
        
    }
}