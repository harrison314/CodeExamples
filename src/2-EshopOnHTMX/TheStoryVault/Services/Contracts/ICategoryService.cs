using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Contracts;

public interface ICategoryService
{
    Task<CategoryData> GetById(int id, CancellationToken cancellationToken = default);
}

public record CategoryInfo(int Id, string Name);

public record CategoryData(int Id, string Name, string Decription);

public abstract class CategoryQuery : QueryObject<Data.BookCategory>
{
}

public class GetAllCategoryQuery: CategoryQuery
{
    public GetAllCategoryQuery()
    {
        
    }

    protected override IQueryable<BookCategory> Filter(IQueryable<BookCategory> query)
    {
        return query;
    }

    protected override IOrderedQueryable<BookCategory>? Order(IQueryable<BookCategory> query)
    {
        return query.OrderBy(c => c.Id);
    }
}