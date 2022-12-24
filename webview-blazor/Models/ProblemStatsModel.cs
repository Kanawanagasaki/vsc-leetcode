namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProblemStatsModel
{
    public required string TotalAccepted { get; init; }
    public required string TotalSubmission { get; init; }
    public required long TotalAcceptedRaw { get; init; }
    public required long TotalSubmissionRaw { get; init; }
    public required string AcRate { get; init; }
}
