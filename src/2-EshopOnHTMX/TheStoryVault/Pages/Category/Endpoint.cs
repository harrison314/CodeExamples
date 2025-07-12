using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TheStoryVault.Services.Contracts;
using CaseR;
using TheStoryVault.Pages.Category.UseCases;

namespace TheStoryVault.Pages.Category;

internal static class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/category/{id}", async (int id,
            IUseCase<GetCathegoryByIdHandler> getCathegoryById,
            CancellationToken cancellationToken) =>
        {
            GetCathegoryByIdResponse data = await getCathegoryById.Execute<GetCathegoryByIdHandler, int, GetCathegoryByIdResponse>(id, cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Category.MainView>(new Dictionary<string, object?>()
            {
                { nameof(MainView.Cathegory), data.Cathegory },
                { nameof(MainView.BookPager),  data.Pager }
            });
        });

        app.MapGet("/category/{id}/books", async (int id,
            [FromQuery(Name = "page")] int page,
            IUseCase<GetBooksByCathegoryHandler> getBooksByCathegory,
            CancellationToken cancellationToken) =>
        {
            BookPager pager = await getBooksByCathegory.Execute<GetBooksByCathegoryHandler, GetBooksByCathegoryQuery, BookPager>(
                new GetBooksByCathegoryQuery(id, page, true), cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Category.AuthorBooks>(new Dictionary<string, object?>()
            {
                { nameof(AuthorBooks.CategoryId), id },
                { nameof(AuthorBooks.BookPager),  pager }
            });
        });
    }
}
