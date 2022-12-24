import * as crypto from "crypto";
import * as vscode from "vscode";

/**
 * This is in memory cache
 */
export class CacheController {

    private _secretStorage: vscode.SecretStorage;
    private _map: Record<string, any> = {};
    private _queue: string[] = [];

    constructor(secretStorage: vscode.SecretStorage) {
        this._secretStorage = secretStorage;
    }

    public async set(key: string, data: any) {
        const authCookie = await this._secretStorage.get("auth_cookie");
        if (!authCookie)
            return;
        const authCookieHash = this.hash(authCookie);
        const keyHash = this.hash(authCookieHash + "." + key);
        this._map[keyHash] = data;
        this._queue.push(keyHash);
        if (this._queue.length > 128)
            delete this._map[this._queue.shift()!];
    }

    public async get(key: string): Promise<any | null> {
        const authCookie = await this._secretStorage.get("auth_cookie");
        if (!authCookie)
            return null;
        const authCookieHash = this.hash(authCookie);
        const keyHash = this.hash(authCookieHash + "." + key);
        if (this._map.hasOwnProperty(keyHash))
            return this._map[keyHash];
        else
            return null;
    }

    private hash(data: string) {
        return crypto.createHash("sha256").update(data).digest("base64url");
    }
}