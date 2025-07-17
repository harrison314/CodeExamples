using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class GetReviewsByBookIdInteractor : IUseCaseInteractor<int, BookReviews>
{
    private readonly IBookService bookService;
    private readonly ITrackingService trackingService;

    public GetReviewsByBookIdInteractor(IBookService bookService, ITrackingService trackingService)
    {
        this.bookService = bookService;
        this.trackingService = trackingService;
    }

    public async Task<BookReviews> Execute(int request, CancellationToken cancellationToken)
    {
        return await this.bookService.GetReviews(request,
                this.trackingService.CookieApi.GetTrackingIdentifier().Identifier,
                 cancellationToken);
    }
}
