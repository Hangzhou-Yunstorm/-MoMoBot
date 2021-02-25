
export enum MessageRoles {
    System = 0,
    Self = 2,
    Other = 4,
}

export enum MessageTypes {
    Text = 2,
    Image = 4,
    Voice = 8,
    Notice = 16
}

export default interface IMessage {
    content: string;
    time: Date;
    who: MessageRoles,
    type: MessageTypes,
    data?: any
}

