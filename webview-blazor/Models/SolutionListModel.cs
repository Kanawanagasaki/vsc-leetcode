namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SolutionListModel
{
    public required bool HasDirectResults { get; init; }
    public required int TotalNum { get; init; }
    public required SolutionModel[] Solutions { get; init; }
}
