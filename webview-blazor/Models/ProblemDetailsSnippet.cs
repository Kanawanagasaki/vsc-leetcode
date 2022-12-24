namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProblemDetailsSnippet
{
    public string Snippet { get; private init; }
    public required string TitleSlug { get; init; }
    public int? Offset { get; init; }
    public string[]? LanguageTags { get; init; }
    public string[]? TopicTags { get; init; }
    public int? Skip { get; init; }
    public int? First { get; init; }
    public int? PageNo { get; init; }
    public int? NumPerPage { get; init; }
    public bool? GetCache { get; init; }

    public ProblemDetailsSnippet(ESnippetType type)
        => Snippet = type.ToString().ToLower();

    public enum ESnippetType
    {
        Title, Content, Stats, Similar, ConsoleConfig, Tags, EditorData, Hints, Submissions, SolutionTags, Solutions, DiscussionTopics
    }
}
