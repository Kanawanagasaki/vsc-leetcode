import * as chromium from "../../util/chromium";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";
import * as vscode from "vscode";
import * as fs from "fs";

export class DownloadChromiumMsgHandler implements IMsgHandler {

    private _messageController: MessageController;
    private _context: vscode.ExtensionContext;

    constructor(messageController: MessageController, context: vscode.ExtensionContext) {
        this._messageController = messageController;
        this._context = context;
    }

    getCommandName(): string {
        return "downloadChromium";
    }

    async execute(webviewSender: string, data: any) {
        try {
            if (fs.existsSync(chromium.getExecutablePath(this._context.extensionPath))) {
                this._messageController.sendMessage(webviewSender, "downloadChromiumSuccess");
                return;
            }
            await chromium.downloadChromium(this._context.extensionPath, (downloadedBytes, totalBytes) => {
                this._messageController.sendMessage(webviewSender, "downloadChromiumProgress", { downloadedBytes, totalBytes });
            });
            this._messageController.sendMessage(webviewSender, "downloadChromiumSuccess");
        }
        catch (err) {
            if (err instanceof Error)
                this._messageController.sendMessage(webviewSender, "downloadChromiumFail", err.message);
            else if (typeof err === "string")
                this._messageController.sendMessage(webviewSender, "downloadChromiumFail", err);
            else
                this._messageController.sendMessage(webviewSender, "downloadChromiumFail", "Failed to download chromium");
        }
    }

}