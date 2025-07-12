using CaseR;

namespace TheStoryVault.Pages.Book.UseCases;

public record BookHasShowEvent(int BookId) : IDomainEvent;
