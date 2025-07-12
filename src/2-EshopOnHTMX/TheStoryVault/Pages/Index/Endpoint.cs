using CaseR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TheStoryVault.Pages.Index.UseCases;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index;

public class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async(IUseCase < GetIndexDataInteractor > getIndexDataInteractor,
            CancellationToken cancellationToken) =>
        {
            IndexData result = await getIndexDataInteractor.Execute(Unit.Value, cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Index.MainView>(new Dictionary<string, object?>()
            {
                { nameof(MainView.Interesting), result.Interesting },
                { nameof(MainView.News), result.News },
                { nameof(MainView.EBookNews), result.EBookNews },
            });
        });

        app.MapGet("/MostViewedTitles/{type}", (BookType type) =>
        {
            return new RazorComponentResult<TheStoryVault.Pages.Index.MostViewedTitles>(new Dictionary<string, object?>()
            {
                { nameof(MostViewedTitles.Type), type }
            });
        })
        .CacheOutput(PolicyNames.Cache.Default);
    }
}
