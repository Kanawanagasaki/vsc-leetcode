import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";

export class GetSettingsMsgHandler implements IMsgHandler {

    private _messageController: MessageController;

    constructor(messageController: MessageController) {
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getSettings";
    }

    execute(webviewSender: string, data: any) {
        const ret = {
            showTagsOnHover: vscode.workspace.getConfiguration("vsc-leetcode").get("showTagsOnHover"),
            defaultLanguage: vscode.workspace.getConfiguration("vsc-leetcode").get("defaultLanguage")
        };

        this._messageController.sendMessage(webviewSender, "settings", ret);
    }

}
