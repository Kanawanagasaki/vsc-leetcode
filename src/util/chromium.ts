import * as os from 'os';
import * as path from 'path';
import * as fs from 'fs';
import * as https from 'https';
import * as extractZip from 'extract-zip';
import { parse as urlParse } from 'url';

const downloadURLs: Record<string, string> = {
    linux: 'https://storage.googleapis.com/chromium-browser-snapshots/Linux_x64/1069273/chrome-linux.zip',
    mac: 'https://storage.googleapis.com/chromium-browser-snapshots/Mac/1069273/chrome-mac.zip',
    mac_arm: 'https://storage.googleapis.com/chromium-browser-snapshots/Mac_Arm/1069273/chrome-mac.zip',
    win32: 'https://storage.googleapis.com/chromium-browser-snapshots/Win/1069273/chrome-win.zip',
    win64: 'https://storage.googleapis.com/chromium-browser-snapshots/Win_x64/1069273/chrome-win.zip',
};

function getUrl() {
    const platform = os.platform();
    switch (platform) {
        case 'darwin': return downloadURLs[os.arch() === 'arm64' ? 'mac_arm' : 'mac'];
        case 'linux': return downloadURLs['linux'];
        case 'win32': return downloadURLs[os.arch() === 'x64' ? 'win64' : 'win32'];
        default:
            throw new Error('Unsupported platform: ' + platform);
    }
}

function downloadFile(url: string, destinationPath: string, progressCallback?: (downloadedBytes: number, totalBytes: number) => void) {
    let fulfill: (value: void | PromiseLike<void>) => void;
    let reject: (err: Error) => void;
    const promise = new Promise<void>((x, y) => {
        fulfill = x;
        reject = y;
    });

    let downloadedBytes = 0;
    let totalBytes = 0;

    const request = https.request({
        ...urlParse(url),
        method: "GET",
        headers: { Connection: 'keep-alive' }
    }, res => {
        if (res.statusCode && Math.floor(res.statusCode / 100) === 3 && res.headers.location)
            downloadFile(res.headers.location, destinationPath, progressCallback);
        else if (res.statusCode !== 200) {
            const error = new Error(`Download failed: server returned code ${res.statusCode}. URL: ${url}`);
            res.resume();
            reject(error);
        }
        else {
            const file = fs.createWriteStream(destinationPath);
            file.on('finish', () => {
                return fulfill();
            });
            file.on('error', error => {
                return reject(error);
            });
            res.pipe(file);
            totalBytes = parseInt(res.headers['content-length']!, 10);
            if (progressCallback) {
                res.on('data', chunk => {
                    downloadedBytes += chunk.length;
                    progressCallback!(downloadedBytes, totalBytes);
                });
            }
        }
    });
    request.end();
    request.on('error', error => reject(error));
    return promise;
}

export async function downloadChromium(extensionDir: string, progressCallback?: (downloadedBytes: number, totalBytes: number) => void) {
    if (os.platform() === 'linux' && os.arch() === 'arm64') {
        if (fs.existsSync('/usr/bin/chromium-browser') || fs.existsSync('/usr/bin/chromium'))
            return;
        throw new Error('The chromium binary is not available for arm64.' +
            '\nIf you are on Ubuntu, you can install with: ' +
            '\n sudo apt install chromium' +
            '\n sudo apt install chromium-browser');
    }

    const dirPath = path.join(extensionDir, "browser");

    if (!fs.existsSync(dirPath))
        fs.mkdirSync(dirPath, { recursive: true });
    const url = getUrl();
    const filename = url.split('/').pop();
    if (!filename)
        throw new Error("Failed to get filename of archive");
    const archivePath = path.join(dirPath, filename);
    try {
        await downloadFile(url, archivePath, progressCallback);
        await extractZip(archivePath, { dir: dirPath });
        fs.chmodSync(getExecutablePath(extensionDir), 0o755);
    } finally {
        if (fs.existsSync(archivePath))
            fs.unlinkSync(archivePath);
    }
}

export function getExecutablePath(extensionDir: string) {
    const platform = os.platform();
    switch (platform) {
        case 'darwin': return path.join(extensionDir, "browser", "chrome-mac", "Chromium.app", "Contents", "MacOS", "Chromium");
        case 'linux': return path.join(extensionDir, "browser", "chrome-linux", "chrome");
        case 'win32': return path.join(extensionDir, "browser", "chrome-win", "chrome.exe");
        default:
            throw new Error('Unsupported platform: ' + platform);
    }
}
