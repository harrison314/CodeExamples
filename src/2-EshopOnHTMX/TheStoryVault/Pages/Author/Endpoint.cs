using Microsoft.AspNetCore.Http.HttpResults;
using TheStoryVault.Pages.Author.UseCases;
using TheStoryVault.Services.Contracts;
using CaseR;

namespace TheStoryVault.Pages.Author;

internal static class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/author/{id}", async (int id, IUseCase<GetAuthorByIdHandler> getAuthorbyId,
            IUseCase<GetBooksByAuthorHanler> getBookByAuthor,
            CancellationToken cancellationToken) =>
        {
            AuthorData author = await getAuthorbyId.Execute(id, cancellationToken);
            IList<BookRecord> books = await getBookByAuthor.Execute(new GetBooksByAuthorQuery(id, 1, 25), cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Author.MainView>(new Dictionary<string, object?>()
            {
                { nameof(MainView.Author), author },
                { nameof(MainView.Books), books }
            });
        });
    }
}
