using CaseR;
using Microsoft.EntityFrameworkCore;
using TheStoryVault.Services.CommonUseCases;
using TheStoryVault.Services.Contracts;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Pages.Author.UseCases;

public class GetAuthorByIdInteractor : IUseCaseInteractor<int, AuthorData>
{
    private StoryVaultContext storyVaultContext;
    private readonly IUseCase<SendMessageInteractor> sendMessageUseCase;

    public GetAuthorByIdInteractor(StoryVaultContext storyVaultContext,
        [FromKeyedServices(UseCaseRelation.Include)] IUseCase<SendMessageInteractor> sendMessageUseCase)
    {
        this.storyVaultContext = storyVaultContext;
        this.sendMessageUseCase = sendMessageUseCase;
    }

    public async Task<AuthorData> Execute(int request, CancellationToken cancellationToken = default)
    {
        await this.sendMessageUseCase.Execute($"Author data requested - {request}", cancellationToken);

        return await this.storyVaultContext.Authors
           .Where(a => a.Id == request)
           .Select(a => new AuthorData(
               a.Id,
               a.Name,
               a.Description))
           .SingleAsync(cancellationToken);
    }
}
