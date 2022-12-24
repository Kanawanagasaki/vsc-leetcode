namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using System.Threading.Tasks;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class Submissions : AProblemChildComponent
{
    [Inject] public required ProblemsService Service { get; init; }

    private SubmissionStateUpdateModel? _submissionState;

    protected override async Task OnInitializedAsync()
    {
        JsService.OnSubmissionStateUpdate += JsService_OnSubmissionStateUpdate;
        Service.OnBeforeSubmit += Service_OnBeforeSubmit;
        Parent.OnProblemChange += Parent_OnProblemChange;

        await base.OnInitializedAsync();
    }

    protected override async Task RequestProblemDetails(ProblemModel problem)
        => await problem.RequestSubmissions(Js, 0);

    private void JsService_OnSubmissionStateUpdate(SubmissionStateUpdateModel? state)
    {
        _submissionState = state;
        if (_submissionState?.State.State != "PENDING" && _submissionState?.State.State != "STARTED" && Parent.Problem?.TitleSlug is not null)
            InvokeAsync(async () => await Parent.Problem.RequestSubmissions(Js, 0, false));
        StateHasChanged();
    }

    private void Service_OnBeforeSubmit()
    {
        _submissionState = null;
        StateHasChanged();
    }

    private void Parent_OnProblemChange()
    {
        _submissionState = null;
        StateHasChanged();
    }

    public override void Dispose()
    {
        JsService.OnSubmissionStateUpdate -= JsService_OnSubmissionStateUpdate;
        Service.OnBeforeSubmit -= Service_OnBeforeSubmit;
        Parent.OnProblemChange -= Parent_OnProblemChange;
        base.Dispose();
    }
}
