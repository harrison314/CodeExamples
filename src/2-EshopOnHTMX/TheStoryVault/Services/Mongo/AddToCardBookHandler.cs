using CaseR;
using MongoDB.Driver;
using TheStoryVault.Pages.Book.UseCases;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Services.Mongo;

//internal class AddToCardBookHandler : IDomainEventHandler<BookAddToCardDomainEvent>
//{
//    private readonly IMongoDatabase mongoDatabase;
//    private readonly ICookieApi cookieApi;

//    public AddToCardBookHandler(IMongoDatabase mongoDatabase, ICookieApi cookieApi)
//    {
//        this.mongoDatabase = mongoDatabase;
//        this.cookieApi = cookieApi;
//    }
//    public async Task Handle(BookAddToCardDomainEvent domainEvent, CancellationToken cancellationToken)
//    {
//        TrackingIdentifierResult identofier = this.cookieApi.GetTrackingIdentifier();

//        await this.mongoDatabase.GetCollection<BookInteraction>(nameof(BookInteraction))
//            .InsertOneAsync(new BookInteraction()
//            {
//                UserId = identofier.Identifier,
//                BookId = domainEvent.BookId,
//                Weigth = 0.72,
//                InteractionType = "AddToCard",
//                Time = DateTime.UtcNow
//            }, cancellationToken: cancellationToken);
//    }
//}
