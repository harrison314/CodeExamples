using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Author.Queries;

public class AuthorBooksQuery : BookRecordQuery
{
    private readonly int authorId;

    public AuthorBooksQuery(int authorId)
    {
        this.authorId = authorId;
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return query.Where(b => b.AuthorId == this.authorId);
    }

    protected override IOrderedQueryable<Services.Data.Book>? Order(IQueryable<Services.Data.Book> query)
    {
        return query.OrderByDescending(b => b.PublishYear)
            .ThenByDescending(b => b.Id);
    }
}
