using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TheStoryVault.Services.Data;

public class BookCategory
{
    public int Id
    {
        get;
        set;
    }
    public string Name
    {
        get;
        set;
    }
    public string Description
    {
        get;
        set;
    }

    public ICollection<Book> Books
    {
        get;
        set;
    } = new List<Book>();

    public BookCategory()
    {
        this.Name = string.Empty;
        this.Description = string.Empty;
    }

    public BookCategory(int id, string name, string description)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
    }
}

internal static class BookCategorySeeder
{
    public static void Seed(EntityTypeBuilder<BookCategory> entityBuilder)
    {
        entityBuilder.HasData(
            new BookCategory(1, "Fiction", "Fiction – novels, short stories, literary works"),
            new BookCategory(2, "Non-Fiction", "Non-Fiction – biographies, history, self-help, science"),
            new BookCategory(3, "Fantasy", "Fantasy – magical worlds, mythical creatures, epic adventures"),
            new BookCategory(4, "Science Fiction", "Science Fiction – futuristic stories, space exploration, AI themes"),
            new BookCategory(5, "Mystery & Thriller", "Mystery & Thriller – detective stories, suspense, crime novels"),
            new BookCategory(6, "Romance", "Romance – love stories, emotional journeys"),
            new BookCategory(7, "Horror", "Horror – chilling tales, supernatural creatures, psychological terror"),
            new BookCategory(8, "Children’s Books", "Children’s Books – picture books, young readers, fairy tales"),
            new BookCategory(9, "Young Adult(YA)", "Young Adult(YA) – teen fiction, adventure, coming-of-age stories"),
            new BookCategory(10, "Historical Fiction", "Historical Fiction – stories set in past eras, historical figures"),
            new BookCategory(11, "Poetry", "Poetry – collections of poems, spoken word, classic verse"),
            new BookCategory(12, "Graphic Novels & Comics", "Graphic Novels & Comics – illustrated stories, superheroes, manga"),
            new BookCategory(13, "Self-Help & Personal Development", "Self-Help & Personal Development – motivation, mental health, success"),
            new BookCategory(14, "Business & Economics", "Business & Economics – finance, leadership, entrepreneurship"),
            new BookCategory(15, "Science & Nature", "Science & Nature – physics, biology, environmental topics"),
            new BookCategory(16, "Philosophy & Psychology", "Philosophy & Psychology – deep thinking, human mind, existential themes"),
            new BookCategory(17, "Religion & Spirituality", "Religion & Spirituality – sacred texts, beliefs, mindfulness"),
            new BookCategory(18, "Travel & Adventure", "Travel & Adventure – guidebooks, travelogues, explorations"),
            new BookCategory(19, "Cookbooks & Food", "Cookbooks & Food – recipes, gastronomy, food culture"),
            new BookCategory(20, "Art & Design", "Art & Design – visual arts, architecture, creativity")
        );
    }
}