using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Cart.UseCases;

public class ClearCartInteractor : IUseCaseInteractor<Unit, Unit>
{
    private readonly IBasketApi basketApi;

    public ClearCartInteractor(IBasketApi basketApi)
    {
        this.basketApi = basketApi;
    }

    public async Task<Unit> Execute(Unit request, CancellationToken cancellationToken)
    {
        await this.basketApi.ClearBasket(cancellationToken);
        return Unit.Value;
    }
}
