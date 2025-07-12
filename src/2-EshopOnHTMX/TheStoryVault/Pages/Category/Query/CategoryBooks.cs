using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Category.Query;

public class CategoryBooks : BookRecordQuery
{
    private readonly int categoryId;

    public CategoryBooks(int categoryId, int page)
    {
        this.categoryId = categoryId;
        this.Take = 20;
        this.Skip = (page - 1) * this.Take;
    }

    protected override IOrderedQueryable<Services.Data.Book>? Order(IQueryable<Services.Data.Book> query)
    {
        return query.OrderByDescending(b => b.PublishYear)
            .ThenByDescending(b => b.Id);
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        return query.Where(b => b.Categories.Any(t => t.Id == this.categoryId));
    }
}
