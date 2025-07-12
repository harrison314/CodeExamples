using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index.Queries;

public class InterestingBookQuery : BookRecordQuery
{
    public InterestingBookQuery()
    {
        this.Take = 10;
        this.Skip = 20;
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return query;
    }

    protected override IOrderedQueryable<Services.Data.Book>? Order(IQueryable<Services.Data.Book> query)
    {
        return query.OrderByDescending(b => EF.Functions.Random());
    }
}