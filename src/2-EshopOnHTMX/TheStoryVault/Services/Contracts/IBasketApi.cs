using System.Threading.Tasks;

namespace TheStoryVault.Services.Contracts;

public interface IBasketApi
{
    Task<int> GetCountItems(CancellationToken cancellationToken);

    Task AddItem(int itemId, int count, CancellationToken cancellationToken);

    Task ClearBasket(CancellationToken cancellationToken);

    Task<IReadOnlyList<BasketItem>> GetBasketItems(CancellationToken cancellationToken);
}


public record BasketItem(int Id, int Count);