namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record RuncodeStateUpdateModel
{
    public required string TitleSlug { get; init; }
    public required RuncodeStateModel State { get; init; }
}
