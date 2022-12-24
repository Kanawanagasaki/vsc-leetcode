namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public class FilterModel
{
    #region SearchKeywords
    public string? SearchKeywords { get; set; }
    public FilterModel WithSearchKeyword(string? searchKeywords)
    {
        SearchKeywords = searchKeywords;
        return this;
    }
    #endregion

    #region Sort
    private ESort _sort = ESort.IdAsc;
    public string? OrderBy
        => _sort switch
        {
            ESort.IdAsc or ESort.IdDesc => "FRONTEND_ID",
            ESort.AcceptanceAsc or ESort.AcceptanceDesc => "AC_RATE",
            ESort.DifficultyAsc or ESort.DifficultyDesc => "DIFFICULTY",
            _ => null
        };
    public string? SortOrder
        => _sort switch
        {
            ESort.IdAsc or ESort.AcceptanceAsc or ESort.DifficultyAsc => "ASCENDING",
            ESort.IdDesc or ESort.AcceptanceDesc or ESort.DifficultyDesc => "DESCENDING",
            _ => null
        };
    public FilterModel WithSort(ESort sort)
    {
        _sort = sort;
        return this;
    }
    public enum ESort
    {
        IdAsc, IdDesc, AcceptanceAsc, AcceptanceDesc, DifficultyAsc, DifficultyDesc
    }
    #endregion

    #region Difficulty
    private EDifficulty _difficulty = EDifficulty.None;
    public string? Difficulty
        => _difficulty switch
        {
            EDifficulty.Easy => "EASY",
            EDifficulty.Medium => "MEDIUM",
            EDifficulty.Hard => "HARD",
            _ => null
        };
    public FilterModel WithDifficulty(EDifficulty difficulty)
    {
        _difficulty = difficulty;
        return this;
    }
    public enum EDifficulty
    {
        None, Easy, Medium, Hard
    }
    #endregion

    #region Status
    private EStatus _status = EStatus.None;
    public string? Status
        => _status switch
        {
            EStatus.Todo => "NOT_STARTED",
            EStatus.Solved => "AC",
            EStatus.Attempted => "TRIED",
            _ => null
        };
    public FilterModel WithStatus(EStatus status)
    {
        _status = status;
        return this;
    }
    public enum EStatus
    {
        None, Todo, Solved, Attempted
    }
    #endregion

    #region PaidOnly
    public bool? PremiumOnly { get; set; }
    public FilterModel WithPaidOnly(bool? paidOnly)
    {
        PremiumOnly = paidOnly;
        return this;
    }
    #endregion

    #region Tags
    public string[]? Tags { get; set; } = null;
    public FilterModel WithTags(string[]? tags)
    {
        if (tags is not null && tags.Length == 0)
            Tags = null;
        else
            Tags = tags;
        return this;
    }
    #endregion

    #region ListId
    public string? ListId { get; set; }
    public FilterModel WithFeaturedList(string? listId)
    {
        ListId = listId;
        return this;
    }
    #endregion

    public override string ToString()
        => string.Join(",", new string[]
        {
            SearchKeywords ?? "",
            OrderBy ?? "",
            SortOrder ?? "",
            Difficulty ?? "",
            Status ?? "",
            PremiumOnly?.ToString() ?? "",
            Tags is null ? "" : string.Join("|", Tags.OrderBy(x => x)),
            ListId ?? ""
        });
}
