namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record CommunitySolutionCommentListModel
{
    public required CommunitySolutionCommentModel[] Data { get; init; }
    public required int TotalNum { get; init; }
}
