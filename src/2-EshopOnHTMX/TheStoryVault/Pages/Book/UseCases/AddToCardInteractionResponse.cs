using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public record AddToCardInteractionResponse(BookRecord Book, decimal TotalPrice);
