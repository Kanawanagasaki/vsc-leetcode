namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record DownloadChromiumProgressModel
{
    public long DownloadedBytes { get; init; } = 0;
    public long TotalBytes { get; init; } = 0;
}
