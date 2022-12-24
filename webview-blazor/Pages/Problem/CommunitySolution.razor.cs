namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using System.Threading.Tasks;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Markdig;
using Microsoft.AspNetCore.Components;

public partial class CommunitySolution : AProblemChildComponent
{
    [Parameter, EditorRequired] public long TopicId { get; init; }

    private ElementReference? _contentRef { get; set; }
    private CommunitySolutionModel? _solution;
    private CommunitySolutionCommentListModel? _comments;
    private int _page = 1;
    private string _contentHtml = "";
    private bool _isRequesting = true;
    private bool _isRequestingComments = true;
    private bool _isHighlighted = false;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        JsService.OnCommunitySolution += JsService_OnCommunitySolution;
        JsService.OnCommunitySolutionComments += JsService_OnCommunitySolutionComments;

        _isRequesting = true;
        _isRequestingComments = true;
        await Js.RequestCommunitySolution(TopicId);
        await Js.RequestCommunitySolutionComments(TopicId, _page, 10, "best");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isHighlighted)
            return;
        if (_contentRef is null)
            return;
        if (_contentHtml.Contains("<pre>") && _contentHtml.Contains("<code>"))
        {
            await Js.HighlightPreCode(_contentRef.Value);
            _isHighlighted = true;
        }
    }

    protected override Task RequestProblemDetails(ProblemModel problem)
        => Task.CompletedTask;

    private void JsService_OnCommunitySolution(CommunitySolutionModel? solution)
    {
        _solution = solution;
        _isRequesting = false;
        if (solution is not null)
            _contentHtml = Markdown.ToHtml(solution.Topic.Post.Content?.Replace("\\n", "\n")?.Replace("\\t", "\t") ?? "");
        else
            _contentHtml = "";
        StateHasChanged();
    }

    private void JsService_OnCommunitySolutionComments(CommunitySolutionCommentListModel? comments)
    {
        _isRequestingComments = false;
        _comments = comments;
        StateHasChanged();
    }

    private async Task NextPage()
    {
        _page++;
        _isRequestingComments = true;
        await Js.RequestCommunitySolutionComments(TopicId, _page, 10, "best");
        StateHasChanged();
    }

    private async Task PrevPage()
    {
        _page--;
        _isRequestingComments = true;
        await Js.RequestCommunitySolutionComments(TopicId, _page, 10, "best");
        StateHasChanged();
    }

    private void Back()
        => Parent.ShowCommunitySolution(null);

    public override void Dispose()
    {
        base.Dispose();

        JsService.OnCommunitySolution -= JsService_OnCommunitySolution;
        JsService.OnCommunitySolutionComments -= JsService_OnCommunitySolutionComments;
    }
}
