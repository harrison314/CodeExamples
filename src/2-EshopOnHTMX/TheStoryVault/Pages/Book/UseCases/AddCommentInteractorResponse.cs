using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public record AddCommentInteractorResponse(IList<TextReview> Reviews, List<KeyValuePair<string, string>> Errors);
