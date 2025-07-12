using Azure.Core;
using CaseR;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book.UseCases;

public class GetBookByIdHandler : IUseCaseInteractor<int, BookData>
{
    private readonly IBookService bookService;
    private readonly IDomainEventPublisher eventPublisher;

    public GetBookByIdHandler(IBookService bookService, IDomainEventPublisher eventPublisher)
    {
        this.bookService = bookService;
        this.eventPublisher = eventPublisher;
    }
    public async Task<BookData> Execute(int request, CancellationToken cancellationToken = default)
    {
        BookData book = await this.bookService.GetBookByIdAsync(request, cancellationToken);
        await this.eventPublisher.Publish(new BookHasShowEvent(book.Id), cancellationToken);
        return book;
    }
}
