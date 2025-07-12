using CaseR;
using System.Collections.Frozen;
using TheStoryVault.Pages.Cart.Queries;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Cart.UseCases;

public class GetCartInteractor : IUseCaseInteractor<Unit, BookCartTotal>
{
    private readonly IBasketApi basketApi;
    private readonly StoryVaultContext storyVaultContext;

    public GetCartInteractor(IBasketApi basketApi,
            StoryVaultContext storyVaultContext)
    {
        this.basketApi = basketApi;
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<BookCartTotal> Execute(Unit request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Services.Contracts.BasketItem> items = await this.basketApi.GetBasketItems(cancellationToken);

        GetBooksByIdsQuery query = new GetBooksByIdsQuery(items.Select(x => x.Id));
        IList<BookRecord> books = await this.storyVaultContext.GetBooksInfos(query, cancellationToken);
        FrozenDictionary<int, BookRecord> bookLookup = books.ToFrozenDictionary(t => t.Id);

        BookCartInfo[] bookCartInfos = new BookCartInfo[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            bookCartInfos[i] = new BookCartInfo(bookLookup[items[i].Id], items[i].Count);
        }

        return new BookCartTotal(bookCartInfos, (decimal)bookCartInfos.Sum(t => t.Count * t.Book.Price));
    }
}
