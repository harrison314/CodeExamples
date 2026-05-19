using CaseR;
using MongoDB.Driver;
using TheStoryVault.Pages.Book.UseCases;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Services.Mongo;

internal class ShowBookHandler : IDomainEventHandler<BookHasShowEvent>
{
    private readonly IMongoDatabase mongoDatabase;
    private readonly ICookieApi cookieApi;

    public ShowBookHandler(IMongoDatabase mongoDatabase, ICookieApi cookieApi)
    {
        this.mongoDatabase = mongoDatabase;
        this.cookieApi = cookieApi;
    }
    public async Task Handle(BookHasShowEvent domainEvent, CancellationToken cancellationToken)
    {
        TrackingIdentifierResult identofier = this.cookieApi.GetTrackingIdentifier();

        await this.mongoDatabase.GetCollection<BookInteraction>(nameof(BookInteraction))
            .InsertOneAsync(new BookInteraction()
            {
                UserId = identofier.Identifier,
                BookId = domainEvent.BookId,
                Weigth = 0.3,
                InteractionType = "Visit",
                Time = DateTime.UtcNow
            }, cancellationToken: cancellationToken);
    }
}
