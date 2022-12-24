import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";
import { setStoragePath } from "../../util/storage-path-utils";

export class SelectStoragePathMsgHandler implements IMsgHandler {
    private _messageController: MessageController;

    constructor(messageController: MessageController) {
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "selectStoragePath";
    }

    async execute(webviewSender: string, data: any) {
        const options: vscode.OpenDialogOptions = {
            canSelectMany: false,
            openLabel: 'Open',
            canSelectFiles: false,
            canSelectFolders: true
        };

        const folderUri = await vscode.window.showOpenDialog(options);
        if (folderUri && folderUri[0]) {
            try {
                setStoragePath(folderUri[0].fsPath);
                this._messageController.executeAll("getStoragePath");
            }
            catch { }
        }
    }
}
