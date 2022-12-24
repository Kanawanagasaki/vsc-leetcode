using Kanawanagasaki.VSCode.LeetCode.WebView.Services;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public class ProblemModel : IDisposable
{
    public string TitleSlug { get; }

    public ProblemTitleModel? Title { get; private set; }
    public ProblemContentModel? Content { get; private set; }
    public ProblemStatsModel? Stats { get; private set; }
    public SimilarProblemModel[]? SimilarProblems { get; private set; }
    public ConsoleConfigModel? ConsoleConfig { get; private set; }
    public TagModel[]? Tags { get; private set; }
    public EditorDataModel? EditorData { get; private set; }
    public HintsModel? Hints { get; private set; }
    public SubmissionListModel? Submissions { get; private set; }
    public SolutionTagListModel? SolutionTags { get; private set; }
    public SolutionListModel? Solutions { get; private set; }
    public DiscussionTopicListModel? Discussions { get; private set; }

    public event Action<EDetails>? OnDetailUpdate;

    public EDetails RequestedDetails { get; private set; } = EDetails.None;

    public ProblemModel(string titleSlug)
    {
        TitleSlug = titleSlug;
        JsService.OnProblemDetails += JsService_OnProblemDetails;
    }

    public async Task RequestTitle(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.Title))
            return;
        RequestedDetails |= EDetails.Title;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Title) { TitleSlug = TitleSlug });
    }

    public async Task RequestContent(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.Content))
            return;
        RequestedDetails |= EDetails.Content;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Content) { TitleSlug = TitleSlug });
    }

    public async Task RequestStats(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.Stats))
            return;
        RequestedDetails |= EDetails.Stats;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Stats) { TitleSlug = TitleSlug });
    }

    public async Task RequestSimilar(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.SimilarProblems))
            return;
        RequestedDetails |= EDetails.SimilarProblems;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Similar) { TitleSlug = TitleSlug });
    }

    public async Task RequestConsoleConfig(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.ConsoleConfig))
            return;
        RequestedDetails |= EDetails.ConsoleConfig;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.ConsoleConfig) { TitleSlug = TitleSlug });
    }

    public async Task RequestTags(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.Tags))
            return;
        RequestedDetails |= EDetails.Tags;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Tags) { TitleSlug = TitleSlug });
    }

    public async Task RequestEditorData(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.EditorData))
            return;
        RequestedDetails |= EDetails.EditorData;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.EditorData) { TitleSlug = TitleSlug });
    }

    public async Task RequestHints(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.Hints))
            return;
        RequestedDetails |= EDetails.Hints;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Hints) { TitleSlug = TitleSlug });
    }

    public async Task RequestSubmissions(JsService js, int offset, bool getCache = true)
    {
        if (RequestedDetails.HasFlag(EDetails.Submissions))
            return;
        RequestedDetails |= EDetails.Submissions;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Submissions)
        {
            TitleSlug = TitleSlug,
            Offset = offset,
            GetCache = getCache
        });
    }

    public async Task RequestSolutionTags(JsService js)
    {
        if (RequestedDetails.HasFlag(EDetails.SolutionTags))
            return;
        RequestedDetails |= EDetails.SolutionTags;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.SolutionTags) { TitleSlug = TitleSlug });
    }

    public async Task RequestSolutions(JsService js, string[] languageTags, string[] topicTags, int skip, int first = 15)
    {
        if (RequestedDetails.HasFlag(EDetails.Solutions))
            return;
        RequestedDetails |= EDetails.Solutions;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.Solutions)
        {
            TitleSlug = TitleSlug,
            LanguageTags = languageTags,
            TopicTags = topicTags,
            Skip = skip,
            First = first
        });
    }

    public async Task RequestDiscussionTopics(JsService js, int pageNo, int numPerPage = 10)
    {
        if (RequestedDetails.HasFlag(EDetails.DiscussionTopics))
            return;
        RequestedDetails |= EDetails.DiscussionTopics;
        await js.RequestProblemDetails(new(ProblemDetailsSnippet.ESnippetType.DiscussionTopics)
        {
            TitleSlug = TitleSlug,
            PageNo = pageNo,
            NumPerPage = numPerPage
        });
    }

    private void JsService_OnProblemDetails(object? data)
    {
        if (data is null)
            return;

        switch (data)
        {
            case ProblemTitleModel title:
                Title = title;
                RequestedDetails = RequestedDetails & ~EDetails.Title;
                OnDetailUpdate?.Invoke(EDetails.Title);
                break;
            case ProblemContentModel content:
                Content = content;
                RequestedDetails = RequestedDetails & ~EDetails.Content;
                OnDetailUpdate?.Invoke(EDetails.Content);
                break;
            case ProblemStatsModel stats:
                Stats = stats;
                RequestedDetails = RequestedDetails & ~EDetails.Stats;
                OnDetailUpdate?.Invoke(EDetails.Stats);
                break;
            case SimilarProblemModel[] similar:
                SimilarProblems = similar;
                RequestedDetails = RequestedDetails & ~EDetails.SimilarProblems;
                OnDetailUpdate?.Invoke(EDetails.SimilarProblems);
                break;
            case ConsoleConfigModel consoleConfig:
                ConsoleConfig = consoleConfig;
                RequestedDetails = RequestedDetails & ~EDetails.ConsoleConfig;
                OnDetailUpdate?.Invoke(EDetails.ConsoleConfig);
                break;
            case TagModel[] tags:
                Tags = tags;
                RequestedDetails = RequestedDetails & ~EDetails.Tags;
                OnDetailUpdate?.Invoke(EDetails.Tags);
                break;
            case EditorDataModel editorData:
                EditorData = editorData;
                RequestedDetails = RequestedDetails & ~EDetails.EditorData;
                OnDetailUpdate?.Invoke(EDetails.EditorData);
                break;
            case HintsModel hints:
                Hints = hints;
                RequestedDetails = RequestedDetails & ~EDetails.Hints;
                OnDetailUpdate?.Invoke(EDetails.Hints);
                break;
            case SubmissionListModel submissions:
                Submissions = submissions;
                RequestedDetails = RequestedDetails & ~EDetails.Submissions;
                OnDetailUpdate?.Invoke(EDetails.Submissions);
                break;
            case SolutionTagListModel solutionTags:
                SolutionTags = solutionTags;
                RequestedDetails = RequestedDetails & ~EDetails.SolutionTags;
                OnDetailUpdate?.Invoke(EDetails.SolutionTags);
                break;
            case SolutionListModel solutions:
                Solutions = solutions;
                RequestedDetails = RequestedDetails & ~EDetails.Solutions;
                OnDetailUpdate?.Invoke(EDetails.Solutions);
                break;
            case DiscussionTopicListModel discussions:
                Discussions = discussions;
                RequestedDetails = RequestedDetails & ~EDetails.DiscussionTopics;
                OnDetailUpdate?.Invoke(EDetails.DiscussionTopics);
                break;
        }
    }

    public void Dispose()
    {
        JsService.OnProblemDetails -= JsService_OnProblemDetails;
    }

    [Flags]
    public enum EDetails : int
    {
        None = 0,
        Title = 1 << 0,
        Content = 1 << 1,
        Stats = 1 << 2,
        SimilarProblems = 1 << 3,
        ConsoleConfig = 1 << 4,
        Tags = 1 << 5,
        EditorData = 1 << 6,
        Hints = 1 << 7,
        Submissions = 1 << 8,
        SolutionTags = 1 << 9,
        Solutions = 1 << 10,
        DiscussionTopics = 1 << 11
    }
}
