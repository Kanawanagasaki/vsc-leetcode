namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProblemSetItemModel
{
    public required double AcRate { get; init; }
    public required string Difficulty { get; init; }
    public required string FrontendQuestionId { get; init; }
    public required bool IsFavor { get; init; }
    public required bool PaidOnly { get; init; }
    public string? Status { get; init; }
    public required string Title { get; init; }
    public required string TitleSlug { get; init; }
    public required TagModel[] TopicTags { get; init; }
    public required bool HasSolution { get; init; }
    public required bool HasVideoSolution { get; init; }
}
