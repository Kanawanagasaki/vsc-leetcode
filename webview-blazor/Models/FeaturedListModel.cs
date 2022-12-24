namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record FeaturedListModel
{
    public string? CoverUrl { get; init; }
    public required string Name { get; init; }
    public string? Link { get; init; }
    public required string IdHash { get; init; }
    public required bool IsPaidOnly { get; init; }
    public string? Description { get; init; }
    public int QuestionCount { get; init; }
}
