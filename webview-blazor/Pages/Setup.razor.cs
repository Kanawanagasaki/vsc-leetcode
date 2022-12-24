namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages;

using System.Threading.Tasks;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class Setup : ComponentBase, IDisposable
{
    [Inject] public required AuthService Auth { get; init; }
    [Inject] public required JsService Js { get; init; }
    [Inject] public required StoragePathService StoragePath { get; init; }

    protected override async Task OnInitializedAsync()
    {
        Auth.OnStateChange += Auth_OnStateChange;
        StoragePath.OnStateChange += StoragePath_OnUpdate;
        await Js.RequestStoragePath();
    }

    private void Auth_OnStateChange(AuthService.AuthState state)
        => StateHasChanged();

    private void StoragePath_OnUpdate(StoragePathModel? model)
        => StateHasChanged();

    public void Dispose()
    {
        Auth.OnStateChange -= Auth_OnStateChange;
        StoragePath.OnStateChange -= StoragePath_OnUpdate;
    }
}
