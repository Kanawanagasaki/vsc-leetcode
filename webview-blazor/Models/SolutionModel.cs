namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SolutionModel
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required int CommentCount { get; init; }
    public required int TopLevelCommentCount { get; init; }
    public required int ViewCount { get; init; }
    public required bool Pinned { get; init; }
    public required bool IsFavorite { get; init; }
    public required SolutionTagModel2[] SolutionTags { get; init; }
    public required PostModel Post { get; init; }
    public string? SearchMeta { get; init; }
}

public record SolutionTagModel2
{
    public required string Name { get; init; }
    public required string Slug { get; init; }
}
