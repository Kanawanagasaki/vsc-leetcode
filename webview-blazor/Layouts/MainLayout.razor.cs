namespace Kanawanagasaki.VSCode.LeetCode.WebView.Layouts;

using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] public required JsService Js { get; init; }

    private bool _shouldDrowShadow = false;
    private ElementReference _bodyRef;

    public async Task OnScroll()
    {
        var scrollTop = await Js.GetScrollTop(_bodyRef);
        _shouldDrowShadow = scrollTop > 0;
    }
}
