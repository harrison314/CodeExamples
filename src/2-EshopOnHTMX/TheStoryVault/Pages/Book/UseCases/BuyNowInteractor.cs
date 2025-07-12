using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class BuyNowInteractor : IUseCaseInteractor<int, Unit>
{
    private readonly IBasketApi baskedApi;
    private readonly IDomainEventPublisher domainEventPublisher;

    public BuyNowInteractor(IBasketApi baskedApi, IDomainEventPublisher domainEventPublisher)
    {
        this.baskedApi = baskedApi;
        this.domainEventPublisher = domainEventPublisher;
    }

    public async Task<Unit> Execute(int request, CancellationToken cancellationToken)
    {
        await this.baskedApi.AddItem(request, 1, cancellationToken);
        await this.domainEventPublisher.Publish(new BookAddToCardDomainEvent(request), cancellationToken);
        return Unit.Value;
    }
}
