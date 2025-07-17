using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Cart.Queries;

public class GetBooksByIdsQuery : BookRecordQuery
{
    private readonly int[] ids;

    public GetBooksByIdsQuery(IEnumerable<int> ids)
    {
        this.ids = ids.ToArray();
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return query.Where(b => this.ids.Contains(b.Id));
    }
}
