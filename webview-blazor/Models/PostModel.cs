namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record PostModel
{
    public required long Id { get; init; }
    public string? Status { get; init; }
    public required int VoteCount { get; init; }
    public required long CreationDate { get; init; }
    public bool? IsHidden { get; init; }
    public required AuthorModel Author { get; init; }

    public int? VoteStatus { get; init; }
    public string? Content { get; init; }
    public long? UpdationDate { get; init; }
    public bool? AuthorIsModerator { get; init; }
    public bool? IsOwnPost { get; init; }
    public object[]? CoinRewards { get; init; }
}
