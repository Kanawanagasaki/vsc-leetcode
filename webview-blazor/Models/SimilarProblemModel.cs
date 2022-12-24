namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SimilarProblemModel
{
    public required string Title { get; init; }
    public required string TitleSlug { get; init; }
    public required string Difficulty { get; init; }
    public string? TranslatedTitle { get; init; }
}
