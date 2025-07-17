namespace TheStoryVault.Services.Data;

public class BasketItem : ICreated
{
    public int Id
    {
        get;
        set;
    }
    public Guid BasketId
    {
        get;
        set;
    }

    public int ItemId
    {
        get;
        set;
    }

    public int Count
    {
        get;
        set;
    }

    public DateTimeOffset CreatedAt
    {
        get;
        set;
    }

    public BasketItem()
    {
        
    }
}