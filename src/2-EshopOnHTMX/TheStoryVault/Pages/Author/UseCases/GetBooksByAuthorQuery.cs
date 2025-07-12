namespace TheStoryVault.Pages.Author.UseCases;

public record GetBooksByAuthorQuery(int AuthorId, int Page, int PageSize);