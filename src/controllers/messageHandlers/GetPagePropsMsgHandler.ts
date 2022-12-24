import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetPagePropsMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getPageProps";
    }

    async execute(webviewSender: string, data: any) {
        const result = await this._leetcodeController.getPageProps();
        this._messageController.sendMessage(webviewSender, "pageProps", result);
    }

}
