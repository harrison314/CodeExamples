using CaseR;

namespace TheStoryVault.Pages.Search.UseCases;

public record SearchDomainEvent(string? Query, int[]? Categories) : IDomainEvent;