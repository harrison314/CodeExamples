using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class BookAddToCardDomainHandler : IDomainEventHandler<BookAddToCardDomainEvent>
{
    private readonly ITrackingService trackingService;

    public BookAddToCardDomainHandler(ITrackingService trackingService)
    {
        this.trackingService = trackingService;
    }

    public async Task Handle(BookAddToCardDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await this.trackingService.TrackBook(domainEvent.BookId, InteractionType.AddToCard);
    }
}