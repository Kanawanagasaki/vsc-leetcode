import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { WebviewController } from "../WebviewController";
import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";
import * as fs from "fs";
import * as path from "path";
import { createSolutionFile, getProblemToFileMap, getSolutionPath, getStorageFolder } from "../../util/storage-path-utils";

const langToExtension: Record<string, string> = {
    cpp: "cpp",
    java: "java",
    python: "py",
    python3: "py",
    mysql: "sql",
    mssql: "sql",
    oraclesql: "sql",
    c: "c",
    csharp: "cs",
    javascript: "js",
    ruby: "rb",
    bash: "sh",
    swift: "swift",
    golang: "go",
    scala: "sc",
    html: "html",
    pythonml: "py",
    kotlin: "kt",
    rust: "rs",
    php: "php",
    typescript: "ts",
    racket: "rkt",
    erlang: "elixir",
    elixir: "ex",
    dart: "dart"
}

export class SolveMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _webviewController: WebviewController;

    constructor(leetcodeController: LeetcodeController, webviewController: WebviewController) {
        this._leetcodeController = leetcodeController;
        this._webviewController = webviewController;
    }

    getCommandName(): string {
        return "solve";
    }

    async execute(webviewSender: string, data: any) {
        try {
            if (!data.hasOwnProperty("titleSlug") || typeof data.titleSlug !== "string")
                return;
            if (!data.hasOwnProperty("langSlug") || typeof data.langSlug !== "string")
                return;

            let solutionPath = getSolutionPath(data.titleSlug, data.langSlug);
            if (solutionPath !== null && fs.existsSync(solutionPath)) {
                const textDocument = await vscode.workspace.openTextDocument(solutionPath);
                await vscode.window.showTextDocument(textDocument, vscode.ViewColumn.One);
                const problemWebview = this._webviewController.getProblemWebview();
                if (problemWebview)
                    problemWebview.reveal(vscode.ViewColumn.Two, true);
                return;
            }

            const editorData = await this._leetcodeController.getEditorData(data.titleSlug);
            if (!editorData) {
                vscode.window.showErrorMessage(`Failed to load data for "${data.titleSlug}".`);
                return;
            }

            const codeSnippet = editorData.codeSnippets.filter(x => x.langSlug == data.langSlug)[0];
            if (!codeSnippet) {
                vscode.window.showErrorMessage(`You cannot solve "${data.titleSlug}" with ${data.langSlug}.`);
                return;
            }

            if (!langToExtension.hasOwnProperty(codeSnippet.langSlug)) {
                vscode.window.showErrorMessage(`Failed to create file for ${data.langSlug} language.`);
                return;
            }

            solutionPath = createSolutionFile(editorData.questionFrontendId, data.titleSlug, codeSnippet.langSlug, langToExtension[codeSnippet.langSlug], codeSnippet.code);
            const textDocument = await vscode.workspace.openTextDocument(solutionPath);
            await vscode.window.showTextDocument(textDocument, vscode.ViewColumn.One);

            const problemWebview = this._webviewController.getProblemWebview();
            if (problemWebview)
                problemWebview.reveal(vscode.ViewColumn.Two, true);
        }
        catch (err) {
            if (err instanceof Error)
                vscode.window.showErrorMessage(err.message);
            console.error(err);
        }
    }

}
