import * as vscode from "vscode";
import puppeteer from 'puppeteer';

export async function signinWithPrompt() {
    const login: string | undefined = await vscode.window.showInputBox({
        prompt: "Enter username or E-mail.",
        ignoreFocusOut: true,
        validateInput: (s: string): string | undefined => s && s.trim() ? undefined : "The input must not be empty"
    });
    if (!login) return;
    const password: string | undefined = await vscode.window.showInputBox({
        prompt: "Enter password.",
        password: true,
        ignoreFocusOut: true,
        validateInput: (s: string): string | undefined => s ? undefined : "Password must not be empty"
    });
    if (!password) return;
    await signin(login, password);
}

export async function signin(login: string, password: string) {

    vscode.window.withProgress({ location: vscode.ProgressLocation.Notification }, async (progress, token) => {
        progress.report({ message: "Sign in to LeetCode" });

        let promiseResolve: (val: any) => any, promiseReject: (reason: string) => any;
        const promise = new Promise((resolve, reject) => [promiseResolve, promiseReject] = [resolve, reject]);

        const browser = await puppeteer.launch({ headless: false });
        const [page] = await browser.pages();

        page.on("response", async response => {
            const url = response.url();
            const method = response.request().method();
            const statusCode = response.status();
            if (url === `https://leetcode.com/`) {

                const cookies = await page.cookies();
                for (const cookie of cookies) {
                    console.log(cookie.domain, cookie.name, cookie.value);
                }

                await browser.close();
                promiseResolve({});
                vscode.window.showInformationMessage("Successfully signed in");
            }
            else if (url === `https://leetcode.com/accounts/login/` && method === "POST" && statusCode === 400) {

                const errors = await response.json();
                const errorMessage = errors?.form?.errors?.[0] ?? "An error occurred while signing in";

                await browser.close();
                promiseReject(errorMessage);
                vscode.window.showErrorMessage(errorMessage);
            }
        });

        await page.goto('https://leetcode.com/accounts/login/');
        await page.waitForSelector(`#initial-loading`, { hidden: true });

        const loginInput = await page.$(`#id_login`);
        await loginInput?.type(login);

        const passwordInput = await page.$(`#id_password`);
        await passwordInput?.type(password);

        const signinBtn = await page.$(`#signin_btn`);
        await signinBtn?.click();

        await promise;
    });
}
