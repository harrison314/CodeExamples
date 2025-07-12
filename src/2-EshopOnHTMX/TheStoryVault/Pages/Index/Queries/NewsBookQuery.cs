using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index.Queries;

public class NewsBookQuery : BookRecordQuery
{
    public NewsBookQuery()
    {
        this.Take = 10;
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return query;
    }

    protected override IOrderedQueryable<Services.Data.Book>? Order(IQueryable<Services.Data.Book> query)
    {
        return query.OrderByDescending(b => b.Id);
    }
}
