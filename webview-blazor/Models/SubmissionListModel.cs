namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SubmissionListModel
{
    public bool? LastKey { get; init; }
    public bool? HasNext { get; init; }
    public SubmissionModel[]? Submissions { get; init; }
}
