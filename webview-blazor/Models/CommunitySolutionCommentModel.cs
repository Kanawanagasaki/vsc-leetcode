namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record CommunitySolutionCommentModel
{
    public required long Id { get; init; }
    public required bool Pinned { get; init; }
    public object? PinnedBy { get; init; }
    public required PostModel Post { get; init; }
    public object? IntentionTag { get; init; }
    public required int NumChildren { get; init; }
}
