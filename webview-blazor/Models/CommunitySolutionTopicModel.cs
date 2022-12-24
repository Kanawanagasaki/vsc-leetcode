namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record CommunitySolutionTopicModel
{
    public required long Id { get; init; }
    public required int ViewCount { get; init; }
    public required int TopLevelCommentCount { get; init; }
    public required int FavoriteCount { get; init; }
    public required bool Subscribed { get; init; }
    public required string Title { get; init; }
    public required bool Pinned { get; init; }
    public required CommunitySolutionTopicTagModel[] SolutionTags { get; init; }
    public required bool HideFromTrending { get; init; }
    public required int CommentCount { get; init; }
    public required bool IsFavorite { get; init; }
    public required PostModel Post { get; init; }
}

public record CommunitySolutionTopicTagModel
{
    public required string Name { get; init; }
    public required string Slug { get; init; }
}
