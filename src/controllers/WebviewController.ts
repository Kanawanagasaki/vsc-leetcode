import { MessageController } from "./MessageController";
import { ProblemTitle } from "./LeetcodeController";
import * as vscode from "vscode";
import * as fs from 'fs';
import * as path from 'path';

export class WebviewController {
    private _context: vscode.ExtensionContext;

    private _menu: vscode.WebviewView | undefined;
    private _problem: vscode.WebviewPanel | undefined;

    constructor(context: vscode.ExtensionContext) {
        this._context = context;
    }

    public resolveMenuWebview(panel: vscode.WebviewView, messageController: MessageController) {
        if (this._menu) {
            messageController.unregisterWebview("vsc-leetcode-home");
            this._menu = undefined;
        }

        panel.webview.options = { enableScripts: true };
        panel.webview.html = this.getHtml(panel.webview, "index.html");

        messageController.registerWebview("vsc-leetcode-home", panel.webview);

        this._menu = panel;

        panel.onDidDispose(() => {
            messageController.unregisterWebview("vsc-leetcode-home");
            this._menu = undefined;
        });
    }

    public createProblemWebview(problemTitle: ProblemTitle, messageController: MessageController) {
        if (this._problem) {
            this._problem.reveal();
            if (this._problem.visible) {
                this._problem.title = `${problemTitle.questionFrontendId}. ${problemTitle.title}`;
                messageController.sendMessage("vsc-leetcode-problem", "updateProblem", problemTitle.titleSlug);
                return;
            }
        }
        else {
            this._problem = vscode.window.createWebviewPanel(
                "vsc-leetcode-problem",
                "Problem",
                vscode.ViewColumn.Nine,
                {
                    enableScripts: true,
                    retainContextWhenHidden: true
                }
            );
        }

        let html = this.getHtml(this._problem.webview, "problem.html");
        html = html.replace(/\${problemTitleSlug}/gi, problemTitle.titleSlug);

        this._problem.webview.html = html;
        this._problem.title = `${problemTitle.questionFrontendId}. ${problemTitle.title}`;
        messageController.registerWebview("vsc-leetcode-problem", this._problem.webview);

        this._problem.onDidDispose(() => {
            this._problem = undefined;
            messageController.unregisterWebview("vsc-leetcode-problem");
        });
    }

    restoreProblemWebview(webviewPanel: vscode.WebviewPanel, problemTitle: ProblemTitle, messageController: MessageController) {
        this._problem = webviewPanel;

        let html = this.getHtml(this._problem.webview, "problem.html");
        html = html.replace(/\${problemTitleSlug}/gi, problemTitle.titleSlug);

        this._problem.webview.html = html;
        this._problem.title = `${problemTitle.questionFrontendId}. ${problemTitle.title}`;
        messageController.registerWebview("vsc-leetcode-problem", this._problem.webview);

        this._problem.onDidDispose(() => {
            this._problem = undefined;
            messageController.unregisterWebview("vsc-leetcode-problem");
        });
    }

    public getProblemWebview() {
        return this._problem;
    }

    private getHtml(webview: vscode.Webview, htmlFile: string) {
        const extensionMediaPath = webview.asWebviewUri(vscode.Uri.joinPath(this._context.extensionUri, "media"));

        const htmlFileUri = vscode.Uri.file(path.join(this._context.extensionPath, "media", htmlFile));
        let html = fs.readFileSync(htmlFileUri.fsPath, 'utf-8');
        html = html.replace(/\${extensionMediaPath}/gi, extensionMediaPath.toString());

        return html;
    }
}
