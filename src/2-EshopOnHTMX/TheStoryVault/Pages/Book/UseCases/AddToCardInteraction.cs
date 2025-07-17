using CaseR;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Implementation;

namespace TheStoryVault.Pages.Book.UseCases;
public class AddToCardInteraction : IUseCaseInteractor<AddToCardInteractionRequest, AddToCardInteractionResponse>
{
    private readonly IBasketApi baskedApi;
    private readonly IBookService bookService;
    private readonly IDomainEventPublisher domainEventPublisher;

    public AddToCardInteraction(IBasketApi baskedApi,
            IBookService bookService,
            IDomainEventPublisher domainEventPublisher)
    {
        this.baskedApi = baskedApi;
        this.bookService = bookService;
        this.domainEventPublisher = domainEventPublisher;
    }

    public async Task<AddToCardInteractionResponse> Execute(AddToCardInteractionRequest request, CancellationToken cancellationToken)
    {
        BookRecord book = await this.bookService.GetBookInfoByIdAsync(request.ItemId, cancellationToken);

        await this.domainEventPublisher.Publish(new BookAddToCardDomainEvent(request.ItemId), cancellationToken);

        await this.baskedApi.AddItem(request.ItemId, 1, cancellationToken);

        IReadOnlyList<BasketItem> items = await this.baskedApi.GetBasketItems(cancellationToken);
        decimal price = await this.bookService.GetPrice(items, cancellationToken);

        return new AddToCardInteractionResponse(book, price);
    }
}
