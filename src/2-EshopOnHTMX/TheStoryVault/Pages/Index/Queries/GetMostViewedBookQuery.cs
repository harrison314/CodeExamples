using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index.Queries;

public class GetMostViewedBookQuery : BookRecordQuery
{
    private readonly BookType bookType;

    public GetMostViewedBookQuery(BookType bookType)
    {
        this.bookType = bookType;
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return this.bookType switch
        {
            BookType.Paperback => query.Where(b => b.BookType == BookType.Paperback),
            BookType.Ebook => query.Where(b => b.BookType == BookType.Ebook),
            _ => query
        };
    }

    protected override IOrderedQueryable<Services.Data.Book>? Order(IQueryable<Services.Data.Book> query)
    {
        return query.OrderByDescending(b => b.Interactions.Sum(t => t.Weight));
    }
}
