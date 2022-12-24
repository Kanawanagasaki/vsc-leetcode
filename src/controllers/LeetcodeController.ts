import fetch, { RequestInit } from "node-fetch";
import * as vscode from "vscode";
import * as fs from "fs";
import * as chromium from "../util/chromium";
import puppeteer from 'puppeteer-core';
import { CacheController } from "./CacheController";
import { getSolutionPath } from "../util/storage-path-utils";

const endpoints = {
    origin: "https://leetcode.com",
    base: "https://leetcode.com/",
    login: "https://leetcode.com/accounts/login/",
    graphql: "https://leetcode.com/graphql",
    problemset: "https://leetcode.com/problemset/all/"
};

export class LeetcodeController {

    private _context: vscode.ExtensionContext;
    private _secretStorage: vscode.SecretStorage;
    private _cache: CacheController;

    constructor(context: vscode.ExtensionContext, cache: CacheController) {
        this._context = context;
        this._secretStorage = this._context.secrets;
        this._cache = cache;
    }

    public async authorize() {
        let promiseResolve: (val: any) => any, promiseReject: (reason: string) => any;
        const promise = new Promise((resolve, reject) => [promiseResolve, promiseReject] = [resolve, reject]);

        const executablePath = chromium.getExecutablePath(this._context.extensionPath);
        const browser = await puppeteer.launch({ headless: false, defaultViewport: null, executablePath });
        const [page] = await browser.pages();

        page.on("response", async response => {
            if (response.url().startsWith(endpoints.base)) {
                const cookies = await page.cookies();
                const authCookie = cookies.map(c => `${c.name}=${c.value}`).join("; ");
                await this._secretStorage.store("auth_cookie", authCookie);

                if (response.url() === endpoints.base) {
                    await browser.close();
                    promiseResolve({});
                }
            }
        });

        page.on("close", _ => {
            promiseReject("Chromium browser was closed");
        });

        await page.goto(endpoints.login);
        await promise;
    }

    public async signout() {
        await this._secretStorage.delete("auth_cookie");
    }

    public async getUserStatus() {
        const json = await this.fetchGraphQl({
            operationName: "globalData",
            query: "query globalData{userStatus{isSignedIn isPremium isVerified username realName avatar activeSessionId}}",
            variables: {}
        }, false);
        if (json === null)
            return this.getDefaultUserStatus();
        return json.data.userStatus;
    }

    public getDefaultUserStatus() {
        return {
            isSignedIn: false,
            isPremium: null,
            isVerified: false,
            username: "",
            realName: null,
            avatar: null,
            activeSessionId: 0
        };
    }

    public async getProblems(categorySlug: string, page: number, filters: any, getCache: boolean) {
        const limit = 50;
        const skip = page * limit;
        const json = await this.fetchGraphQl({
            query: "query problemsetQuestionList($categorySlug: String, $limit: Int, $skip: Int, $filters: QuestionListFilterInput) { problemsetQuestionList: questionList( categorySlug: $categorySlug limit: $limit skip: $skip filters: $filters) { total: totalNum questions: data { acRate difficulty freqBar frontendQuestionId: questionFrontendId isFavor paidOnly: isPaidOnly status title titleSlug topicTags { name id slug } hasSolution hasVideoSolution } } }",
            variables: { categorySlug, skip, limit, filters }
        }, getCache);
        const ret = json?.data?.problemsetQuestionList;
        if (ret === null || ret === undefined)
            return { page: 0, limit: 0, skip: 0, total: 0, questions: [] };
        ret.page = page;
        ret.limit = limit;
        ret.skip = skip;
        return ret;
    }

