using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Category;
public record BookPager(IList<BookRecord> Books,
    int Page,
    bool CanNext)
{
    public bool CanPrevious => this.Page > 1;
}
