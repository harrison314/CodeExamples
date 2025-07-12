using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Infrastructure.Migration;

internal static class Migrator
{
    public static async Task Migrate(IServiceProvider serviceProvider,
        string? csvPath)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        StoryVaultContext context = scope.ServiceProvider.GetRequiredService<StoryVaultContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        BookCategory[] categories = await context.BookCategories.ToArrayAsync();
        Bogus.DataSets.Lorem lorem = new Bogus.DataSets.Lorem(locale: "en");

        int i = 1;
        foreach ((string isbn, string title, string author, int yearOfPublication, string publisher, string coverImageUrl) ddd in GetData(csvPath))
        {
            i++;

            Author? author = await context.Authors.FirstOrDefaultAsync(a => a.Name == ddd.author);
            if (author == null)
            {
                author = new Author()
                {
                    Name = ddd.author,
                    Description = $"Author {ddd.author} description. "
                    + lorem.Paragraphs(2, 7, separator: "\n"),

                    CreatedAt = DateTimeOffset.UtcNow
                };
                context.Authors.Add(author);
            }

            int numberOdCategories = 1;
            if (Random.Shared.Next(0, 100) > 80)
            {
                numberOdCategories++;

                if (Random.Shared.Next(0, 75) > 80)
                {
                    numberOdCategories++;

                    if (Random.Shared.Next(0, 75) > 80)
                    {
                        numberOdCategories++;
                    }
                }
            }

            Book book = new Book()
            {
                Isbn = ddd.isbn,
                Title = ddd.title,
                Author = author,
                PublishYear = ddd.yearOfPublication,
                Publisher = ddd.publisher,
                CoverImageUrl = UpdateUrl(ddd.coverImageUrl),
                CreatedAt = DateTimeOffset.UtcNow,
                BookType = i % 5 < 3 ? Services.Contracts.BookType.Paperback : Services.Contracts.BookType.Ebook,
                Description = lorem.Paragraphs(3, 12, separator: "\n"),
                Price = (decimal)(Random.Shared.Next(15, 1500) / 10.0),
                Categories = Random.Shared.GetItems(categories, numberOdCategories),
                Reviews = CreateReviews(i < 10)
            };

            book.SearchText = $"{book.Title}\n{book.Author.Name}\n{book.Description}\n{book.Publisher}\n{book.Isbn}\n{book.Description}\n{book.BookType}\n{string.Join(' ',
              book.Categories.Select(t => t.Name))}".ToLowerInvariant();

            context.Books.Add(book);

            await context.SaveChangesAsync();

            if (i % 100 == 0)
            {
                int index = i;
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    Console.WriteLine($"Migrated book {index}: {book.Title} by {book.Author.Name} ({book.PublishYear}) - ISBN: {book.Isbn}");
                });
            }

            if (i > 1500)
            {
                return;
            }
        }

    }
    private static List<BookReview> CreateReviews(bool forceEnable)
    {
        List<BookReview> reviews = new List<BookReview>();
        Bogus.DataSets.Name name = new Bogus.DataSets.Name();
        Bogus.DataSets.Lorem lorem = new Bogus.DataSets.Lorem(locale: "en");

        int starsValues = Random.Shared.Next(forceEnable ? 0 : -5, forceEnable ? 250 : 35);
        if (starsValues > 0)
        {
            for (int i = 0; i < starsValues; i++)
            {
                reviews.Add(new BookReview()
                {
                    AddStars = DateTimeOffset.UtcNow,
                    Stars = Random.Shared.Next(1, 6),
                    UserId = name.FullName()
                });
            }
        }

        int commentCounts = Random.Shared.Next(forceEnable ? 0 : -20, 8);
        if (commentCounts > 0)
        {
            for (int i = 0; i < commentCounts; i++)
            {
                reviews.Add(new BookReview()
                {
                    AddComment = DateTimeOffset.Now.AddSeconds(Random.Shared.Next(-60 * 60 * 24 * 365, 0)),
                    Text = lorem.Sentences(Random.Shared.Next(1, 8), " "),
                    Title = string.Join(" ", lorem.Words(Random.Shared.Next(1, 4))),
                    UserId = name.FullName(),

                    AddStars = DateTimeOffset.UtcNow,
                    Stars = Random.Shared.Next(1, 6),
                });
            }
        }

        return reviews;
    }

    private static string UpdateUrl(string coverImageUrl)
    {
        return coverImageUrl.Replace("http://", "https://");
    }

    private static IEnumerable<(string isbn, string title, string author, int yearOfPublication, string publisher, string coverImageUrl)> GetData(string? path)
    {
        if(string.IsNullOrEmpty(path))
        {
            return MigratorData.GetData();
        }
        else
        {
            return ReadFile(path);
        }
    }

    private static IEnumerable<(string isbn, string title, string author, int yearOfPublication, string publisher, string coverImageUrl)> ReadFile(string path)
    {

        yield return ("9781447273301", "Children of time", "Adrian Tchaikovsky", 2016, "Pan Macmillan", "https://mrtns.sk/tovar/_xl/299/xl299333.jpg?v=17498738852");
        yield return ("720361", "Children of Ruin", "Adrian Tchaikovsky", 2020, "Pan Macmillan", "https://mrtns.sk/tovar/_xl/720/xl720361.jpg?v=17498738742");
        yield return ("9781529087192", "Children of Memory", "Adrian Tchaikovsky", 2023, "Pan Macmillan", "https://mrtns.sk/tovar/_xl/1919/xl1919053.jpg?v=17498738632");
        yield return ("9781035013807", "Shroud", "Adrian Tchaikovsky", 2025, "Pan Macmillan", "https://mrtns.sk/tovar/_xl/2859/xl2859277.jpg?v=17498738562");
        yield return ("9781529051902", "Shards of Earth ", "Adrian Tchaikovsky", 2022, "Tor", "https://mrtns.sk/tovar/_xl/1453/xl1453941.jpg?v=17498738662");
        yield return ("9781529051957", "Eyes of the Void", "Adrian Tchaikovsky", 2023, "Tor", "https://mrtns.sk/tovar/_xl/1889/xl1889479.jpg?v=17498738642");
        yield return ("9781529052008", "Lords of Uncreation", "Adrian Tchaikovsky", 2023, "Tor", "https://mrtns.sk/tovar/_xl/2418/xl2418271.jpg?v=17498738582");

        using StreamReader reader = new StreamReader(path);
        string? line;
        _ = reader.ReadLine();

        while ((line = reader.ReadLine()) != null)
        {
            //parse CSV line with quotes and commas 

            string[] parts = ParseCsvLine(line).ToArray();
            if (parts.Length < 8) continue; // Skip lines with insufficient data
            yield return (parts[0], parts[1], parts[2], int.Parse(parts[3]), parts[4], parts[6]);
        }
    }

    public static void BuildDataFile(string path, string outpath, int count)
    {
        using StreamWriter writer = new StreamWriter(outpath);

        foreach ((string isbn, string title, string author, int yearOfPublication, string publisher, string coverImageUrl) item in ReadFile(path).Take(count))
        {
            writer.WriteLine($"""
                yield return ("{item.isbn}", "{item.title}", "{item.author}", {item.yearOfPublication}, "{item.publisher}", "{item.coverImageUrl}");
                """);
        }

        writer.Flush();
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> values = new List<string>();
        bool insideQuotes = false;
        string currentValue = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && (insideQuotes || i > 0 && line[i - 1] == ','))
            {
                insideQuotes = !insideQuotes;
                continue;
            }

            if (c == ',' && !insideQuotes)
            {
                values.Add(currentValue.Trim());
                currentValue = "";
            }
            else
            {
                currentValue += c;
            }
        }

        values.Add(currentValue.Trim());
        return values;
    }
}
