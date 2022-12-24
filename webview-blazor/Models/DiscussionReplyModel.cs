namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record DiscussionReplyModel
{
    public required long Id { get; init; }
    public required bool Pinned { get; init; }
    public string? PinnedBy { get; init; }
    public required PostModel Post { get; init; }
}
