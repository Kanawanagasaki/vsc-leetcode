import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetDiscussionRepliesMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getDiscussionReplies";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("commentId"))
            return;
        if (!data.hasOwnProperty("uuid"))
            return;

        const result = await this._leetcodeController.getDiscussionReplies(data.commentId);
        this._messageController.sendMessage(webviewSender, "discussionReplies", { uuid: data.uuid, result });
    }

}
