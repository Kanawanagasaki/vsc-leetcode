namespace Kanawanagasaki.VSCode.LeetCode.WebView;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class App : IDisposable
{
    [Inject] public required JsService Js { get; init; }
    [Inject] public required AuthService Auth { get; init; }
    [Inject] public required StoragePathService StoragePath { get; init; }
    [Inject] public required PagePropsService PageProps { get; init; }
    [Inject] public required SettingsService Settings { get; init; }

    public bool IsSettingsShown { get; private set; } = false;
    private string? _webviewContext;
    private bool _isChromiumInstalled = false;
    private DownloadChromiumProgressModel? _chromiumProgress = null;
    private string? _chromiumError = null;

    protected override async Task OnInitializedAsync()
    {
        _webviewContext = Js.Context;

        Auth.OnStateChange += _ => StateHasChanged();
        StoragePath.OnStateChange += _ => StateHasChanged();

        JsService.OnDownloadChromiumProgress += JsService_OnDownloadChromiumProgress;
        JsService.OnDownloadChromiumSuccess += JsService_OnDownloadChromiumSuccess;
        JsService.OnDownloadChromiumFail += JsService_OnDownloadChromiumFail;

        await Js.Init();
        await Auth.Init();
        await StoragePath.Init();
        await PageProps.Init();
        await Settings.Init();

        await Js.RequestDownloadChromium();
    }

    private void JsService_OnDownloadChromiumProgress(DownloadChromiumProgressModel progress)
    {
        _chromiumProgress = progress;
        StateHasChanged();
    }

    private void JsService_OnDownloadChromiumSuccess()
    {
        _isChromiumInstalled = true;
        StateHasChanged();
    }

    private void JsService_OnDownloadChromiumFail(string error)
    {
        _chromiumError = error;
        StateHasChanged();
    }

    public void ShowSettings()
    {
        IsSettingsShown = true;
        StateHasChanged();
    }

    public void HideSettings()
    {
        IsSettingsShown = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        JsService.OnDownloadChromiumProgress -= JsService_OnDownloadChromiumProgress;
        JsService.OnDownloadChromiumSuccess -= JsService_OnDownloadChromiumSuccess;
        JsService.OnDownloadChromiumFail -= JsService_OnDownloadChromiumFail;
    }
}
