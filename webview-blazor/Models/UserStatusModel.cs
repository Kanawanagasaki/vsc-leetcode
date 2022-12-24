namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record UserStatusModel
{
    public required bool IsSignedIn { get; init; }

    public bool? IsPremium { get; init; }

    public required bool IsVerified { get; init; }

    public required string Username { get; init; }

    public string? RealName { get; init; }

    public string? Avatar { get; init; }

    public required long ActiveSessionId { get; init; }
}
