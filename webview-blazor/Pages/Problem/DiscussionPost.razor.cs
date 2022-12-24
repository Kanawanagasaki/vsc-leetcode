namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class DiscussionPost : ComponentBase
{
    [Inject] public required ProblemsService Service { get; init; }
    [Inject] public required JsService Js { get; init; }

    [Parameter, EditorRequired] public required DiscussionModel Model { get; init; }

    private ElementReference? _bodyRef;

    private bool _isLoadingReplies = false;
    private DiscussionReplyModel[]? _replies;
    private string _htmlContent = "";

    protected override void OnInitialized()
        => _htmlContent = Markdig.Markdown.ToHtml(Model.Post.Content?.Replace("\\n", "\n")?.Replace("\\t", "\t") ?? "");

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        if (_bodyRef is null)
            return;
        if (_htmlContent.Contains("<pre>") && _htmlContent.Contains("<code>"))
            await Js.HighlightPreCode(_bodyRef.Value);
    }

    private async Task ShowReplies()
    {
        if (_isLoadingReplies)
            return;
        _isLoadingReplies = true;
        StateHasChanged();

        _replies = await Service.GetDiscussionReplies(Model.Id);

        _isLoadingReplies = false;
    }
}
