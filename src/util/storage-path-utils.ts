import * as vscode from "vscode";
import * as fs from "fs";
import * as path from "path";
import * as hidefile from "hidefile";

export function getStoragePath() {
    const storagePath = vscode.workspace.getConfiguration("vsc-leetcode").get<string>("storagePath");
    if (storagePath)
        return fs.realpathSync(storagePath);
    else return null;
}

export async function setStoragePath(path: string) {
    if (!fs.existsSync(path) || !fs.lstatSync(path).isDirectory())
        throw new Error("Storage path must exist and must be a directory");
    await vscode.workspace.getConfiguration("vsc-leetcode").update("storagePath", path, true);
}

export function getStorageFolder(): string {
    const storagePath = getStoragePath();
    if (!storagePath)
        throw new Error("Storage path is not specified.");
    if (!fs.existsSync(storagePath))
        throw new Error("Directory for storage path does not exists.");
    if (!fs.lstatSync(storagePath).isDirectory())
        throw new Error("Storage path is not a directory.");

    const storageFolder = path.join(storagePath, ".vsc-leetcode");
    if (!fs.existsSync(storageFolder)) {
        fs.mkdirSync(storageFolder);
        hidefile.hideSync(storageFolder);
    }
    if (!fs.lstatSync(storageFolder).isDirectory())
        throw new Error(".vsc-leetcode is not a directory.");

    return storageFolder;
}

export function getProblemToFileMap(): Record<string, Record<string, string>> {
    const storageFolder = getStorageFolder();
    const problemToFilePath = path.join(storageFolder, "problemToFile.json");
    if (fs.existsSync(problemToFilePath)) {
        try {
            const content = fs.readFileSync(problemToFilePath, "utf8");
            return JSON.parse(content);
        }
        catch {
            return {};
        }
    }
    return {};
}

export function saveProblemToFileMap(map: Record<string, Record<string, string>>) {
    const storageFolder = getStorageFolder();
    const problemToFilePath = path.join(storageFolder, "problemToFile.json");
    fs.writeFileSync(problemToFilePath, JSON.stringify(map));
}

export function getFileToProblemMap(): Record<string, { titleSlug: string, langSlug: string }> {
    const storageFolder = getStorageFolder();
    const fileToProblemPath = path.join(storageFolder, "fileToProblem.json");
    if (fs.existsSync(fileToProblemPath)) {
        try {
            const content = fs.readFileSync(fileToProblemPath, "utf8");
            return JSON.parse(content);
        }
        catch {
            return {};
        }
    }
    return {};
}

export function saveFileToProblemMap(map: Record<string, { titleSlug: string, langSlug: string }>) {
    const storageFolder = getStorageFolder();
    const fileToProblemPath = path.join(storageFolder, "fileToProblem.json");
    fs.writeFileSync(fileToProblemPath, JSON.stringify(map));
}

export function getSolutionPath(titleSlug: string, langSlug: string): string | null {
    const storagePath = getStoragePath();
    if (!storagePath)
        throw new Error("Storage path is not specified.");
    const problemToFile = getProblemToFileMap();
    if (problemToFile.hasOwnProperty(titleSlug) &&
        typeof problemToFile[titleSlug] === "object" &&
        problemToFile[titleSlug].hasOwnProperty(langSlug) &&
        typeof problemToFile[titleSlug][langSlug] === "string")
        return path.join(storagePath, problemToFile[titleSlug][langSlug]);

    return null;
}

export function getProblemDetailsByFilePath(filepath: string) {
    try {
        if (!fs.existsSync(filepath))
            return null;
        const realpath = fs.realpathSync(filepath);
        const filename = path.basename(realpath);
        const dir = path.dirname(realpath);
        const fileToProblemPath = path.join(dir, ".vsc-leetcode", "fileToProblem.json");
        if (!fs.existsSync(fileToProblemPath))
            return null;
        const fileToProblem = JSON.parse(fs.readFileSync(fileToProblemPath, "utf8")) as Record<string, { titleSlug: string, langSlug: string }>;
        if (fileToProblem.hasOwnProperty(filename))
            return fileToProblem[filename];
        else
            return null;
    }
    catch {
        return null;
    }
}

export function createSolutionFile(id: string, titleSlug: string, langSlug: string, extension: string, content: string): string {
    const storagePath = getStoragePath();
    if (!storagePath)
        throw new Error("Storage path is not specified.");

    const filename = `${id}-${titleSlug}.${extension}`;
    const fullFilename = path.join(storagePath, filename);
    if (!fs.existsSync(fullFilename))
        fs.writeFileSync(fullFilename, content, "utf8");

    const problemToFile = getProblemToFileMap();
    if (!problemToFile.hasOwnProperty(titleSlug) || typeof problemToFile[titleSlug] !== "object")
        problemToFile[titleSlug] = {};
    problemToFile[titleSlug][langSlug] = filename;
    saveProblemToFileMap(problemToFile);

    const fileToProblem = getFileToProblemMap();
    fileToProblem[filename] = { titleSlug, langSlug };
    saveFileToProblemMap(fileToProblem);

    return fullFilename;
}
