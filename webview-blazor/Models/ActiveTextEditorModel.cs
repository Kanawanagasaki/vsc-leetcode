namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ActiveTextEditorModel
{
    public required string TitleSlug { get; init; }
    public required string LangSlug { get; init; }
}
