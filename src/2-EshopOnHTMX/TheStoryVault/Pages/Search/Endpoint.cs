using CaseR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using TheStoryVault.Pages.Search.Query;
using TheStoryVault.Pages.Search.UseCases;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Search;

internal static class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/search", async Task<IResult> ([FromQuery(Name = "q")] string? q,
            [FromQuery(Name = "categories")] int[]? categories,
            HttpContext httpContext,
            IUseCase<SearchInteractor> searchInteractor,
            CancellationToken cancellationToken) =>
        {
            return await SearchResults(q, categories, httpContext, searchInteractor, false, cancellationToken);
        });

        app.MapGet("/search/inner", async Task<IResult> ([FromQuery(Name = "q")] string? q,
             [FromQuery(Name = "categories")] int[]? categories,
            HttpContext httpContext,
            IUseCase<SearchInteractor> searchInteractor,
            CancellationToken cancellationToken) =>
        {
            return await SearchResults(q, categories, httpContext, searchInteractor, true, cancellationToken);
        });
    }

    private static async Task<IResult> SearchResults(string? q,
        int[]? categories,
        HttpContext httpContext,
        IUseCase<SearchInteractor> searchInteractor,
        bool isInner,
        CancellationToken cancellationToken)
    {
        SearchModel model = await searchInteractor.Execute(new SearchInteractorQuery(q, categories), cancellationToken);

        StringBuilder uriBuilder = new StringBuilder("/search?");
        uriBuilder.Append($"q={Uri.EscapeDataString(q ?? string.Empty)}");
        if (categories != null && categories.Length > 0)
        {
            foreach (int category in categories)
            {
                uriBuilder.Append($"&categories={category}");
            }
        }

        httpContext.SendHtmxPushUrlHistory(uriBuilder.ToString());

        if (httpContext.IsHtmxRequest())
        {
            if (isInner)
            {
                return new RazorComponentResult<TheStoryVault.Pages.Search.SearchResult>(new Dictionary<string, object?>()
                {
                    { nameof(SearchResult.Results), model.Results }
                });
            }
            else
            {
                return new RazorComponentResult<TheStoryVault.Pages.Search.Search>(new Dictionary<string, object?>()
                {
                    { nameof(Search.Model), model }
                });
            }
        }
        else
        {
            return new RazorComponentResult<TheStoryVault.Pages.Search.MainView>(new Dictionary<string, object?>()
            {
                { nameof(Search.Model), model }
            });
        }
    }
}
