import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class SignOutMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }
    
    getCommandName(): string {
        return "signout";
    }

    async execute(webviewSender: string, data: any) {
        await this._leetcodeController.signout();
        this._messageController.sendMessage(webviewSender, "updateUserStatus", this._leetcodeController.getDefaultUserStatus());
    }

}