    public async getProblemTitle(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query questionTitle($titleSlug: String!) { question(titleSlug: $titleSlug) { questionId questionFrontendId title titleSlug isPaidOnly difficulty likes dislikes }}",
            variables: { titleSlug }
        });
        return json?.data?.question as ProblemTitle | null | undefined;
    }

    public async getProblemContent(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query questionContent($titleSlug: String!) { question(titleSlug: $titleSlug) { content mysqlSchemas } }",
            variables: { titleSlug }
        });
        return json?.data?.question as ProblemContent | null | undefined;
    }

    public async getProblemStats(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query questionStats($titleSlug: String!) { question(titleSlug: $titleSlug) { stats } }",
            variables: { titleSlug }
        });
        return JSON.parse(json?.data?.question?.stats ?? "null") as ProblemStats | null;
    }

    public async getSimilarProblems(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query SimilarQuestions($titleSlug: String!) { question(titleSlug: $titleSlug) { similarQuestions }}",
            variables: { titleSlug }
        });
        return JSON.parse(json?.data?.question?.similarQuestions ?? "null") as SimilarProblem[] | null;
    }

    public async getConsoleConfig(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query consolePanelConfig($titleSlug: String!) { question(titleSlug: $titleSlug) { questionId questionFrontendId questionTitle enableDebugger enableRunCode enableSubmit enableTestMode exampleTestcaseList metaData } }",
            variables: { titleSlug }
        });
        return json?.data?.question as ConsoleConfig | null | undefined;
    }

    public async getProblemTags(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query singleQuestionTopicTags($titleSlug: String!) { question(titleSlug: $titleSlug) { topicTags { id name slug } } }",
            variables: { titleSlug }
        });
        return json?.data?.question?.topicTags as Tag[] | null | undefined;
    }

    public async getEditorData(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query questionEditorData($titleSlug: String!) { question(titleSlug: $titleSlug) { questionId questionFrontendId codeSnippets { lang langSlug code } envInfo enableRunCode } }",
            variables: { titleSlug }
        });
        return json?.data?.question as EditorData | null | undefined;
    }

    public async getHints(titleSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query questionHints($titleSlug: String!) { question(titleSlug: $titleSlug) { hints } }",
            variables: { titleSlug }
        });
        return json?.data?.question as Hints | null | undefined;
    }

    public async getSubmissionList(questionSlug: string, offset: number = 0, limit: number = 20, lastKey: string | null = null, getCache: boolean) {
        const json = await this.fetchGraphQl({
            query: "query submissionList($offset: Int!, $limit: Int!, $lastKey: String, $questionSlug: String!, $lang: Int, $status: Int) { questionSubmissionList( offset: $offset limit: $limit lastKey: $lastKey questionSlug: $questionSlug lang: $lang status: $status ) { lastKey hasNext submissions { id title titleSlug status statusDisplay lang langName runtime timestamp url isPending memory hasNotes } } }",
            variables: { questionSlug, offset, limit, lastKey }
        }, getCache);
        return json?.data?.questionSubmissionList as SubmissionList | null | undefined;
    }

    public async getSolutionTags(questionSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query solutionTags($questionSlug: String!) { solutionTopicTags(questionSlug: $questionSlug) { name slug count } solutionLanguageTags(questionSlug: $questionSlug) { name slug count } }",
            variables: { questionSlug }
        });
        return json.data as SolutionTagList | null | undefined;
    }

    public async getSolutions(questionSlug: string, languageTags: string[] = [], topicTags: string[] = [], skip: number = 0, first: number = 15) {
        const json = await this.fetchGraphQl({
            query: "query communitySolutions($questionSlug: String!, $skip: Int!, $first: Int!, $query: String, $orderBy: TopicSortingOption, $languageTags: [String!], $topicTags: [String!]) { questionSolutions( filters: {questionSlug: $questionSlug, skip: $skip, first: $first, query: $query, orderBy: $orderBy, languageTags: $languageTags, topicTags: $topicTags} ) { hasDirectResults totalNum solutions { id title commentCount topLevelCommentCount viewCount pinned isFavorite solutionTags { name slug } post { id status voteCount creationDate isHidden author { username isActive nameColor activeBadge { displayName icon } profile { userAvatar reputation } } } searchMeta { content contentType commentAuthor { username } replyAuthor { username } highlights } } } }",
            variables: { languageTags, topicTags, questionSlug, skip, first, orderBy: "hot" }
        });
        return json.data.questionSolutions as SolutionList | null | undefined;
    }

    public async getDiscussionTopic(questionSlug: string) {
        const json = await this.fetchGraphQl({
            query: "query discussionTopic($questionSlug: String!) { questionDiscussionTopic(questionSlug: $questionSlug) { id commentCount topLevelCommentCount } }",
            variables: { questionSlug }
        });
        return json.data.questionDiscussionTopic as DiscussionTopic | null | undefined;
    }

    public async getDiscussions(topicId: string, pageNo: number = 1, numPerPage: number = 10) {
        const json = await this.fetchGraphQl({
            query: "query questionDiscussComments($topicId: Int!, $orderBy: String = \"newest_to_oldest\", $pageNo: Int = 1, $numPerPage: Int = 10) { topicComments( topicId: $topicId orderBy: $orderBy pageNo: $pageNo numPerPage: $numPerPage ) { data { id pinned pinnedBy { username } post { ...DiscussPost } intentionTag { slug } numChildren } totalNum } }fragment DiscussPost on PostNode { id voteCount voteStatus content updationDate creationDate status isHidden coinRewards { id score description date } author { isDiscussAdmin isDiscussStaff username nameColor activeBadge { displayName icon } profile { userAvatar reputation } isActive } authorIsModerator isOwnPost }",
            variables: { topicId, pageNo, numPerPage, orderBy: "best" }
        });
        const result = json?.data?.topicComments as DiscussionList | null | undefined;
        if (result === null || result === undefined)
            return null;
        result.pageNo = pageNo;
        result.numPerPage = numPerPage;
        return result;
    }

    public async getDiscussionReplies(commentId: string) {
        const json = await this.fetchGraphQl({
            query: "query commentReplies($commentId: Int!) { commentReplies(commentId: $commentId) { id pinned pinnedBy { username } post { ...DiscussPost } } }fragment DiscussPost on PostNode { id voteCount voteStatus content updationDate creationDate status isHidden coinRewards { id score description date } author { isDiscussAdmin isDiscussStaff username nameColor activeBadge { displayName icon } profile { userAvatar reputation } isActive } authorIsModerator isOwnPost }",
            variables: { commentId }
        });
        return json.data.commentReplies as DiscussionReply[] | null | undefined;
    }

    public async getPageProps() {
        try {
            const problemsetResponse = await this.fetchWithCookies(endpoints.problemset, {
                method: "GET"
            });
            const problemsetHtml = await problemsetResponse.text();

            const buildId = problemsetHtml.match(/\"buildId\"[ \n\t]{0,}:[ \n\t]{0,}\"([^\"]+)\"/);
            if (!buildId || buildId.length < 2)
                return;

            const url = `https://leetcode.com/_next/data/${buildId[1]}/problemset/all.json`;
            const response = await this.fetchWithCookies(url, {
                method: "GET"
            });
            if (response.status !== 200)
                return { featuredLists: [], topicTags: [], problemsetCategories: [] };
            const json: any = await response.json();
            return {
                featuredLists: json.pageProps.featuredLists,
                topicTags: json.pageProps.topicTags,
                problemsetCategories: json.pageProps.problemsetCategories
            };
        }
        catch (err) {
            console.error(err);
            return { featuredLists: [], topicTags: [], problemsetCategories: [] };
        }

    }

    public async runCode(titleSlug: string, langSlug: string, dataInput: string) {

        const solutionPath = getSolutionPath(titleSlug, langSlug);
        if (!solutionPath || !fs.existsSync(solutionPath))
            throw new Error(`${langSlug} solution for problem ${titleSlug} not found`);

        const solutionCode = fs.readFileSync(solutionPath, "utf8");

        const problem = await this.getProblemTitle(titleSlug);
        if (!problem)
            throw new Error(`Failed to fetch problem details`);

        const referer = `https://leetcode.com/problems/${titleSlug}/`;
        const url = `https://leetcode.com/problems/${titleSlug}/interpret_solution/`;
        const json = {
            data_input: dataInput,
            lang: langSlug,
            question_id: problem.questionId,
            typed_code: solutionCode
        };
        const response = await this.fetchWithCookies(url, {
            method: "POST",
            body: JSON.stringify(json),
            headers: {
                "Content-Type": "application/json",
                "Origin": endpoints.origin,
                "Referer": referer,
                "x-csrftoken": await this.getCsrfTokenFromCookie() ?? ""
            }
        });
        if (response.status !== 200)
            throw new Error(`Failed to run code. ${response.status} ${response.statusText}`);
        const data: any = await response.json();
        if (!data.hasOwnProperty("interpret_id"))
            throw new Error(`Failed to run code${data.hasOwnProperty("error") ? ". " + data.error : ""}`);
        return data.interpret_id as string;
    }

    public async getRuncodeState(titleSlug: string, interpretId: string): Promise<RuncodeState> {
        const referer = `https://leetcode.com/problems/${titleSlug}/`;
        const url = `https://leetcode.com/submissions/detail/${interpretId}/check/`;
        const response = await this.fetchWithCookies(url, {
            method: "GET",
            headers: {
                "Origin": endpoints.origin,
                "Referer": referer,
                "x-csrftoken": await this.getCsrfTokenFromCookie() ?? ""
            }
        });
        if (response.status !== 200)
            throw new Error(`Failed to get runcode state. ${response.status} ${response.statusText}`);
        const data: any = await response.json();
        return {
            state: data.state,
            codeAnswer: data.code_answer,
            correctAnswer: data.correct_answer,
            elapsedTime: data.elapsed_time,
            expectedCodeAnswer: data.expected_code_answer,
            runSuccess: data.run_success,
            statusMemory: data.status_memory,
            statusRuntime: data.status_runtime,
            statusMessage: data.status_msg,
            stdOutput: data.std_output,
            totalCorrect: data.total_correct,
            totalTestcases: data.total_testcases,
            compileError: data.compile_error,
            fullCompileError: data.full_compile_error,
            runtimeError: data.runtime_error,
            fullRuntimeError: data.full_runtime_error
        };
    }

    public async submit(titleSlug: string, langSlug: string) {

        const solutionPath = getSolutionPath(titleSlug, langSlug);
        if (!solutionPath || !fs.existsSync(solutionPath))
            throw new Error(`${langSlug} solution for problem ${titleSlug} not found`);

        const solutionCode = fs.readFileSync(solutionPath, "utf8");

        const problem = await this.getProblemTitle(titleSlug);
        if (!problem)
            throw new Error(`Failed to fetch problem details`);

        const referer = `https://leetcode.com/problems/${titleSlug}/`;
        const url = `https://leetcode.com/problems/${titleSlug}/submit/`;
        const json = {
            lang: langSlug,
            question_id: problem.questionId,
            typed_code: solutionCode
        };
        const response = await this.fetchWithCookies(url, {
            method: "POST",
            body: JSON.stringify(json),
            headers: {
                "Content-Type": "application/json",
                "Origin": endpoints.origin,
                "Referer": referer,
                "x-csrftoken": await this.getCsrfTokenFromCookie() ?? ""
            }
        });
        if (response.status !== 200)
            throw new Error(`Failed to submit code. ${response.status} ${response.statusText}`);
        const data: any = await response.json();
        if (!data.hasOwnProperty("submission_id"))
            throw new Error(`Failed to submit code${data.hasOwnProperty("error") ? ". " + data.error : ""}`);
        return data.submission_id as number;
    }

    public async getSubmissionState(titleSlug: string, submissionId: number): Promise<SubmissionState> {
        const referer = `https://leetcode.com/problems/${titleSlug}/`;
        const url = `https://leetcode.com/submissions/detail/${submissionId}/check/`;
        const response = await this.fetchWithCookies(url, {
            method: "GET",
            headers: {
                "Origin": endpoints.origin,
                "Referer": referer,
                "x-csrftoken": await this.getCsrfTokenFromCookie() ?? ""
            }
        });
        if (response.status !== 200)
            throw new Error(`Failed to get submission state. ${response.status} ${response.statusText}`);
        const data: any = await response.json();
        return {
            state: data.state,
            statusCode: data.status_code,
            lang: data.lang,
            runSuccess: data.run_success,
            statusRuntime: data.status_runtime,
            memory: data.memory,
            questionId: data.question_id,
            elapsedTime: data.elapsed_time,
            compareResult: data.compare_result,
            codeOutput: data.code_output,
            stdOutput: data.std_output,
            lastTestcase: data.last_testcase,
            expectedOutput: data.expected_output,
            taskFinishTime: data.task_finish_time,
            totalCorrect: data.total_correct,
            totalTestcases: data.total_testcases,
            runtimePercentile: data.runtime_percentile,
            statusMemory: data.status_memory,
            memoryPercentile: data.memory_percentile,
            prettyLang: data.pretty_lang,
            submissionId: data.submission_id,
            statusMessage: data.status_msg,
            compileError: data.compile_error,
            fullCompileError: data.full_compile_error,
            runtimeError: data.runtime_error,
            fullRuntimeError: data.full_runtime_error
        };
    }

    public async getCommunitySolution(topicId: number) {
        const json = await this.fetchGraphQl({
            query: "query communitySolution($topicId: Int!) { isSolutionTopic(id: $topicId) topic(id: $topicId) { id viewCount topLevelCommentCount favoriteCount subscribed title pinned solutionTags { name slug } hideFromTrending commentCount isFavorite post { id voteCount voteStatus content updationDate creationDate status isHidden author { isDiscussAdmin isDiscussStaff username nameColor activeBadge { displayName icon } profile { userAvatar reputation } isActive } authorIsModerator isOwnPost } } relatedSolutions(topicId: $topicId) { id post { author { username profile { userAvatar } } } title solutionTags { name slug } } }",
            variables: { topicId }
        });
        return json?.data as CommunitySolution | null | undefined;
    }

    public async getCommunitySolutionComments(topicId: number, pageNo: number, numPerPage: number, orderBy: string) {
        const json = await this.fetchGraphQl({
            query: "query questionDiscussComments($topicId: Int!, $orderBy: String = \"newest_to_oldest\", $pageNo: Int = 1, $numPerPage: Int = 10) { topicComments( topicId: $topicId orderBy: $orderBy pageNo: $pageNo numPerPage: $numPerPage ) { data { id pinned pinnedBy { username } post { ...DiscussPost } intentionTag { slug } numChildren } totalNum } }fragment DiscussPost on PostNode { id voteCount voteStatus content updationDate creationDate status isHidden coinRewards { id score description date } author { isDiscussAdmin isDiscussStaff username nameColor activeBadge { displayName icon } profile { userAvatar reputation } isActive } authorIsModerator isOwnPost }",
            variables: { topicId, pageNo, numPerPage, orderBy }
        });
        return json?.data?.topicComments as CommunitySolutionCommentList | null;
    }

    public async getCommunitySolutionCommentReplies(commentId: number) {
        const json = await this.fetchGraphQl({
            query: "query commentReplies($commentId: Int!) { commentReplies(commentId: $commentId) { id pinned pinnedBy { username } post { ...DiscussPost } } }fragment DiscussPost on PostNode { id voteCount voteStatus content updationDate creationDate status isHidden coinRewards { id score description date } author { isDiscussAdmin isDiscussStaff username nameColor activeBadge { displayName icon } profile { userAvatar reputation } isActive } authorIsModerator isOwnPost }",
            variables: { commentId }
        });
        return json?.data?.commentReplies as CommunitySolutionCommentReply[] | null;
    }

    private async fetchGraphQl(options: { operationName?: string, variables: any, query: string }, getCache = true) {
        try {
            const cacheKey = JSON.stringify(options);
            if (getCache) {
                const cacheValue = await this._cache.get(cacheKey);
                if (cacheValue)
                    return cacheValue;
            }

            const response = await this.fetchWithCookies(endpoints.graphql, {
                method: "POST",
                body: JSON.stringify(options),
                headers: {
                    "Content-Type": "application/json"
                }
            });
            if (response.status !== 200)
                return null;
            const data: any = await response.json();
            await this._cache.set(cacheKey, data);
            return data;
        }
        catch (err) {
            console.log("In LeetcodeController at fetchGraphQl():", err, { options, getCache: getCache });
            return null;
        }
    }

    private async fetchWithCookies(endpoint: string, init: RequestInit, redirectCount: number = 20) {
        const cookie = await this._secretStorage.get("auth_cookie");
        init.redirect = "manual";
        init.headers = Object.assign(init.headers ?? {}, { "Cookie": cookie ?? "" });
        let response = await fetch(endpoint, init);
        const setCookieHeader = response.headers.get("set-cookie");
        const setCookies = setCookieHeader?.matchAll(/(^|,)\W?([^=;]+=[^=;,]+)/g);
        if (setCookies) {
            const kvCookie = (cookie ?? "").split(";").filter(kv => kv.includes("=")).map(kv => kv.trim().split("="));
            for (const [a, b, setCookie] of setCookies) {
                const kvSetCookie = setCookie.split("=");
                const index = kvCookie.findIndex(kv => kv[0] == kvSetCookie[0]);
                if (index >= 0)
                    kvCookie[index][1] = kvSetCookie[1];
                else
                    kvCookie.push(kvSetCookie);
            }
            const authCookie = kvCookie.map(c => `${c[0]}=${c[1]}`).join("; ");
            await this._secretStorage.store("auth_cookie", authCookie);
        }

        const location = response.headers.get("location");
        if (location && Math.floor(response.status / 100) == 3) {
            if (redirectCount > 0) {
                const redirectUrl = new URL(location, response.url);
                response = await this.fetchWithCookies(redirectUrl.href, init, redirectCount - 1);
            }
            else throw new Error("In fetchWithCookies(): Number of redirects exceeded");
        }

        return response;
    }

    private async getCsrfTokenFromCookie() {
        const cookie = await this._secretStorage.get("auth_cookie");
        if (!cookie)
            return null;
        const kvCookie = cookie.split(";").map(kv => kv.trim().split("="));
        const index = kvCookie.findIndex(kv => kv[0] == "csrftoken");
        if (index < 0)
            return null;
        return kvCookie[index][1];
    }
}

