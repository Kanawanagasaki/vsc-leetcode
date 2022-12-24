namespace Kanawanagasaki.VSCode.LeetCode.WebView.Components;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class Header : ComponentBase, IDisposable
{
    [Inject] public required JsService Js { get; init; }
    [Inject] public required AuthService Auth { get; init; }
    [CascadingParameter] public required App App { get; init; }
    private Error? _signinError = null;
    private bool _isChromiumInstalled = false;

    protected override void OnInitialized()
    {
        Auth.OnStateChange += Auth_OnStateChange;
        JsService.OnSigninError += JsService_OnSigninError;
        JsService.OnDownloadChromiumSuccess += JsService_OnDownloadChromiumSuccess;
    }

    private void Auth_OnStateChange(AuthService.AuthState state)
    {
        if (state == AuthService.AuthState.Authorized)
            _signinError = null;
        StateHasChanged();
    }

    private void JsService_OnSigninError(Error? err)
    {
        _signinError = err;
        StateHasChanged();
    }

    private void JsService_OnDownloadChromiumSuccess()
    {
        _isChromiumInstalled = true;
        StateHasChanged();
    }

    private void OnSettingsClick()
    {
        if (App.IsSettingsShown)
            App.HideSettings();
        else
            App.ShowSettings();
    }

    private async Task SignOut()
    {
        await Auth.SignOut();
        _signinError = null;
    }

    public void Dispose()
    {
        Auth.OnStateChange -= Auth_OnStateChange;
        JsService.OnSigninError -= JsService_OnSigninError;
        JsService.OnDownloadChromiumSuccess -= JsService_OnDownloadChromiumSuccess;
    }
}
