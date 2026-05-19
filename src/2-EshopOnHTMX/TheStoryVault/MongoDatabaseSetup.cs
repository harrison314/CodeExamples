namespace TheStoryVault;

public class MongoDatabaseSetup
{
    public string ConnectionString
    {
        get;
        set;
    }

    public string DatabaseName
    {
        get;
        set;
    }

    public MongoDatabaseSetup()
    {
        this.ConnectionString = string.Empty;
        this.DatabaseName = string.Empty;
    }
}
