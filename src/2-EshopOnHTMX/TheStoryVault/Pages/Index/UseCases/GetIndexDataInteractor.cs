using CaseR;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Index.UseCases;

public class GetIndexDataInteractor : IUseCaseInteractor<Unit, IndexData>
{
    private readonly StoryVaultContext storyVaultContext;

    public GetIndexDataInteractor(StoryVaultContext storyVaultContext)
    {
        this.storyVaultContext = storyVaultContext;
    }

    public async Task<IndexData> Execute(Unit request, CancellationToken cancellationToken)
    {
        IList<BookRecord> interesting = await this.storyVaultContext.GetBooksInfos(new Queries.InterestingBookQuery(), cancellationToken);
        IList<BookRecord> news = await this.storyVaultContext.GetBooksInfos(new Queries.NewsBookQuery(), cancellationToken);
        IList<BookRecord> ebookNews = await this.storyVaultContext.GetBooksInfos(new Queries.NewsEbookQuery(), cancellationToken);

        return new IndexData(interesting, news, ebookNews);
    }
}
