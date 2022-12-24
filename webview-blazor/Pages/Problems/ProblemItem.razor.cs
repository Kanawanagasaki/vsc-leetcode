namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problems;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class ProblemItem : ComponentBase, IDisposable
{
    [Inject] public required AuthService Auth { get; init; }
    [Inject] public required SettingsService Settings { get; init; }
    [Inject] public required JsService Js { get; init; }
    [Parameter] public ProblemSetItemModel? Model { get; init; }

    protected override void OnInitialized()
    {
        Settings.OnChange += Settings_OnChange;
    }

    private void Settings_OnChange(SettingsModel model)
        => StateHasChanged();

    private async Task OpenWebview()
    {
        if (Model is null)
            return;
        await Js.OpenProblemWebview(Model.TitleSlug);
    }

    public void Dispose()
    {
        Settings.OnChange -= Settings_OnChange;
    }
}
