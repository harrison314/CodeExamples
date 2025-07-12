using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Implementation;

public class CategoryService : ICategoryService
{
    private readonly StoryVaultContext storyVaultContext;

    public CategoryService(StoryVaultContext storyVaultContext)
    {
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<CategoryData> GetById(int id, CancellationToken cancellationToken = default)
    {
        CategoryData? category = await this.storyVaultContext.BookCategories
            .Where(c => c.Id == id)
            .Select(c => new CategoryData(c.Id, c.Name, c.Description))
            .FirstOrDefaultAsync(cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }
        return category;
    }
}
