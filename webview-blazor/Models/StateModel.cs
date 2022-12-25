namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record StateModel
{
    public string? ProblemTitleSlug { get; set; }
    public FilterModel? LastFilter { get; set; }
    public string? LastCategorySlug { get; set; }
}
