import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetProblemsMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getProblems";
    }

    async execute(webviewSender: string, data: any) {
        const options = Object.assign({
            categorySlug: "",
            page: 0,
            filters: {},
            getCache: true
        }, data);
        if (!options.hasOwnProperty("uuid"))
            return;

        const result = await this._leetcodeController.getProblems(options.categorySlug, options.page, options.filters, options.getCache);
        this._messageController.sendMessage(webviewSender, "problemset", { uuid: options.uuid, result });
    }

}
