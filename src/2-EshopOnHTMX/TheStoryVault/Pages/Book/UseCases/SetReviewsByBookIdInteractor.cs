using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class SetReviewsByBookIdInteractor : IUseCaseInteractor<SetReviewsByBookIdRequest, Unit>
{
    private readonly IBookService bookService;
    private readonly ITrackingService trackingService;

    public SetReviewsByBookIdInteractor(IBookService bookService, ITrackingService trackingService)
    {
        this.bookService = bookService;
        this.trackingService = trackingService;
    }

    public async Task<Unit> Execute(SetReviewsByBookIdRequest request, CancellationToken cancellationToken)
    {
        string userId = this.trackingService.CookieApi.GetTrackingIdentifier().Identifier;

        await this.bookService.SetReview(userId, request.BookId, request.Value, CancellationToken.None);

        return Unit.Value;
    }
}