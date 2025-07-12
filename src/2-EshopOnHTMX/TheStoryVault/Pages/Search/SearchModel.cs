using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Search;

public class SearchModel
{
    public required IList<CategoryInfo> Categories { get; init; }

    public required string Query { get; init; }

    public required IList<BookRecord> Results { get; init; }
}
