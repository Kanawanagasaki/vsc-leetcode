namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record DiscussionTopicModel
{
    public required long Id { get; init; }
    public required int CommentCount { get; init; }
    public required int TopLevelCommentCount { get; init; }
}
