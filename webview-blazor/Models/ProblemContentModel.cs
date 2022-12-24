namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ProblemContentModel
{
    public required string Content { get; init; }
    public required string[] MysqlSchemas { get; init; }
}
