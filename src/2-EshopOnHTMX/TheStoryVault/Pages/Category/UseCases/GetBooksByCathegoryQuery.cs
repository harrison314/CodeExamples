namespace TheStoryVault.Pages.Category.UseCases;

public record GetBooksByCathegoryQuery(int CathegoryId, int Page, bool CheckCathegory, int? Take = 25);
