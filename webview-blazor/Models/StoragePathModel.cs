namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record StoragePathModel
{
    public required bool Exists { get; init; }
    public string? Path { get; init; }
}