using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index.Queries;

public class NewsEbookQuery : BookRecordQuery
{
    public NewsEbookQuery()
    {
        this.Take = 10;
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return query.Where(b => b.Author.Name == "Adrian Tchaikovsky");
    }

    protected override IOrderedQueryable<Services.Data.Book>? Order(IQueryable<Services.Data.Book> query)
    {
        return query.OrderByDescending(b => b.PublishYear)
            .ThenByDescending(b => b.Id);
    }
}
