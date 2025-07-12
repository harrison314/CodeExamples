using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Category.UseCases;

public record GetCathegoryByIdResponse(CategoryData Cathegory, BookPager Pager);