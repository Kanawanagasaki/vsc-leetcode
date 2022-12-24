namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record ConsoleConfigModel
{
    public required string QuestionId { get; init; }
    public required string QuestionFrontendId { get; init; }
    public required string QuestionTitle { get; init; }
    public required bool EnableDebugger { get; init; }
    public required bool EnableRunCode { get; init; }
    public required bool EnableSubmit { get; init; }
    public required bool EnableTestMode { get; init; }
    public required string[] ExampleTestcaseList { get; init; }
    public required string MetaData { get; init; }
}
