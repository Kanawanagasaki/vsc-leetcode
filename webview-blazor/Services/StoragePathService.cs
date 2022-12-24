using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Services;

public class StoragePathService
{
    public StoragePathModel? Model { get; private set; }

    public event Action<StoragePathModel?>? OnStateChange;

    private JsService _js;

    public StoragePathService(JsService js)
        => _js = js;

    public async Task Init()
    {
        JsService.OnStoragePath += model =>
        {
            Model = model;
            OnStateChange?.Invoke(Model);
        };
        await _js.RequestStoragePath();
    }
}
