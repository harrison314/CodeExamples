using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Implementation;

public class AuthorService : IAuthorService
{
    private readonly StoryVaultContext storyVaultContext;

    public AuthorService(StoryVaultContext storyVaultContext)
    {
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<AuthorData> GetAuthorByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await this.storyVaultContext.Authors
            .Where(a => a.Id == id)
            .Select(a => new AuthorData(
                a.Id,
                a.Name,
                a.Description))
            .SingleAsync(cancellationToken);
    }
}
