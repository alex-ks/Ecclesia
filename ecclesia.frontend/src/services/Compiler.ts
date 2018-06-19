interface IPartialCheckResult
{
    name: string;
    opens: string[];
}

export default class Compiler
{
    private url: string;

    public constructor(url: string)
    {
        this.url = url;
    }

    public partialCheck(source: string): Promise<IPartialCheckResult>
    {
        return new Promise<IPartialCheckResult>((resolve, reject) =>
        {
            let requester = new XMLHttpRequest();
            requester.open('POST', this.url, false);
            requester.setRequestHeader("Content-Type", "application/json");
            requester.send(JSON.stringify({ source: source }));

            if (requester.status == 200)
                resolve(JSON.parse(requester.responseText) as IPartialCheckResult);
            else
                reject(new Error([requester.status, requester.responseText].join(": ")));
        });
    }

    public compileCode(code: string): Promise<string>
    {
        return new Promise<string>((resolve, reject) => 
        {
            let requester = new XMLHttpRequest();
            requester.open('POST', this.url, false);
            requester.setRequestHeader("Content-Type", "application/json");
            requester.send(JSON.stringify({ source: code }));

            if (requester.status == 200)
                resolve(requester.responseText);
            else
                reject(new Error([requester.status, requester.responseText].join(": ")));
        });
    }
}