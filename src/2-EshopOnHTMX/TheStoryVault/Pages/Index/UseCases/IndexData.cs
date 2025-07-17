using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages.Index.UseCases;

public record IndexData(IList<BookRecord> Interesting, IList<BookRecord> News, IList<BookRecord> EBookNews);