namespace TheStoryVault.Services.Contracts;

public interface ICookieApi
{
    Guid? SetTrackingIdentifier(bool isAnonym);

    TrackingIdentifierResult GetTrackingIdentifier();

    void SetBasketIdentifier(Guid basketId);

    Guid? GetBasketIdentifier();
}

public enum TrackingIdentifierState
{
    None,
    Disabled,
    Enabled
}

public record struct TrackingIdentifierResult(TrackingIdentifierState State, string Identifier);