import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetUserStatusMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getAuthUser";
    }

    async execute(webviewSender: string, data: any) {
        const userstatus = await this._leetcodeController.getUserStatus();
        this._messageController.sendMessage(webviewSender, "updateUserStatus", userstatus);
    }

}
