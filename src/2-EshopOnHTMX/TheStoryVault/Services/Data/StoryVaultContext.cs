using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TheStoryVault.Services.Data;

public class StoryVaultContext : DbContext
{
    public DbSet<Author> Authors
    {
        get;
        set;
    }

    public DbSet<BookCategory> BookCategories
    {
        get;
        set;
    }

    public DbSet<Book> Books
    {
        get;
        set;
    }

    public DbSet<BookReview> BookReviews
    {
        get;
        set;
    }

    public DbSet<BasketItem> BasketItems
    {
        get;
        set;
    }

    public DbSet<BookInteraction> BookInteraction
    {
        get;
        set;
    }

    public StoryVaultContext(DbContextOptions<StoryVaultContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(1024);
            entity.Property(a => a.Description);
            entity.Property(a => a.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(1024);
            entity.Property(a => a.Description);

            BookCategorySeeder.Seed(entity);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(1024);
            entity.Property(b => b.Description);
            entity.Property(b => b.CreatedAt).IsRequired();
            entity.Property(b => b.AuthorId).IsRequired();
            entity.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);
            entity.HasMany(b => b.Categories)
                .WithMany(c => c.Books)
                .UsingEntity("CategoryBook");

            entity.HasIndex(b => b.PublishYear);
            entity.HasIndex(b => b.AuthorId);
        });

        modelBuilder.Entity<BookReview>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).HasMaxLength(1024);
            entity.HasOne<Book>(t => t.Book)
                .WithMany(t => t.Reviews)
                .HasForeignKey(t => t.BookId);

            entity.HasIndex(b => b.BookId);
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(b => b.BasketId);
        });

        modelBuilder.Entity<BookInteraction>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(t => t.UserId).IsRequired().HasMaxLength(64);
            entity.HasOne(t => t.Book)
                .WithMany(t => t.Interactions)
                .HasForeignKey(t => t.BookId);

            entity.HasIndex(b => b.BookId);
            entity.HasIndex(b => b.UserId);
        });


        if (this.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                IEnumerable<System.Reflection.PropertyInfo> properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                               || p.PropertyType == typeof(DateTimeOffset?));

                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }
    }
}
