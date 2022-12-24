using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

public partial class Discussions : AProblemChildComponent
{
    private DiscussionListModel? _list => Parent.Problem?.Discussions?.List;

    protected override async Task RequestProblemDetails(ProblemModel problem)
        => await problem.RequestDiscussionTopics(Js, 1);

    private async Task DiscussionNextPage()
    {
        if (Parent.Problem?.Discussions is null)
            return;
        var pageNo = Parent.Problem.Discussions.List.PageNo + 1;
        var numPerPage = Parent.Problem.Discussions.List.NumPerPage;
        StateHasChanged();
        await Parent.Problem.RequestDiscussionTopics(Js, pageNo, numPerPage);
    }

    private async Task DiscussionPrevPage()
    {
        if (Parent.Problem?.Discussions is null)
            return;
        var pageNo = Parent.Problem.Discussions.List.PageNo - 1;
        var numPerPage = Parent.Problem.Discussions.List.NumPerPage;
        StateHasChanged();
        await Parent.Problem.RequestDiscussionTopics(Js, pageNo, numPerPage);
    }
}
