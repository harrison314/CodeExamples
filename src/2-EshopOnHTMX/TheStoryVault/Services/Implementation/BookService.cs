using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Implementation;

public class BookService : IBookService
{
    private readonly StoryVaultContext storyVaultContext;

    public BookService(StoryVaultContext storyVaultContext)
    {
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<BookData> GetBookByIdAsync(int id, CancellationToken cancellationToken)
    {
        Book? book = await this.storyVaultContext.Books
            .Include(b => b.Author)
            .Where(t => t.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        CategoryInfo[] categories = await this.storyVaultContext.Books
            .Where(b => b.Id == id)
            .SelectMany(c => c.Categories)
            .Select(c => new CategoryInfo(c.Id, c.Name))
            .ToArrayAsync(cancellationToken);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found.");
        }

        return new BookData(
            Id: book.Id,
            Title: book.Title,
            Author: book.Author.Name,
            AuthorId: book.AuthorId,
            Description: book.Description,
            CoverImageUrl: book.CoverImageUrl,
            PublishedDate: new DateTime(book.PublishYear, 1, 1),
            Categories: categories,
            PageCount: 400,
            Language: "English",
            ISBN: book.Isbn,
            Publisher: book.Publisher,
            BookType: book.BookType,
            Price: (decimal)book.Price
            );
    }

    public async Task<BookRecord> GetBookInfoByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await this.storyVaultContext.Books
           .Where(b => b.Id == id)
           .Select(b => new BookRecord(
                b.Id,
               b.Title,
                b.Author.Name,
               (double)b.Price,
                8,
                b.CoverImageUrl,
                b.BookType))
           .SingleAsync(cancellationToken);
    }

    public async Task<decimal> GetPrice(IEnumerable<Contracts.BasketItem> books, CancellationToken cancellationToken)
    {
        int[] idsArray = books.Select(t => t.Id).ToArray();

        Dictionary<int, decimal> dictionary = await this.storyVaultContext.Books
            .Where(b => idsArray.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, b => b.Price, cancellationToken);

        return books.Sum(b => dictionary[b.Id] * b.Count);
    }

    public async Task<BookReviews> GetReviews(int id, string? userId, CancellationToken cancellationToken)
    {
        Dictionary<int, int> starsCounts = await this.storyVaultContext.BookReviews.Where(t => t.BookId == id && t.Stars > 0)
              .GroupBy(t => t.Stars)
              .ToDictionaryAsync(t => t.Key, t => t.Count());

        int? currentUserStars = null;

        if (!string.IsNullOrEmpty(userId))
        {
            currentUserStars = await this.storyVaultContext.BookReviews.Where(t => t.BookId == id && t.Stars > 0 && t.UserId == userId)
                .Select(t => t.Stars)
                .SingleOrDefaultAsync(cancellationToken);

        }

        return new BookReviews(starsCounts, currentUserStars);
    }

    public async Task SetReview(string userId, int id, int value, CancellationToken cancellationToken)
    {
        BookReview? review = await this.storyVaultContext.BookReviews.Where(t => t.BookId == id && t.UserId == userId)
              .SingleOrDefaultAsync(cancellationToken);

        if (review == null)
        {
            review = new BookReview()
            {
                BookId = id,
                Stars = value,
                UserId = userId,
                AddStars = DateTimeOffset.UtcNow
            };

            await this.storyVaultContext.AddAsync(review);
        }
        else
        {
            if (review.Stars != value)
            {
                review.Stars = value;
                review.AddStars = DateTimeOffset.UtcNow;
            }
        }

        await this.storyVaultContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetTextReview(string userId, int id, string title, string text, CancellationToken cancellationToken)
    {
        BookReview? review = await this.storyVaultContext.BookReviews.Where(t => t.BookId == id && t.UserId == userId)
              .SingleOrDefaultAsync(cancellationToken);

        if (review == null)
        {
            review = new BookReview()
            {
                BookId = id,
                UserId = userId,
                Title = title.Trim(),
                Text = text.Trim(),
                AddComment = DateTimeOffset.UtcNow
            };

            await this.storyVaultContext.AddAsync(review);
        }
        else
        {
            review.Title = title.Trim();
            review.Text = text.Trim();
            review.AddComment = DateTimeOffset.UtcNow;
        }

        await this.storyVaultContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<TextReview>> GetTextReviews(int id, CancellationToken cancellationToken)
    {
        return await this.storyVaultContext.BookReviews
            .Where(t => t.BookId == id && t.AddComment != null)
            .OrderByDescending(t => t.AddComment)
            .Select(t => new TextReview()
            {
                ReviewAt = t.AddComment!.Value,
                Stars = t.Stars,
                Text = t.Text!,
                Title = t.Title!,
                UserId = t.UserId
            })
            .ToListAsync(cancellationToken);
    }
}
