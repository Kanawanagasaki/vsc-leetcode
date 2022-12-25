namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using System.Threading.Tasks;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class ProblemPage : ComponentBase
{
    [Inject] public required JsService Js { get; init; }
    [Inject] public required AuthService Auth { get; init; }
    [Inject] public required SettingsService Settings { get; init; }
    [Inject] public required StoragePathService StoragePath { get; init; }

    public ProblemModel? Problem { get; private set; }
    public event Action? OnProblemChange;

    private ActiveTextEditorModel? _activeTextEditor;
    private long? _communitySolutionTopicId = null;

    protected override async Task OnInitializedAsync()
    {
        var problemTitleSlug = Js.ProblemTitleSlug;
        if (problemTitleSlug is not null)
        {
            Problem = new ProblemModel(problemTitleSlug);
            Problem.OnDetailUpdate += Problem_OnDetailUpdate;
            OnProblemChange?.Invoke();
        }

        JsService.OnChangeActiveTextEditor += JsService_OnChangeActiveTextEditor;
        JsService.OnUpdateProblem += JsService_OnUpdateProblem;
        Settings.OnChange += Settings_OnChange;
        StoragePath.OnStateChange += StoragePath_OnStateChange;
        await Js.RequestActiveTextEditor();

        await RequestProblemDetails();
    }

    private async Task RequestProblemDetails()
    {
        if (Problem is null)
            return;

        var state = await Js.GetState();
        if (state is null)
            state = new();
        state.ProblemTitleSlug = Problem.TitleSlug;

        await Task.WhenAll(new[]
        {
            Problem.RequestSolutions(Js, Array.Empty<string>(), Array.Empty<string>(), 0),
            Problem.RequestDiscussionTopics(Js, 0),
            Js.SetState(state)
        });
    }

    private void Problem_OnDetailUpdate(ProblemModel.EDetails _)
        => StateHasChanged();

    private void JsService_OnUpdateProblem(string titleSlug)
    {
        if (Problem is not null)
        {
            Problem.OnDetailUpdate -= Problem_OnDetailUpdate;
            Problem.Dispose();
        }
        Problem = new ProblemModel(titleSlug);
        Problem.OnDetailUpdate += Problem_OnDetailUpdate;
        OnProblemChange?.Invoke();
        StateHasChanged();
        InvokeAsync(RequestProblemDetails);
    }

    private void Settings_OnChange(SettingsModel settings)
        => StateHasChanged();

    private void StoragePath_OnStateChange(StoragePathModel? storagePath)
        => StateHasChanged();

    private void JsService_OnChangeActiveTextEditor(ActiveTextEditorModel? activeTextEditor)
    {
        _activeTextEditor = activeTextEditor;
        StateHasChanged();
    }

    public void ShowCommunitySolution(long? topicId)
    {
        _communitySolutionTopicId = topicId;
        StateHasChanged();
    }

    public void Dispose()
    {
        if (Problem is not null)
        {
            Problem.OnDetailUpdate -= Problem_OnDetailUpdate;
            Problem.Dispose();
        }
        JsService.OnChangeActiveTextEditor -= JsService_OnChangeActiveTextEditor;
        JsService.OnUpdateProblem -= JsService_OnUpdateProblem;
        Settings.OnChange -= Settings_OnChange;
        StoragePath.OnStateChange -= StoragePath_OnStateChange;
    }
}
