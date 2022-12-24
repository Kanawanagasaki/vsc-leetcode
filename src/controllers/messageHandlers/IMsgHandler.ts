export interface IMsgHandler {
    getCommandName():string;
    execute(webviewSender: string, data:any):void;
}
