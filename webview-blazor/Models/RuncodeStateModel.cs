namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record RuncodeStateModel
{
    public required string State { get; init; }
    public bool? RunSuccess { get; init; }
    public string? StatusRuntime { get; init; }
    public string? StatusMemory { get; init; }
    public string? StatusMessage { get; init; }
    public string[]? CodeAnswer { get; init; }
    public string[]? StdOutput { get; init; }
    public int? ElapsedTime { get; init; }
    public string[]? ExpectedCodeAnswer { get; init; }
    public bool? CorrectAnswer { get; init; }
    public int? TotalCorrect { get; init; }
    public int? TotalTestcases { get; init; }
    public string? CompileError { get; init; }
    public string? FullCompileError { get; init; }
    public string? RuntimeError { get; init; }
    public string? FullRuntimeError { get; init; }
}
