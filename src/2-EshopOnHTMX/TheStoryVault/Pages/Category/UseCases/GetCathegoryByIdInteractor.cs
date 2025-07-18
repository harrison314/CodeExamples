using TheStoryVault.Services.Contracts;
using CaseR;

namespace TheStoryVault.Pages.Category.UseCases;

public class GetCathegoryByIdInteractor : IUseCaseInteractor<int, GetCathegoryByIdResponse>
{
    private readonly ICategoryService categoryService;
    private readonly IUseCase<GetBooksByCathegoryInteractor> getBooksByCathegory;

    public GetCathegoryByIdInteractor(ICategoryService categoryService,
        IUseCase<GetBooksByCathegoryInteractor> getBooksByCathegory)
    {
        this.categoryService = categoryService;
        this.getBooksByCathegory = getBooksByCathegory;
    }

    public async Task<GetCathegoryByIdResponse> Execute(int request, CancellationToken cancellationToken = default)
    {
        CategoryData cathegoryData = await this.categoryService.GetById(request, cancellationToken);

        BookPager books = await this.getBooksByCathegory.Execute<GetBooksByCathegoryInteractor, GetBooksByCathegoryQuery, BookPager>(
            new GetBooksByCathegoryQuery(request, 1, false), cancellationToken);

        return new GetCathegoryByIdResponse(cathegoryData, books);
    }
}
