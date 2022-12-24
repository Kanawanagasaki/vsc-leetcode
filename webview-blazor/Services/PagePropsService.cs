using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Services;

public class PagePropsService
{
    public PagePropsModel? Props { get; private set; } = null;
    public event Action? OnStateChange;

    private JsService _js;

    public PagePropsService(JsService js)
    {
        _js = js;
        JsService.OnPageProps += JsService_OnPageProps;
    }

    public async Task Init()
        => await _js.RequestPageProps();

    private void JsService_OnPageProps(PagePropsModel props)
    {
        Props = props;
        OnStateChange?.Invoke();
    }
}
