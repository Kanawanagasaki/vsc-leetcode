namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record PagePropsModel
{
    public CategoryModel[] ProblemsetCategories { get; set; } = Array.Empty<CategoryModel>();
    public FeaturedListModel[] FeaturedLists { get; set; } = Array.Empty<FeaturedListModel>();
    public TagModel[] TopicTags { get; set; } = Array.Empty<TagModel>();
}
