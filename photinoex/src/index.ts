import { Guid } from "guid-typescript";

export interface IPhotino {
    sendMessage: (message: string) => void;
    receiveMessage: (callback: (message: string) => void) => void;
}

export interface IPhotinoRequest<T = object> {
    Id: string;
    Body: string | T;
}

export interface IPhotinoResponse {
    Id: string;
    Body: string;
}

type PhotinoPendings = Map<string, (value: string) => void>;

const ipc = window.external as unknown as IPhotino;

export class PhotinoClient {
    private pendings: PhotinoPendings = new Map();

    constructor() {
        ipc.receiveMessage((responseMessage: string) => {
            const response = JSON.parse(responseMessage) as IPhotinoResponse;
            this.pendings.get(response.Id)?.(response.Body);
        });
    }

    query = (message: string | object) => {
        const request: IPhotinoRequest = {
            Id: Guid.create().toString(),
            Body: message
        };

        ipc.sendMessage(JSON.stringify(request));

        return new Promise((resolve, _reject) => {
            this.pendings.set(request.Id, resolve);
        });
    }
}