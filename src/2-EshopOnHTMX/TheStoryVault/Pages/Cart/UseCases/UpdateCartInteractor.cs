using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Cart.UseCases;

public class UpdateCartInteractor : IUseCaseInteractor<UpdateCartInteractorRequest, BookCartTotal>
{
    private readonly IBasketApi basketApi;
    private readonly IUseCase<GetCartInteractor> getCartInteractor;

    public UpdateCartInteractor(IBasketApi basketApi,
          [FromKeyedServices(UseCaseRelation.Include)] IUseCase<GetCartInteractor> getCartInteractor)
    {
        this.basketApi = basketApi;
        this.getCartInteractor = getCartInteractor;
    }

    public async Task<BookCartTotal> Execute(UpdateCartInteractorRequest request, CancellationToken cancellationToken)
    {
        await this.basketApi.AddItem(request.ItemId,
            request.CanAdd ? 1 : -1,
            cancellationToken);

        return await this.getCartInteractor.Execute(Unit.Value, cancellationToken);
    }
}
