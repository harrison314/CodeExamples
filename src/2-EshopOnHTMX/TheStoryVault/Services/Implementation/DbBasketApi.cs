using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Services.Implementation;

public class DbBasketApi : IBasketApi
{
    private readonly ICookieApi cookieApi;
    private readonly StoryVaultContext storyVaultContext;

    public DbBasketApi(ICookieApi cookieApi, StoryVaultContext storyVaultContext)
    {
        this.cookieApi = cookieApi;
        this.storyVaultContext = storyVaultContext;
    }

    public async Task AddItem(int itemId, int count, CancellationToken cancellationToken)
    {
        Guid? basketIdOptional = this.cookieApi.GetBasketIdentifier();
        Guid basketId;
        if (basketIdOptional.HasValue)
        {
            basketId = basketIdOptional.Value;
        }
        else
        {
            basketId = Guid.CreateVersion7();
            this.cookieApi.SetBasketIdentifier(basketId);
        }

        TheStoryVault.Services.Data.BasketItem? item = await this.storyVaultContext.BasketItems.Where(t => t.BasketId == basketId && t.ItemId == itemId)
            .SingleOrDefaultAsync(cancellationToken);

        if (item == null)
        {
            item = new TheStoryVault.Services.Data.BasketItem()
            {
                ItemId = itemId,
                BasketId = basketId,
                Count = count,
                CreatedAt = DateTime.UtcNow,
            };

            await this.storyVaultContext.BasketItems.AddAsync(item, cancellationToken);
        }
        else
        {
            if (item.Count + count <= 0)
            {
                this.storyVaultContext.BasketItems.Remove(item);
            }
            else
            {
                item.Count += count;
            }
        }

        await this.storyVaultContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearBasket(CancellationToken cancellationToken)
    {
        Guid? basketIdOptional = this.cookieApi.GetBasketIdentifier();
        if (basketIdOptional.HasValue)
        {
            await this.storyVaultContext.BasketItems.Where(t => t.BasketId == basketIdOptional)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<TheStoryVault.Services.Contracts.BasketItem>> GetBasketItems(CancellationToken cancellationToken)
    {
        Guid? basketIdOptional = this.cookieApi.GetBasketIdentifier();
        if (basketIdOptional.HasValue)
        {
            var list = await this.storyVaultContext.BasketItems.Where(t => t.BasketId == basketIdOptional)
                  .OrderBy(t => t.CreatedAt)
                  .Select(t => new { t.ItemId, t.Count })
                  .ToListAsync(cancellationToken);

            return list.Select(t => new TheStoryVault.Services.Contracts.BasketItem(t.ItemId, t.Count)).ToList();
        }

        return new List<TheStoryVault.Services.Contracts.BasketItem>();
    }

    public async Task<int> GetCountItems(CancellationToken cancellationToken)
    {
        Guid? basketIdOptional = this.cookieApi.GetBasketIdentifier();
        if (basketIdOptional.HasValue)
        {
            return await this.storyVaultContext.BasketItems.Where(t => t.BasketId == basketIdOptional)
                  .SumAsync(t => t.Count, cancellationToken);
        }

        return 0;
    }
}