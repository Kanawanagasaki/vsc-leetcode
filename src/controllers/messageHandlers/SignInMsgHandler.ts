import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class SignInMsgHandler implements IMsgHandler {
    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "signin";
    }

    async execute(webviewSender: string, data: any) {

        try {
            await this._leetcodeController.authorize();
            const userstatus = await this._leetcodeController.getUserStatus();
            this._messageController.sendMessage(webviewSender, "updateUserStatus", userstatus);
        }
        catch (err) {
            if (err instanceof Error)
                this._messageController.sendMessage(webviewSender, "signinError", { message: err.message });
            else if (typeof err == "string")
                this._messageController.sendMessage(webviewSender, "signinError", { message: err });
            else
                this._messageController.sendMessage(webviewSender, "signinError", { message: "Unexpected error occurred while trying to sign in" });
            this._messageController.sendMessage(webviewSender, "updateUserStatus", this._leetcodeController.getDefaultUserStatus());
        }
    }
}
