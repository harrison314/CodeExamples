using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class GetCommentsIneractor : IUseCaseInteractor<GetCommentsIneractorRequest, IList<TextReview>>
{
    private readonly IBookService bookService;

    public GetCommentsIneractor(IBookService bookService)
    {
        this.bookService = bookService;
    }

    public async Task<IList<TextReview>> Execute(GetCommentsIneractorRequest request, CancellationToken cancellationToken)
    {
        System.Diagnostics.Debug.Assert(request.PageNumber == 0);

        IList<TextReview> reviews = await this.bookService.GetTextReviews(request.BookId, cancellationToken);

        return reviews;
    }
}
