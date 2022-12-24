import { LeetcodeController } from "../LeetcodeController";
import { MessageController } from "../MessageController";
import { IMsgHandler } from "./IMsgHandler";

export class GetProblemDetailsMsgHandler implements IMsgHandler {

    private _leetcodeController: LeetcodeController;
    private _messageController: MessageController;

    constructor(leetcodeController: LeetcodeController, messageController: MessageController) {
        this._leetcodeController = leetcodeController;
        this._messageController = messageController;
    }

    getCommandName(): string {
        return "getProblemDetails";
    }

    async execute(webviewSender: string, data: any) {
        if (!data.hasOwnProperty("snippet") || typeof data.snippet !== "string")
            return;
        if (!data.hasOwnProperty("titleSlug") || typeof data.titleSlug !== "string")
            return;

        let result: any = null;

        switch (data.snippet) {
            case "title":
                result = await this._leetcodeController.getProblemTitle(data.titleSlug);
                break;
            case "content":
                result = await this._leetcodeController.getProblemContent(data.titleSlug);
                break;
            case "stats":
                result = await this._leetcodeController.getProblemStats(data.titleSlug);
                break;
            case "similar":
                result = await this._leetcodeController.getSimilarProblems(data.titleSlug);
                break;
            case "consoleconfig":
                result = await this._leetcodeController.getConsoleConfig(data.titleSlug);
                break;
            case "tags":
                result = await this._leetcodeController.getProblemTags(data.titleSlug);
                break;
            case "editordata":
                result = await this._leetcodeController.getEditorData(data.titleSlug);
                break;
            case "hints":
                result = await this._leetcodeController.getHints(data.titleSlug);
                break;
            case "submissions":
                result = await this._leetcodeController.getSubmissionList(data.titleSlug,
                    data.hasOwnProperty("offset") ? data.offset : undefined,
                    data.hasOwnProperty("limit") ? data.limit : undefined,
                    data.hasOwnProperty("lastKey") ? data.lastKey : undefined,
                    data.hasOwnProperty("getCache") && data.getCache);
                break;
            case "solutiontags":
                result = await this._leetcodeController.getSolutionTags(data.titleSlug);
                break;
            case "solutions":
                result = await this._leetcodeController.getSolutions(data.titleSlug,
                    data.hasOwnProperty("languageTags") ? data.languageTags : undefined,
                    data.hasOwnProperty("topicTags") ? data.topicTags : undefined,
                    data.hasOwnProperty("skip") ? data.skip : undefined,
                    data.hasOwnProperty("first") ? data.first : undefined);
                break;
            case "discussiontopics":
                const topic = await this._leetcodeController.getDiscussionTopic(data.titleSlug);
                if (!topic)
                    break;
                const list = await this._leetcodeController.getDiscussions(topic.id,
                    data.hasOwnProperty("pageNo") ? data.pageNo : undefined,
                    data.hasOwnProperty("numPerPage") ? data.numPerPage : undefined);
                result = { topic, list };
                break;
        }
        this._messageController.sendMessage(webviewSender, "problemDetails", { snippet: data.snippet, result: JSON.stringify(result) });
    }
}
