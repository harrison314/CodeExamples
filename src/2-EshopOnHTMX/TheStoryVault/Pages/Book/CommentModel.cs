namespace TheStoryVault.Pages.Book;

public class CommentModel
{
    public string Title
    {
        get;
        set;
    }

    public string Review
    {
        get;
        set;
    }

    public CommentModel()
    {
        this.Title = string.Empty;
        this.Review = string.Empty;
    }
}
