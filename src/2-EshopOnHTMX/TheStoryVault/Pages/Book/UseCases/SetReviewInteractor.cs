using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class SetReviewInteractor : IUseCaseInteractor<SetReviewInteractorRequest, BookReviews>
{
    private readonly IBookService bookService;
    private readonly ITrackingService trackingService;

    public SetReviewInteractor(IBookService bookService,
            ITrackingService trackingService)
    {
        this.bookService = bookService;
        this.trackingService = trackingService;
    }

    public async Task<BookReviews> Execute(SetReviewInteractorRequest request, CancellationToken cancellationToken)
    {
        string userId = this.trackingService.CookieApi.GetTrackingIdentifier().Identifier;
        await this.bookService.SetReview(userId, request.BookId, request.ReviewValue, CancellationToken.None);
        BookReviews reviews = await this.bookService.GetReviews(request.BookId,
            userId,
            cancellationToken);

        return reviews;
    }
}
