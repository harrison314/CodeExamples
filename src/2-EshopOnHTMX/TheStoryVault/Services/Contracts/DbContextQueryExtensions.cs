using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Contracts;

public static class DbContextQueryExtensions
{
    public static async Task<IList<BookRecord>> GetBooksInfos(this StoryVaultContext dbContext,
        BookRecordQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Book> booksQuery = query.Apply(dbContext.Books);

        return await booksQuery.Select(b => new BookRecord(
                b.Id,
                b.Title,
                b.Author.Name,
                (double)b.Price,
                8,
                b.CoverImageUrl,
                b.BookType))
            .ToListAsync(cancellationToken);
    }

    public static async Task<IList<CategoryInfo>> GetCategories(this StoryVaultContext dbContext,
        CategoryQuery query,
        CancellationToken cancellationToken)
    {
        return await query.Apply(dbContext.BookCategories)
            .Select(c => new CategoryInfo(c.Id, c.Name))
            .ToListAsync(cancellationToken);
    }
}
