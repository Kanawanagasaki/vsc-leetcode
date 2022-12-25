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
    public string? OrderBy { get; set; }
    public string? SortOrder { get; set; }
    public FilterModel WithSort(ESort sort)
    {
        OrderBy = sort switch
        {
            ESort.IdAsc or ESort.IdDesc => "FRONTEND_ID",
            ESort.AcceptanceAsc or ESort.AcceptanceDesc => "AC_RATE",
            ESort.DifficultyAsc or ESort.DifficultyDesc => "DIFFICULTY",
            _ => null
        };
        SortOrder = sort switch
        {
            ESort.IdAsc or ESort.AcceptanceAsc or ESort.DifficultyAsc => "ASCENDING",
            ESort.IdDesc or ESort.AcceptanceDesc or ESort.DifficultyDesc => "DESCENDING",
            _ => null
        };
        return this;
    }
    public ESort GetSort()
        => (OrderBy, SortOrder) switch
        {
            ("FRONTEND_ID", "ASCENDING") => ESort.IdAsc,
            ("FRONTEND_ID", "DESCENDING") => ESort.IdDesc,
            ("AC_RATE", "ASCENDING") => ESort.AcceptanceAsc,
            ("AC_RATE", "DESCENDING") => ESort.AcceptanceDesc,
            ("DIFFICULTY", "ASCENDING") => ESort.DifficultyAsc,
            ("DIFFICULTY", "DESCENDING") => ESort.DifficultyDesc,
            _ => ESort.IdAsc
        };
    public enum ESort
    {
        IdAsc, IdDesc, AcceptanceAsc, AcceptanceDesc, DifficultyAsc, DifficultyDesc
    }
    #endregion

    #region Difficulty
    public string? Difficulty { get; set; }
    public FilterModel WithDifficulty(EDifficulty difficulty)
    {
        Difficulty = difficulty switch
        {
            EDifficulty.Easy => "EASY",
            EDifficulty.Medium => "MEDIUM",
            EDifficulty.Hard => "HARD",
            _ => null
        };
        return this;
    }
    public EDifficulty GetDifficulty()
        => Difficulty switch
        {
            "EASY" => EDifficulty.Easy,
            "MEDIUM" => EDifficulty.Medium,
            "HARD" => EDifficulty.Hard,
            _ => EDifficulty.None
        };
    public enum EDifficulty
    {
        None, Easy, Medium, Hard
    }
    #endregion

    #region Status
    public string? Status { get; set; }
    public FilterModel WithStatus(EStatus status)
    {
        Status = status switch
        {
            EStatus.Todo => "NOT_STARTED",
            EStatus.Solved => "AC",
            EStatus.Attempted => "TRIED",
            _ => null
        };
        return this;
    }
    public EStatus GetStatus()
        => Status switch
        {
            "NOT_STARTED" => EStatus.Todo,
            "AC" => EStatus.Solved,
            "TRIED" => EStatus.Attempted,
            _ => EStatus.None
        };
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
