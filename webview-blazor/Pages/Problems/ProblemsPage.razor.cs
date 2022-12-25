namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages.Problems;

using System.Threading.Tasks;
using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

public partial class ProblemsPage : ComponentBase, IDisposable
{
    [Inject] public required ProblemsService Service { get; init; }
    [Inject] public required PagePropsService PageProps { get; init; }
    [Inject] public required AuthService Auth { get; init; }
    [Inject] public required JsService Js { get; init; }

    private Virtualize<ProblemSetItemModel>? _virtualizeRef;

    private int _totalProblems = 0;

    private bool _isLoading = true;
    private bool _isSortShown = false;
    private bool _isFiltersShown = false;

    private string _searchKeywords = "";
    private FilterModel.ESort _sort = FilterModel.ESort.IdAsc;
    private FilterModel.EDifficulty _difficulty = FilterModel.EDifficulty.None;
    private FilterModel.EStatus _status = FilterModel.EStatus.None;
    private string? _categorySlug = null;
    private List<string> _tags = new();
    private string? _featuredList = null;
    private bool _getCache = true;

    protected override async Task OnInitializedAsync()
    {
        PageProps.OnStateChange += PageProps_OnStateChange;
        JsService.OnSubmissionStateUpdate += JsService_OnSubmissionStateUpdate;

        var state = await Js.GetState();
        Console.WriteLine(state);
        if (state?.LastFilter is not null)
        {
            _searchKeywords = state.LastFilter.SearchKeywords ?? "";
            _sort = state.LastFilter.GetSort();
            _difficulty = state.LastFilter.GetDifficulty();
            _status = state.LastFilter.GetStatus();
            _tags = state.LastFilter.Tags?.ToList() ?? new();
            _featuredList = state.LastFilter.ListId;
        }
        if (state?.LastCategorySlug is not null)
            _categorySlug = state.LastCategorySlug;
    }

    private void PageProps_OnStateChange()
        => StateHasChanged();

    private void JsService_OnSubmissionStateUpdate(SubmissionStateUpdateModel? state)
    {
        if (state is null)
            return;
        if (state.State.RunSuccess is null)
            return;
        _getCache = false;
        InvokeAsync(RefreshProblems);
    }

    private async ValueTask<ItemsProviderResult<ProblemSetItemModel>> ProblemsProvider(ItemsProviderRequest req)
    {
        _isLoading = true;
        StateHasChanged();

        var ret = new List<ProblemSetItemModel>();
        int endIndex = req.StartIndex + req.Count - 1;
        int endPage = endIndex / 50;
        for (int page = req.StartIndex / 50; page <= endPage; page++)
        {
            var filters = new FilterModel()
                .WithSearchKeyword(string.IsNullOrWhiteSpace(_searchKeywords) ? null : _searchKeywords)
                .WithSort(_sort)
                .WithDifficulty(_difficulty)
                .WithStatus(_status)
                .WithTags(_tags.Count == 0 ? null : _tags.ToArray())
                .WithFeaturedList(_featuredList);
            var set = await Service.GetProblems(_categorySlug ?? "", page, filters, _getCache);
            var state = await Js.GetState();
            if (state is null)
                state = new();
            state.LastFilter = filters;
            state.LastCategorySlug = _categorySlug;
            await Js.SetState(state);

            _totalProblems = set.Total;
            var startInnerIndex = set.Skip < req.StartIndex ? req.StartIndex - set.Skip : 0;
            var endInnerIndex = set.Skip + set.Questions.Length - 1 < endIndex ? set.Questions.Length - 1 : endIndex - set.Skip;
            for (int i = startInnerIndex; i <= endInnerIndex; i++)
                ret.Add(set.Questions[i]);
        }

        if (!_getCache)
            _getCache = true;
        _isLoading = false;
        StateHasChanged();

        return new(ret, _totalProblems);
    }

    private async Task RefreshProblems()
    {
        if (_virtualizeRef is not null)
            await _virtualizeRef.RefreshDataAsync();
        StateHasChanged();
    }

    private async Task OnSearchChange(ChangeEventArgs args)
    {
        _searchKeywords = args.Value?.ToString() ?? "";
        await RefreshProblems();
    }

    private void ShowSort()
    {
        _isFiltersShown = false;
        _isSortShown = !_isSortShown;
    }

    private async Task OnSortChange(ChangeEventArgs args)
    {
        if (Enum.TryParse<FilterModel.ESort>(args.Value?.ToString(), out var sort) && sort != _sort)
        {
            _sort = sort;
            await RefreshProblems();
        }
    }

    private void ShowFilter()
    {
        _isSortShown = false;
        _isFiltersShown = !_isFiltersShown;
    }

    private async Task OnDifficultyChange(FilterModel.EDifficulty difficulty)
    {
        if (_difficulty != difficulty)
        {
            _difficulty = difficulty;
            await RefreshProblems();
        }
    }

    private async Task OnStatusChange(FilterModel.EStatus status)
    {
        if (_status != status)
        {
            _status = status;
            await RefreshProblems();
        }
    }

    private async Task OnCategoryChange(string? categorySlug)
    {
        if (_categorySlug != categorySlug)
        {
            _categorySlug = categorySlug;
            await RefreshProblems();
        }
    }

    private async Task OnTagAdd(string tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
            await RefreshProblems();
        }
    }

    private async Task OnTagRemove(string tag)
    {
        if (_tags.Contains(tag))
        {
            _tags.Remove(tag);
            await RefreshProblems();
        }
    }

    private async Task OnFeaturedListChange(string? featuredList)
    {
        if (_featuredList != featuredList)
        {
            _featuredList = featuredList;
            await RefreshProblems();
        }
    }

    public void Dispose()
    {
        PageProps.OnStateChange -= PageProps_OnStateChange;
        JsService.OnSubmissionStateUpdate -= JsService_OnSubmissionStateUpdate;
    }
}
