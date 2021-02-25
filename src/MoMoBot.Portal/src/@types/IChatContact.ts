export default interface IChatContact<T extends number | string> {
    id: T;
    name: string;
    time: Date;
    unread: boolean;
    message?: string;
    online?:boolean;
    groupName?:string;
    uid?: string;
    avatar?: string;
}