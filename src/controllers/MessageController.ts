import * as vscode from 'vscode';
import { LeetcodeController } from './LeetcodeController';
import { DownloadChromiumMsgHandler } from './messageHandlers/DownloadChromiumMsgHandler';
import { GetActiveTextEditorMsgHandler } from './messageHandlers/GetActiveTextEditorMsgHandler';
import { GetCommunitySolutionCommentRepliesMsgHandler } from './messageHandlers/GetCommunitySolutionCommentRepliesMsgHandler';
import { GetCommunitySolutionCommentsMsgHandler } from './messageHandlers/GetCommunitySolutionCommentsMsgHandler';
import { GetCommunitySolutionMsgHandler } from './messageHandlers/GetCommunitySolutionMsgHandler';
import { GetDiscussionRepliesMsgHandler } from './messageHandlers/GetDiscussionRepliesMsgHandler';
import { GetPagePropsMsgHandler } from './messageHandlers/GetPagePropsMsgHandler';
import { GetProblemDetailsMsgHandler } from './messageHandlers/GetProblemDetailsMsgHandler';
import { GetProblemsMsgHandler } from './messageHandlers/GetProblemsMsgHandler';
import { GetSettingsMsgHandler } from './messageHandlers/GetSettingsMsgHandler';
import { GetStoragePathMsgHandler } from './messageHandlers/GetStoragePathMsgHandler';
import { GetUserStatusMsgHandler } from './messageHandlers/GetUserStatusMsgHandler';
import { IMsgHandler } from './messageHandlers/IMsgHandler';
import { OpenProblemMsgHandler } from './messageHandlers/OpenProblemMsgHandler';
import { RunCodeMsgHandler } from './messageHandlers/RunCodeMsgHandler';
import { SelectStoragePathMsgHandler } from './messageHandlers/SelectStoragePathMsgHandler';
import { SetSettingsMsgHandler } from './messageHandlers/SetSettingsMsgHandler';
import { SignInMsgHandler } from './messageHandlers/SignInMsgHandler';
import { SignOutMsgHandler } from './messageHandlers/SignOutMsgHandler';
import { SolveMsgHandler } from './messageHandlers/SolveMsgHandler';
import { SubmitMsgHandler } from './messageHandlers/SubmitMsgHandler';
import { WebviewController } from './WebviewController';

export class MessageController {

    private _webviews: Record<string, vscode.Webview> = {};
    private _handlers: Record<string, IMsgHandler> = {};

    constructor(leetcodeController: LeetcodeController, webviewController: WebviewController, context: vscode.ExtensionContext) {
        this.registerMessageHandler(new DownloadChromiumMsgHandler(this, context));
        this.registerMessageHandler(new GetUserStatusMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new SignInMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new SignOutMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetStoragePathMsgHandler(this));
        this.registerMessageHandler(new SelectStoragePathMsgHandler(this));
        this.registerMessageHandler(new GetProblemsMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetPagePropsMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetSettingsMsgHandler(this));
        this.registerMessageHandler(new SetSettingsMsgHandler(this));
        this.registerMessageHandler(new OpenProblemMsgHandler(leetcodeController, this, webviewController));
        this.registerMessageHandler(new GetProblemDetailsMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetDiscussionRepliesMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new SolveMsgHandler(leetcodeController, webviewController));
        this.registerMessageHandler(new GetActiveTextEditorMsgHandler(this));
        this.registerMessageHandler(new RunCodeMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new SubmitMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetCommunitySolutionMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetCommunitySolutionCommentsMsgHandler(leetcodeController, this));
        this.registerMessageHandler(new GetCommunitySolutionCommentRepliesMsgHandler(leetcodeController, this));
    }

    public registerWebview(name: string, webview: vscode.Webview) {
        webview.onDidReceiveMessage(msg => this.processMessage(name, msg));
        this._webviews[name] = webview;
    }

    public unregisterWebview(name: string) {
        if (this._webviews.hasOwnProperty(name))
            delete this._webviews[name];
    }

    private registerMessageHandler(handler: IMsgHandler) {
        this._handlers[handler.getCommandName()] = handler;
    }

    private processMessage(webviewName: string, msg: any) {
        const { sender, command, data } = msg;
        if (sender !== "vsc-leetcode")
            return;

        if (this._handlers.hasOwnProperty(command))
            this._handlers[command].execute(webviewName, data);
        else console.log(`Handler for ${command} command not found`);
    }

    public execute(webviewName: string, command: string, data: any = undefined) {
        if (this._handlers.hasOwnProperty(command))
            this._handlers[command].execute(webviewName, data);
        else console.log(`Handler for ${command} command not found`);
    }

    public executeAll(command: string, data: any = undefined) {
        if (this._handlers.hasOwnProperty(command))
            for (const webviewName in this._webviews)
                this._handlers[command].execute(webviewName, data);
        else console.log(`Handler for ${command} command not found`);
    }

    public sendMessage(webviewName: string, command: string, data: any = {}) {
        if (!this._webviews.hasOwnProperty(webviewName))
            return;

        this._webviews[webviewName].postMessage({ sender: 'vsc-leetcode', command, data });
    }

    public sendMessageAll(command: string, data: any) {
        for (const webviewName in this._webviews)
            this.sendMessage(webviewName, command, data);
    }
}
