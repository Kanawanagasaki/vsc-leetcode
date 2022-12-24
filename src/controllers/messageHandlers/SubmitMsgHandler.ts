import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";
import { getProblemDetailsByFilePath } from "../../util/storage-path-utils";

export class SubmitMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "submit";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("titleSlug"))
            return;

        let langSlug: string | null = null;

        for (const textEditor of vscode.window.visibleTextEditors) {
            const problemDetails = getProblemDetailsByFilePath(textEditor.document.fileName);
            if (problemDetails && problemDetails.titleSlug == data.titleSlug) {
                langSlug = problemDetails.langSlug;
                break;
            }
        }

        if (!langSlug) {
            vscode.window.showErrorMessage(`Failed to find an appropriate text editor to submit the code for ${data.titleSlug}`);
            return;
        }

        try {

            const submissionId = await this._leetcodeController.submit(data.titleSlug, langSlug);
            let tryCount = 0;
            const intervalId = setInterval(async () => {
                const state = await this._leetcodeController.getSubmissionState(data.titleSlug, submissionId);
                if (state.state !== "PENDING" && state.state !== "STARTED") {
                    clearInterval(intervalId);
                    this._messageController.sendMessageAll("submissionStateUpdate", { titleSlug: data.titleSlug, state });
                }
                else if (tryCount > 20) {
                    clearInterval(intervalId);
                    this._messageController.sendMessage(webviewSender, "submissionStateUpdate", { titleSlug: data.titleSlug, state: { state: "TIMEOUT", statusMessage: "Timeout" } });
                }
                else {
                    this._messageController.sendMessage(webviewSender, "submissionStateUpdate", { titleSlug: data.titleSlug, state });
                    tryCount++;
                }
            }, 2500);

        }
        catch (err) {
            if (err instanceof Error)
                vscode.window.showErrorMessage(err.message);
            else
                vscode.window.showErrorMessage("Failed to submit your code");
            console.error(err);
            this._messageController.sendMessage(webviewSender, "submissionStateUpdate", { titleSlug: data.titleSlug, state: { state: "ERROR", statusMessage: "Error" } });
        }
    }

}
