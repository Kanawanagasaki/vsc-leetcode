import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetCommunitySolutionCommentRepliesMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getCommunitySolutionCommentReplies";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("commentId"))
            return;
        if (!data.hasOwnProperty("uuid"))
            return;

        const result = await this._leetcodeController.getCommunitySolutionCommentReplies(data.commentId);
        this._messageController.sendMessage(webviewSender, "communitySolutionCommentReplies", { uuid: data.uuid, result });
    }

}
