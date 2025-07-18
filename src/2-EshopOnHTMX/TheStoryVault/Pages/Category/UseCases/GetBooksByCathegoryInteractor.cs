using TheStoryVault.Services.Contracts;
using CaseR;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Category.UseCases;

public class GetBooksByCathegoryInteractor : IUseCaseInteractor<GetBooksByCathegoryQuery, BookPager>
{
    private readonly StoryVaultContext storyVaultContext;
    private readonly ICategoryService categoryService;

    public GetBooksByCathegoryInteractor(StoryVaultContext storyVaultContext, ICategoryService categoryService)
    {
        this.storyVaultContext = storyVaultContext;
        this.categoryService = categoryService;
    }

    public async Task<BookPager> Execute(GetBooksByCathegoryQuery request, CancellationToken cancellationToken = default)
    {
        _ = await this.categoryService.GetById(request.CathegoryId, cancellationToken);

        Query.CategoryBooks query = new Query.CategoryBooks(request.CathegoryId, request.Page);
        IList<BookRecord> books = await this.storyVaultContext.GetBooksInfos(query, cancellationToken);

        BookPager pager = new BookPager(books, request.Page, books.Count >= query.Take!.Value);

        return pager;
    }
}
