using System.Linq.Expressions;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Search.Query;

public class SearchQuery : BookRecordQuery
{
    private readonly string queryString;
    private readonly int[]? categories;

    public SearchQuery(string queryString, int[]? categories)
    {
        this.queryString = queryString;
        this.categories = categories;
    }

    protected override IQueryable<Services.Data.Book> Filter(IQueryable<Services.Data.Book> query)
    {
        if (this.categories != null && this.categories.Length > 0)
        {
            query = query.Where(b => b.Categories.Any(c => this.categories.Contains(c.Id)));
        }

        if (!string.IsNullOrWhiteSpace(this.queryString))
        {
            query = this.FilterByQueryString(query);
        }

        return query;
    }

    private IQueryable<Services.Data.Book> FilterByQueryString(IQueryable<Services.Data.Book> query)
    {
        string[] words = this.queryString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Expression<Func<Services.Data.Book, bool>>? combinedExpression = null;
        foreach (string word in words)
        {
            string trimmedWord = word.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(trimmedWord))
            {
                if (combinedExpression == null)
                {
                    combinedExpression = b => b.SearchText.Contains(trimmedWord);
                }
                else
                {
                    combinedExpression = CombineOr(combinedExpression, b => b.SearchText.Contains(trimmedWord));
                }
            }
        }

        if (combinedExpression != null)
        {
            query = query.Where(combinedExpression);
        }

        return query;
    }

    static Expression<Func<T, bool>> CombineOr<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "t");
        BinaryExpression combined = Expression.OrElse(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter)
        );
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}
