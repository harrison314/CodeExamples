using Azure.Core;
using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;
public class AddCommentInteractor : IUseCaseInteractor<AddCommentInteractorRequest, AddCommentInteractorResponse>
{
    private readonly IBookService bookService;
    private readonly ICookieApi cookieApi;

    public AddCommentInteractor(IBookService bookService,
            ICookieApi cookieApi)
    {
        this.bookService = bookService;
        this.cookieApi = cookieApi;
    }

    public async Task<AddCommentInteractorResponse> Execute(AddCommentInteractorRequest request, CancellationToken cancellationToken)
    {
        List<KeyValuePair<string, string>> errors = new List<KeyValuePair<string, string>>();
        if (string.IsNullOrWhiteSpace(request.Model.Title))
        {
            errors.Add(new KeyValuePair<string, string>(nameof(request.Model.Title), "Title is required."));
        }

        if (request.Model.Title != null && request.Model.Title.Contains("abraka", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new KeyValuePair<string, string>(nameof(request.Model.Title), "Title contains abraka."));
        }

        if (string.IsNullOrWhiteSpace(request.Model.Review))
        {
            errors.Add(new KeyValuePair<string, string>(nameof(request.Model.Review), "Review is required."));
        }

        if (errors.Count == 0)
        {
            TrackingIdentifierResult tracingIdentofier = this.cookieApi.GetTrackingIdentifier();
            await this.bookService.SetTextReview(tracingIdentofier.Identifier,
                request.BookId,
                request.Model.Title!,
                request.Model.Review!,
                cancellationToken);
        }

        IList<TextReview> reviews = await this.bookService.GetTextReviews(request.BookId, cancellationToken);

        return new AddCommentInteractorResponse(reviews, errors);
    }
}
