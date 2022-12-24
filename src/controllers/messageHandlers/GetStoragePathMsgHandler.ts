import * as vscode from "vscode";
import * as fs from "fs"
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";
import { getStoragePath } from "../../util/storage-path-utils";

export class GetStoragePathMsgHandler implements IMsgHandler {

    private _messageController: MessageController;

    constructor(messageController: MessageController) {
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getStoragePath";
    }

    execute(webviewSender: string, data: any) {
        const storagePath = getStoragePath();
        if (typeof storagePath === "string" && fs.existsSync(storagePath))
            this._messageController.sendMessage(webviewSender, "storagePath", { exists: true, path: storagePath });
        else
            this._messageController.sendMessage(webviewSender, "storagePath", { exists: false });
    }

}
