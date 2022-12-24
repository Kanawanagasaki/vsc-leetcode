namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SubmissionModel
{
    public required string Id {get; init; }
    public required string Title {get; init; }
    public required string TitleSlug {get; init; }
    public required int Status {get; init; }
    public required string StatusDisplay {get; init; }
    public required string Lang {get; init; }
    public required string LangName {get; init; }
    public required string Runtime {get; init; }
    public required string Timestamp {get; init; }
    public required string Url {get; init; }
    public required string IsPending {get; init; }
    public required string Memory {get; init; }
    public required bool HasNotes {get; init; }
}
