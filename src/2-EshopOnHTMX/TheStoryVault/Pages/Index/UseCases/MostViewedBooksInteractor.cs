using CaseR;
using TheStoryVault.Pages.Index.Queries;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Index.UseCases;

public class MostViewedBooksInteractor : IUseCaseInteractor<MostViewedBooksInteractorRequest, IList<BookRecord>>
{
    private readonly StoryVaultContext storyVaultContext;

    public MostViewedBooksInteractor(StoryVaultContext storyVaultContext)
    {
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<IList<BookRecord>> Execute(MostViewedBooksInteractorRequest request, CancellationToken cancellationToken)
    {
        GetMostViewedBookQuery query = new GetMostViewedBookQuery(request.BookType)
        {
            Take = request.Count
        };

        return await this.storyVaultContext.GetBooksInfos(query, cancellationToken);
    }
}
