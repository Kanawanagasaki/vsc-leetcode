namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using System.Threading.Tasks;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Markdig;
using Microsoft.AspNetCore.Components;

public partial class CommunitySolutionComment : ComponentBase
{
    [Inject] public required ProblemsService Service { get; init; }
    [Inject] public required JsService Js { get; init; }
    [Parameter, EditorRequired] public required CommunitySolutionCommentModel Comment { get; init; }

    private string _contentHtml = "";
    private ElementReference? _body { get; set; }

    private bool _isLoadingReplies = false;
    private CommunitySolutionCommentReplyModel[]? _replies;

    protected override void OnInitialized()
    {
        _contentHtml = Markdown.ToHtml(Comment.Post.Content?.Replace("\\n", "\n")?.Replace("\\t", "\t") ?? "");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        if (_body is null)
            return;

        if (_contentHtml.Contains("<pre>") && _contentHtml.Contains("<code>"))
            await Js.HighlightPreCode(_body.Value);
    }

    private async Task ShowReplies()
    {
        if (_isLoadingReplies)
            return;
        _isLoadingReplies = true;
        StateHasChanged();

        _replies = await Service.GetCommunitySolutionCommentReplies(Comment.Id);

        _isLoadingReplies = false;
    }
}
