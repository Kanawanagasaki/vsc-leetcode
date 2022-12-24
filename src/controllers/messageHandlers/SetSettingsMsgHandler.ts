import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";
import { MessageController } from "../MessageController";

export class SetSettingsMsgHandler implements IMsgHandler {

    private _messageController: MessageController;

    constructor(messageController: MessageController) {
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "setSettings";
    }

    async execute(webviewSender: string, data: any) {
        const settings = Object.assign({
            showTagsOnHover: false,
            defaultLanguage: ""
        }, data);

        const config = vscode.workspace.getConfiguration("vsc-leetcode");
        await config.update("showTagsOnHover", settings.showTagsOnHover, true);
        await config.update("defaultLanguage", settings.defaultLanguage, true);

        this._messageController.executeAll("getSettings");
    }

}
