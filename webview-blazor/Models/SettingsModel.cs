namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SettingsModel
{
    public bool ShowTagsOnHover { get; init; }
    public string? DefaultLanguage { get; init; }
}
