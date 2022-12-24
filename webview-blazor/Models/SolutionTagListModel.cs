namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SolutionTagListModel
{
    public required SolutionTagModel[] SolutionTopicTags { get; init; }
    public required SolutionTagModel[] SolutionLanguageTags { get; init; }
}

public record SolutionTagModel
{
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required int Count { get; init; }
}
