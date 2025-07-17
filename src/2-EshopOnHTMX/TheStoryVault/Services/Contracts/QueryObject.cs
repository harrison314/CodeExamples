using Microsoft.EntityFrameworkCore;

namespace TheStoryVault.Services.Contracts;

public abstract class QueryObject<TEntity>
{
    public int? Skip
    {
        get;
        set;
    }

    public int? Take
    {
        get;
        set;
    }

    protected QueryObject()
    {

    }

    protected abstract IQueryable<TEntity> Filter(IQueryable<TEntity> query);

    protected virtual IOrderedQueryable<TEntity>? Order(IQueryable<TEntity> query)
    {
        return null;
    }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
    {
        query = this.Filter(query);
        IOrderedQueryable<TEntity>? orderedQuery = this.Order(query);
        if (orderedQuery != null)
        {
            query = orderedQuery;
        }

        if (this.Skip.HasValue)
        {
            query = query.Skip(this.Skip.Value);
        }

        if (this.Take.HasValue)
        {
            query = query.Take(this.Take.Value);
        }

        return query.TagWith($"Query-{this.GetType().Namespace}.{this.GetType().Name}");
    }
}
