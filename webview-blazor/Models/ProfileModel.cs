namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProfileModel
{
    public required string UserAvatar { get; init; }
    public required int Reputation { get; init; }
}
