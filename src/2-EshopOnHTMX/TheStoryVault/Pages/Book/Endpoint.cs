using CaseR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TheStoryVault.Pages.Book.UseCases;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Book;

internal static class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/book/{id}", async (int id,
            IUseCase<GetBookByIdInteractor> getBookById,
            IUseCase<GetReviewsByBookIdInteractor> getReviewsByBookId,
            CancellationToken cancellationToken) =>
        {
            BookData book = await getBookById.Execute<GetBookByIdInteractor, int, BookData>(id, cancellationToken);
            BookReviews reviews = await getReviewsByBookId.Execute<GetReviewsByBookIdInteractor, int, BookReviews>(id, cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Book.MainView>(new Dictionary<string, object?>()
            {
                { nameof(MainView.Book), book },
                { nameof(MainView.BookReviews), reviews}
            });
        });

        app.MapPost("/book/card", async ([FromForm] int itemId,
            IUseCase<AddToCardInteraction> addToCardInteraction,
            CancellationToken cancellationToken) =>
        {
            AddToCardInteractionResponse result = await addToCardInteraction.Execute(new AddToCardInteractionRequest(itemId), cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Book.OrderDialog>(new Dictionary<string, object?>()
            {
                { nameof(OrderDialog.Book), result.Book },
                { nameof(OrderDialog.Price), result.TotalPrice },
            });
        });

        app.MapPost("/book/buyNow", async ([FromForm] int itemId,
            IUseCase<BuyNowInteractor> buyNowInteractor,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            await buyNowInteractor.Execute(itemId, cancellationToken);

            httpContext.Response.Headers.Append("HX-Redirect", "/cart");
            return Results.NoContent();
        });

        app.MapGet("/book/card/close", () =>
        {
            return Results.Content(string.Empty, "text/html");
        });

        app.MapPost("/book/{id}/review", async (int id,
            [FromForm(Name = "review")] int review,
            IUseCase<SetReviewInteractor> setReviewInteractor,
            CancellationToken cancellationToken) =>
        {
            BookReviews reviews = await setReviewInteractor.Execute(new SetReviewInteractorRequest(id, review), cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Book.Reviews>(new Dictionary<string, object?>()
            {
                { nameof(Reviews.BookId), id },
                { nameof(Reviews.BookReviews), reviews}
            });
        });

        app.MapGet("/book/{id}/comments", async (int id,
            IUseCase<GetCommentsIneractor> getCommentsInteractor,
            CancellationToken cancellationToken) =>
        {
            IList<TextReview> reviews = await getCommentsInteractor.Execute(new GetCommentsIneractorRequest(id, 0, 150), cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Book.Comments>(new Dictionary<string, object?>()
            {
                { nameof(Comments.BookId), id },
                { nameof(Comments.Reviews), reviews },
                { nameof(Comments.FormErrors), null }
            });
        });

        app.MapPost("/book/{id}/comments", async (int id,
            [FromForm] CommentModel model,
            IUseCase<AddCommentInteractor> addCommentInteractor,
            CancellationToken cancellationToken) =>
        {
            AddCommentInteractorResponse response = await addCommentInteractor.Execute(new AddCommentInteractorRequest(id, model), cancellationToken);

            return new RazorComponentResult<TheStoryVault.Pages.Book.Comments>(new Dictionary<string, object?>()
            {
                { nameof(Comments.BookId), id },
                { nameof(Comments.Reviews), response.Reviews },
                { nameof(Comments.FormErrors), response.Errors }
            });
        });

        app.MapPost("/book/like", ([FromForm] int itemId) =>
        {
            throw new ApplicationException($"Something is wrong in /book/like with {itemId}");
        });
    }
}
