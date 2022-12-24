namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record EditorDataModel
{
    public required string QuestionId { get; init; }
    public required string QuestionFrontendId { get; init; }
    public required EditorDataCodeSnippetsModel[] CodeSnippets { get; init; }
    public required string EnvInfo { get; init; }
}

public record EditorDataCodeSnippetsModel
{
    public required string Lang { get; init; }
    public required string LangSlug { get; init; }
    public required string Code { get; init; }
}
