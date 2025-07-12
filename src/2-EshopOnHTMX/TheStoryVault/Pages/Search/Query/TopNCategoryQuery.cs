using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Search.Query;

public class TopNCathegoryQuery : CategoryQuery
{
    public TopNCathegoryQuery(int topN)
    {
        this.Take = topN;
    }

    protected override IQueryable<BookCategory> Filter(IQueryable<BookCategory> query)
    {
        return query;
    }

    protected override IOrderedQueryable<BookCategory>? Order(IQueryable<BookCategory> query)
    {
        return query.OrderByDescending(c => c.Books.Count())
            .ThenBy(c => c.Id);
    }
}
