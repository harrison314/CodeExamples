using CaseR;

namespace TheStoryVault.Pages.Book.UseCases;

public record BookAddToCardDomainEvent(int BookId) : IDomainEvent;
