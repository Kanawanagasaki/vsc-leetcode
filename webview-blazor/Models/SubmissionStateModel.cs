namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record SubmissionStateModel
{
    public required string State { get; init; }
    public int? StatusCode { get; init; }
    public string? Lang { get; init; }
    public bool? RunSuccess { get; init; }
    public string? StatusRuntime { get; init; }
    public int? Memory { get; init; }
    public string? QuestionId { get; init; }
    public int? ElapsedTime { get; init; }
    public string? CompareResult { get; init; }
    public string? CodeOutput { get; init; }
    public string? StdOutput { get; init; }
    public string? LastTestcase { get; init; }
    public string? ExpectedOutput { get; init; }
    public long? TaskFinishTime { get; init; }
    public int? TotalCorrect { get; init; }
    public int? TotalTestcases { get; init; }
    public float? RuntimePercentile { get; init; }
    public string? StatusMemory { get; init; }
    public float? MemoryPercentile { get; init; }
    public string? PrettyLang { get; init; }
    public string? SubmissionId { get; init; }
    public string? StatusMessage { get; init; }
    public string? CompileError { get; init; }
    public string? FullCompileError { get; init; }
    public string? RuntimeError { get; init; }
    public string? FullRuntimeError { get; init; }
}
