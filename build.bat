rmdir .\media /s /q
mkdir .\media
xcopy /s /y /e .\src\media .\media > nul
dotnet build webview-blazor/Kanawanagasaki.VSCode.LeetCode.WebView.csproj -p:BlazorEnableCompression=false
xcopy /s /y /e .\webview-blazor\bin\Debug\net7.0\wwwroot .\media > nul
xcopy /s /y /e .\webview-blazor\wwwroot .\media > nul
echo f | xcopy /y /f .\webview-blazor\obj\Debug\net7.0\scopedcss\bundle\Kanawanagasaki.VSCode.LeetCode.WebView.styles.css .\media\css\Kanawanagasaki.VSCode.LeetCode.WebView.styles.css > nul
