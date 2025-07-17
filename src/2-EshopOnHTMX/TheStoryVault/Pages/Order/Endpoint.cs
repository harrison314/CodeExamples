using Microsoft.AspNetCore.Http.HttpResults;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Order;

internal static class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/order", () =>
        {
            return new RazorComponentResult<TheStoryVault.Pages.Order.MainView>(new Dictionary<string, object?>()
            {
            });
        });
    }
}
