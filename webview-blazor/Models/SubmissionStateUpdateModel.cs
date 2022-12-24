namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SubmissionStateUpdateModel
{
    public required string TitleSlug { get; init; }
    public required SubmissionStateModel State { get; init; }
}
