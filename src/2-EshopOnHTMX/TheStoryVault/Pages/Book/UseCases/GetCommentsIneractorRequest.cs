namespace TheStoryVault.Pages.Book.UseCases;

public record GetCommentsIneractorRequest(int BookId, int PageNumber, int PageSize);
