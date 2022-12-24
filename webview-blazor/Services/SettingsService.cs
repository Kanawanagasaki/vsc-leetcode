using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Services;

public class SettingsService
{
    public SettingsModel Model { get; private set; } = new();

    public event Action<SettingsModel>? OnChange;

    private JsService _js;

    public SettingsService(JsService js)
        => _js = js;

    public async Task Init()
    {
        JsService.OnSettings += JsService_OnSettings;

        await _js.RequestSettings();
    }

    private void JsService_OnSettings(SettingsModel model)
    {
        Model = model;
        OnChange?.Invoke(Model);
    }

    public async Task SetShowTagsOnHover(bool flag)
        => await _js.SetSettings(Model with { ShowTagsOnHover = flag });

    public async Task SetDefaultLanguage(string lang)
        => await _js.SetSettings(Model with { DefaultLanguage = lang });
}
