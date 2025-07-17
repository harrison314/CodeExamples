using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index.UseCases;

public record MostViewedBooksInteractorRequest(BookType BookType, int Count);
