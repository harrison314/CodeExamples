using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class BookHasShowEventHandler : IDomainEventHandler<BookHasShowEvent>
{
    private readonly ITrackingService trackingService;

    public BookHasShowEventHandler(ITrackingService trackingService)
    {
        this.trackingService = trackingService;
    }

    public async Task Handle(BookHasShowEvent domainEvent, CancellationToken cancellationToken)
    {
        await this.trackingService.TrackBook(domainEvent.BookId, InteractionType.Visit);
    }
}