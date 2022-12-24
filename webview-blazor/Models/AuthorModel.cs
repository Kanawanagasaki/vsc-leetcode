namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record AuthorModel
{
    public required string Username { get; init; }
    public string? NameColor { get; init; }
    public BadgeModel? ActiveBadge { get; init; }
    public required bool IsActive { get; init; }
    public required ProfileModel Profile { get; init; }

    public bool? IsDiscussAdmin { get; init; }
    public bool? IsDiscussStaff { get; init; }
}
