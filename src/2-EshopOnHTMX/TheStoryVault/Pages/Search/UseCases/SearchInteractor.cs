using CaseR;
using TheStoryVault.Pages.Search.Query;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;
using TheStoryVault.Services.Implementation;

namespace TheStoryVault.Pages.Search.UseCases;

public class SearchInteractor : IUseCaseInteractor<SearchInteractorQuery, SearchModel>
{
    private readonly StoryVaultContext storyVaultContext;
    private readonly IDomainEventPublisher domainEventPublisher;

    public SearchInteractor(StoryVaultContext storyVaultContext,
        IDomainEventPublisher domainEventPublisher)
    {
        this.storyVaultContext = storyVaultContext;
        this.domainEventPublisher = domainEventPublisher;
    }

    public async Task<SearchModel> Execute(SearchInteractorQuery request, CancellationToken cancellationToken)
    {
        SearchQuery searchQuery = new SearchQuery(request.Query ?? string.Empty, request.Categories);
        searchQuery.Take = 30;

        IList<BookRecord> books = await this.storyVaultContext.GetBooksInfos(searchQuery, cancellationToken);

        IList<CategoryInfo> categoryInfo = await this.storyVaultContext.GetCategories(new TopNCathegoryQuery(10), cancellationToken);

        SearchModel model = new SearchModel()
        {
            Query = request.Query ?? string.Empty,
            Results = books,
            Categories = categoryInfo
        };

        await this.domainEventPublisher.Publish(new SearchDomainEvent(request.Query, request.Categories), cancellationToken);

        return model;
    }
}
