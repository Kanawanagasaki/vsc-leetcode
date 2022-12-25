namespace Kanawanagasaki.VSCode.LeetCode.WebView.Services;

using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public partial class JsService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    #region Global fields
    public JSObject? VscLeetCode => OperatingSystem.IsBrowser() ? JSHost.GlobalThis.GetPropertyAsJSObject("vscLeetcode") : null;
    public string? NavigateTo => OperatingSystem.IsBrowser() ? VscLeetCode?.GetPropertyAsString("navigateTo") : null;
    public string? ExtensionMediaPath => OperatingSystem.IsBrowser() ? VscLeetCode?.GetPropertyAsString("extensionMediaPath") : null;
    public string? Context => OperatingSystem.IsBrowser() ? VscLeetCode?.GetPropertyAsString("context") : null;
    public string? ProblemTitleSlug => OperatingSystem.IsBrowser() ? VscLeetCode?.GetPropertyAsString("problemTitleSlug") : null;
    #endregion

    #region Init
    private IJSRuntime _js;
    public JsService(IJSRuntime js)
        => _js = js;
    public async Task Init()
        => await _js.InvokeVoidAsync("globalThis.vscLeetcode.init");
    #endregion

    #region Global methods
    public async Task<int> GetScrollTop(ElementReference elRef)
        => await _js.InvokeAsync<int>("globalThis.vscLeetcode.getScrollTop", elRef);
    public async Task ClickOnEl(string elId)
        => await _js.InvokeVoidAsync("globalThis.vscLeetcode.clickOnEl", elId);
    public async Task SetState(StateModel state)
        => await _js.InvokeVoidAsync("globalThis.vscLeetcode.setState", state);
    public async Task<StateModel?> GetState()
        => await _js.InvokeAsync<StateModel?>("globalThis.vscLeetcode.getState");
    public async Task HighlightPreCode(ElementReference el, string? lang = null)
        => await _js.InvokeVoidAsync("globalThis.vscLeetcode.highlightPreCode", el, lang);
    #endregion

    #region Messaging
    public static event Action<DownloadChromiumProgressModel>? OnDownloadChromiumProgress;
    public static event Action? OnDownloadChromiumSuccess;
    public static event Action<string>? OnDownloadChromiumFail;
    public static event Action<UserStatusModel?>? OnUserStatusUpdate;
    public static event Action<Error?>? OnSigninError;
    public static event Action<StoragePathModel>? OnStoragePath;
    public static event Action<UuidResult<ProblemSetModel>?>? OnProblemSet;
    public static event Action<PagePropsModel>? OnPageProps;
    public static event Action<SettingsModel>? OnSettings;
    public static event Action<string>? OnUpdateProblem;
    public static event Action<object?>? OnProblemDetails;
    public static event Action<UuidResult<DiscussionReplyModel[]>?>? OnDiscussionReplies;
    public static event Action<ActiveTextEditorModel?>? OnChangeActiveTextEditor;
    public static event Action<RuncodeStateUpdateModel?>? OnRuncodeStateUpdate;
    public static event Action<SubmissionStateUpdateModel?>? OnSubmissionStateUpdate;
    public static event Action<CommunitySolutionModel?>? OnCommunitySolution;
    public static event Action<CommunitySolutionCommentListModel?>? OnCommunitySolutionComments;
    public static event Action<UuidResult<CommunitySolutionCommentReplyModel[]>?>? OnCommunitySolutionCommentReply;
    [JSExport]
    private static void OnMessage(string command, string data)
    {
        try
        {
            switch (command)
            {
                case "downloadChromiumProgress":
                    OnDownloadChromiumProgress?.Invoke(JsonSerializer.Deserialize<DownloadChromiumProgressModel>(data, _jsonOptions) ?? new());
                    break;
                case "downloadChromiumSuccess":
                    OnDownloadChromiumSuccess?.Invoke();
                    break;
                case "downloadChromiumFail":
                    OnDownloadChromiumFail?.Invoke(JsonSerializer.Deserialize<string>(data, _jsonOptions) ?? "");
                    break;
                case "updateUserStatus":
                    OnUserStatusUpdate?.Invoke(JsonSerializer.Deserialize<UserStatusModel>(data, _jsonOptions));
                    break;
                case "signinError":
                    OnSigninError?.Invoke(JsonSerializer.Deserialize<Error>(data, _jsonOptions));
                    break;
                case "storagePath":
                    OnStoragePath?.Invoke(JsonSerializer.Deserialize<StoragePathModel>(data, _jsonOptions) ?? new() { Exists = false });
                    break;
                case "problemset":
                    OnProblemSet?.Invoke(JsonSerializer.Deserialize<UuidResult<ProblemSetModel>>(data, _jsonOptions));
                    break;
                case "pageProps":
                    OnPageProps?.Invoke(JsonSerializer.Deserialize<PagePropsModel>(data, _jsonOptions) ?? new());
                    break;
                case "settings":
                    OnSettings?.Invoke(JsonSerializer.Deserialize<SettingsModel>(data, _jsonOptions) ?? new());
                    break;
                case "updateProblem":
                    OnUpdateProblem?.Invoke(JsonSerializer.Deserialize<string>(data, _jsonOptions) ?? "");
                    break;
                case "problemDetails":
                    var bundle = JsonSerializer.Deserialize<JsonElement>(data, _jsonOptions);
                    if (!bundle.TryGetProperty("snippet", out var snippet))
                    {
                        Console.Error.WriteLine($"In JsService.OnMessage() when executing command {command}: Failed to get snippet property\n{data}");
                        break;
                    }
                    if (!bundle.TryGetProperty("result", out var result))
                    {
                        Console.Error.WriteLine($"In JsService.OnMessage() when executing command {command}: Failed to get result property\n{data}");
                        break;
                    }
                    switch (snippet.GetString())
                    {
                        case "title":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<ProblemTitleModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "content":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<ProblemContentModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "stats":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<ProblemStatsModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "similar":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<SimilarProblemModel[]>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "consoleconfig":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<ConsoleConfigModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "tags":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<TagModel[]>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "editordata":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<EditorDataModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "hints":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<HintsModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "submissions":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<SubmissionListModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "solutiontags":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<SolutionTagListModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "solutions":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<SolutionListModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        case "discussiontopics":
                            OnProblemDetails?.Invoke(JsonSerializer.Deserialize<DiscussionTopicListModel>(result.GetString() ?? "null", _jsonOptions));
                            break;
                        default:
                            Console.Error.WriteLine($"In JsService.OnMessage() when executing command {command}: Snippet {snippet.GetString()} not found");
                            break;
                    }
                    break;
                case "discussionReplies":
                    OnDiscussionReplies?.Invoke(JsonSerializer.Deserialize<UuidResult<DiscussionReplyModel[]>>(data, _jsonOptions));
                    break;
                case "changeActiveTextEditor":
                    OnChangeActiveTextEditor?.Invoke(JsonSerializer.Deserialize<ActiveTextEditorModel>(data, _jsonOptions));
                    break;
                case "runcodeStateUpdate":
                    OnRuncodeStateUpdate?.Invoke(JsonSerializer.Deserialize<RuncodeStateUpdateModel>(data, _jsonOptions));
                    break;
                case "submissionStateUpdate":
                    OnSubmissionStateUpdate?.Invoke(JsonSerializer.Deserialize<SubmissionStateUpdateModel>(data, _jsonOptions));
                    break;
                case "communitySolution":
                    OnCommunitySolution?.Invoke(JsonSerializer.Deserialize<CommunitySolutionModel>(data, _jsonOptions));
                    break;
                case "communitySolutionComments":
                    OnCommunitySolutionComments?.Invoke(JsonSerializer.Deserialize<CommunitySolutionCommentListModel>(data, _jsonOptions));
                    break;
                case "communitySolutionCommentReplies":
                    OnCommunitySolutionCommentReply?.Invoke(JsonSerializer.Deserialize<UuidResult<CommunitySolutionCommentReplyModel[]>>(data, _jsonOptions));
                    break;
                default:
                    Console.Error.WriteLine($"In JsService.OnMessage(): Command {command} not found");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"In JsService.OnMessage(): Exception thrown while processing command {command}\n{e.Message}\nData:\n{data}");
        }
    }

    public async Task RequestDownloadChromium() => await SendMessage("downloadChromium");
    public async Task RequestSignin() => await SendMessage("signin");
    public async Task RequestSignout() => await SendMessage("signout");
    public async Task RequestUserStatus() => await SendMessage("getAuthUser");
    public async Task RequestStoragePath() => await SendMessage("getStoragePath");
    public async Task SelectStoragePath() => await SendMessage("selectStoragePath");
    public async Task RequestProblems(string categorySlug, int page, FilterModel filters, Guid uuid, bool getCache)
        => await SendMessage("getProblems", new { categorySlug, page, filters, uuid = uuid.ToString(), getCache });
    public async Task RequestPageProps() => await SendMessage("getPageProps");
    public async Task RequestSettings() => await SendMessage("getSettings");
    public async Task SetSettings(SettingsModel settings) => await SendMessage("setSettings", settings);
    public async Task OpenProblemWebview(string titleSlug) => await SendMessage("openProblem", new { titleSlug });
    public async Task RequestProblemDetails(ProblemDetailsSnippet snippet) => await SendMessage("getProblemDetails", snippet);
    public async Task RequestDiscussionReplies(long commentId, Guid uuid)
        => await SendMessage("getDiscussionReplies", new { commentId, uuid = uuid.ToString() });
    public async Task Solve(string titleSlug, string langSlug) => await SendMessage("solve", new { titleSlug, langSlug });
    public async Task RequestActiveTextEditor() => await SendMessage("getActiveTextEditor");
    public async Task RunCode(string titleSlug, string dataInput) => await SendMessage("runCode", new { titleSlug, dataInput });
    public async Task Submit(string titleSlug) => await SendMessage("submit", new { titleSlug });
    public async Task RequestCommunitySolution(long topicId) => await SendMessage("getCommunitySolution", new { topicId });
    public async Task RequestCommunitySolutionComments(long topicId, int pageNo, int numPerPage, string orderBy)
        => await SendMessage("getCommunitySolutionComments", new { topicId, pageNo, numPerPage, orderBy });
    public async Task RequestCommunitySolutionCommentReplies(long commentId, Guid uuid)
        => await SendMessage("getCommunitySolutionCommentReplies", new { commentId, uuid = uuid.ToString() });
    private async Task SendMessage(string command, object? data = null)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        await _js.InvokeVoidAsync("globalThis.vscLeetcode.sendMessage", command, json);
    }
    #endregion

}
