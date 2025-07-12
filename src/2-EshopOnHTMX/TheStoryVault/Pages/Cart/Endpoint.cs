using CaseR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheStoryVault.Pages.Cart.UseCases;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Cart;

internal class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/cart", static async (IUseCase<GetCartInteractor> getCartInteractor,
            CancellationToken cancellationToken) =>
        {
            BookCartTotal bookCartTotal = await getCartInteractor.Execute(Unit.Value, cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Cart.MainView>(new Dictionary<string, object?>()
            {
                { nameof(MainView.Cart), bookCartTotal }
            });
        });

        app.MapDelete("/cart", async (IUseCase<ClearCartInteractor> clearCartInteractor,
            HttpContext ctx,
            CancellationToken cancellationToken) =>
        {
            await clearCartInteractor.Execute(Unit.Value, cancellationToken);

            ctx.Response.Headers.Append("HX-Refresh", "true");
            return Results.Content(string.Empty, "text/html");
        });

        app.MapPost("/cart/{type}/{id}", async (string type,
            int id,
            IUseCase<UpdateCartInteractor> updateCartInteractor,
            CancellationToken cancellationToken) =>
        {
            bool adding;
            if (type == "add")
            {
                adding = true;
            }
            else if (type == "remove")
            {
                adding = false;
            }
            else
            {
                return Results.BadRequest("Invalid type. Use 'add' or 'remove'.");
            }

            BookCartTotal bookCartTotal = await updateCartInteractor.Execute(
                new UpdateCartInteractorRequest(id, adding),
                cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Cart.CartGrid>(new Dictionary<string, object?>()
            {
                { nameof(CartGrid.Cart), bookCartTotal },
                { nameof(CartGrid.SwapCardButton), true }
            });
        });
    }
}

