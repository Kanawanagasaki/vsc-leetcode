namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record DiscussionListModel
{
    public required DiscussionModel[] Data { get; init; }
    public required int TotalNum { get; init; }
    public required int PageNo { get; init; }
    public required int NumPerPage { get; init; }
}
