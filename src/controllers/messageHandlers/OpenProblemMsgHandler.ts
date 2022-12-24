import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";
import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { WebviewController } from "../WebviewController";

export class OpenProblemMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;
    private _webviewController: WebviewController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController, webviewController: WebviewController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
        this._webviewController = webviewController;
    }

    getCommandName(): string {
        return "openProblem";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("titleSlug") || typeof data.titleSlug !== "string") {
            vscode.window.showErrorMessage("Failed to fetch problem data.");
            return;
        }

        vscode.window.withProgress({
            location: vscode.ProgressLocation.Notification,
            cancellable: false,
            title: "Loading Problem"
        }, async progress => {
            progress.report({ message: "Loading " + data.titleSlug + "..." });

            const problemTitle = await this._leetcodeController.getProblemTitle(data.titleSlug);
            if (problemTitle) {
                this._webviewController.createProblemWebview(problemTitle, this._messageController);

                progress.report({ message: "Loading " + data.titleSlug, increment: 100 });
            }
            else
                vscode.window.showErrorMessage("Failed to fetch problem data.");
        });
    }

}