export interface ProblemTitle {
    questionId: string;
    questionFrontendId: string;
    title: string;
    titleSlug: string;
    isPaidOnly: boolean;
    difficulty: string;
    likes: number;
    dislikes: number;
}

export interface ProblemContent {
    content: string;
    mysqlSchemas: string[];
}

export interface ProblemStats {
    totalAccepted: string;
    totalSubmission: string;
    totalAcceptedRaw: number; // 64
    totalSubmissionRaw: number; // 64
    acRate: string;
}

export interface SimilarProblem {
    title: string;
    titleSlug: string;
    difficulty: string;
    translatedTitle: string | null | undefined;
}

export interface ConsoleConfig {
    questionId: string;
    questionFrontendId: string;
    questionTitle: string;
    enableDebugger: boolean;
    enableRunCode: boolean;
    enableSubmit: boolean;
    enableTestMode: boolean;
    exampleTestcaseList: string[];
    metaData: string;
}

export interface Tag {
    id: string;
    name: string;
    slug: string;
}

export interface EditorData {
    questionId: string;
    questionFrontendId: string;
    codeSnippets: { lang: string, langSlug: string, code: string }[];
    envInfo: string;
}

export interface Hints {
    hints: string[];
}

export interface Submission {
    id: string;
    title: string;
    titleSlug: string;
    status: number;
    statusDisplay: string;
    lang: string;
    langName: string;
    runtime: string;
    timestamp: string;
    url: string;
    isPending: string;
    memory: string;
    hasNotes: boolean;
}

