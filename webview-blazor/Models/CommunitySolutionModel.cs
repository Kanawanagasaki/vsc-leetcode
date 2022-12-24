namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record CommunitySolutionModel
{
    public required bool IsSolutionTopic { get; init; }
    public required CommunitySolutionTopicModel Topic { get; init; }
}
