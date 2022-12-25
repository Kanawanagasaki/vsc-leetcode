import * as vscode from 'vscode';
import { MessageController } from './controllers/MessageController';
import { LeetcodeController } from './controllers/LeetcodeController';
import { WebviewController } from './controllers/WebviewController';
import { CacheController } from './controllers/CacheController';

export function activate(context: vscode.ExtensionContext) {

	const cacheController = new CacheController(context.secrets);
	const leetcodeController = new LeetcodeController(context, cacheController);
	const webviewController = new WebviewController(context);
	const messageController = new MessageController(leetcodeController, webviewController, context);

	context.subscriptions.push(
		vscode.window.registerWebviewViewProvider("vsc-leetcode-home", {
			resolveWebviewView: (panel, webviewContext, token) => webviewController.resolveMenuWebview(panel, messageController)
		})
	);

	vscode.workspace.onDidChangeConfiguration(ev => {
		if (ev.affectsConfiguration("vsc-leetcode")) {
			messageController.executeAll("getStoragePath");
			messageController.executeAll("getSettings");
		}
	});

	vscode.window.onDidChangeVisibleTextEditors(_ => {
		messageController.executeAll("getActiveTextEditor");
	});

	vscode.window.registerWebviewPanelSerializer("vsc-leetcode-problem", {
		async deserializeWebviewPanel(webviewPanel, state: { problemTitleSlug?: string }) {
			if (!state.hasOwnProperty("problemTitleSlug") || typeof state.problemTitleSlug !== "string")
				return;
			const problemTitle = await leetcodeController.getProblemTitle(state.problemTitleSlug);
			if (problemTitle)
				webviewController.restoreProblemWebview(webviewPanel, problemTitle, messageController);
		}
	});
}

export function deactivate(context: vscode.ExtensionContext) { }
