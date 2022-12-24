using System;

namespace Kanawanagasaki.VSCode.LeetCode.WebView.Models;

public record UuidResult<T> where T : class
{
    public required Guid Uuid { get; init; }
    public required T Result { get; init; }
}
