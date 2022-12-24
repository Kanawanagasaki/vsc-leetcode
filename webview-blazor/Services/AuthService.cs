namespace Kanawanagasaki.VSCode.LeetCode.WebView.Services;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public class AuthService
{
    public AuthState State { get; private set; } = AuthState.Pending;
    public UserStatusModel? Status { get; private set; } = null;
    public event Action<AuthState>? OnStateChange;

    private JsService _js;

    public AuthService(JsService js)
        => _js = js;

    public async Task Init()
    {
        JsService.OnUserStatusUpdate += status =>
        {
            Status = status;
            State = status is not null && status.IsSignedIn ? AuthState.Authorized : AuthState.NotAuthorized;
            OnStateChange?.Invoke(State);
        };
        await _js.RequestUserStatus();
    }

    public async Task SignIn()
    {
        State = AuthState.Pending;
        OnStateChange?.Invoke(State);
        await _js.RequestSignin();
    }

    public async Task SignOut()
    {
        State = AuthState.Pending;
        OnStateChange?.Invoke(State);
        await _js.RequestSignout();
    }

    public enum AuthState
    {
        Pending, Authorized, NotAuthorized
    }
}
