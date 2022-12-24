namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public partial class Description : AProblemChildComponent
{
    private bool _showCodeDropdown = false;
    private List<int> _shownHints = new();

    protected override async Task RequestProblemDetails(ProblemModel problem)
        => await Task.WhenAll(new[]
        {
            problem.RequestTitle(Js),
            problem.RequestContent(Js),
            problem.RequestStats(Js),
            problem.RequestSimilar(Js),
            problem.RequestTags(Js),
            problem.RequestEditorData(Js),
            problem.RequestHints(Js)
        });
    
    protected override void OnDetailUpdate(ProblemModel.EDetails details)
    {
        if (details == ProblemModel.EDetails.Hints)
            _shownHints.Clear();
        base.OnDetailUpdate(details);
    }

    private async Task Solve(string langSlug)
    {
        if (Parent.Problem?.TitleSlug is null)
            return;
        await Js.Solve(Parent.Problem.TitleSlug, langSlug);
    }

    private async Task OpenProblem(string titleSlug)
        => await Js.OpenProblemWebview(titleSlug);
}
