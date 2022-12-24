namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class RunCode : AProblemChildComponent
{
    [Inject] public required ProblemsService Service { get; init; }

    private Dictionary<Guid, string> _testcases = new();
    private RuncodeStateUpdateModel? _runcodeState;
    private SubmissionStateUpdateModel? _submissionState;

    protected override async Task OnInitializedAsync()
    {
        JsService.OnRuncodeStateUpdate += JsService_OnRuncodeStateUpdate;
        JsService.OnSubmissionStateUpdate += JsService_OnSubmissionStateUpdate;

        await base.OnInitializedAsync();
    }

    protected override async Task RequestProblemDetails(ProblemModel problem)
        => await problem.RequestConsoleConfig(Js);

    private void JsService_OnRuncodeStateUpdate(RuncodeStateUpdateModel? state)
    {
        _runcodeState = state;
        StateHasChanged();
    }

    private void JsService_OnSubmissionStateUpdate(SubmissionStateUpdateModel? state)
    {
        _submissionState = state;
        StateHasChanged();
    }

    private void AddTestcase()
    {
        _testcases.Add(Guid.NewGuid(), "");
        _runcodeState = null;
    }

    private void ChangeTestcase(Guid key, string value)
    {
        _testcases[key] = value;
        _runcodeState = null;
    }

    private void RemoveTestcase(Guid key)
    {
        _testcases.Remove(key);
        _runcodeState = null;
    }

    private async Task Run()
    {
        if (Parent.Problem is null)
            return;
        if (Parent.Problem.ConsoleConfig is null)
            return;
        if (Service.IsRunningCode)
            return;

        _runcodeState = null;
        await Service.Run(Parent.Problem.TitleSlug, string.Join("\n", Parent.Problem.ConsoleConfig.ExampleTestcaseList.Concat(_testcases.Values)));
    }

    private async Task Submit()
    {
        if (Parent.Problem is null)
            return;
        if (Service.IsRunningCode)
            return;

        await Service.Submit(Parent.Problem.TitleSlug);
        await Js.ClickOnEl("tab-submissions");
    }

    public override void Dispose()
    {
        JsService.OnRuncodeStateUpdate -= JsService_OnRuncodeStateUpdate;
        JsService.OnSubmissionStateUpdate -= JsService_OnSubmissionStateUpdate;
        base.Dispose();
    }
}
