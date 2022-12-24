namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record CommunitySolutionCommentReplyModel
{
    public required long Id { get; init; }
    public required bool Pinned { get; init; }
    public object? PinnedBy { get; init; }
    public required PostModel Post { get; init; }
}
