namespace TheStoryVault.Services.Contracts;

public interface ITrackingService
{
    ICookieApi CookieApi { get; }

    Task TrackBook(int bookId, InteractionType interactionType);
}

public enum InteractionType
{
    Visit,
    AddToCard,
    Buy
}
