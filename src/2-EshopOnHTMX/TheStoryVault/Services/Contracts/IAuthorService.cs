namespace TheStoryVault.Services.Contracts;

public interface IAuthorService
{
    Task<AuthorData> GetAuthorByIdAsync(int id, CancellationToken cancellationToken);
}

public record AuthorData(
    int Id,
    string Name,
    string? Description = null
);