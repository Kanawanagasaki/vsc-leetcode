namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProblemTitleModel
{
    public required string QuestionId { get; init; }
    public required string QuestionFrontendId { get; init; }
    public required string Title { get; init; }
    public required string TitleSlug { get; init; }
    public required bool IsPaidOnly { get; init; }
    public required string Difficulty { get; init; }
    public required int Likes { get; init; }
    public required int Dislikes { get; init; }
}
