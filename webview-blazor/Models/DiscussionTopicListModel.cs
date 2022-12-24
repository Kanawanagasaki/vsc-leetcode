namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record DiscussionTopicListModel
{
    public required DiscussionTopicModel Topic { get; init; }
    public required DiscussionListModel List { get; init; }
}
