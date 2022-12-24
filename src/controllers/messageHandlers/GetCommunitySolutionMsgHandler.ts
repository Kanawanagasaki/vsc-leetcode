import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetCommunitySolutionMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getCommunitySolution";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("topicId"))
            return;

        const result = await this._leetcodeController.getCommunitySolution(data.topicId);
        this._messageController.sendMessage(webviewSender, "communitySolution", result);
    }

}
