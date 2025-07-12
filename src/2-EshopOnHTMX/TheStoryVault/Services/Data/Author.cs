namespace TheStoryVault.Services.Data;

public class Author : ICreated
{
    public int Id
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public DateTimeOffset CreatedAt
    {
        get;
        set;
    }

    public ICollection<Book> Books
    {
        get;
        set;
    } = new List<Book>();

    public Author()
    {
        this.Name = string.Empty;
        this.Description = string.Empty;
    }
}
