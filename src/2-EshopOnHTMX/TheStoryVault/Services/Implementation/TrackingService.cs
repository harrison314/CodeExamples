using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Implementation;

public class TrackingService : ITrackingService
{
    private readonly ICookieApi cookieApi;
    private readonly StoryVaultContext storyVaultContext;

    public ICookieApi CookieApi { get => this.cookieApi; }

    public TrackingService(ICookieApi cookieApi, StoryVaultContext storyVaultContext)
    {
        this.cookieApi = cookieApi;
        this.storyVaultContext = storyVaultContext;
    }

    public async Task TrackBook(int bookId, InteractionType interactionType)
    {
        TrackingIdentifierResult interaction = this.cookieApi.GetTrackingIdentifier();

        this.storyVaultContext.BookInteraction.Add(new BookInteraction()
        {
            BookId = bookId,
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = interaction.Identifier,
            Weight = interactionType switch
            {
                InteractionType.Buy => 0.6f,
                InteractionType.Visit => 0.2f,
                InteractionType.AddToCard => 0.4f,
                _ => throw new NotImplementedException()
            }
        });

        await this.storyVaultContext.SaveChangesAsync();
    }
}