export interface SubmissionList {
    lastKey: boolean | null;
    hasNext: boolean | null;
    submissions: Submission[] | null;
}

export interface SolutionTagList {
    solutionTopicTags: { name: string, slug: string, count: number }[];
    solutionLanguageTags: { name: string, slug: string, count: number }[];
}

export interface Solution {
    id: number; // 64
    title: string;
    commentCount: number;
    topLevelCommentCount: number;
    viewCount: number;
    pinned: boolean;
    isFavorite: boolean;
    solutionTags: { name: string, slug: string }[];
    post: Post;
    searchMeta: string | null;
}

export interface SolutionList {
    hasDirectResults: boolean;
    totalNum: number;
    solutions: Solution[];
}

export interface CommunitySolutionTopic {
    id: number;
    viewCount: number;
    topLevelCommentCount: number;
    favoriteCount: number;
    subscribed: boolean;
    title: string;
    pinned: boolean;
    solutionTags: { name: string, slug: string }[];
    hideFromTrending: boolean;
    commentCount: number;
    isFavorite: boolean;
    post: Post;
}

export interface CommunitySolution {
    isSolutionTopic: boolean;
    topic: CommunitySolutionTopic;
}

export interface CommunitySolutionComment {
    id: number /* 64 */;
    pinned: boolean;
    pinnedBy: any | null;
    post: Post;
    intentionTag: any | null;
    numChildren: number;
}

