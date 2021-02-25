export default interface IChatRecord<T extends number | string> {
    id: T;
    title: string;
    
    
    messages: [];
    groupName?:string;
    online?:boolean;
}