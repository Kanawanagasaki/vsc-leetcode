namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record DiscussionModel
{
    public required long Id { get; init; }
    public required bool Pinned { get; init; }
    public required PostModel Post { get; init; }
    public required int NumChildren { get; init; }
}