export interface CommunitySolutionCommentList {
    data: CommunitySolutionComment[];
    totalNum: number;
}

export interface CommunitySolutionCommentReply {
    id: number /* 64 */;
    pinned: boolean;
    pinnedBy: any | null;
    post: Post;
}

export interface DiscussionTopic {
    id: string;
    commentCount: number;
    topLevelCommentCount: number;
}

export interface Discussion {
    id: number; // 64
    pinned: boolean;
    pinnedBy: any;
    post: Post;
    intentionTag: any;
    numChildren: number;
}

export interface DiscussionList {
    data: Discussion[];
    totalNum: number;
    pageNo: number;
    numPerPage: number;
}

export interface DiscussionReply {
    id: number; // 64
    pinned: boolean;
    pinnedBy: string | null;
    post: Post;
}

export interface RuncodeState {
    state: string;
    runSuccess?: boolean;
    statusRuntime?: string;
    statusMemory?: string;
    statusMessage?: string;
    codeAnswer?: string[];
    stdOutput?: string[];
    elapsedTime?: number;
    expectedCodeAnswer?: string[];
    correctAnswer?: boolean;
    totalCorrect?: number;
    totalTestcases?: number;
    compileError?: string;
    fullCompileError?: string;
    runtimeError?: string;
    fullRuntimeError?: string;
}

