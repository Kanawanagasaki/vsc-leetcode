namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProblemSetModel
{
    public int Page { get; set; } = 0;
    public int Skip { get; set; } = 0;
    public int Limit { get; set; } = 0;
    public int Total { get; set; } = 0;
    public ProblemSetItemModel[] Questions { get; set; } = Array.Empty<ProblemSetItemModel>();
}
