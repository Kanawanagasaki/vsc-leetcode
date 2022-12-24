namespace Kanawanagasaki.VSCode.LeetCode.WebView.Components;

using Microsoft.AspNetCore.Components;

public partial class Collapsible : ComponentBase
{
    [Parameter, EditorRequired] public required string Title { get; init; }
    [Parameter, EditorRequired] public required RenderFragment ChildContent { get; init; }

    private bool _isCollapsed = true;
}
