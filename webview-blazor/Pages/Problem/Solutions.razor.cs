using Kanawanagasaki.VSCode.LeetCode.WebView.Models;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problem;

public partial class Solutions : AProblemChildComponent
{
    private HashSet<string> _solutionsSelectedTags = new();
    private string? _solutionsSelectedLanguage = null;
    private int _page = 0;

    protected override async Task RequestProblemDetails(ProblemModel problem)
        => await Task.WhenAll(new[]
        {
            problem.RequestSolutions
            (
                Js,
                _solutionsSelectedLanguage is null ? Array.Empty<string>() : new[] { _solutionsSelectedLanguage },
                _solutionsSelectedTags.ToArray(),
                _page * 15
            ),
            problem.RequestSolutionTags(Js)
        });

    private async Task OnSolutionsTagSelected(string tagSlug)
    {
        _solutionsSelectedTags.Add(tagSlug);

        if (Parent.Problem is null)
            return;

        await Parent.Problem.RequestSolutions
        (
            Js,
            _solutionsSelectedLanguage is null ? Array.Empty<string>() : new[] { _solutionsSelectedLanguage },
            _solutionsSelectedTags.ToArray(),
            _page * 15
        );
    }

    private async Task OnSolutionsTagUnselected(string tagSlug)
    {
        _solutionsSelectedTags.Remove(tagSlug);

        if (Parent.Problem is null)
            return;

        await Parent.Problem.RequestSolutions
        (
            Js,
            _solutionsSelectedLanguage is null ? Array.Empty<string>() : new[] { _solutionsSelectedLanguage },
            _solutionsSelectedTags.ToArray(),
            _page * 15
        );
    }

    private async Task OnSolutionsLanguageChange(string? language)
    {
        _solutionsSelectedLanguage = language;

        if (Parent.Problem is null)
            return;

        await Parent.Problem.RequestSolutions
        (
            Js,
            _solutionsSelectedLanguage is null ? Array.Empty<string>() : new[] { _solutionsSelectedLanguage },
            _solutionsSelectedTags.ToArray(),
            _page * 15
        );
    }

    private async Task NextPage()
    {
        if (Parent.Problem is null)
            return;

        _page++;
        await Parent.Problem.RequestSolutions
        (
            Js,
            _solutionsSelectedLanguage is null ? Array.Empty<string>() : new[] { _solutionsSelectedLanguage },
            _solutionsSelectedTags.ToArray(),
            _page * 15
        );
        StateHasChanged();
    }

    private async Task PrevPage()
    {
        if (Parent.Problem?.Discussions is null)
            return;

        _page--;
        await Parent.Problem.RequestSolutions
        (
            Js,
            _solutionsSelectedLanguage is null ? Array.Empty<string>() : new[] { _solutionsSelectedLanguage },
            _solutionsSelectedTags.ToArray(),
            _page * 15
        );
        StateHasChanged();
    }

    private void ShowSolution(long solutionId)
        => Parent.ShowCommunitySolution(solutionId);
}
