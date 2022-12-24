namespace Kanawanagasaki.VSCode.LeetCode.WebView.Pages;

using Kanawanagasaki.VSCode.LeetCode.WebView.Models;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;
using Microsoft.AspNetCore.Components;

public partial class Settings : ComponentBase, IDisposable
{
    private static readonly IReadOnlyDictionary<string, string> KEY_VALUE_LANG = new Dictionary<string, string>
    {
        { "", "-" },
        { "cpp", "C++" },
        { "java", "Java" },
        { "python", "Python" },
        { "python3", "Python3" },
        { "mysql", "MySQL" },
        { "mssql", "MS SQL Server" },
        { "oraclesql", "Oracle" },
        { "c", "C" },
        { "csharp", "C#" },
        { "javascript", "JavaScript" },
        { "ruby", "Ruby" },
        { "bash", "Bash" },
        { "swift", "Swift" },
        { "golang", "Go" },
        { "scala", "Scala" },
        { "html", "HTML" },
        { "pythonml", "Python ML" },
        { "kotlin", "Kotlin" },
        { "rust", "Rust" },
        { "php", "PHP" },
        { "typescript", "TypeScript" },
        { "racket", "Racket" },
        { "erlang", "Erlang" },
        { "elixir", "Elixir" },
        { "dart", "Dart" }
    };

    [Inject] public required SettingsService Service { get; init; }
    [Inject] public required StoragePathService StoragePath { get; init; }
    [Inject] public required JsService Js { get; init; }

    protected override void OnInitialized()
    {
        Service.OnChange += Service_OnChange;
        StoragePath.OnStateChange += StoragePath_OnStateChange;
    }

    private void Service_OnChange(SettingsModel model)
        => StateHasChanged();

    private void StoragePath_OnStateChange(StoragePathModel? model)
        => StateHasChanged();

    private async Task ToggleShowTagsOnHover()
        => await Service.SetShowTagsOnHover(!Service.Model.ShowTagsOnHover);

    private async Task OnDefaultLanguageChange(ChangeEventArgs args)
        => await Service.SetDefaultLanguage(args.Value?.ToString() ?? "");

    public void Dispose()
    {
        Service.OnChange -= Service_OnChange;
        StoragePath.OnStateChange -= StoragePath_OnStateChange;
    }
}
