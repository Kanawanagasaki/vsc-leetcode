rmdir .\media /s /q
mkdir .\media
xcopy /s /y /e .\src\media .\media > nul

rmdir .\webview-blazor\bin /s /q
rmdir .\webview-blazor\obj /s /q

dotnet publish webview-blazor/Kanawanagasaki.VSCode.LeetCode.WebView.csproj -c Release -o webview-blazor/bin/Publish
xcopy /s /y /e .\webview-blazor\bin\Publish\wwwroot .\media > nul
move /y .\media\Kanawanagasaki.VSCode.LeetCode.WebView.styles.css .\media\css\Kanawanagasaki.VSCode.LeetCode.WebView.styles.css > nul
