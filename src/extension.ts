import * as vscode from 'vscode';
import { signinWithPrompt } from './commands/signin';

export function activate(context: vscode.ExtensionContext) {

	context.subscriptions.push(
		vscode.commands.registerCommand('vsc-leetcode.signin', () => signinWithPrompt())
	);
}

export function deactivate() { }
