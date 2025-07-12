using TheStoryVault.Services.Contracts;
using CaseR;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Author.UseCases;

public class GetBooksByAuthorHanler : IUseCaseInteractor<GetBooksByAuthorQuery, IList<BookRecord>>
{
    private readonly StoryVaultContext storyVaultContext;

    public GetBooksByAuthorHanler(StoryVaultContext storyVaultContext)
    {
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<IList<BookRecord>> Execute(GetBooksByAuthorQuery request, CancellationToken cancellationToken = default)
    {
        Queries.AuthorBooksQuery query = new Queries.AuthorBooksQuery(request.AuthorId)
        {
            Skip = (request.Page - 1) * request.PageSize,
            Take = request.PageSize
        };

       return await this.storyVaultContext.GetBooksInfos(query, cancellationToken);
    }
}
