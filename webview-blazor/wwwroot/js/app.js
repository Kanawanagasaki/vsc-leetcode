const vscode = acquireVsCodeApi();

globalThis.vscLeetcode = Object.assign({
    getScrollTop: (el) => el.scrollTop,
    init: async () => {
        const { getAssemblyExports } = await globalThis.getDotnetRuntime(0);
        const exports = await getAssemblyExports("Kanawanagasaki.VSCode.LeetCode.WebView.dll");

        window.addEventListener('message', ev => {
            const { sender, command, data } = ev.data;
            if (sender !== "vsc-leetcode")
                return;
            exports.Kanawanagasaki.VSCode.LeetCode.WebView.Services.JsService.OnMessage(command, JSON.stringify(data));
        });
    },
    sendMessage: (command, data) => {
        vscode.postMessage({
            sender: "vsc-leetcode",
            command,
            data: JSON.parse(data)
        });
    },
    setState: vscode.setState,
    getState: vscode.getState,
    clickOnEl: (elId) => {
        const el = document.getElementById(elId);
        if (el)
            el.click();
    },
    highlightPreCode: (el, lang) => {
        if (el) {
            const preCode = el.querySelectorAll('pre code');
            for (const el2 of preCode) {
                if (lang) {
                    el2.classList.add("language-" + lang);
                }
                hljs.highlightElement(el2);
            }
        }
    }
}, globalThis.vscLeetcode);

Blazor.start({ loadBootResource: (type, name, defaultUri, integrity) => globalThis.vscLeetcode.extensionMediaPath + "/_framework/" + name });