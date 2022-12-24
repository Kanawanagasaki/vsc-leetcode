import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetCommunitySolutionCommentsMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getCommunitySolutionComments";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("topicId"))
            return;

        const options = Object.assign({
            pageNo: 1,
            numPerPage: 10,
            orderBy: "best"
        }, data);

        const result = await this._leetcodeController.getCommunitySolutionComments(options.topicId, options.pageNo, options.numPerPage, options.orderBy);
        this._messageController.sendMessage(webviewSender, "communitySolutionComments", result);
    }

}