export interface SubmissionState {
    state: string;
    statusCode?: number;
    lang?: string;
    runSuccess?: boolean;
    statusRuntime?: string;
    memory?: number;
    questionId?: string;
    elapsedTime?: number;
    compareResult?: string;
    codeOutput?: string;
    stdOutput?: string;
    lastTestcase?: string;
    expectedOutput?: string;
    taskFinishTime?: number;
    totalCorrect?: number;
    totalTestcases?: number;
    runtimePercentile?: number; // float
    statusMemory?: string;
    memoryPercentile?: number; // float
    prettyLang?: string;
    submissionId?: string;
    statusMessage?: string;
    compileError?: string;
    fullCompileError?: string;
    runtimeError?: string;
    fullRuntimeError?: string;
}

export interface Post {
    id: number /* 64 */;
    status: string | null;
    voteCount: number;
    creationDate: number /* 64 */;
    isHidden: boolean | null;
    author: Author;

    voteStatus: number | undefined;
    content: string | undefined;
    updationDate: number | undefined /* 64 */;
    authorIsModerator: boolean | undefined;
    isOwnPost: boolean | undefined;
    coinRewards: any[] | undefined;
}

export interface Author {
    username: string;
    nameColor: string | null;
    activeBadge: Badge | null;
    isActive: boolean;
    profile: Profile;

    isDiscussAdmin: boolean | undefined;
    isDiscussStaff: boolean | undefined;
}

export interface Profile {
    userAvatar: string;
    reputation: number;
}

export interface Badge {
    displayName: string;
    icon: string;
}
