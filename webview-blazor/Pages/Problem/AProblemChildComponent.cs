namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public abstract class AProblemChildComponent : ComponentBase, IDisposable
{
    [Inject] public required JsService Js { get; init; }
    [Inject] public required AuthService Auth { get; init; }
    [Inject] public required StoragePathService StoragePath { get; init; }
    [Inject] public required SettingsService Settings { get; init; }

    [CascadingParameter] public required ProblemPage Parent { get; init; }

    protected override async Task OnInitializedAsync()
    {
        if (Parent.Problem is not null)
        {
            Parent.Problem.OnDetailUpdate += OnDetailUpdate;
            await RequestProblemDetails(Parent.Problem);
        }
        Parent.OnProblemChange += OnProblemChange;
    }

    protected virtual void OnProblemChange()
        => InvokeAsync(async () =>
        {
            if (Parent.Problem is not null)
            {
                Parent.Problem.OnDetailUpdate += OnDetailUpdate;
                await RequestProblemDetails(Parent.Problem);
            }
            StateHasChanged();
        });

    protected virtual void OnDetailUpdate(ProblemModel.EDetails details)
        => StateHasChanged();

    public virtual void Dispose()
    {
        if (Parent.Problem is not null)
            Parent.Problem.OnDetailUpdate -= OnDetailUpdate;
        Parent.OnProblemChange -= OnProblemChange;
    }

    protected abstract Task RequestProblemDetails(ProblemModel problem);
}
