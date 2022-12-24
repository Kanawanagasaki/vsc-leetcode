namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record TagModel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public string? TranslatedName { get; init; }
    public int? QuestionCount { get; init; }
}
