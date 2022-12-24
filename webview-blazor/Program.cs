using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Kanawanagasaki.VSCode.LeetCode.WebView;
using Kanawanagasaki.VSCode.LeetCode.WebView.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<JsService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<StoragePathService>();
builder.Services.AddSingleton<ProblemsService>();
builder.Services.AddSingleton<PagePropsService>();
builder.Services.AddSingleton<SettingsService>();

await builder.Build().RunAsync();
