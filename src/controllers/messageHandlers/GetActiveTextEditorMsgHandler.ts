import * as vscode from "vscode";
import { getProblemDetailsByFilePath } from "../../util/storage-path-utils";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetActiveTextEditorMsgHandler implements IMsgHandler {

    private _messageController: MessageController;

    constructor(messageController: MessageController) {
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getActiveTextEditor";
    }

    execute(webviewSender: string, data: any) {
        for (const textEditor of vscode.window.visibleTextEditors) {
            const problemDetails = getProblemDetailsByFilePath(textEditor.document.fileName);
            if (problemDetails) {
                this._messageController.sendMessageAll("changeActiveTextEditor", problemDetails);
                return;
            }
        }
        this._messageController.sendMessageAll("changeActiveTextEditor", null);
    }

}
